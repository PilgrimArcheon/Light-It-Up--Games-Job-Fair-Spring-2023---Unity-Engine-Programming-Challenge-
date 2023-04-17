using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour 
{
	public List<Dialogue> dialogues;//List of Dialogue Conversations on a GO (NPC)
	public bool popUp;//isPopUp
	[HideInInspector] public Animator anim;//Get Animator...
	[HideInInspector] public Dialogue dialogue;//To Set Current Dialogue

	public void Start() 
	{
		anim = GetComponentInParent<Animator>();
	}
	
	public void PlayDialogue()//Play the Dialogue
	{
		if(dialogues.Count > 1)
		{
			if((dialogues[1].requiredMission != null && dialogues[1].requiredMission.isCompleted))//Check Conditions...
			{
				dialogues.Remove(dialogues[0]);//Remove Played Dialogue
			}
		}	

		if(dialogues.Count != 0)
		{
			dialogue = dialogues[0];//Make first Dialogue in the List current dialogue
			DialogueManager.Instance.StartDialogue(dialogue);//Make this current Dialogue and Start Dialogue
		}
	}

	public void PlayAnim(AnimActions actions)//Playe the Dialogue Animation...
	{
		if(actions == AnimActions.Idle)//If Set Idle
		{
			anim.SetBool("idle", true);
			anim.SetBool("talking", false);
		}
		else if(actions == AnimActions.Talking)//If Set To Talking
		{
			anim.SetBool("talking", true);
			anim.SetBool("idle", false);
		}
	}
}
