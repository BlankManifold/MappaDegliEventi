using Godot;

public partial class AddPointPopup : Control
{	
	private Globals.PointInfo _info = new Globals.PointInfo(0,"",0,0,"");
	private LineEdit _name;
	private TextEdit _description;
	private SpinBox _intensity;
	private SpinBox _impact;


	[Signal]
	public delegate void AddPointEventHandler(Globals.PointInfo info);
	[Signal]
	public delegate void ExitAddPointPopUpEventHandler();

    public override void _Ready()
    {
		_name = GetNode<LineEdit>("%NameLineEdit");
		_description = GetNode<TextEdit>("%DescriptionTextEdit");
		_impact = GetNode<SpinBox>("%ImpactSpinBox");
		_intensity = GetNode<SpinBox>("%IntensitySpinBox");
    }

	public void _on_exit_button_button_down()
	{
		this.Visible = false;
		EmitSignal(SignalName.ExitAddPointPopUp);
	}
	
}
