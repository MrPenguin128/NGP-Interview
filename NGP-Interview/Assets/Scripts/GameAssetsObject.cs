using InventorySystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Assets", menuName = "Scriptable Objects/GameAssets")]
public class GameAssetsObject : ScriptableObject
{
    #region Singleton
    const string PATH = "Databases/Game Assets";
    static GameAssetsObject _instance;
    public static GameAssetsObject Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<GameAssetsObject>(PATH);
            return _instance;
        }
    }
    #endregion

    public DamagePopup DamagePopupPrefab;
}
