using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveAndLoad;
using InventorySystem;
using UI;
using WaveSystem;

public class SaveManager : MonoBehaviourSingletonPersistent<SaveManager>
{
    public void Save()
    {
        SaveSystem.Save(GameManager.Player.Inventory.Data, WaveManager.Instance.GetData());
        HUDManager.Instance?.OnSave();
    }
    public bool Load()
    {
        if (SaveSystem.FileExists)
        {
            var carrier = DataCarrier.CreateDataCarrier(SaveSystem.Load());
            carrier.HoldDataWithArg((scene, loadMode) =>
            {
                GameManager.Instance.LoadData(carrier.arg as List<object>);
            });
            SceneLoader.Instance.LoadLevel(SceneLoader.GAMEPLAY_SCENE);
            return true;
        }
        return false;
    }
}
