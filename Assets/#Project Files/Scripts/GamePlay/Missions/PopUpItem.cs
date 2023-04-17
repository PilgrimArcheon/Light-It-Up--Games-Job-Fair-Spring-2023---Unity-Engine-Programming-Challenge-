using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpItem : MonoBehaviour
{
    public string popUpInfo;//PopUp Info Details
    
    public void PopUp()//Call PopUp Method
    {
        MenuManager.Instance.OpenMenu("popUp");//Open PopUp Menu
        MenuManager.Instance.InMenu();//Update that Player is in Menu (Player can't move)
        Cursor.lockState = CursorLockMode.None;//Unlock Cursor for navigation...

        if(gameObject.GetComponent<DialogueChecker>() != null)
        {
            gameObject.GetComponent<DialogueChecker>().OnStartDialogue();//If a Dialogue Checker is found, Play Dialogue
        }
    }
}