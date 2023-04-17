using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour, I_InventoryItem
{
    public int ammoValue;//AmmoVlaue per Reload

    public void Use()//Use Ammo Item (i.e Reload)
    {
        if(FindObjectOfType<PlayerStats>().ammo < 100)
        {
            FindObjectOfType<PlayerStats>().ammo += ammoValue;;// Ammo Stats Update
            Inventory.Instance.RemoveItem(Inventory.Instance.activeUsableItem);
            FindObjectOfType<FirstPersonShooterController>().OnUsedEquiped();
            FindObjectOfType<PlayerController>().AnimReload();//Play Reload Animation
        }
    }
}