using Godot;

public partial class Main : Node
{
    private Node _currentScene;
	private MarginContainer _currentSceneContainer;
    public override void _Ready()
    {
        Handlers.SaveLoadHandler.LoadMapGalleryData();
        
		_currentSceneContainer = GetNode<MarginContainer>("%CurrentSceneContainer");
        
		MainMenu mainMenuScene = Globals.PackedScenes.MainMenu.Instantiate<MainMenu>();
		mainMenuScene.ButtonDown += OnMainMenuButtonDown;
		_AddNewScene(mainMenuScene);
    }

    private void _RemoveCurrentScene()
    {
		if (_currentScene != null)
		{
    	    _currentSceneContainer.RemoveChild(_currentScene);
	        _currentScene.QueueFree();
		}
    }
    private void _AddNewScene(Node newScene)
    {
        _currentScene = newScene;
        _currentSceneContainer.AddChild(_currentScene);
    }

    public void OnMainMenuButtonDown(string name)
    {
        switch (name)
        {
            case "NewMapPlotButton":
                _RemoveCurrentScene();
                Globals.PackedScenes.MappaUI.Instantiate<MappaUI>();
                MappaUI mappaUIScene = Globals.PackedScenes.MappaUI.Instantiate<MappaUI>();
                mappaUIScene.GoBackButtonDown += OnMappaUIGoBackButtonDown;
                _AddNewScene(mappaUIScene);
                break;
            case "MapGalleryButton":
                _RemoveCurrentScene();
                Globals.PackedScenes.MapsGallery.Instantiate<MapsGallery>();
                MapsGallery mapsGalleryScene = Globals.PackedScenes.MapsGallery.Instantiate<MapsGallery>();
                mapsGalleryScene.MapSelected += OnMapsGalleryMapSelected;
                _AddNewScene(mapsGalleryScene);
                break;
            default:
                break;
        }
    }
    public void OnMapsGalleryMapSelected(string identifier)
    {
		MapPlotRes mapPlotRes = Handlers.SaveLoadHandler.LoadMapPlot(identifier);
		_RemoveCurrentScene();
        MappaUI mappaUIScene = Globals.PackedScenes.MappaUI.Instantiate<MappaUI>();
		mappaUIScene.Init(mapPlotRes);
		mappaUIScene.GoBackButtonDown += OnMappaUIGoBackButtonDown;
		_AddNewScene(mappaUIScene);
    }
    public void OnMappaUIGoBackButtonDown()
    {
		_RemoveCurrentScene();
        MainMenu mainMenuScene = Globals.PackedScenes.MainMenu.Instantiate<MainMenu>();
		mainMenuScene.ButtonDown += OnMainMenuButtonDown;
		_AddNewScene(mainMenuScene);
    }
}
