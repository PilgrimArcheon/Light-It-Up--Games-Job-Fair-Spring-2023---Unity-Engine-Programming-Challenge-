using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;//Instance Menu Script

    [SerializeField] public Menu[] menus;//Ref All Menus in the Scene
    bool inMenu;

    void Awake()
    {
        Instance = this;//Make this an Instance
        Time.timeScale = 1f;//Set TIme Scale to 1 i.e Active
        inMenu = false;
        if(SceneManager.GetActiveScene().name == "MainMenu")
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenMenu(string menuName)//To Open Menu
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void CloseMenu(Menu menu)//Close the Menu
    {
        menu.Close();
    }

    public void InMenu()// Check if player is in a UI Menu (Dialogue/PopUp)
    {
        inMenu = true;
        CheckEnemies(false);//Disable all AI when Interacting with a Menu
    }

    public void NotInMenu()//Check is out of Menu
    {
        inMenu = false;
        CheckEnemies(true);//Enables all enemy AI
    }

    public void Play(string Game)//Load Specific Playable Scene
    {
        SceneManager.LoadScene(Game);
    }

    public void MainMenu()//Got to Main Menu Scene
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
            return;
        if(FindObjectOfType<PlayerInput>().pause && !inMenu)//Get Pause Input
        {
            OpenMenu("pause");//Open Pause Menu
            FindObjectOfType<PlayerController>()._isInteracting = true;//Set Player Interacting (with UI) true
            Pause();//Pause Gameplay
            FindObjectOfType<PlayerInput>().pause = false;
        }
    }

    public void Pause()//Pause the Game
    {
        CheckEnemies(false);//Make sure all enemies movement are paused and stopped
        Cursor.lockState = CursorLockMode.None;//Unlock Cursor for Navigation
        Time.timeScale = 0f;//Pause Time Scale
    }

    public void Resume()//Resume the Game
    {
        CheckEnemies(true);//Reset all enemyAIs to move
        FindObjectOfType<PlayerInput>().cursorLocked = true;//Lock Cursor
        Cursor.lockState = CursorLockMode.Locked;//
        FindObjectOfType<PlayerController>()._isInteracting = false;//Disable Interaction with UI
        Time.timeScale = 1f;// Unpause Time Scale
    }

    public void Restart()//Restart the Game
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//Activate the Game Scene Again
    }

    public void Quit()//Quit the Game App
    {
        Application.Quit();
    }

    public void CheckEnemies(bool enemyActive)//Check through all enemyAI Script and disable them
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI _enemy in enemies)
        {
            _enemy.enabled = true;
        }
    }

    public void ResetSave()// Reset all saved Data from Menu Input
    {
        SaveManager.Instance.ResetSave();
    }
}