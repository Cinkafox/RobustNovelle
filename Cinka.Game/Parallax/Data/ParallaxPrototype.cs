using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Parallax.Data;

/// <summary>
/// Prototype data for a parallax.
/// </summary>
[Prototype("parallax")]
public sealed partial class ParallaxPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Parallax layers.
    /// </summary>
    [DataField("layers")]
    public List<ParallaxLayerConfig> Layers { get; private set; } = new();
}
