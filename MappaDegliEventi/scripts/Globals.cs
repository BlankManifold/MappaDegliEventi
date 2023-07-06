using Godot;

namespace Globals
{
    public struct GameInfo
    {
        public static string Version = "0.1"; 
    }
    public static class PackedScenes
    {
        public static PackedScene Point = (PackedScene)ResourceLoader.Load("res://scenes/Point.tscn");
        public static PackedScene GhostPoint = (PackedScene)ResourceLoader.Load("res://scenes/GhostPoint.tscn");
        public static PackedScene MappaUI = (PackedScene)ResourceLoader.Load("res://scenes/MappaUI.tscn");
    }

    public partial class PointInfo : GodotObject
	{
		public uint id;
		public string name;
		public int X;
		public int Y;
		public string description;

		public PointInfo(uint id_, string name_, int X_, int Y_, string description_)
		{
			id = id_; 
			name = name_;
			X = X_;
			Y = Y_;
			description = description_;
		}
		public PointInfo(PointInfo info)
		{
			id = info.id; 
			name = info.name;
			X = info.X;
			Y = info.Y;
			description = info.description;
		}
		public void Clear()
		{
			id = 0; 
			name = "";
			X = 0;
			Y = 0;
			description = "";
		}
	}
}