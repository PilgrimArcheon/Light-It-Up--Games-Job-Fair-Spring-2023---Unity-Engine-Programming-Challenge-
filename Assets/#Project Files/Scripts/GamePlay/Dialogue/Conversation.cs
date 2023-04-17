using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newConversation", menuName = "Dialogue/Conversation")]
[System.Serializable]
public class Conversation : ScriptableObject
{
	public Sentence[] sentences;//Array of Sentences in the Conversation
}