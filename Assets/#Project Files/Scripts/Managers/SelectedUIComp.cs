using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedUIComp : MonoBehaviour
{
	public GameObject ui_GO;//Object In Scene to Highlight
	EventSystem eventSystem;//Event Systme
	void OnEnable()
	{
		eventSystem = EventSystem.current;//Reference event system
		eventSystem.SetSelectedGameObject(ui_GO);//Set a button to be the Highlighted UI_GO
	}
}
