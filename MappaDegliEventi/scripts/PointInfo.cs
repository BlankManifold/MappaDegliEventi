using Godot;

public partial class PointInfo : Resource
	{
		[Export] public int id;
		[Export] public string name;
		[Export] public int X;
		[Export] public int Y;
		[Export] public string description;
		[Export] public Color color;
		public PointInfo()
		{
			id = 0; 
			name = "";
			X = 0;
			Y = 0;
			description = "";
			color = new Color(1,1,1);
		}
		public PointInfo(int id_, string name_, int X_, int Y_, string description_, Color? color_ = null)
		{
			id = id_; 
			name = name_;
			X = X_;
			Y = Y_;
			description = description_;
			if (color != null)
				color = (Color)color_;
			else
				color = new Color(1,1,1);
		}
		public PointInfo(PointInfo info)
		{
			id = info.id; 
			name = info.name;
			X = info.X;
			Y = info.Y;
			description = info.description;
			color = info.color;
		}
		public void Clear()
		{
			id = 0; 
			name = "";
			X = 0;
			Y = 0;
			description = "";
			color = new Color(1, 1, 1);
		}
	}
