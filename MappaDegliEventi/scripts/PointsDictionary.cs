using Godot;
using System.Collections.Generic;

public struct PointsDictionary
{
    private Dictionary<Vector2I, List<Point>> _dict;

    public void Add(Vector2I coords, Point point)
    {
        if (_dict.ContainsKey(coords))
        {
            foreach (Point p in _dict[coords])
            {
                p.MultiFlag = true;
                p.Visible = false;
            }
            _dict[coords].Add(point);

            point.MultiFlag = true;
            point.Visible = true;

            return;
        }

        _dict[coords] = new List<Point>() {point};
        point.MultiFlag = false;
    }

    public void Remove(Vector2I coords, Point point)
    {
        _dict[coords].Remove(point);

        if (_dict[coords].Count >= 1)
        {
            _dict[coords][_dict[coords].Count - 1].Visible = true;
        }

        if (_dict[coords].Count == 1)
        {
            _dict[coords][0].MultiFlag = false;
            return;
        }

        if (_dict[coords].Count == 0)
        {
            _dict.Remove(coords);
            return;
        }
    }
    public void Remove(Point point)
    {
        Vector2I coords = new Vector2I(point.Info.X, point.Info.Y);
        Remove(coords, point);
    }

    public void Move(Vector2I from, Vector2I to, Point point)
    {
        Remove(from, point);
        Add(to, point);
    }

    public bool HasMulti(Vector2I coords)
    {
        if (!_dict.ContainsKey(coords))
            return false;

        return (_dict[coords].Count > 1);
    }
    public bool HasPoint(Vector2I coords)
    {
        return _dict.ContainsKey(coords);
    }
    public void Clear()
    {
        _dict = new Dictionary<Vector2I, List<Point>> {};
    }
    public void UpdateNextPointVisibility(Vector2I coords, bool visible)
    {
        _dict[coords][_dict[coords].Count - 2].Visible = visible;
    }
    public void UpdateMultiPointsOrder(Vector2I coords, Point point)
    {
        _dict[coords].Remove(point);
        _dict[coords].Add(point);
    }
    public Point GoToNextMultiPoint(Vector2I coords, bool inverse=false)
    {
        if (!inverse)
        {
            Point lastPoint = _dict[coords][_dict[coords].Count-1];
            _dict[coords].Remove(lastPoint);
            _dict[coords].Insert(0,lastPoint);
        }
        else
        {
            Point firstPoint = _dict[coords][0];
            _dict[coords].Remove(firstPoint);
            _dict[coords].Add(firstPoint);
        }

        return _dict[coords][_dict[coords].Count-1];
    }
    public PointsDictionary()
    {
        _dict = new Dictionary<Vector2I, List<Point>> {};
    }
}