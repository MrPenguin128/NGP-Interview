using SaveAndLoad;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button loadBT;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadBT.interactable = SaveSystem.FileExists;
    }

    public void NewGame()
    {
        SceneLoader.Instance.LoadLevel(SceneLoader.GAMEPLAY_SCENE);
    }
    public void Load()
    {
        SaveManager.Instance.Load();
    }
}
