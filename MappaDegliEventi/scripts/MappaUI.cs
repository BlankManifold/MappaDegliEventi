using Godot;
using System;

public partial class MappaUI : Control
{
	private InformationBox _informationBox;
	private LineEdit _mappaNameLineEdit;

	[Signal]
	public delegate void AddedPointEventHandler(Point point);

    public override void _Ready()
    {
        _informationBox = GetNode<InformationBox>("%InformationBox");
        _mappaNameLineEdit = GetNode<LineEdit>("%MappaName");

		_mappaNameLineEdit.Text = DateTime.Now.ToString();
    }
    
	public void _on_mappa_plot_added_point(Point point)
	{
		point.Hovering += _informationBox.OnPointHovering;
		point.Selected += _informationBox.OnPointSelected;
	}
	
}
