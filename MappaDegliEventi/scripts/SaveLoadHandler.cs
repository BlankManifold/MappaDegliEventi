using Godot;
using System;

namespace Handlers
{
    public static class SaveLoadHandler
    {
        static public void SaveMapPlot(Godot.Collections.Array<Node> points, string name, string identifier = null)
        {
			MapPlotRes mapPlotRes = new MapPlotRes();
			mapPlotRes.MapName = name;
			mapPlotRes.Identifier = identifier;

			if (mapPlotRes.Identifier == null)
			{
				mapPlotRes.Identifier = $"{Convert.ToString(0,16)}";
			}
			
			foreach (Point point in points)
			{
				mapPlotRes.PointInfoList.Add(point.Info);
			}
			System.IO.Directory.CreateDirectory(Globals.Paths.SaveMappaPlot);
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot,$"map_{mapPlotRes.Identifier}.tres");
			ResourceSaver.Save(mapPlotRes, file_path);

			Globals.MapGalleryData.Add(mapPlotRes);
        }

        static public MapPlotRes LoadMapPlot(string identifier)
        {
			string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot,$"map_{identifier}.tres");
			if (!ResourceLoader.Exists(file_path))
			{
				return null;	
			}

        	MapPlotRes mapPlotRes = (MapPlotRes)ResourceLoader.Load(file_path, cacheMode:ResourceLoader.CacheMode.Ignore);
			return mapPlotRes;
		}

		static public void LoadMapGalleryData()
    	{
			// foreach (string path in System.IO.Directory.GetFiles("/Users/lucastefanelli/Library/Application Support/Godot/app_userdata/MappaDegliEventi/maps"))
			foreach (string path in DirAccess.Open(Globals.Paths.SaveMappaPlot).GetFiles())
			{
				string file_path = System.IO.Path.Combine(Globals.Paths.SaveMappaPlot,path);
				MapPlotRes mapPlotRes = (MapPlotRes)ResourceLoader.Load(file_path, cacheMode:ResourceLoader.CacheMode.Ignore);
				Globals.MapGalleryData.Add(mapPlotRes);
			}
    	}

    }

}