using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDataUI : MonoBehaviour
{
    public Sprite[] popUpSprites;//Pop Up Secret Pics
    public Text info;//Pop Up Text Info
    public Image activePopUp;//Get Image to show Secret Info
    
    public void PopUpObjects(string popUpInfo)//PopUp Secret 
    {
        for (int i = 0; i < popUpSprites.Length; i++)//Check popUp Id
        {
            if(popUpSprites[i].name == popUpInfo)
            {
                activePopUp.sprite = popUpSprites[i];//Show Active Pic
                info.text = popUpInfo;//Uodate infoText
            }
        }
    }
}