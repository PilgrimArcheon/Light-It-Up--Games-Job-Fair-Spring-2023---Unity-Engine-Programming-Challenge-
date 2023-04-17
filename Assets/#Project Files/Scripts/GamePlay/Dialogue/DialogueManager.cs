using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour 
{
	public static DialogueManager Instance;//Instance this Dialogue MANAGER
	public GameObject dialogueBox;//Dialogue Box for Dialogue...
	public GameObject popUpCloseButton;//PopUpClose button to return to GamePlay
	public Text nameText;//Name of Speaker
	public Text dialogueText;//Dialogue Current Sentence text
	public DialogueTrigger currentDialogue;//Current On going DialogueTrigger Component
	PlayerInput playerInput;//Player Input
	AudioSource actorVoiceOver;// Recorded Voice Overs for Sentences

	private Queue<Sentence> sentences;//Sentence queue
	private Dialogue activeDialogue;//Current playing Dialogue
	[HideInInspector] public bool canInteract;// Check if Dialogue can be interactible
	[HideInInspector] public bool hasStartedDialogue;//Has Dialogue started

	void Awake()
	{
		Instance = this;//Instance this as the Dialogue Manager
	}
	
	void Start () 
	{
		//Reference all the Component and Global Variables
		sentences = new Queue<Sentence>();
		playerInput = FindObjectOfType<PlayerInput>();
		actorVoiceOver = gameObject.AddComponent<AudioSource>();
		actorVoiceOver.volume = 0.75f;
	}

	public void StartDialogue (Dialogue dialogue)//Start the Dialogue
	{
		if(!canInteract)
			return;

		dialogueBox.SetActive(true);//Set Dialogue box Open
		popUpCloseButton.SetActive(false);//Pop Up Close
		MenuManager.Instance.InMenu();//MenuManager update to show that player is in Menu (Dialogue)
		hasStartedDialogue = true;//Started Dialogue
		activeDialogue = dialogue;//Set Active Dialogue to chosen Dialogue

		sentences.Clear();//Clear previous Dialogue

		foreach (Sentence _sentence in dialogue.conversation.sentences)
		{
			sentences.Enqueue(_sentence);//Add sentences of current Dialogue to a queue
		}

		DisplayNextSentence();//Display Sentence 
	}

	public void DisplayNextSentence ()//Display Dialogue Sentences
	{
		if (sentences.Count == 0)
		{
			if(currentDialogue == null) return; 
			if(currentDialogue.popUp)
				popUpCloseButton.SetActive(true);
			else
				EndDialogue();
			return;
		}

		Sentence _sentence = sentences.Dequeue();
		//Stop Active VoiceOver Playing
		actorVoiceOver.Stop();
		//Play Audio on the Actor if audio exists
		if(_sentence.voiceOver != null)
		{
			actorVoiceOver.clip = _sentence.voiceOver;
			actorVoiceOver.Play();
		}

		if(_sentence.actor == Actor.NPC)//Get actor name if NPC
		{
			nameText.text = activeDialogue.name;
		}
		else//Else Get Responder or Player Name
		{
			nameText.text = activeDialogue.responder;
		}
			
		StopAllCoroutines();
		StartCoroutine(TypeSentence(_sentence.sentence));//Type out Sentences
	}

	IEnumerator TypeSentence (string sentence)//Type Senetences
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
		dialogueText.text = sentence;
		yield return new WaitForSeconds(actorVoiceOver.clip.length + 0.25f);
		DisplayNextSentence();
	}

	public void EndDialogue()// Check and End all active Dialogue (sentences and conversations Included)
	{
		actorVoiceOver.Stop();
		dialogueBox.SetActive(false);
		hasStartedDialogue = false;
		MenuManager.Instance.OpenMenu("playerHud");
		MenuManager.Instance.NotInMenu();
		Inventory.Instance.indicator.SetActive(false);//Close off Indicator

		if(currentDialogue.dialogue.isMission)//Check if current Dialogue has a Mission
		{
			if(!currentDialogue.dialogue.mission.isCompleted)
				currentDialogue.dialogue.mission.ShowUI();
			else
				currentDialogue.dialogue.mission.MissionComplete();
		}

		if(currentDialogue.dialogues.Count > 1)//Does Current Dialogue have more conversations...
		{
			if(currentDialogue.dialogues[1].autoStart)//Check for auto Start on next Dialogue
			{
				currentDialogue.dialogues.Remove(currentDialogue.dialogues[0]);//Remove Current Dialogue
			}
		}
		canInteract = false;//Reset canInteract...
		if(currentDialogue.transform.parent.gameObject != null)//Confirm...
			currentDialogue.transform.parent.gameObject.SetActive(false);//Set DialogueTrigger parent holder GO to false
		currentDialogue = null;//Return Current Dialogue to null (No active Dialogue)
	}
}
