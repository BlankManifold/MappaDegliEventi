using Godot;

public partial class MappaUI : Control
{
	private string _mapPlotIdentifier = null;
	private InformationBox _informationBox;
	private LineEdit _mappaNameLineEdit;
	private MappaPlot _mapPlot;
	private PointsList _pointList;

	[Signal]
	public delegate void ReadyToLoadFromResourceEventHandler();
	[Signal]
	public delegate void GoBackButtonDownEventHandler();

	public async void Init(MapPlotRes mapPlotRes)
	{
		await ToSignal(this, SignalName.ReadyToLoadFromResource);
		_LoadMapFromResource(mapPlotRes);
	}
    public override void _Ready()
    {
        _informationBox = GetNode<InformationBox>("%InformationBox");
        _mappaNameLineEdit = GetNode<LineEdit>("%MappaName");
        _mapPlot = GetNode<MappaPlot>("%MappaPlot");
        _pointList = GetNode<PointsList>("%PointList");

		EmitSignal(SignalName.ReadyToLoadFromResource);
    }
	private void _LoadMapFromResource(MapPlotRes mapPlotRes)
	{
		_mappaNameLineEdit.Text = mapPlotRes.MapName;
		_mapPlotIdentifier = mapPlotRes.Identifier;

		Godot.Collections.Array<PointInfo> pointInfoList = (Godot.Collections.Array<PointInfo>)mapPlotRes.PointInfoList;
		foreach (PointInfo info in pointInfoList)
		{
			_mapPlot.CreateAPoint(info, true);
			_mapPlot.RemoveGhost(info);
			_pointList.AddAPoint(info);
		}; 
	}
	public void _on_save_button_button_down()
	{
		Handlers.SaveLoadHandler.SaveMapPlot(_mapPlot.GetPoints(), _mappaNameLineEdit.Text, _mapPlotIdentifier);
	}
	public void _on_go_back_button_button_down()
	{
		EmitSignal(SignalName.GoBackButtonDown);
	}
	public void _on_clear_plot_button_button_down()
	{
		_mapPlot.Clear();
		_informationBox.Clear();
		_pointList.Clear();
	}
	public void _on_mappa_plot_created_a_point(Point point, bool from_loading)
	{
		point.Selected += _informationBox.OnPointSelected; 
		point.Hovering += _informationBox.OnPointHovering; 
		
		if (!from_loading)
			_informationBox.UpdateSelection(point);
	}
	
}
