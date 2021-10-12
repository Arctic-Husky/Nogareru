using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausarJogo : MonoBehaviour
{
    // Codigo estranho, mudar depois

    private GameObject menuPause;

    private void Start()
    {
        menuPause = GameObject.FindWithTag("PauseMenu");

        if (menuPause != null)
        {
            menuPause.SetActive(false);
        }

        InputManager.Controls.Player.OpenMenu.performed += ctx => Menu();
    }

    private void Menu()
    {
        if(menuPause != null)
        {
            Debug.Log(menuPause.name);
            if (menuPause.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                InputManager.Remove(ActionMapNames.Player);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                InputManager.Add(ActionMapNames.Player);
                InputManager.Controls.Player.OpenMenu.Enable();
            }
            menuPause.SetActive(!menuPause.activeSelf);
        }
        
    }
}
