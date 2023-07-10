using Godot;
using System.Collections.Generic;

public partial class MapsGallery : Control
{
	private GridContainer _gridContainer;

	[Signal]
	public delegate void MapSelectedEventHandler(string identifier);
	public override void _Ready()
	{
		_gridContainer = GetNode<GridContainer>("%GridContainer");
		_CreateGalleryMapButtons();
	}

	private void _CreateGalleryMapButtons()
	{
		foreach(KeyValuePair<string,Dictionary<string,string>> entry in Globals.MapGalleryData.MapsDict)
		{
			GalleryMapButton galleryMapButton = Globals.PackedScenes.GalleryMapButton.Instantiate<GalleryMapButton>();
			galleryMapButton.Init(entry.Key, entry.Value["name"]);
			_gridContainer.AddChild(galleryMapButton);
			galleryMapButton.Selected += OnSelected;
		}
	}

	public void OnSelected(string identifier)
	{
		EmitSignal(SignalName.MapSelected, identifier);
	}
}
