using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using Random = UnityEngine.Random;
public class DropSystem : MonoBehaviour
{
    //------------------------------------
    //          PREMADE ASSET
    //------------------------------------

    [Header("Item Drop Settings")]
    [SerializeField] List<DropInfo> droppableItems = new List<DropInfo>();
    [Tooltip("How many times the drop calculation will be made")]
    [SerializeField] int dropRepetitions;
    [Tooltip("Percentage chance of a item drop, is calculated in every drop repetition")]
    [Range(0,100)][SerializeField] int dropChance;
    [SerializeField] int minDropAmount;
    [SerializeField] int maxDropAmount;
    public void DropItem()
    {
        //For each dropRepetition, allows to drop different items, in different amounts
        for (int i = 0; i < dropRepetitions; i++)
        {
            int r = Random.Range(0, 100);
            if(r < dropChance)
                Drop();
        }
    }
    void Drop()
    {
        int totalDropWeight = 0;
        foreach (var info in droppableItems)
        {
            totalDropWeight += info.weight;
        }
        int randomWeight = Random.Range(0, totalDropWeight);
        droppableItems.Sort();
        droppableItems.Reverse();
        for (int i = 0; i < droppableItems.Count; i++)
        {
            if (randomWeight <= droppableItems[i].weight)
            {
                ItemObject item = droppableItems[i].item;
                if (i + 1 < droppableItems.Count)
                {
                    List<DropInfo> sameWeightItems = new List<DropInfo>
                    {
                        droppableItems[i]
                    };
                    for (int j = i; j < droppableItems.Count; j++)
                    {
                        if (droppableItems[j].weight == droppableItems[i].weight)
                            sameWeightItems.Add(droppableItems[j]);
                        else
                            break;
                    }
                    item = sameWeightItems[Random.Range(0, sameWeightItems.Count)].item;
                }
                int itemAmount = item.Stackable ? Random.Range(minDropAmount, maxDropAmount + 1) : 1;
                ItemPickup itemObject = Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                itemObject.SetItem(item, itemAmount);
                return;
            }
            else
                randomWeight -= droppableItems[i].weight;
        }
    }

    [Serializable]
    //Saves the item and his weight to be dropped, the bigger the weight, the bigger the drop chances
    public class DropInfo : IComparable
    {
        [Tooltip("Which item will be dropped")]
        public ItemObject item;
        [Tooltip("Chance of item being dropped")]
        public int weight;

        public int CompareTo(object obj)
        {
            return weight.CompareTo((obj as DropInfo)?.weight ?? 1);
        }
    }
}
