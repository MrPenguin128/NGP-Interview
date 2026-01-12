using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveAndLoad;
using InventorySystem;

public class SaveManager : MonoBehaviourSingletonPersistent<SaveManager>
{
    public InventoryData inventoryData;

    public void NewGame()
    {
        inventoryData = null;
    }
    public void Save()
    {
        SaveSystem.Save(GameManager.Player.Inventory.Data);
    }
    public bool Load()
    {
        if (SaveSystem.FileExists)
        {
            GameManager.Player.Inventory.LoadData(SaveSystem.Load<InventoryData>());
            return true;
        }
        return false;
    }
}
