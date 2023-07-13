using Godot;
using System.Collections.Generic;

public partial class MapsGallery : Control
{
	private GridContainer _gridContainer;
	private PanelContainer _selectionPopUp;
	private Label _selectionPopUpName;
	private GalleryMapIcon _selectedIcon;

	[Signal]
	public delegate void MapSelectedEventHandler(string identifier);
	[Signal]
	public delegate void GoBackButtonDownEventHandler();

	public override void _Ready()
	{
		_gridContainer = GetNode<GridContainer>("%GridContainer");
		_selectionPopUp = GetNode<PanelContainer>("%SelectionPopup");
		_selectionPopUpName = GetNode<Label>("%SelectionPopupName");

		_CreateGalleryMapButtons();
	}

	private void _CreateGalleryMapButtons()
	{
		foreach(KeyValuePair<string,Dictionary<string,string>> entry in Globals.MapGalleryData.MapsDict)
		{
			GalleryMapIcon galleryMapIcon = Globals.PackedScenes.GalleryMapButton.Instantiate<GalleryMapIcon>();
			galleryMapIcon.Init(entry.Key, entry.Value["name"]);
			_gridContainer.AddChild(galleryMapIcon);
			galleryMapIcon.Selected += OnSelected;
		}
	}
	private void _ShowSelectionPopup(string name)
	{
		_selectionPopUp.Visible = true;
		_selectionPopUpName.Text = name;
	}
	private void _HideSelectionPopup()
	{
		_selectedIcon = null;
		_selectionPopUp.Visible = false;
		_selectionPopUpName.Text = "";
	}
	
	
	public void OnSelected(GalleryMapIcon icon)
	{
		_selectedIcon = icon;
		_ShowSelectionPopup(icon.MapName);
	}
	public void _on_load_map_button_button_down()
	{
		string identifier = _selectedIcon.MapIdentifier;
		_HideSelectionPopup();
		EmitSignal(SignalName.MapSelected,identifier);
	}
	public void _on_go_back_button_button_down()
	{
		_HideSelectionPopup();
		EmitSignal(SignalName.GoBackButtonDown);
	}
	public void _on_close_selection_button_button_down()
	{
		_HideSelectionPopup();
	}
	public void _on_delete_map_button_button_down()
	{
		Globals.MapGalleryData.Remove(_selectedIcon.MapIdentifier);
		Handlers.SaveLoadHandler.RemoveMapPlot(_selectedIcon.MapIdentifier);
		_gridContainer.RemoveChild(_selectedIcon);
		_selectedIcon.QueueFree();
		_HideSelectionPopup();
	}
}
