using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, I_InventoryItem
{
    public float foodEnergyValue;//Energy Value Item adds to player Stats

    public void Use()// Use Food Item
    {
        if(FindObjectOfType<PlayerStats>().energy < 70)// Check Energy Value is above threshold (i.e 70)
        {
            FindObjectOfType<PlayerStats>().energy += foodEnergyValue;//Update Player Stat Value
            Inventory.Instance.RemoveItem(Inventory.Instance.activeUsableItem);//Used...
            FindObjectOfType<FirstPersonShooterController>().OnUsedEquiped();
        }
    }
}