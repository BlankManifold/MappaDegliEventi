using Godot;

public partial class PointListButton : Button
{
    private int _pointId;
    public int PointId
    {
        get { return _pointId; }
        set
        {
            Text = $"{value.ToString()}. {_pointName}";
            _pointId = value;
        }
    }
    private string _pointName;
    public string PointName
    {
        get { return _pointName; }
        set
        {
            Text = $"{_pointId.ToString()}. {value}";
            _pointName = value;
        }
    }
}
