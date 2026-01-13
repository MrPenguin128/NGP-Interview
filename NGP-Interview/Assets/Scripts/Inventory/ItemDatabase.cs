using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item Database", menuName = "Scriptable Objects/Inventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        #region Singleton
        const string PATH = "Databases/Item Database";
        static ItemDatabase _instance;
        public static ItemDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<ItemDatabase>(PATH);
                return _instance;
            }
        }
        #endregion

        [SerializeField] List<ItemObject> items = new List<ItemObject>();
        public ItemObject Get(string id)
        {
            foreach (var item in items)
            {
                if (item != null && item.Id == id)
                    return item;
            }
            return null;
        }
        public ItemObject GetRandom()
        {
            if (items.Count <= 0) return null;
            return items[Random.Range(0, items.Count)];
        }
        public void AddEntry(ItemObject obj)
        {
            if (!items.Contains(obj))
            {
                items.Add(obj);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            items.RemoveAll(item => item == null);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

    }
}