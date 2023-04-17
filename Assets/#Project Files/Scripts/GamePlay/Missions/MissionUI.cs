using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    GameObject missionUI;//missionUI GameObject
    GameObject completeMissionUI;//completeMission 
    public MissionState missionState;//Mission State
    public string missionInfo;//Mission Details
    public bool isCompleted;//Check if the mission is Complete
    public int completedMissionId;//Mission ID
    public bool gameComplete;//Is the Game Done...
    public bool onInput;//Is the Mission Ended or started on Input??
    
    Transform missionUIHolder;//Get the MissionUI Holder Trans
    void Start()
    {
        //Get all References and variables
        missionUIHolder = GameObject.Find("MissionUI").transform;//Assign MissionUI holder
        missionUI = missionUIHolder.GetChild(1).GetChild(0).gameObject;//assign mission UI
        completeMissionUI = missionUIHolder.GetChild(0).GetChild(0).gameObject;//assign mission complete UI
    }

    public void ShowUI()//Check all necessarry variables and Show UI GO
    {
        if(gameComplete)
        {
            GameComplete();
            return;
        }
            
        if(missionUI == null)
            return;
        missionUI.SetActive(true);
        missionUI.GetComponent<Text>().text = missionInfo;//Update MissionUI Text 
        MissionsManager.Instance.GetMissionSound();
        gameObject.SetActive(false);     
    }

    public void MissionComplete()//Mission Complete
    {
        isCompleted = true;
        MissionsManager.Instance.GetActiveMission(completedMissionId);
        MissionsManager.Instance.CompleteMissionSound();
        completeMissionUI.SetActive(true);
        missionUI.SetActive(false);
        Invoke("Done", 3f);
        if(gameComplete)
            GameComplete();    
    }

    public void Done()//Done with Mission
    {
        completeMissionUI.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        if(onInput)
            return;
        if(other.gameObject.tag == "Player")//Confirm the Player Tag...
        {
            if(missionState != MissionState.CompleteMission)//Check if the MissionState is "Complete"
                ShowUI();//ShowUI...
            else
                MissionComplete();//MissionComplete...
        }
    }

    //For Demo Build...//
    public void GameComplete()
    {
        Invoke("Completed", 3f);// Call Complete Game In in 3 seconds
    }

    void Completed()//Complete Mission
    {
        MissionComplete();
        missionUI.SetActive(false);
        completeMissionUI.SetActive(false);
        //Game Completed
    }
}

public enum MissionState //MissionStates (DoMission, CompleteMission)
{
    DoMission,
    CompleteMission
}