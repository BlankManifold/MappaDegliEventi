using Godot;

namespace Handlers
{
    public partial class MapAndInformationsHandler : Node
    {
        private Point _selectedPoint = null;
        public Point SelectedPoint
        {
            get { return _selectedPoint; }
            set
            {
                _selectedPoint = value;

                GetTree().CallGroup("points", Point.MethodName.ChangeZIndex, new Variant[] { 0 });
                if (_selectedPoint != null)
                    _selectedPoint.ChangeZIndex(1);
            }
        }
        private Point _hoveredPoint = null;
        private Point _possibleSelectedPoint = null;

        private PointsList _pointsList;
        private InformationBox _informationBox;
        private MappaPlot _mapPlot;
        private ChangeColorContainer _changeColorContainer;
        private bool _draggable = false;
        private double _clickTime = 0d;
        private double _clickThreshold = 0.5d;

        private enum State { Idle, Selected, Dragging, ColorSelection }
        private State _state = State.Idle;
        private State _storedState = State.Idle;
        private bool _enableInteraction = false;
        private Vector2 _relativeClickPosition;

        public override void _Ready()
        {
            _informationBox = GetNode<InformationBox>("%InformationBox");
            _mapPlot = GetNode<MappaPlot>("%MappaPlot");
            _pointsList = GetNode<PointsList>("%PointList");
            _changeColorContainer = GetNode<ChangeColorContainer>("%ChangeColorContainer");

            _mapPlot.PointListButtonDown += OnPointListButtonDown;
            _mapPlot.GhostPointButtonDown += OnGhostPointButtonDown;
            _mapPlot.ModifiedPoint += OnModifiedPointFromPlot;

            _informationBox.AddedPoint += OnAddedPoint;
            _informationBox.RemovedPoint += OnRemovedPoint;
            _informationBox.ModifiedPoint += OnModifiedPoint;
            _informationBox.ModifiedPoint += OnModifiedPoint;

            _changeColorContainer.ChangedColor += OnChangedPointColor;
            _changeColorContainer.ColorPickerShow += OnColorPickerShow;
        }
        public override void _PhysicsProcess(double delta)
        {
            _HandleMapHovering();

            switch (_state)
            {
                case State.Idle:
                    _HandleIdle();
                    _storedState = _state;
                    break;
                case State.Selected:
                    _HandleSelected(delta);
                    _storedState = _state;
                    break;
                case State.Dragging:
                    _HandleDragging();
                    _storedState = _state;
                    break;
                case State.ColorSelection:
                    break;
            }

        }

        private bool _IsMouseHovering()
        {
            Vector2 mousePosition = GetViewport().GetMousePosition();
            return (mousePosition.X < _mapPlot.GlobalPosition.X + _mapPlot.Size.X) &&
                    (mousePosition.Y < _mapPlot.GlobalPosition.Y + _mapPlot.Size.Y) &&
                    (mousePosition.X > _mapPlot.GlobalPosition.X) &&
                    (mousePosition.Y > _mapPlot.GlobalPosition.Y);
        }
        private void _HandleMapHovering()
        {
            _enableInteraction = _IsMouseHovering();
        }
        private void _HandleIdle()
        {
            if (!_enableInteraction)
                return;
            // se clicco su qualcuno se non ho nessun altro selezionato 
            // -> vai a modalità selezione e ricorda su chi hai cliccato
            if (Input.IsActionJustPressed("point_selected"))
            {
                if (_hoveredPoint != null)
                {
                    _possibleSelectedPoint = _hoveredPoint;
                    _relativeClickPosition = GetViewport().GetMousePosition() - _hoveredPoint.GlobalPosition;
                    _state = State.Selected;
                }
                return;
            }
        }
        private void _HandleSelected(double delta)
        {
            if (!_enableInteraction)
                return;
            // se rilascio prima di passare a dragging 
            // -> aggiorno selezione (cambio selezione o deseleziono attuale) 
            if (Input.IsActionJustReleased("point_selected"))
            {
                if (_hoveredPoint != null)
                {
                    _UpdateSelected(_hoveredPoint);
                }

                _state = (_selectedPoint == null) ? State.Idle : State.Selected;

                return;
            }

            // A. se ho qualcuno di selezionato e clicco su un punto 
            // -> aggiorno possibile punto selezionato 
            // B. se ho qualcuno di selezionato e fuori 
            // -> deseleziona corrente
            if (Input.IsActionJustPressed("point_selected"))
            {
                if (_hoveredPoint != null)
                {
                    _possibleSelectedPoint = _hoveredPoint;
                    _relativeClickPosition = GetViewport().GetMousePosition() - _hoveredPoint.GlobalPosition;
                    return;
                }
                _DeselectCurrent();
                _state = State.Idle;
                return;
            }

            // se tengo premuto abbastanza o mi muovo abbastanza (possible punto gia cliccato)
            // -> cambio selezione (se necessario) e vai a dragging
            if (Input.IsActionPressed("point_selected"))
            {
                _clickTime += delta;
                Vector2 shift = _relativeClickPosition - (GetViewport().GetMousePosition() - _possibleSelectedPoint.GlobalPosition);

                if (_clickTime >= _clickThreshold || (shift.Dot(shift) > 10))
                {
                    if (_selectedPoint != _possibleSelectedPoint)
                    {
                        _UpdateSelected(_possibleSelectedPoint);
                    }

                    _clickTime = 0d;
                    _state = State.Dragging;
                    _mapPlot.UpdateNextPointVisibility(_selectedPoint.Info, true);
                }
                return;
            }

            // se sono su una selezione multipla e vado a selezioni sotto
            // -> cambia selezione a punto subito successivo (ciclico)        
            if (Input.IsActionJustPressed("next_multi_point"))
            {
                if (_mapPlot.HasMulti(_selectedPoint))
                {
                    _selectedPoint.Visible = false;
                    _UpdateSelected(_mapPlot.GoToNextMultiPoint(_selectedPoint.Info));
                    _selectedPoint.Visible = true;
                }
            }
            if (Input.IsActionJustPressed("previsous_multi_point"))
            {
                if (_mapPlot.HasMulti(_selectedPoint))
                {
                    _selectedPoint.Visible = false;
                    _UpdateSelected(_mapPlot.GoToNextMultiPoint(_selectedPoint.Info, true));
                    _selectedPoint.Visible = true;
                }
            }

        }
        private void _HandleDragging()
        {
            if (!_enableInteraction)
            {
                _mapPlot.ResetPointPosition(_selectedPoint);
                _mapPlot.UpdateNextPointVisibility(_selectedPoint.Info, false);

                _state = State.Selected;
                return;
            }

            if (Input.IsActionJustReleased("point_selected"))
            {
                _mapPlot.SetUpChangePointPosition(_selectedPoint, GetViewport().GetMousePosition() - _relativeClickPosition);
                _state = State.Selected;
                return;
            }
            _mapPlot.DragPointTo(_selectedPoint, GetViewport().GetMousePosition() - _relativeClickPosition);
        }
        private void _UpdateHovering(Point point, bool hovering)
        {
            point.UpdateHovering(hovering);
            _hoveredPoint = hovering ? point : null;

            if (_selectedPoint == null)
            {
                _informationBox.UpdateHovering(_hoveredPoint);
            }
        }
        private void _UpdateSelected(Point point)
        {
            if (_selectedPoint == point)
            {
                _DeselectCurrent();
                return;
            }

            if (_selectedPoint != null)
            {
                _selectedPoint.UpdateSelection(false);
            }

            point.UpdateSelection(true);
            _informationBox.UpdateSelection(point);
            this.SelectedPoint = point;
            _changeColorContainer.Enabled = true;
        }
        private void _DeselectCurrent()
        {
            _selectedPoint.UpdateSelection(false);
            this.SelectedPoint = null;
            _possibleSelectedPoint = null;
            _changeColorContainer.Enabled = false;
        }
        private void _GoIdle()
        {
            _possibleSelectedPoint = null;
            _state = State.Idle;
        }


        public void Clear()
        {
            _selectedPoint = null;
            _hoveredPoint = null;
        }


        public void OnHovering(Point point, bool hovering)
        {
            _UpdateHovering(point, hovering);
        }
        public void OnAddedPoint(PointInfo info)
        {
            Point point = _mapPlot.AddedPoint(info);
            point.Hovering += OnHovering;

            _UpdateSelected(point);
            _pointsList.AddAPoint(info);

            _GoIdle();
        }
        public void OnRemovedPoint()
        {
            _pointsList.RemovePoint(_selectedPoint.Info.id);
            _mapPlot.RemovedPoint(_selectedPoint);
            _DeselectCurrent();

            _GoIdle();
        }
        public void OnModifiedPoint(PointInfo info)
        {
            _mapPlot.ModifyPoint(_selectedPoint, info);
            _pointsList.ModifyPoint(info);
        }
        public void OnModifiedPointFromPlot(PointInfo info)
        {
            _pointsList.ModifyPoint(info);
            _informationBox.Update(info);
        }
        public void OnPointListButtonDown(Point point)
        {
            _UpdateSelected(point);

            if (_selectedPoint != null)
            {
                _selectedPoint.Visible = true;
                _mapPlot.UpdateMultiPointsOrder(_selectedPoint);
                _mapPlot.UpdateNextPointVisibility(_selectedPoint.Info, false);
                _state = State.Selected;
                return;
            }

            _state = State.Idle;
        }
        public void OnGhostPointButtonDown(Vector2I coords, int id)
        {
            if (_selectedPoint != null)
            {
                _DeselectCurrent();
                _GoIdle();
            }

            _informationBox.ActiveAddPointState(coords, id);
        }
        public void OnChangedPointColor(Color color, bool allPoints)
        {
            if (!allPoints)
            {
                _selectedPoint.ChangeColor(color);
                return;
            }

            GetTree().CallGroup("points", Point.MethodName.ChangeColor, new Variant[] { color });
        }
        public void OnColorPickerShow(bool hide)
        {
            if (hide)
            {
                _state = _storedState;
                return;
            }

            _state = State.ColorSelection;
        }

    }


}