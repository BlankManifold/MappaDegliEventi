using Godot;

public partial class PointsList : Button
{
	private VBoxContainer _listContainer;

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

		_listContainer.AddChild(pointButton);
	}
    
	public void _on_toggled(bool pressed)
	{
		_listContainer.Visible = pressed;
	}

	public void _on_information_box_added_point(PointInfo info)
	{
		AddAPoint(info);
	}
	public void _on_information_box_modified_point(Point point, PointInfo info)
	{
		int id = (int)info.id-1;
		_listContainer.GetChild<PointListButton>(id).PointName = info.name;
	}
	public void _on_information_box_removed_point(Point point)
	{
		int id = (int)point.Info.id-1;
		PointListButton childToBeRemoved = _listContainer.GetChild<PointListButton>(id);
		_listContainer.RemoveChild(childToBeRemoved);
		childToBeRemoved.QueueFree();

		for (int i = id; i < _listContainer.GetChildCount(); i++)
		{
			PointListButton pointButton = _listContainer.GetChild<PointListButton>(i);
			pointButton.PointId -= 1;
		}
	}


}
