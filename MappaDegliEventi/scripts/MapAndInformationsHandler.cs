using Godot;

namespace Handlers
{
    public partial class MapAndInformationsHandler : Node
    {
        private Point _selectedPoint = null;
        private Point _hoveredPoint = null;

        private PointsList _pointsList;
        private InformationBox _informationBox;
        private MappaPlot _mapPlot;

        public override void _Ready()
        {
            _informationBox = GetNode<InformationBox>("%InformationBox");
            _mapPlot = GetNode<MappaPlot>("%MappaPlot");
            _pointsList = GetNode<PointsList>("%PointList");

            _mapPlot.PointListButtonDown += OnPointListButtonDown;
            _mapPlot.GhostPointButtonDown += OnGhostPointButtonDown;
            _informationBox.AddedPoint += OnAddedPoint;
            _informationBox.RemovedPoint += OnRemovedPoint;
            _informationBox.ModifiedPoint += OnModifiedPoint;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("point_selected"))
            {
                if (_hoveredPoint != null)
                {
                    UpdateSelected(_hoveredPoint);
                }

            }
            
            @event.Dispose();
        }

        public void Clear()
        {
            _selectedPoint = null;
            _hoveredPoint = null;
        }

        public void UpdateHovering(Point point, bool hovering)
        {
            GD.Print($"Hovering changed ->: {point.Info.id}, {hovering}");

            point.UpdateHovering(hovering);
            _hoveredPoint = hovering ? point : null;
            

            if (_selectedPoint == null)
            {
                _informationBox.UpdateHovering(_hoveredPoint);
            }
        }

        public void UpdateSelected(Point point)
        {
            if (_selectedPoint == point)
            {
                GD.Print($"Deselected: {point.Info.id}");
                DeselectCurrent();
                return;
            }
            
			if (_selectedPoint != null)
			{
                GD.Print($"Deselected: {_selectedPoint.Info.id}");
            	_selectedPoint.UpdateSelection(false);
			}

            point.UpdateSelection(true);
            _informationBox.UpdateSelection(point);
            _selectedPoint = point;
            GD.Print($"Selected: {point.Info.id}");

        }
        public void DeselectCurrent()
        {
            _selectedPoint.UpdateSelection(false);
            _selectedPoint = null;
        }

        public void OnAddedPoint(PointInfo info)
        {
			Point point = _mapPlot.AddedPoint(info);
			point.Hovering += UpdateHovering; 

            UpdateSelected(point);
			_pointsList.AddAPoint(info);
        }
        public void OnRemovedPoint()
        {
			_mapPlot.RemovedPoint(_selectedPoint);
			_pointsList.RemovePoint(_selectedPoint.Info.id);
            DeselectCurrent();
        }
        public void OnModifiedPoint(PointInfo info)
        {
			_mapPlot.ModifyPoint(_selectedPoint, info);
			_pointsList.ModifyPoint(info);
        }
        public void OnPointListButtonDown(Point point)
        {
            UpdateSelected(point);
        }
        public void OnGhostPointButtonDown(Vector2I coords, int id)
        {
            if (_selectedPoint != null)
                DeselectCurrent();
            
            _informationBox.ActiveAddPointState(coords, id);
        }
    }


}