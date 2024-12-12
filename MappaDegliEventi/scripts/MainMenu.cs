using Godot;

public partial class MainMenu : Control
{
    [Signal]
    public delegate void ButtonDownEventHandler(string name);

    public override void _Ready()
    {
        foreach (Button button in GetTree().GetNodesInGroup("MainButton"))
        {
            button.ButtonDown += () => EmitSignal(SignalName.ButtonDown, new Variant[] { button.Name });
        }
    }
}
