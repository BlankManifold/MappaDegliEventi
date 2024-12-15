using Godot;

public partial class Main : Node
{
    private Node _currentScene;
    private MarginContainer _currentSceneContainer;

    public override void _Ready()
    {
        Handlers.SaveLoadHandler.CreateMainConfig();
        Handlers.SaveLoadHandler.LoadMainConfig();
        Handlers.SaveLoadHandler.CreateSaveMapDir();

        Handlers.SaveLoadHandler.LoadMapGalleryData();

        _currentSceneContainer = GetNode<MarginContainer>("%CurrentSceneContainer");

        _GoToMainMenu();
    }

    #region Private methods
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
    private void _GoToMainMenu()
    {
        _RemoveCurrentScene();
        MainMenu mainMenuScene = Globals.PackedScenes.MainMenu.Instantiate<MainMenu>();
        mainMenuScene.ButtonDown += OnMainMenuButtonDown;
        _AddNewScene(mainMenuScene);
    }
    private void _GoToMap(string identifier = null)
    {
        _RemoveCurrentScene();
        MappaUI mappaUIScene = Globals.PackedScenes.MappaUI.Instantiate<MappaUI>();
        mappaUIScene.GoBackButtonDown += OnMappaUIGoBackButtonDown;

        if (identifier != null)
        {
            MapPlotRes mapPlotRes = Handlers.SaveLoadHandler.LoadMapPlot(identifier);
            mappaUIScene.Init(mapPlotRes);
        }

        _AddNewScene(mappaUIScene);
    }
    private void _GoToMapsGallery()
    {
        _RemoveCurrentScene();
        Globals.PackedScenes.MapsGallery.Instantiate<MapsGallery>();
        MapsGallery mapsGalleryScene = Globals.PackedScenes.MapsGallery.Instantiate<MapsGallery>();
        mapsGalleryScene.MapSelected += OnMapsGalleryMapSelected;
        mapsGalleryScene.GoBackButtonDown += OnMapsGalleryGoBackButtonDown;
        _AddNewScene(mapsGalleryScene);
    }
    #endregion

    #region Response to signals
    public void OnMainMenuButtonDown(string name)
    {
        switch (name)
        {
            case "NewMapPlotButton":
                _GoToMap(null);
                break;
            case "MapGalleryButton":
                _GoToMapsGallery();
                break;
            default:
                break;
        }
    }
    public void OnMapsGalleryMapSelected(string identifier)
    {
        _GoToMap(identifier);
    }
    public void OnMapsGalleryGoBackButtonDown()
    {
        _GoToMainMenu();
    }
    public void OnMappaUIGoBackButtonDown()
    {
        _GoToMainMenu();
    }
    #endregion
}
