using Godot;

public partial class PointsList : Button
{
	private VBoxContainer _listContainer;

    public override void _Ready()
    {
		_listContainer = GetNode<VBoxContainer>("%ListContainer");
    }
    
	public void _on_toggled(bool pressed)
	{
		_listContainer.Visible = pressed;
	}

	public void _on_information_box_added_point(Globals.PointInfo info)
	{
		int id = _listContainer.GetChildCount()+1;
		Button button = new Button();
		button.Text = $"{id.ToString()}. {info.name}";
		button.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
		button.Alignment = HorizontalAlignment.Left;

		_listContainer.AddChild(button);
	}


}
