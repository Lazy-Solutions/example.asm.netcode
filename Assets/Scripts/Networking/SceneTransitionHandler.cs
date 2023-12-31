#if ADVANCED_SCENE_MANAGER

using System.Collections;
using AdvancedSceneManager.Models;
using AdvancedSceneManager.Utility;
using Lazy.Utility;
using UnityEngine;

public class SceneTransitionHandler : Singleton<SceneTransitionHandler>
{
    internal void Transition(IEnumerator loadSequence, Scene loadingScene = null) 
        => PerformTransition(loadSequence, loadingScene).StartCoroutine();

    IEnumerator PerformTransition(IEnumerator loadSequence, Scene loadingScene = null)
    {
        // since client doenst actually load a collection we have to manually load it
        if (loadingScene is not null)
           yield return LoadingScreenUtility.OpenLoadingScreen(loadingScene);

        yield return loadSequence;

        if (loadingScene is not null)
            yield return LoadingScreenUtility.CloseAll();

        // alternativly if you know you will use loading screen everytime
        //LoadingScreenUtility.DoAction(loadingscene, loadSequence, callback);
    }
}
#endif