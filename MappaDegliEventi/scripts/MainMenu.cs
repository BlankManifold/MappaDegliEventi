using Godot;
using System;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
        foreach (Button button in GetTree().GetNodesInGroup("MainButton"))
        {
            button.ButtonDown += () => OnMainButtonDown(button.Name);
        }
	}

	public void OnMainButtonDown(string name)
    {
        switch (name)
        {
            case "NewMapPlotButton":
                GetTree().ChangeSceneToPacked(Globals.PackedScenes.MappaUI);
                break;
            default:
                break;
        }
    }


}
