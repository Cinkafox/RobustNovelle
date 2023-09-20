using System;
using System.IO;
using Robust.Client.Audio;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Cinka.Game.Audio.Data;

[TypeSerializer]
public sealed class AudioSerializer : ITypeSerializer<AudioSpecifier, ValueDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        var path = serializationManager.Read<ResPath>(node,context);
        try
        {
            if (!dependencies.Resolve<IResourceCache>().TryGetResource<AudioResource>(path, out _))
            {
                return new ErrorNode(node, $"Could not find a audio file: {path}");
            }

            return new ValidatedValueNode(node);
        }
        catch (Exception e)
        {
            return new ErrorNode(node, $"Failed parsing audio. ({node.Value}) ({e.Message}");
        }
    }

    public AudioSpecifier Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<AudioSpecifier>? instanceProvider = null)
    {
        var path = serializationManager.Read<ResPath>(node, context);
        return new AudioSpecifier(dependencies.Resolve<IResourceCache>().GetResource<AudioResource>(path).AudioStream,
            path);
    }

    public DataNode Write(ISerializationManager serializationManager, AudioSpecifier value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        return serializationManager.WriteValue(value.Path);
    }
}