using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : Singleton<SceneLoader>
{
    //------------------------------------
    //          PREMADE ASSET
    //------------------------------------
    #region Constants
    public const string GAMEPLAY_SCENE = "Gameplay";
    public const string MainMenu_SCENE = "MainMenu";
    #endregion
    public bool IsLoading { get; private set; } = false;

    public void LoadLevel(int sceneIndex) => LoadLevel(SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    public void LoadLevel(string sceneName)
    {
        if (IsLoading) return;
        LoadAsync(sceneName);
    }
    public void LoadAdditiveScene(int sceneIndex) => LoadAdditiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    public void LoadAdditiveScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void UnloadAdditiveScenes()
    {
        if (SceneManager.sceneCount > 1)
        {
            for (int i = 1; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).isSubScene)
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }
    }

    public void RestartLevel()
    {
        if (IsLoading) return;
        IsLoading = true;
        LoadAsync(SceneManager.GetActiveScene().name);
    }

    public void CloseGame()
    {
        Debug.Log("bye :D");
        Application.Quit();
    }

    //Shows the async loading process via the referenced slide, then load the scene
    void LoadAsync(string sceneName)
    {
        IsLoading = true;
        Time.timeScale = 1;
        UnloadAdditiveScenes();
        SceneManager.LoadSceneAsync(sceneName);
    }
}
