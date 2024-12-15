using Godot;
using System;

public partial class MapPlotRes : Resource
{
    [Export] public string Identifier;
    [Export] public string MapName;
    [Export] public Godot.Collections.Array<PointInfoRes> PointInfoList;

    public MapPlotRes()
    {
        PointInfoList = [];
        Identifier = $"{Convert.ToString(0, 16)}";
        MapName = "";
    }

}
