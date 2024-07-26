using AdvancedSceneManager.Models;

public class GameSettings : Singleton<GameSettings>
{
    public GameMode GameMode;

    // Used to select next level to load.
    public SceneCollection LoadLevel;
}
