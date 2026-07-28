namespace Content.Client.Dialog.Data;

[DataDefinition]
public sealed partial class DialogButton
{
    [DataField] public Dialog[]? Dialog;
    [DataField] public string Name = "default";
}