using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject objectToShow;//GameObject showing Controls as Tutorial..
    public bool closeOff;//Check On Last Tutorial GameObject to close all Tut_GO for good.
    
    private void OnTriggerEnter(Collider other) //Show on Player Trigger Enter
    {
        if(other.gameObject.tag == "Player")
        {
            if(!closeOff)//Close off GO covers all Tut_GO
                objectToShow.SetActive(true);
            else
                objectToShow.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other) //Hide on Player Trigger Exit
    {
        if(other.gameObject.tag == "Player")
        {
            objectToShow.SetActive(false);
        }
    }
}
