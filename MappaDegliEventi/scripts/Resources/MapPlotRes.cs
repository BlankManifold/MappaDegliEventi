using Godot;
using System;

public partial class MapPlotRes : Resource
{
    [Export] public string Identifier;
    [Export] public string MapName;
    [Export] public Godot.Collections.Array<PointInfo> PointInfoList;

    public MapPlotRes() 
    {
        PointInfoList = new Godot.Collections.Array<PointInfo> {};     
        Identifier = $"{Convert.ToString(0,16)}";   
        MapName = "";   
    }

}
