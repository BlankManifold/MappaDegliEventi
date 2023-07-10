using Godot;

public partial class Point : Button
{
	private PointInfo _info;
	public PointInfo Info
	{
		get {return _info;}
	}
	private VBoxContainer _popupInfo;

	[Signal]
	public delegate void HoveringEventHandler(Point point, bool on);
	[Signal]
	public delegate void SelectedEventHandler(Point point, bool pressed);

	public void Init(PointInfo info)
	{
		_info = info;
		GetNode<Label>("%Name").Text = info.name;
		GetNode<Label>("%IdLabel").Text = info.id.ToString();
	}
	public override void _Ready()
	{
		if (_info==null)
			_info = new PointInfo(0,"",0,0,"");
		_popupInfo = GetNode<VBoxContainer>("%PopUpInfo");
		_popupInfo.Visible = false;
		AddToGroup("points");
	}

	public void _on_mouse_entered()
	{
		_popupInfo.Visible = true;
		EmitSignal(SignalName.Hovering, this, true);
	}
	public void _on_mouse_exited()
	{
		_popupInfo.Visible = false;
		EmitSignal(SignalName.Hovering, this, false);
	}
	public void _on_toggled(bool pressed)
	{
		EmitSignal(SignalName.Selected, this, pressed);
	}
}
