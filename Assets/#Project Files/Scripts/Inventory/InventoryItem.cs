using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public InventoryItemData inventoryData;
    public Item itemType;
}

public enum Item// Enum to Set up Collectible Inventory Item Types
{
    RadioActiveGO,//For Energy Source
    Ammo,//For Weapon 
    Food,//For Player Energy Stat
    Null //Null
}