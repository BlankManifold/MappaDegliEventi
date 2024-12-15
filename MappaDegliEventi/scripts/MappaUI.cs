using Godot;

public partial class MappaUI : Control
{
	private string _mapPlotIdentifier = null;
	private InformationBox _informationBox;
	private LineEdit _mappaNameLineEdit;
	private MappaPlot _mapPlot;
	private PointsList _pointList;

	private Handlers.MapAndInfosHandler _MapAndInfosHandler;

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
		_MapAndInfosHandler = GetNode<Handlers.MapAndInfosHandler>("%MapAndInfosHandler");

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

		foreach (PointInfoRes info in mapPlotRes.PointInfoList)
		{
			Point point = _mapPlot.AddedPoint(info);
			point.Hovering += _MapAndInfosHandler.OnHovering;

			_pointList.AddAPoint(info);
		};
	}
	public void _on_save_button_button_down()
	{
		Handlers.SaveLoadHandler.SaveMapPlot(_mapPlot.Points(), _mappaNameLineEdit.Text, _mapPlotIdentifier);
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
		_MapAndInfosHandler.Clear();
	}

}
