using Godot;
using System.Collections.Generic;


namespace Globals
{
    public readonly struct GameInfo
    {
        public static readonly string Version = "0.1";
    }
    public static class MapGalleryData
    {
        public static int CurrentIdenfier;
        public static Dictionary<string, Dictionary<string, string>> MapsDict = new Dictionary<string, Dictionary<string, string>> { };
        public static void Add(MapPlotRes mapPlotRes)
        {
            MapsDict[mapPlotRes.Identifier] = new Dictionary<string, string>
            {
                ["name"] = mapPlotRes.MapName,
                ["album"] = "",
            };
        }
        public static void Remove(string identifier)
        {
            MapsDict.Remove(identifier);
        }
    }
    public static class PackedScenes
    {
        public readonly static PackedScene Point = (PackedScene)ResourceLoader.Load("res://scenes/Point.tscn");
        public readonly static PackedScene PointListButton = (PackedScene)ResourceLoader.Load("res://scenes/PointListButton.tscn");
        public readonly static PackedScene GhostPoint = (PackedScene)ResourceLoader.Load("res://scenes/GhostPoint.tscn");
        public readonly static PackedScene MapUI = (PackedScene)ResourceLoader.Load("res://scenes/MapUI.tscn");
        public readonly static PackedScene MapsGallery = (PackedScene)ResourceLoader.Load("res://scenes/MapsGallery.tscn");
        public readonly static PackedScene GalleryMapButton = (PackedScene)ResourceLoader.Load("res://scenes/GalleryMapButton.tscn");
        public readonly static PackedScene MainMenu = (PackedScene)ResourceLoader.Load("res://scenes/MainMenu.tscn");
    }
    public static class Paths
    {
        public readonly static string SaveMapPlot = "user://maps";
        public readonly static string SaveConfigs = "user://configs";
    }
}