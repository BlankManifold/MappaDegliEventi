using Godot;
using System;

public partial class Point : Control
{
    private PointInfoRes _info;
    public PointInfoRes Info
    {
        get { return _info; }
    }
    private VBoxContainer _popupInfo;
    private ColorRect _multiFlagRect;
    private bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            Modulate = new Color(_info.Color, 0.75f + Convert.ToInt32(value) * 0.25f);
        }
    }
    private bool _multiFlag = false;
    public bool MultiFlag
    {
        get { return _multiFlag; }
        set
        {
            _multiFlag = value;
            _multiFlagRect.Visible = value;
        }
    }


    [Signal]
    public delegate void HoveringEventHandler(Point point, bool on);

    public void Init(PointInfoRes info)
    {
        _info = info;
        GetNode<Label>("%Name").Text = info.Name;
        GetNode<Label>("%IdLabel").Text = info.Id.ToString();
        Modulate = new Color(_info.Color, 0.75f + Convert.ToInt32(_selected) * 0.25f);
    }
    public void Update(PointInfoRes info, Vector2 position)
    {
        Position = position;
        _info = info;
        AddToGroup("points");
        GetNode<Label>("%Name").Text = info.Name;
        GetNode<Label>("%IdLabel").Text = info.Id.ToString();
    }
    public override void _Ready()
    {
        if (_info == null)
            _info = new PointInfoRes();
        _popupInfo = GetNode<VBoxContainer>("%PopUpInfo");
        _popupInfo.Visible = false;
        AddToGroup("points");

        _multiFlagRect = GetNode<ColorRect>("%MultiFlagRect");

        this.Selected = false;
    }

    public void UpdateSelection(bool select)
    {
        this.Selected = select;
    }
    public void UpdateHovering(bool hovering)
    {
        _popupInfo.Visible = hovering;
    }
    public void ChangeColor(Color color)
    {
        _info.Color = color;
        Modulate = new Color(_info.Color, 0.75f + Convert.ToInt32(_selected) * 0.25f);
    }
    public void ChangeZIndex(int zIndex)
    {
        ZIndex = zIndex;
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
