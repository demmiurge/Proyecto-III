using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    private HUDManager hudManager;

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hudManager.GoToPauseMenuScreen();
        }
    }

    void Update()
    {
        if (hudManager == null) hudManager = GameManager.instance.GetHUDManager();
    }
}
