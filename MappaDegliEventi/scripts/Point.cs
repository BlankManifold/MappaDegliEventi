using Godot;

public partial class Point : Control
{
    private PointInfo _info;
    public PointInfo Info
    {
        get { return _info; }
    }
    private VBoxContainer _popupInfo;

    [Signal]
    public delegate void HoveringEventHandler(Point point, bool on);
    [Signal]
    public delegate void SelectedEventHandler(Point point, bool pressed);

    public void Init(PointInfo info)
    {
        _info = info;
        GetNode<Label>("%Name").Text = info.name;
        GetNode<Label>("%IdLabel").Text = info.id.ToString();
    }
    public void Update(PointInfo info, Vector2 position)
    {
        Position = position;
        _info = info;
        AddToGroup("points");
        GetNode<Label>("%Name").Text = info.name;
        GetNode<Label>("%IdLabel").Text = info.id.ToString();
    }
    public override void _Ready()
    {
        if (_info == null)
            _info = new PointInfo(0, "", 0, 0, "");
        _popupInfo = GetNode<VBoxContainer>("%PopUpInfo");
        _popupInfo.Visible = false;
        AddToGroup("points");
    }

    public void UpdateSelection(bool select)
    {
        if (select)
        {
            GetNode<ColorRect>("ColorRect").Modulate = new Color(1, 0, 0, 1);
        }
        else
        {
            GetNode<ColorRect>("ColorRect").Modulate = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateHovering(bool hovering)
    {
        _popupInfo.Visible = hovering;
    }

    public void _on_mouse_entered()
    {
       EmitSignal(SignalName.Hovering, this, true);
    }
    public void _on_mouse_exited()
    {
       EmitSignal(SignalName.Hovering, this, false);
    }
}
