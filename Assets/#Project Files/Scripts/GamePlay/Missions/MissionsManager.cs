using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsManager : MonoBehaviour
{
    public int _missionid;//Current Mission ID
    public static MissionsManager Instance;//MissionManager Instance
    public List<Mission> missions = new List<Mission>();//List Of Missions
    public AudioSource missionSound;//Mission Sfx Audio Source
    public AudioClip newMissionSound, completedMissionSound;//Sfx Audio Clips
    Transform player;//Player Transform

    void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;//Set this to Instance
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;//Set player Tansform
        GetActiveMission(_missionid);//Get current Mission Id  
        if(_missionid == 0) Inventory.Instance.hasRadioActiveGO = true;//Give player a Radio Active Energy Source On First Mission
        else Inventory.Instance.hasRadioActiveGO = false;//If Not Don't
        player.position = missions[_missionid].checkPoint.position;//Set Player to Current Mission Start point (CheckPoint)
    }

    public void GetActiveMission(int missionId)//Get Active Mission with its MissionID 
    {
        foreach (Mission _mission in missions)//Check all missions in the List and Compare IDs
        {
            if(_mission.missionId == missionId)
                _mission.Active();//Set the matching mission Active
            else
                _mission.NotActive();//Set the other InActive
        }
    }

    public void GetMissionSound()//Play Get Mission Sfx
    {
        missionSound.PlayOneShot(newMissionSound);
    }

    public void CompleteMissionSound()//Play Complete Mission Sfx
    {
        missionSound.PlayOneShot(completedMissionSound);
    }
}