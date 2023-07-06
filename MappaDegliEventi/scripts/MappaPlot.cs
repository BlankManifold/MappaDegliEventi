using Godot;
using System;

public partial class MappaPlot : Control
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
	private Node2D _GhostPoints; 
	private Vector2I _x_ticks_padding = new Vector2I(5,1);
	private Vector2I _y_ticks_padding = new Vector2I(5,0);
	private Vector2 _origin;
	private int _num_of_lines;
	private Vector2 _lines_spacing;
	private GhostPoint _selected_ghost_point;

	[Signal]
	public delegate void GhostPointButtonDownEventHandler(Vector2I coords, uint id);
	[Signal]
	public delegate void AddedPointEventHandler(Point point);

	public override void _Ready()
	{
		_XLines = GetNode<Node2D>("%XLines");
		_YLines = GetNode<Node2D>("%YLines");
		_XTicks = GetNode<Node2D>("%XTicks");
		_YTicks = GetNode<Node2D>("%YTicks");
		_Points = GetNode<Node2D>("%Points");
		_GhostPoints = GetNode<Node2D>("%GhostPoints");
		PackedScene ghost_point_scene = Globals.PackedScenes.GhostPoint;


		_num_of_lines = 2*_max_value + 1;
		_origin = GetRect().Size/2;
		
		_lines_spacing = GetRect().Size / (_num_of_lines+1);

		for (int i = 1; i <= _num_of_lines; i++)
		{
			Vector2 pos = i*_lines_spacing;

			Vector2 x_A = new Vector2(pos.X, 0);
			Vector2 x_B = new Vector2(pos.X, GetRect().Size.Y);
			Vector2 y_A = new Vector2(0, pos.Y);
			Vector2 y_B = new Vector2(GetRect().Size.X, pos.Y);

			_XLines.AddChild(_CreateLine(i, x_A, x_B));
			_YLines.AddChild(_CreateLine(i, y_A, y_B));

			Label x_tick = _CreateTick((i-1-_max_value).ToString(),new Vector2(pos.X, _origin.Y) + _x_ticks_padding);
			Label y_tick = _CreateTick((_max_value-i+1).ToString(),new Vector2(_origin.X, pos.Y) + _y_ticks_padding);

			_XTicks.AddChild(x_tick);
			_YTicks.AddChild(y_tick);

			if (i == _max_value+1)
			{
				x_tick.Visible = false;
				y_tick.Visible = false;
			}

			for (int j = 1; j <= _num_of_lines; j++)
			{
				float shift_Y = j*_lines_spacing.Y;
				GhostPoint ghost_point = ghost_point_scene.Instantiate<GhostPoint>();
				Vector2I ghost_coords = new Vector2I(i-1-_max_value,_max_value-j+1);
				// FIXME min tra linespacing e misura std, ma sempre quadrato o simmetrico
				Vector2 ghost_size = new Vector2(20,20);
				Vector2 ghost_pos = new Vector2(pos.X, shift_Y) - ghost_size/2;

				ghost_point.Init(ghost_pos, ghost_coords, ghost_size);
				_GhostPoints.AddChild(ghost_point);
				ghost_point.GhostPointButtonDown += OnGhostPointButtonDown;
			}
		}
	}

	private Label _CreateTick(string text, Vector2 position)
	{
		Label tick = new Label();

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
		Line2D line = new Line2D();

		line.Width = 1;
		float alpha = 0.5f;

		if (i == _max_value+1)
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
		Vector2 pos = new Vector2I(x,-y)*_lines_spacing + _origin;
		return pos;
	}

	public void OnGhostPointButtonDown(GhostPoint ghost)
	{
		if (_selected_ghost_point != null)
			_selected_ghost_point.Deselect();
		_selected_ghost_point = ghost;
		EmitSignal(SignalName.GhostPointButtonDown, ghost.Coords,(uint)_Points.GetChildCount()+1);
	}
	public void _on_resized()
	{
		Vector2 old_lines_spacing = _lines_spacing;
		_lines_spacing = GetRect().Size / (_num_of_lines+1);
		Vector2 affine_factor = _lines_spacing/old_lines_spacing;

		_origin =  GetRect().Size / 2;

		for (int i = 1; i <= _num_of_lines; i++)
		{
			Line2D x_line = _XLines.GetChild<Line2D>(i-1);
			Line2D y_line = _YLines.GetChild<Line2D>(i-1);
			Label x_tick = _XTicks.GetChild<Label>(i-1);
			Label y_tick = _YTicks.GetChild<Label>(i-1);

			Vector2 coords = i*_lines_spacing;
			
			Vector2 x_A = new Vector2(coords.X, 0);
			Vector2 x_B = new Vector2(coords.X, GetRect().Size.Y);
			Vector2 y_A = new Vector2(0, coords.Y);
			Vector2 y_B = new Vector2(GetRect().Size.X, coords.Y);
						
			x_line.Points = new Vector2[] {x_A, x_B};
			y_line.Points = new Vector2[] {y_A, y_B};

			x_tick.Position = new Vector2(coords.X, _origin.Y) + _x_ticks_padding;
			y_tick.Position = new Vector2(_origin.X, coords.Y) + _y_ticks_padding;
		}
		
		foreach (Point point in _Points.GetChildren())
			point.Position = affine_factor*(point.Position+point.Size/2)-point.Size/2;

		foreach (GhostPoint ghost_point in _GhostPoints.GetChildren())
			ghost_point.Position = affine_factor*(ghost_point.Position+ghost_point.Size/2)-ghost_point.Size/2;
	}
	public void _on_information_box_added_point(Globals.PointInfo info)
	{
		if (_selected_ghost_point != null)
		{
			_selected_ghost_point.Deselect();
			_selected_ghost_point.QueueFree();
			_selected_ghost_point = null;
		}

		PackedScene point_scene = Globals.PackedScenes.Point;
		Point point = point_scene.Instantiate<Point>();

		point.Position = _CoordsToPos(info.X,info.Y) - point.Size/2;
		point.Init(info);

		_Points.AddChild(point);
		EmitSignal(SignalName.AddedPoint, point);
	}
	
	public void _on_information_box_removed_point(Point point)
	{
		int id = (int)point.Info.id;
		point.QueueFree();

		for (int i = id; i < _Points.GetChildCount(); i++)
		{
			Point p = _Points.GetChild<Point>(i);
			p.Info.id -= 1;
			p.Init(p.Info);
		}
	}
	
	public void _on_information_box_modified_point(Point point, Globals.PointInfo info)
	{
		point.Init(info); 
	}

	// public void _on_add_point_popup_exit_add_point_pop_up()
	// {
	// 	if (_selected_ghost_point != null)
	// 	{
	// 		_selected_ghost_point.Deselect();
	// 		_selected_ghost_point = null;
	// 	}
	// }
}
