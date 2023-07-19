using Godot;

public partial class ChangeColorContainer : HBoxContainer
{
	[Signal]
	public delegate void ChangedColorEventHandler(Color color, bool allPoints = false);
	[Signal]
	public delegate void ColorPickerShowEventHandler(bool hide=false);

	private Window _colorPickerWindow;
	private ColorRect _recentColorRect;
	private ColorPicker _colorPicker;
	private bool _enabled = false;
	public bool Enabled { get {return _enabled;} set {_enabled = value;} }

	public override void _Ready()
	{
		_colorPickerWindow = GetNode<Window>("%ColorPickerWindow");
		_colorPicker = GetNode<ColorPicker>("%ColorPicker");
		_recentColorRect = GetNode<ColorRect>("%RecentColorRect");
	}
	
	public void _on_recent_color_button_button_down()
	{
		if (_enabled)
			EmitSignal(SignalName.ChangedColor, _recentColorRect.Color, false);
	}
	public void _on_color_picker_button_button_down()
	{
		if (!_colorPickerWindow.Visible)
		{
			_colorPickerWindow.Show();
			EmitSignal(SignalName.ColorPickerShow, false);
		}
		else
		{
			_colorPickerWindow.EmitSignal(Window.SignalName.CloseRequested);
		}
	}

	public void _on_change_all_color_button_button_down()
	{
		EmitSignal(SignalName.ChangedColor, _colorPicker.Color, true);
	}

	public void _on_color_picker_color_changed(Color color)
	{
		_recentColorRect.Color = color;
		
		if (_enabled)
			EmitSignal(SignalName.ChangedColor, color, false);
	}

	public void _on_color_picker_window_close_requested()
	{
		_colorPickerWindow.Hide();
		EmitSignal(SignalName.ColorPickerShow, true);
	}

}
