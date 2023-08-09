#if ADVANCED_SCENE_MANAGER

using AdvancedSceneManager.Models;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private SceneCollection nextLevel;
    public void StartHost()
    {
        ConnectionManager.Instance.StartHost();
    }
    public void Join()
    {
        ConnectionManager.Instance.StartClient();
    }
    public void changelevel()
    {
        nextLevel.Open();
    }

}

#endif