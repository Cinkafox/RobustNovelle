using Content.Client.GameVariables;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Client.PasterString.Data;

public sealed class SmartString
{
    private readonly IDependencyCollection _collection;
    private string? _value;

    public SmartString(string rawString, IDependencyCollection collection)
    {
        _collection = collection;
        RawString = rawString;
    }

    public SmartString(string rawString) : this(rawString, IoCManager.Instance!)
    {
    }

    public string RawString { get; }

    public static implicit operator SmartString(string str)
    {
        return new SmartString(str);
    }

    public static implicit operator string(SmartString smartString)
    {
        return smartString.ToString();
    }

    public override string ToString()
    {
        if (!RawString.Contains('$'))
        {
            _value = RawString;
            return RawString;
        }

        _value = "";

        var varMan = _collection.Resolve<VariableManager>();

        foreach (var str in RawString.Split(' '))
        {
            if (str[0] == '$')
            {
                _value += varMan.GetValue(str[1..]) + " ";
                continue;
            }

            _value += str + " ";
        }

        return _value;
    }
}

[TypeSerializer]
public sealed class SmartStringSerializer : ITypeSerializer<SmartString, ValueDataNode>, ITypeCopier<SmartString>,
    ITypeCopyCreator<SmartString>
{
    public void CopyTo(ISerializationManager serializationManager, SmartString source, ref SmartString target,
        IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
    {
        target = new SmartString(source.RawString, dependencies);
    }

    public SmartString CreateCopy(ISerializationManager serializationManager, SmartString source,
        IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
    {
        return new SmartString(source.RawString, dependencies);
    }

    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }

    public SmartString Read(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<SmartString>? instanceProvider = null)
    {
        return new SmartString(node.Value, dependencies);
    }

    public DataNode Write(ISerializationManager serializationManager, SmartString value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        return new ValueDataNode(value.RawString);
    }
}