using Godot;
using System;
using System.Linq;

namespace Handlers
{
	public partial class SaveLoadHandler : Node
	{
		static public void SaveMapPlot(Godot.Collections.Array<Node> points, string name, string identifier = null)
		{
			MapPlotRes mapPlotRes = new()
			{
				MapName = name,
				Identifier = identifier
			};

			if (mapPlotRes.Identifier == null)
			{
				Globals.MapGalleryData.CurrentIdenfier += 1;
				_UpdateLastIdenfierMainConfig();

				mapPlotRes.Identifier = $"{Convert.ToString(Globals.MapGalleryData.CurrentIdenfier, 16)}";
			}

			foreach (Point point in points.Cast<Point>())
			{
				mapPlotRes.PointInfoList.Add(point.Info);
			}
			System.IO.Directory.CreateDirectory(Globals.Paths.SaveMappaPlot);
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot, $"map_{mapPlotRes.Identifier}.tres");
			ResourceSaver.Save(mapPlotRes, file_path);

			Globals.MapGalleryData.Add(mapPlotRes);
		}
		static public MapPlotRes LoadMapPlot(string identifier)
		{
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot, $"map_{identifier}.tres");
			if (!ResourceLoader.Exists(file_path))
			{
				return null;
			}

			MapPlotRes mapPlotRes = ResourceLoader.Load<MapPlotRes>(file_path, cacheMode: ResourceLoader.CacheMode.Ignore);
			return mapPlotRes;
		}
		static public void RemoveMapPlot(string identifier)
		{
			string fname = $"map_{identifier}.tres";
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot, fname);
			if (ResourceLoader.Exists(file_path))
			{
				DirAccess.Open(Globals.Paths.SaveMappaPlot).Remove(fname);
				return;
			}
		}
		static public void LoadMainConfig()
		{
			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new();
			Error err = config.Load(configFilePath);
			if (err != Error.Ok) { return; }

			int lastMapIdenfier = (int)config.GetValue("maps", "last_identifier");
			Globals.MapGalleryData.CurrentIdenfier = lastMapIdenfier + 1;
		}
		static public void CreateMainConfig()
		{
			if (DirAccess.Open(Globals.Paths.SaveConfigs) != null)
			{
				return;
			}
			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.MakeDir("configs");

			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new();

			config.SetValue("maps", "last_identifier", 0);
			config.Save(configFilePath);
		}
		static public void CreateSaveMapDir()
		{
			if (DirAccess.Open(Globals.Paths.SaveMappaPlot) != null)
			{
				return;
			}
			DirAccess dirAccess = DirAccess.Open("user://");
			dirAccess.MakeDir("maps");
		}
		static private void _UpdateLastIdenfierMainConfig()
		{
			string configFilePath = System.IO.Path.Combine(Globals.Paths.SaveConfigs, $"main_config.cfg");
			ConfigFile config = new();

			Error err = config.Load(configFilePath);
			if (err != Error.Ok) { return; }

			config.SetValue("maps", "last_identifier", Globals.MapGalleryData.CurrentIdenfier);
			config.Save(configFilePath);
		}
		static public void LoadMapGalleryData()
		{
			// foreach (string path in System.IO.Directory.GetFiles("/Users/lucastefanelli/Library/Application Support/Godot/app_userdata/MappaDegliEventi/maps"))
			foreach (string path in DirAccess.Open(Globals.Paths.SaveMappaPlot).GetFiles())
			{
				string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot, path);
				MapPlotRes mapPlotRes = ResourceLoader.Load<MapPlotRes>(file_path, cacheMode: ResourceLoader.CacheMode.Ignore);
				Globals.MapGalleryData.Add(mapPlotRes);
			}
		}

	}

}