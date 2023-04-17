using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public GameObject missionComponents;//Mission GO Component...
    public int missionId;//Mission ID
    public Transform checkPoint;//CheckPoint On Mission

    public void NotActive()//Set mission As Not Active
    {
        if(missionComponents != null)//Only If Mission Exists...
            missionComponents.SetActive(false);//Set Mission Components GO to False
        gameObject.SetActive(false);//And set GO false.
    }

    public void Active()//Set Mission As Active
    {
        missionComponents.SetActive(true);//Set Missions Components GO active and then Update Item Makers for that Mission's Components
        FindObjectOfType<PlayerController>()._compassBar.UpdateItemMarkers();
    }
}