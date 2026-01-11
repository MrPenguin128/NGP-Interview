using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ItemObject", menuName = "Scriptable Objects/Inventory/Items/Item Object")]
    public class ItemObject : ScriptableObject
    {
        public string Id;
        [Header("UI")]
        public string DisplayName;
        public Sprite Icon;

        [Header("Stack Settings")]
        public bool Stackable = true;
        public int MaxStack = 99;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (string.IsNullOrEmpty(Id) || Id != name.ToLower().Replace(" ", "_"))
                    Id = name.ToLower().Replace(" ", "_");
                if (ItemDatabase.Instance != null)
                    ItemDatabase.Instance.AddEntry(this);
            }
        }
#endif
    }
}