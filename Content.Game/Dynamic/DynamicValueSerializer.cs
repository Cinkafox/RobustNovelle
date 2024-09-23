﻿using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Game.Dynamic;

[TypeSerializer]
public sealed class DynamicValueSerializer : ITypeSerializer<Game.Dynamic.DynamicValue, MappingDataNode>, ITypeSerializer<Game.Dynamic.DynamicValue, ValueDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        if (!node.TryGet("valueType", out ValueDataNode? valueType))
            return new FieldNotFoundErrorNode(new ValueDataNode("valueType"), typeof(string));
        if(!node.TryGet("value", out var value))
            return new FieldNotFoundErrorNode(new ValueDataNode("value"), typeof(string));
        return new ValidatedMappingNode([]);
    }

    public Game.Dynamic.DynamicValue Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Game.Dynamic.DynamicValue>? instanceProvider = null)
    {
        var valueType = (ValueDataNode)node["valueType"];
        var value = node["value"];
        
        var doLazy = false;
        if (node.TryGet("isLazy", out var lazyNode))
            doLazy = serializationManager.Read<bool>(lazyNode);
        
        
        if (valueType.Value == Game.Dynamic.DynamicValue.ReadByPrototypeCommand)
            return serializationManager.Read<Game.Dynamic.DynamicValue?>(value) ?? throw new InvalidOperationException();
        
        var type = serializationManager.Read<Type?>(valueType);
        if (type is null)
            throw new InvalidMappingException("NO TYPE " + valueType.Value);
        
        if(doLazy) 
            return new Game.Dynamic.DynamicValue(valueType.Value, 
                new LazyDynamicValue(() => serializationManager.Read(type, value, context)!));
        
        return new Game.Dynamic.DynamicValue(valueType.Value, serializationManager.Read(type, value, context)!);
    }

    public DataNode Write(ISerializationManager serializationManager, Game.Dynamic.DynamicValue value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        throw new NotImplementedException();
    }

    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }

    public Game.Dynamic.DynamicValue Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Game.Dynamic.DynamicValue>? instanceProvider = null)
    {
        var value = serializationManager.Read<string?>(node);
        if (value is null) throw new Exception("FUCK!");

        if (value[0] == '#')
        {
            var color = serializationManager.Read<Color>(node);
            return new Game.Dynamic.DynamicValue("Color", color);
        }

        return new Game.Dynamic.DynamicValue(nameof(ProtoId<DynamicValuePrototype>), 
            new LazyDynamicValue(
                () =>
                {
                    var id = serializationManager.Read<ProtoId<DynamicValuePrototype>>(node);
                    return dependencies.Resolve<IPrototypeManager>().Index(id).Value.GetValueObject();
                }
                ));
    }
}