using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
	public string name;//Name of Speaker or Interactible GO (NPC)
	public string responder;//Name of Responder or Player
	public string description;//Description of Dialogue on going...
	public bool isMission;// Does the Dialogue lead to a Mission...
	public bool autoStart;//Does the Dialogue auto start 

	public Conversation conversation;//Conversation Component for the Dialogue
	
	[HideInInspector] public string[] sentences;//Get All Sentences in the Dialogue

	public MissionUI mission;//Mission if Dialogue leads to a Mission
	public MissionUI requiredMission;//Completed Mission if Dialogue needs a mission to be Completed before being initiated
}
