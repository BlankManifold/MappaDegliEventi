using Godot;

public partial class GalleryMapIcon : VBoxContainer
{
	private string _mapName;
	public string MapName {get {return _mapName;}}
	private string _mapIdentifier;
	public string MapIdentifier {get {return _mapIdentifier;}}

	private Label _mapNameLabel;

	[Signal]
	public delegate void SelectedEventHandler(GalleryMapIcon icon);
	public void Init(string identifier, string name)
	{
		_mapName = name;
		_mapIdentifier = identifier;
	}
	public override void _Ready()
	{
		_mapNameLabel = GetNode<Label>("%MapNameLabel");
		_mapNameLabel.Text = _mapName;
	}

	public void _on_button_button_down()
	{
		EmitSignal(SignalName.Selected,this);
	}
}
