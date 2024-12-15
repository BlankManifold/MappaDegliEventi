using Godot;

public partial class PointInfoRes : Resource
{
	[Export] public int Id;
	[Export] public string Name;
	[Export] public int X;
	[Export] public int Y;
	[Export] public string Description;
	[Export] public Color Color;
	public PointInfoRes()
	{
		Id = 0;
		Name = "";
		X = 0;
		Y = 0;
		Description = "";
		Color = new Color(1, 1, 1);
	}
	public PointInfoRes(int id, string name, int x, int y, string description, Color? color = null)
	{
		Id = id;
		Name = name;
		X = x;
		Y = y;
		Description = description;
		if (color != null)
			Color = (Color)color;
		else
			Color = new Color(1, 1, 1);
	}
	public PointInfoRes(PointInfoRes info)
	{
		Id = info.Id;
		Name = info.Name;
		X = info.X;
		Y = info.Y;
		Description = info.Description;
		Color = info.Color;
	}
	public void Clear()
	{
		Id = 0;
		Name = "";
		X = 0;
		Y = 0;
		Description = "";
		Color = new Color(1, 1, 1);
	}
}
