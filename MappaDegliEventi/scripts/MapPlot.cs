using Godot;
using System.Collections.Generic;
using System.Linq;
public partial class MapPlot : Control
{
    [Export]
    private int _max_value = 10;
    [Export]
    private Font _ticks_font = null;

    private Node2D _XLines;
    private Node2D _YLines;
    private Node2D _XTicks;
    private Node2D _YTicks;
    private Node2D _Points;
    public Godot.Collections.Array<Node> Points()
    {
        return _Points.GetChildren();
    }
    private Node2D _GhostPoints;
    private Dictionary<Vector2I, GhostPoint> _GhostPointsDict = new() { };
    private PointsDictionary _PointsDict = new();
    private Vector2I _x_ticks_padding = new(5, 1);
    private Vector2I _y_ticks_padding = new(5, 0);
    private Vector2 _origin;
    private int _num_of_lines;
    private Vector2 _lines_spacing;
    private GhostPoint _selected_ghost_point;

    [Signal]
    public delegate void GhostPointButtonDownEventHandler(Vector2I coords, int id);
    [Signal]
    public delegate void CreatedAPointEventHandler(Point point);
    [Signal]
    public delegate void PointListButtonDownEventHandler(Point point);
    [Signal]
    public delegate void ModifiedPointEventHandler(PointInfoRes info);


    public override void _Ready()
    {
        _XLines = GetNode<Node2D>("%XLines");
        _YLines = GetNode<Node2D>("%YLines");
        _XTicks = GetNode<Node2D>("%XTicks");
        _YTicks = GetNode<Node2D>("%YTicks");
        _Points = GetNode<Node2D>("%Points");
        _GhostPoints = GetNode<Node2D>("%GhostPoints");

        PackedScene ghost_point_scene = Globals.PackedScenes.GhostPoint;

        _num_of_lines = 2 * _max_value + 1;
        _origin = GetRect().Size / 2;

        _lines_spacing = GetRect().Size / (_num_of_lines + 1);

        for (int i = 1; i <= _num_of_lines; i++)
        {
            Vector2 pos = i * _lines_spacing;

            Vector2 x_A = new(pos.X, 0);
            Vector2 x_B = new(pos.X, GetRect().Size.Y);
            Vector2 y_A = new(0, pos.Y);
            Vector2 y_B = new(GetRect().Size.X, pos.Y);

            _XLines.AddChild(_CreateLine(i, x_A, x_B));
            _YLines.AddChild(_CreateLine(i, y_A, y_B));

            Label x_tick = _CreateTick((i - 1 - _max_value).ToString(), new Vector2(pos.X, _origin.Y) + _x_ticks_padding);
            Label y_tick = _CreateTick((_max_value - i + 1).ToString(), new Vector2(_origin.X, pos.Y) + _y_ticks_padding);

            _XTicks.AddChild(x_tick);
            _YTicks.AddChild(y_tick);

            if (i == _max_value + 1)
            {
                x_tick.Visible = false;
                y_tick.Visible = false;
            }

            for (int j = 1; j <= _num_of_lines; j++)
            {
                float shift_Y = j * _lines_spacing.Y;
                GhostPoint ghost_point = ghost_point_scene.Instantiate<GhostPoint>();
                Vector2I ghost_coords = new(i - 1 - _max_value, _max_value - j + 1);
                // FIXME min tra linespacing e misura std, ma sempre quadrato o simmetrico
                Vector2 ghost_size = new(20, 20);
                Vector2 ghost_pos = new Vector2(pos.X, shift_Y) - ghost_size / 2;

                ghost_point.Init(ghost_pos, ghost_coords, ghost_size);
                _GhostPoints.AddChild(ghost_point);
                ghost_point.GhostPointButtonDown += OnGhostPointButtonDown;
                _GhostPointsDict.Add(ghost_coords, ghost_point);
            }
        }
    }

    #region Private methods
    private Label _CreateTick(string text, Vector2 position)
    {
        Label tick = new();

        if (_ticks_font != null)
            tick.AddThemeFontOverride("ticks", _ticks_font);

        tick.Text = text;
        tick.HorizontalAlignment = HorizontalAlignment.Center;
        tick.VerticalAlignment = VerticalAlignment.Center;
        tick.Position = position;

        return tick;
    }
    private Line2D _CreateLine(int i, Vector2 A, Vector2 B)
    {
        Line2D line = new();

        line.Width = 1;
        float alpha = 0.5f;

        if (i == _max_value + 1)
        {
            line.Width = 3;
            alpha = 1f;
        }

        line.AddPoint(A);
        line.AddPoint(B);
        line.DefaultColor = new Color(line.DefaultColor, alpha);

        return line;
    }
    private Vector2 _CoordsToPos(int x, int y)
    {
        Vector2 pos = new Vector2I(x, -y) * _lines_spacing + _origin;
        return pos;
    }
    private Vector2I _PosToCoords(Vector2 pos)
    {
        Vector2 coords_f = (pos - _origin) / _lines_spacing;
        coords_f = new Vector2(coords_f.X, -coords_f.Y);

        return (Vector2I)coords_f.Round();
    }
    private Point _CreateAPoint(PointInfoRes info)
    {
        PackedScene point_scene = Globals.PackedScenes.Point;
        Point point = point_scene.Instantiate<Point>();

        point.Update(info, _CoordsToPos(info.X, info.Y) - point.Size / 2);
        _Points.AddChild(point);
        _PointsDict.Add(new Vector2I(info.X, info.Y), point);

        return point;
    }
    private void _RemoveGhost(PointInfoRes info)
    {
        Vector2I coords = new(info.X, info.Y);
        if (!_GhostPointsDict.TryGetValue(coords, out GhostPoint ghost))
        {
            _selected_ghost_point = null;
            return;
        }

        _GhostPointsDict.Remove(coords);
        ghost.QueueFree();

        _selected_ghost_point = null;
    }
    private void _AddGhost(PointInfoRes info)
    {
        // FIXME min tra linespacing e misura std, ma sempre quadrato o simmetrico
        Vector2I coords = new(info.X, info.Y);

        if (_GhostPointsDict.ContainsKey(coords))
        {
            _selected_ghost_point = null;
            return;
        }

        Vector2 size = new(20, 20);
        Vector2 pos = _CoordsToPos(coords.X, coords.Y) - size / 2;

        GhostPoint ghost = Globals.PackedScenes.GhostPoint.Instantiate<GhostPoint>();
        ghost.Init(pos, coords, size);
        _GhostPoints.AddChild(ghost);
        ghost.GhostPointButtonDown += OnGhostPointButtonDown;

        _GhostPointsDict.Add(coords, ghost);
        _selected_ghost_point = null;
    }
    #endregion

    #region Public methods  
    public void Clear()
    {
        _GhostPointsDict = [];
        _PointsDict.Clear();

        if (Points().Count == 0) return;

        foreach (Point point in Points().Cast<Point>())
        {
            _AddGhost(point.Info);
            _Points.RemoveChild(point);
            point.QueueFree();
        }

    }
    public Point AddedPoint(PointInfoRes info)
    {
        _RemoveGhost(info);
        return _CreateAPoint(info);
    }
    public void RemovedPoint(Point point)
    {
        _PointsDict.Remove(point);
        if (!_PointsDict.HasPoint(new Vector2I(point.Info.X, point.Info.Y)))
            _AddGhost(point.Info);

        int id = point.Info.Id;

        _Points.RemoveChild(point);
        point.QueueFree();

        for (int i = id - 1; i < _Points.GetChildCount(); i++)
        {
            Point p = _Points.GetChild<Point>(i);
            p.Info.Id -= 1;
            p.Init(p.Info);
        }
    }
    public void ModifyPoint(Point point, PointInfoRes info)
    {
        Vector2I from = new(point.Info.X, point.Info.Y);
        Vector2I to = new(info.X, info.Y);

        _PointsDict.Move(from, to, point);

        if (!_PointsDict.HasPoint(from))
            _AddGhost(point.Info);

        point.Update(info, _CoordsToPos(info.X, info.Y) - point.Size / 2);
        _RemoveGhost(info);
    }
    public void DragPointTo(Point point, Vector2 position)
    {
        point.GlobalPosition = point.GlobalPosition.Lerp(position, 0.8f);
    }
    public void SetUpChangePointPosition(Point point, Vector2 position)
    {
        Vector2I coords = _PosToCoords(position - GlobalPosition + point.Size / 2);

        // if (!_GhostPointsDict.ContainsKey(coords))
        // {
        //     point.Position = _CoordsToPos(point.Info.X, point.Info.Y) - point.Size / 2;
        //     return;
        // }

        PointInfoRes info = new(point.Info)
        {
            X = coords.X,
            Y = coords.Y
        };

        ModifyPoint(point, info);
        EmitSignal(SignalName.ModifiedPoint, info);
    }
    public void UpdateNextPointVisibility(PointInfoRes info, bool visible)
    {
        Vector2I coords = new(info.X, info.Y);

        if (_PointsDict.HasMulti(coords))
            _PointsDict.UpdateNextPointVisibility(coords, visible);
    }
    public void UpdateMultiPointsOrder(Point point)
    {
        Vector2I coords = new(point.Info.X, point.Info.Y);

        if (_PointsDict.HasMulti(coords))
            _PointsDict.UpdateMultiPointsOrder(coords, point);
    }
    public Point GoToNextMultiPoint(PointInfoRes info, bool inverse = false)
    {
        Vector2I coords = new(info.X, info.Y);
        return _PointsDict.GoToNextMultiPoint(coords, inverse);
    }
    public bool HasMulti(Point point)
    {
        Vector2I coords = new(point.Info.X, point.Info.Y);
        return _PointsDict.HasMulti(coords);
    }
    public void ResetPointPosition(Point point)
    {
        point.Position = _CoordsToPos(point.Info.X, point.Info.Y) - point.Size / 2;
    }
    #endregion


    #region Response to signals
    public void OnGhostPointButtonDown(GhostPoint ghost)
    {
        _selected_ghost_point?.Deselect();
        _selected_ghost_point = ghost;
        EmitSignal(SignalName.GhostPointButtonDown, ghost.Coords, _Points.GetChildCount() + 1);
    }
    public void _on_resized()
    {
        Vector2 old_lines_spacing = _lines_spacing;
        _lines_spacing = GetRect().Size / (_num_of_lines + 1);
        Vector2 affine_factor = _lines_spacing / old_lines_spacing;

        _origin = GetRect().Size / 2;

        if (!IsNodeReady()) return;

        for (int i = 1; i <= _num_of_lines; i++)
        {
            Line2D x_line = _XLines.GetChild<Line2D>(i - 1);
            Line2D y_line = _YLines.GetChild<Line2D>(i - 1);
            Label x_tick = _XTicks.GetChild<Label>(i - 1);
            Label y_tick = _YTicks.GetChild<Label>(i - 1);

            Vector2 coords = i * _lines_spacing;

            Vector2 x_A = new(coords.X, 0);
            Vector2 x_B = new(coords.X, GetRect().Size.Y);
            Vector2 y_A = new(0, coords.Y);
            Vector2 y_B = new(GetRect().Size.X, coords.Y);

            x_line.Points = [x_A, x_B];
            y_line.Points = [y_A, y_B];

            x_tick.Position = new Vector2(coords.X, _origin.Y) + _x_ticks_padding;
            y_tick.Position = new Vector2(_origin.X, coords.Y) + _y_ticks_padding;
        }

        foreach (GhostPoint ghost_point in _GhostPoints.GetChildren())
            ghost_point.Position = affine_factor * (ghost_point.Position + ghost_point.Size / 2) - ghost_point.Size / 2;

        foreach (Point point in Points())
            point.Position = affine_factor * (point.Position + point.Size / 2) - point.Size / 2;
    }
    public void _on_point_list_point_list_button_selected(int id)
    {
        Point point = _Points.GetChild<Point>(id - 1);
        EmitSignal(SignalName.PointListButtonDown, point);
    }
    #endregion
}
