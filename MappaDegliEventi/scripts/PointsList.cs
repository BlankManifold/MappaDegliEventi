using System.Linq;
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

	public void AddAPoint(PointInfoRes info)
	{
		int id = _listContainer.GetChildCount() + 1;
		PackedScene pointListButtonSene = Globals.PackedScenes.PointListButton;
		PointListButton pointButton = pointListButtonSene.Instantiate<PointListButton>();
		pointButton.PointId = id;
		pointButton.PointName = info.Name;
		pointButton.ButtonDown += () => EmitSignal(SignalName.PointListButtonSelected, new Variant[] { pointButton.PointId });

		_listContainer.AddChild(pointButton);
	}
	public void Clear()
	{
		foreach (PointListButton pointButton in _listContainer.GetChildren().Cast<PointListButton>())
		{
			_listContainer.RemoveChild(pointButton);
			pointButton.QueueFree();
		}
		EmitSignal(SignalName.Toggled, false);
	}
	public void ModifyPoint(PointInfoRes info)
	{
		_listContainer.GetChild<PointListButton>(info.Id - 1).PointName = info.Name;
	}
	public void RemovePoint(int id)
	{
		PointListButton childToBeRemoved = _listContainer.GetChild<PointListButton>(id - 1);
		_listContainer.RemoveChild(childToBeRemoved);
		childToBeRemoved.QueueFree();

		for (int i = id - 1; i < _listContainer.GetChildCount(); i++)
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
