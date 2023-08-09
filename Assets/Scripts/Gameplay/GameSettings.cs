#if ADVANCED_SCENE_MANAGER
using AdvancedSceneManager.Models;
using UnityEngine;

public class GameSettings : Singleton<GameSettings>
{
    public GameMode GameMode;

    // Used to select next level to load.
    public SceneCollection LoadLevel;
}
#endif