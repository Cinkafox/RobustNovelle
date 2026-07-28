using System.Text;
using System.Text.RegularExpressions;

namespace Content.Client.GameVariables;

public sealed class VariableStringParser
{
    public IVariableContainer Variables { get; }

    public VariableStringParser(IVariableContainer container)
    {
        Variables = container;
    }
    
    private static string NormalizeName(string name) =>
        name.StartsWith("#") ? name.Substring(1) : name;

    /// <summary>
    /// Парсит строку и возвращает результат как строку.
    /// Поддерживает #variable и $(expression).
    /// </summary>
    public string Parse(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var result = ProcessExpressions(input);
        result = ProcessVariableRefs(result);
        return result;
    }

    /// <summary>
    /// Вычисляет выражение и возвращает результат как объект.
    /// Если выражение — это просто #variable, возвращает его значение напрямую.
    /// Если это арифметическое выражение, возвращает double.
    /// </summary>
    public object? EvaluateToObject(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;
        input = input.Trim();

        // Если это просто #variable
        if (input.StartsWith("#") && !input.Contains("$(") && !input.Contains(" "))
        {
            var name = NormalizeName(input);
            return Variables.Get(name);
        }

        // Если это $(...)
        if (input.StartsWith("$(") && input.EndsWith(")"))
        {
            var expr = input.Substring(2, input.Length - 3).Trim();
            return EvaluateToObjectInternal(expr);
        }

        // Иначе — вычислить как выражение
        return EvaluateToObjectInternal(input);
    }

    // ---------- Внутренние методы ----------

    private object? EvaluateToObjectInternal(string expr)
    {
        var tokens = Tokenizer.Tokenize(expr);
        
        // Если это просто одна переменная — вернуть её значение как есть
        if (tokens.Count == 1 && tokens[0].Kind == TokKind.Variable)
        {
            return Variables.Get(tokens[0].Value);
        }
        
        // Иначе — вычислить как числовое выражение
        var parser = new Parser(tokens, Variables);
        var result = parser.ParseExpression();
        if (parser.Pos < tokens.Count)
            throw new Exception($"Unexpected token: {tokens[parser.Pos]}");
        return result;
    }

    private string ProcessExpressions(string input)
    {
        var sb = new StringBuilder();
        var i = 0;
        while (i < input.Length)
        {
            if (i + 1 < input.Length && input[i] == '$' && input[i + 1] == '(')
            {
                var start = i + 2;
                int depth = 1, j = start;
                while (j < input.Length && depth > 0)
                {
                    if (input[j] == '(') depth++;
                    else if (input[j] == ')') depth--;
                    if (depth > 0) j++;
                }
                var expr = input.Substring(start, j - start).Trim();
                
                // Вычисляем как объект (может быть строка или число)
                var result = EvaluateToObjectInternal(expr);
                
                if (result is double d)
                    sb.Append(FormatNumber(d));
                else
                    sb.Append(result?.ToString() ?? "");
                
                i = j + 1;
            }
            else
            {
                sb.Append(input[i++]);
            }
        }
        return sb.ToString();
    }

    private string ProcessVariableRefs(string input)
    {
        return Regex.Replace(input, @"#(\w+)", m =>
        {
            var name = m.Groups[1].Value;
            var val = Variables.Get(name);
            return val?.ToString() ?? "";
        });
    }

    private static string FormatNumber(double v)
    {
        if (double.IsNaN(v) || double.IsInfinity(v)) return v.ToString();
        if (v == Math.Floor(v) && Math.Abs(v) < 1e15) return ((long)v).ToString();
        return v.ToString("0.##########");
    }

    // ============================================================
    //  ТОКЕНИЗАТОР (обновлён для сравнений и true/false)
    // ============================================================
    private enum TokKind { Number, Variable, Op, Assign, Compare, LParen, RParen }
    
    private class Token 
    { 
        public TokKind Kind; 
        public string Value; 
        public Token(TokKind k, string v) { Kind = k; Value = v; } 
        public override string ToString() => $"{Kind}:{Value}"; 
    }

    private static class Tokenizer
    {
        public static List<Token> Tokenize(string s)
        {
            var list = new List<Token>();
            var i = 0;
            while (i < s.Length)
            {
                var c = s[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                // Числа
                if (char.IsDigit(c) || (c == '.' && i + 1 < s.Length && char.IsDigit(s[i + 1])))
                {
                    var start = i;
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
                    list.Add(new Token(TokKind.Number, s.Substring(start, i - start)));
                    continue;
                }

                // Переменные (#name)
                if (c == '#')
                {
                    i++; 
                    var start = i;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_')) i++;
                    list.Add(new Token(TokKind.Variable, s.Substring(start, i - start)));
                    continue;
                }

                // Идентификаторы (true/false)
                if (char.IsLetter(c))
                {
                    var start = i;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_')) i++;
                    var word = s.Substring(start, i - start);
                    
                    if (word == "true")
                        list.Add(new Token(TokKind.Number, "1"));
                    else if (word == "false")
                        list.Add(new Token(TokKind.Number, "0"));
                    else
                        throw new Exception($"Unknown identifier '{word}'. Use 'true' or 'false'.");
                    continue;
                }

                // Операторы сравнения (двухсимвольные сначала)
                if (c == '=' && i + 1 < s.Length && s[i + 1] == '=') 
                { list.Add(new Token(TokKind.Compare, "==")); i += 2; continue; }
                if (c == '!' && i + 1 < s.Length && s[i + 1] == '=') 
                { list.Add(new Token(TokKind.Compare, "!=")); i += 2; continue; }
                if (c == '<' && i + 1 < s.Length && s[i + 1] == '=') 
                { list.Add(new Token(TokKind.Compare, "<=")); i += 2; continue; }
                if (c == '>' && i + 1 < s.Length && s[i + 1] == '=') 
                { list.Add(new Token(TokKind.Compare, ">=")); i += 2; continue; }
                
                // Одиночные операторы сравнения
                if (c == '<') { list.Add(new Token(TokKind.Compare, "<")); i++; continue; }
                if (c == '>') { list.Add(new Token(TokKind.Compare, ">")); i++; continue; }

                // Присваивание
                if (c == '=') { list.Add(new Token(TokKind.Assign, "=")); i++; continue; }
                
                // Скобки
                if (c == '(') { list.Add(new Token(TokKind.LParen, "(")); i++; continue; }
                if (c == ')') { list.Add(new Token(TokKind.RParen, ")")); i++; continue; }
                
                // Арифметические операторы
                if (c == '+' || c == '-' || c == '*' || c == '/')
                { list.Add(new Token(TokKind.Op, c.ToString())); i++; continue; }

                throw new Exception($"Unexpected character '{c}'");
            }
            return list;
        }
    }

    // ============================================================
    //  ПАРСЕР (обновлён для операций сравнения)
    // ============================================================
    private sealed class Parser
    {
        private readonly List<Token> _t;
        private readonly IVariableContainer _container;
        public int Pos;

        public Parser(List<Token> t, IVariableContainer container)
        { _t = t; _container = container; Pos = 0; }

        private Token Peek() => Pos < _t.Count ? _t[Pos] : null!;
        private Token Consume() => _t[Pos++];

        // Грамматика:
        // Expr    -> Assign
        // Assign  -> Var '=' Expr | Compare
        // Compare -> Add ( ('=='|'!='|'<'|'>'|'<='|'>=') Add )*
        // Add     -> Mul ( ('+'|'-') Mul )*
        // Mul     -> Unary ( ('*'|'/') Unary )*
        // Unary   -> '-' Unary | Primary
        // Primary -> Number | Var | '(' Expr ')'

        public double ParseExpression() => ParseAssign();

        private double ParseAssign()
        {
            if (Peek()?.Kind == TokKind.Variable && Pos + 1 < _t.Count && _t[Pos + 1].Kind == TokKind.Assign)
            {
                var name = Consume().Value;
                Consume();
                var newVal = ParseAssign();
                var oldVal = ToDouble(_container.Get(name)!);
                _container.Set(name, newVal);
                return oldVal;
            }
            return ParseCompare();
        }

        private double ParseCompare()
        {
            var left = ParseAdd();
            while (Peek()?.Kind == TokKind.Compare)
            {
                var op = Consume().Value;
                var right = ParseAdd();
                var result = op switch
                {
                    "==" => left == right,
                    "!=" => left != right,
                    "<"  => left < right,
                    ">"  => left > right,
                    "<=" => left <= right,
                    ">=" => left >= right,
                    _ => throw new Exception($"Unknown comparison operator: {op}")
                };
                left = result ? 1.0 : 0.0;
            }
            return left;
        }

        private double ParseAdd()
        {
            var left = ParseMul();
            while (Peek()?.Kind == TokKind.Op && (Peek().Value == "+" || Peek().Value == "-"))
            {
                var op = Consume().Value;
                var right = ParseMul();
                left = op == "+" ? left + right : left - right;
            }
            return left;
        }

        private double ParseMul()
        {
            var left = ParseUnary();
            while (Peek()?.Kind == TokKind.Op && (Peek().Value == "*" || Peek().Value == "/"))
            {
                var op = Consume().Value;
                var right = ParseUnary();
                if (op == "*") left *= right;
                else
                {
                    if (right == 0) throw new DivideByZeroException();
                    left /= right;
                }
            }
            return left;
        }

        private double ParseUnary()
        {
            if (Peek()?.Kind == TokKind.Op && Peek().Value == "-")
            {
                Consume();
                return -ParseUnary();
            }
            return ParsePrimary();
        }

        private double ParsePrimary()
        {
            var t = Peek();
            if (t == null) throw new Exception("Unexpected end of expression");

            if (t.Kind == TokKind.Number)
            {
                Consume();
                return double.Parse(t.Value, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (t.Kind == TokKind.Variable)
            {
                Consume();
                return ToDouble(_container.Get(t.Value)!);
            }
            if (t.Kind == TokKind.LParen)
            {
                Consume();
                var v = ParseExpression();
                if (Peek()?.Kind != TokKind.RParen)
                    throw new Exception("Missing ')'");
                Consume();
                return v;
            }
            throw new Exception($"Unexpected token: {t}");
        }

        private static double ToDouble(object v)
        {
            if (v == null) return 0;
            if (v is double d) return d;
            if (v is int i) return i;
            if (v is long l) return l;
            if (v is float f) return f;
            if (v is decimal dec) return (double)dec;
            if (v is bool b) return b ? 1.0 : 0.0;
            if (double.TryParse(v.ToString(), System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out var r)) return r;
            return 0;
        }
    }
}