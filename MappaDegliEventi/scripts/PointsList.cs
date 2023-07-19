using Godot;

public partial class PointsList : Button
{
	private VBoxContainer _listContainer;

	[Signal]
    public delegate void PointListButtonSelectedEventHandler(int id);
	
	public override void _Ready()
    {
		_listContainer = GetNode<VBoxContainer>("%ListContainer");
    }
	
	public void AddAPoint(PointInfo info)
	{
		int id = _listContainer.GetChildCount()+1;
		PackedScene pointListButtonSene = Globals.PackedScenes.PointListButton;
		PointListButton pointButton = pointListButtonSene.Instantiate<PointListButton>();
		pointButton.PointId = id;
		pointButton.PointName = info.name;
		pointButton.ButtonDown += () => EmitSignal(SignalName.PointListButtonSelected, new Variant[] {pointButton.PointId});
		
		_listContainer.AddChild(pointButton);
	}
    public void Clear()
	{
		foreach (PointListButton pointButton in _listContainer.GetChildren())
		{
			_listContainer.RemoveChild(pointButton);
			pointButton.QueueFree();
		}
		EmitSignal(SignalName.Toggled, false);
	}
	public void ModifyPoint(PointInfo info)
	{
		_listContainer.GetChild<PointListButton>(info.id-1).PointName = info.name;
	}
	public void RemovePoint(int id)
	{
		PointListButton childToBeRemoved = _listContainer.GetChild<PointListButton>(id-1);
		_listContainer.RemoveChild(childToBeRemoved);
		childToBeRemoved.QueueFree();

		for (int i = id-1; i < _listContainer.GetChildCount(); i++)
		{
			PointListButton pointButton = _listContainer.GetChild<PointListButton>(i);
			pointButton.PointId -= 1;
		}
	}

	public void _on_toggled(bool pressed)
	{
		_listContainer.Visible = pressed;
	}
}
