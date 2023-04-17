using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioActiveGO : MonoBehaviour, I_InventoryItem
{
    public bool closeToPowerPlant;//If Player is Close to Plant
    public GameObject powerPlants;

    public void Use()
    {
        Debug.Log("USE RadioActiveGO!!!!!!!!!");//Use PowerPlant and Set is Active
        if(closeToPowerPlant && powerPlants != null)
        {
            FindObjectOfType<FirstPersonShooterController>().OnUsedEquiped();
            foreach (Transform child in powerPlants.transform)
            {
                child.gameObject.GetComponent<GlowEnergy>().UpdateEnergyGlow();
            }
            //Call PowerPlant Use and Light it Up
            powerPlants.GetComponent<EnergyGlowTrigger>().UncoveredEnergyGlow();
            powerPlants.GetComponent<Collider>().enabled = false;
            Inventory.Instance.indicator.SetActive(false);
            Inventory.Instance.hasRadioActiveGO = false;
            Inventory.Instance.sfx.PlayOneShot(Inventory.Instance.radioActiveGOSfx);
            powerPlants = null;
            gameObject.SetActive(false);
        } 
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "PowerPlantsTrigger")
        {
            closeToPowerPlant = true;
            powerPlants = other.gameObject;
            Inventory.Instance.indicator.SetActive(true);
            Inventory.Instance.needRadioActiveGO.SetActive(false);
        }

        if(other.gameObject.tag == "PopUp")
        {
            if(other.gameObject.GetComponent<PopUpItem>() != null)
            {
                Inventory.Instance.popUpItem = other.gameObject.GetComponent<PopUpItem>();
            }
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == "PowerPlantsTrigger")
        {
            closeToPowerPlant = false;
            Inventory.Instance.indicator.SetActive(false);
            powerPlants = null;
        }

        if(other.gameObject.tag == "PopUp")
            Inventory.Instance.popUpItem = null;
    }
}