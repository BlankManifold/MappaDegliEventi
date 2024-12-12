using Godot;

public partial class GhostPoint : Button
{
	private ColorRect _colorRect;
	private float _focused_alpha = 0.5f;
	private Vector2I _coords;
	public Vector2I Coords { get { return _coords; } }
	private bool _selected = false;

	[Signal]
	public delegate void GhostPointButtonDownEventHandler(GhostPoint ghost);


	public void Init(Vector2 position, Vector2I coords, Vector2 size)
	{
		Size = size;
		Position = position;
		_coords = coords;
	}
	public override void _Ready()
	{
		_colorRect = GetNode<ColorRect>("%ColorRect");
		_colorRect.Color = new Color(_colorRect.Color, 0f);
	}

	public void Deselect()
	{
		_selected = false;
		_colorRect.Color = new Color(_colorRect.Color, 0f);
	}
	public void _on_mouse_entered()
	{
		if (!_selected)
			_colorRect.Color = new Color(_colorRect.Color, _focused_alpha);
	}
	public void _on_mouse_exited()
	{
		if (!_selected)
			_colorRect.Color = new Color(_colorRect.Color, 0f);
	}

	public void _on_button_down()
	{
		if (_selected)
		{
			Deselect();
			return;
		}

		_selected = true;
		EmitSignal(SignalName.GhostPointButtonDown, this);
	}

}
