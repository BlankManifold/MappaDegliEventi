using Godot;

public partial class PointInfo : Resource
	{
		[Export] public int id;
		[Export] public string name;
		[Export] public int X;
		[Export] public int Y;
		[Export] public string description;
		public PointInfo()
		{
			id = 0; 
			name = "";
			X = 0;
			Y = 0;
			description = "";
		}
		public PointInfo(int id_, string name_, int X_, int Y_, string description_)
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
