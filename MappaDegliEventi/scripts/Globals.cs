using Godot;
using System.Collections.Generic;


namespace Globals
{
    public struct GameInfo
    {
        public static string Version = "0.1"; 
    }
    public static class MapGalleryData
    {
        public static Dictionary<string, Dictionary<string, string>> MapsDict = new Dictionary<string, Dictionary<string, string>> {};
        public static void Add(MapPlotRes mapPlotRes)
        {
            MapsDict[mapPlotRes.Identifier] = new Dictionary<string, string> 
            {
                ["name"]= mapPlotRes.MapName,
                ["album"]= "",
            };
        }
    }
    public static class PackedScenes
    {
        public static PackedScene Point = (PackedScene)ResourceLoader.Load("res://scenes/Point.tscn");
        public static PackedScene PointListButton = (PackedScene)ResourceLoader.Load("res://scenes/PointListButton.tscn");
        public static PackedScene GhostPoint = (PackedScene)ResourceLoader.Load("res://scenes/GhostPoint.tscn");
        public static PackedScene MappaUI = (PackedScene)ResourceLoader.Load("res://scenes/MappaUI.tscn");
        public static PackedScene MapsGallery = (PackedScene)ResourceLoader.Load("res://scenes/MapsGallery.tscn");
        public static PackedScene GalleryMapButton = (PackedScene)ResourceLoader.Load("res://scenes/GalleryMapButton.tscn");
        public static PackedScene MainMenu = (PackedScene)ResourceLoader.Load("res://scenes/MainMenu.tscn");
    }
    public struct Paths
    {
        public static string SaveMappaPlot = "user://maps";
        // public static string MappaPlotResScript = "res://scripts/MapPlotRes.cs";
    }
}