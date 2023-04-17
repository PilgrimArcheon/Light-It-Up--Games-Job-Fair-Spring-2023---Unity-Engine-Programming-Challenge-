using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sentence
{
	public Actor actor;//Current Speaker of Sentence
	public AnimActions actions;//Current Playing Animation

	[TextArea(3, 10)]
	public string sentence;//Spoken Sentences...
	public AudioClip voiceOver;//Voice over Audio Clip
}

public enum Actor//Actor Enum
{
	Player,
	NPC
}

public enum AnimActions//Anim Enum
{
	Idle,
	Talking
}
