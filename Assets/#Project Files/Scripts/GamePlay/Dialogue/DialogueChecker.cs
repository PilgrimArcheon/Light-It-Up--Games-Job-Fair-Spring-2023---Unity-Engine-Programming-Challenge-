using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChecker : MonoBehaviour 
{
	public bool onInput;//Check If Dialogue requires Player Input
	
	void OnTriggerEnter(Collider other)// On TriggerEnter
	{
		if(other.gameObject.tag == "Player" && !onInput)//Confirm player and onInput
		{
			OnStartDialogue();//Start Dialogue 
		}
	}

	public void OnStartDialogue()
	{
		if(transform.GetChild(0).gameObject != null)//Get the child and Start Dialogue
		{
			DialogueManager.Instance.canInteract = true;
			DialogueManager.Instance.currentDialogue = GetComponentInChildren<DialogueTrigger>();//Assign Dialogue Trigger to start Dialogue Sequence
			DialogueManager.Instance.currentDialogue.PlayDialogue();
		}	
	}

	void OnTriggerExit(Collider other) //On Trigger Exit
	{
		if(other.gameObject.tag == "Player")
		{
			DialogueManager.Instance.canInteract = false;//Reset Dialogue UI Interaction
		}
	}
}
