using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;//Instance of Inventory 
    //Varaibles and References
    public bool canReceiveInput;
    public bool hasRadioActiveGO;
    public InventoryItem currentPickableItem;
    public PopUpItem popUpItem;
    public Item activeUsableItem;
    public GameObject inventoryHolderUI;
    public GameObject indicator, needRadioActiveGO;
    public bool canSelectItem;
    bool shownInventory;

    public Dictionary<Item, int> numberOfItems;//Dictionary Holding all Inventory Item Types and the available Item Value
    public Text ammoText, foodText;//UI Showing Item Values
    public AudioSource sfx;//SoundFx for Equiping And Use
    public AudioClip pickUpSfx, openSfx, useSfx, radioActiveGOSfx;
    void Awake()
    {
        if(Instance == null)//Get the Instance fo Inventory
            Instance = this;
        else
            Destroy(this.gameObject);//Destroy if one already exist in scene
    }

    void Start()
    {
        //Set and Get access to all references
        numberOfItems = new Dictionary<Item, int>()//Set up Dictionary Data for all Inventory Items On GameStart
        {
            {Item.Ammo, 1},
            {Item.Food, 1},
        };
        //Get all Inventory Indicators and UI GameObjects
        indicator = GameObject.Find("Indicator").transform.GetChild(0).gameObject;
        inventoryHolderUI = GameObject.Find("InventoryUI").transform.GetChild(0).gameObject;
        needRadioActiveGO = GameObject.Find("NeedRadioActiveGO").transform.GetChild(0).gameObject;
        UpdateTextUI();//Call Update UI Method
    }

    public void SelectActiveItem(Item itemType)//OnSelceting an Item
    {
        if(numberOfItems[itemType] > 0)//Check through all Available items and confirm Item to select
        {
            activeUsableItem = itemType;
            sfx.PlayOneShot(pickUpSfx);//Play Sfx
        }
    }

    public void UseEquipedItem()//On Use Equipped and Selected Item
    {
        RemoveItem(activeUsableItem);//Remove the Used Item...
        sfx.PlayOneShot(useSfx);//Play Sfx
    }

    public void ShowInventoryItem()//Show Inventory Items (UI)
    {
        if(!shownInventory)
        {
            inventoryHolderUI.SetActive(true);//Activate the GameObject
            sfx.PlayOneShot(openSfx);//PlaySfx
            canSelectItem = true;
            shownInventory = true;
        }
    }

    public void HideInventoryItem()//Hide Inventory Items (UI)
    {
        inventoryHolderUI.SetActive(false);//Deactivate the GameObject
        canSelectItem = false;
        shownInventory = false;
    }
    
    public void AddItem()//On Pick up, Add Item to Invetory Items List Depending on Item Type
    {
        canReceiveInput = false;//Reset Can PickUp
        if(currentPickableItem != null)
        {
            if(currentPickableItem.itemType != Item.RadioActiveGO)//Check current Pickable Item
            {
                PickUp();//PickUp Item
                numberOfItems[currentPickableItem.itemType]++;//Update Dictionary
                UpdateTextUI(); //Update UI
            }
            else 
            {
                if(!hasRadioActiveGO)
                {
                    PickUp();//PickUP Item
                    hasRadioActiveGO = true;
                }
            }

            currentPickableItem = null;//Set back to Null On Pick Up
            indicator.SetActive(false);//Close Indicator      
        }
    }

    void PickUp()//Call On Item PickUp
    {
        sfx.PlayOneShot(pickUpSfx);//Play Sfx
        DestroyImmediate(currentPickableItem.gameObject, true);//Destroy the Pick Up GameObject
        FindObjectOfType<CompassBar>().UpdateItemMarkers();// Update the Trackable Markers
    }

    public void RemoveItem(Item itemType)//Call to Remove Item From List
    {
        if(numberOfItems[itemType] > 0)// Check through Dictionary and Remove appropriate ItemType
            numberOfItems[itemType]--;
        UpdateTextUI();//Update UI accordingly
    }

    public void UpdateTextUI()//Call Update UI 
    {
        ammoText.text = numberOfItems[Item.Ammo].ToString("0");//Update Ammo for Weapon's Reloading
        foodText.text = numberOfItems[Item.Food].ToString("0");//Update Food for Energy
    }
}