using Godot;

public partial class InformationBox : Button
{
    public enum ButtonsState { None, Add, Modify }
    private Label _idLabel;
    private LineEdit _nameLabel;
    private LineEdit _dateLabel;
    private SpinBox _intensitySpinBox;
    private SpinBox _impactSpinBox;
    private TextEdit _descriptionLabel;
    private VBoxContainer _informationContainer;

    private Button _addButton;
    private Button _removeButton;
    private Button _modifyButton;
    private PointInfo _info;

    private ButtonsState _state = ButtonsState.Add;
    public ButtonsState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (_state)
            {
                case ButtonsState.Add:
                    _addButton.Disabled = false;
                    _modifyButton.Disabled = true;
                    _removeButton.Disabled = true;
                    break;
                case ButtonsState.Modify:
                    _addButton.Disabled = true;
                    _modifyButton.Disabled = false;
                    _removeButton.Disabled = false;
                    break;
                case ButtonsState.None:
                    _addButton.Disabled = true;
                    _modifyButton.Disabled = true;
                    _removeButton.Disabled = true;
                    break;
            }
        }
    }


    [Signal]
    public delegate void AddedPointEventHandler(PointInfo pointInfo);
    [Signal]
    public delegate void ModifiedPointEventHandler(PointInfo info);
    [Signal]
    public delegate void RemovedPointEventHandler();

    public override void _Ready()
    {
        _idLabel = GetNode<Label>("%IdLabel");
        _nameLabel = GetNode<LineEdit>("%NameLabel");
        _dateLabel = GetNode<LineEdit>("%DateLabel");
        _intensitySpinBox = GetNode<SpinBox>("%IntensitySpinBox");
        _impactSpinBox = GetNode<SpinBox>("%ImpactSpinBox");
        _descriptionLabel = GetNode<TextEdit>("%DescriptionLabel");
        _informationContainer = GetNode<VBoxContainer>("%InformationContainer");
        _addButton = GetNode<Button>("%AddPoint");
        _modifyButton = GetNode<Button>("%ModifyPoint");
        _removeButton = GetNode<Button>("%RemovePoint");

        _info = new PointInfo(0, "", 0, 0, "");
        _informationContainer.Visible = false;
    }

    public void Update(PointInfo info)
    {
        _info = new PointInfo(info);
        _idLabel.Text = _info.id.ToString();
        _nameLabel.Text = _info.name;
        _impactSpinBox.Value = _info.X;
        _intensitySpinBox.Value = _info.Y;
        _descriptionLabel.Text = _info.description;
    }
    public void Update(Vector2I coords, int id)
    {
        _info.Clear();

        _info.id = id;
        _info.X = coords.X;
        _info.Y = coords.Y;

        _idLabel.Text = _info.id.ToString();
        _nameLabel.Text = "";
        _impactSpinBox.Value = _info.X;
        _intensitySpinBox.Value = _info.Y;
        _descriptionLabel.Text = "";
    }
    public void Clear()
    {
        _info.Clear();
        _idLabel.Text = "-";
        _nameLabel.Text = "";
        _intensitySpinBox.Value = 0;
        _impactSpinBox.Value = 0;
        _descriptionLabel.Text = "";
        this.State = ButtonsState.Add;
    }

    private void FocusNameLineEdit()
    {
        _nameLabel.GrabFocus();
    }
    public void UpdateHovering(Point point)
    {
        if (point != null)
        {
            Update(point.Info);
            return;
        }

        Clear();
    }
    public void UpdateSelection(Point point)
    {
        ButtonPressed = true;
        this.State = ButtonsState.Modify;
        Update(point.Info);
        return;
    }
    public void ActiveAddPointState(Vector2I coords, int id)
    {
        EmitSignal(SignalName.Toggled, true);
        this.State = ButtonsState.Add;
        Update(coords, id);
    }

    public void _on_add_point_button_down()
    {
        _info.id = _idLabel.Text.ToInt();
        _info.name = _nameLabel.Text;
        _info.X = (int)_impactSpinBox.Value;
        _info.Y = (int)_intensitySpinBox.Value;
        _info.description = _descriptionLabel.Text;

        this.State = ButtonsState.Modify;
        CallDeferred(MethodName.FocusNameLineEdit);
        EmitSignal(SignalName.AddedPoint, new PointInfo(_info));
    }
    public void _on_modify_point_button_down()
    {
        _info.id = _idLabel.Text.ToInt();
        _info.name = _nameLabel.Text;
        _info.X = (int)_impactSpinBox.Value;
        _info.Y = (int)_intensitySpinBox.Value;
        _info.description = _descriptionLabel.Text;

        EmitSignal(SignalName.ModifiedPoint, new PointInfo(_info));
        this.State = ButtonsState.Modify;
    }
    public void _on_remove_point_button_down()
    {
        EmitSignal(SignalName.RemovedPoint);
    }
    public void _on_toggled(bool pressed)
    {
        _informationContainer.Visible = pressed;
        CallDeferred(MethodName.FocusNameLineEdit);
    }

}
