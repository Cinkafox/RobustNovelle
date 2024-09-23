using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Game.Dynamic;

[TypeSerializer]
public sealed class DynamicValueSerializer : ITypeSerializer<DynamicValue, MappingDataNode>, ITypeSerializer<DynamicValue, ValueDataNode>
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

    public DynamicValue Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<DynamicValue>? instanceProvider = null)
    {
        var compound = GetCompound(node, dependencies, serializationManager);
        if (compound.Type is null) 
            throw new Exception();
        
        var type = GetType(serializationManager, compound.Type);
        
        if(compound.DoLazy) 
            return new DynamicValue(compound.Type.Value, 
                new LazyDynamicValue(() => serializationManager.Read(type, compound.Value, context)!));
        
        return new DynamicValue(compound.Type.Value, serializationManager.Read(type, compound.Value, context)!);
    }

    public DataNode Write(ISerializationManager serializationManager, DynamicValue value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        throw new NotImplementedException();
    }

    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }

    public DynamicValue Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<DynamicValue>? instanceProvider = null)
    {
        var value = serializationManager.Read<string?>(node);
        if (value is null) throw new Exception("FUCK!");

        if (value[0] == '#')
        {
            var color = serializationManager.Read<Color>(node);
            return new DynamicValue("Color", color);
        }

        return new DynamicValue(nameof(ProtoId<DynamicValuePrototype>), 
            new LazyDynamicValue(
                () =>
                {
                    var id = serializationManager.Read<ProtoId<DynamicValuePrototype>>(node);
                    return dependencies.Resolve<IPrototypeManager>().Index(id).Value.GetValueObject();
                }
                ));
    }

    private Type GetType(ISerializationManager serializationManager, ValueDataNode? valueType)
    {
        if (valueType is null) 
            throw new Exception();
        var type = serializationManager.Read<Type?>(valueType);
        if (type is null)
            throw new InvalidMappingException("NO TYPE " + valueType.Value);
        return type;
    }

    private DynamicValueCompound GetCompound(MappingDataNode root,IDependencyCollection dependencies, ISerializationManager serializationManager)
    {
        var compound = new DynamicValueCompound(root, serializationManager);

        if (TryGetParent(root, dependencies, out var parentMapping))
        {
            var parentCompound = GetCompound(parentMapping, dependencies,
                serializationManager);
            
            compound.Type = parentCompound.Type;
            var type = GetType(serializationManager,parentCompound.Type);

            compound.Value = serializationManager.PushComposition(type, [parentCompound.Value],
                    compound.Value);
        }

        return compound;
    }

    private bool TryGetParent(MappingDataNode node, IDependencyCollection dependencies,[NotNullWhen(true)] out MappingDataNode? protoMapping)
    {
        protoMapping = null;
        if (!node.TryGet("parent", out var parentNode))
            return false;

        if (parentNode is ValueDataNode valueDataNode &&
            dependencies.Resolve<IPrototypeManager>().TryGetMapping(typeof(DynamicValuePrototype),
                valueDataNode.Value, out var protoValMapping))
        {
            protoMapping = protoValMapping.Get<MappingDataNode>("value");
            return true;
        }

        if (parentNode is MappingDataNode mappingDataNode)
        {
            protoMapping = mappingDataNode;
            return true;
        }

        return false;
    }
}

public sealed class DynamicValueCompound
{
    public ValueDataNode? Type;
    public DataNode Value;
    public bool DoLazy;

    public DynamicValueCompound(MappingDataNode root, ISerializationManager serializationManager)
    {
        if (root.TryGet<ValueDataNode>("valueType", out var type))
        {
            Type = type;
        }
        
        Value = root.Get("value");
        
        if (root.TryGet("isLazy", out var lazyNode))
            DoLazy = serializationManager.Read<bool>(lazyNode);
    }
}