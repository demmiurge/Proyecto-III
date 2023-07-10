using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManage : MonoBehaviour
{
    public PlayerInput playerInputMap;
    public CameraMovement2 cameraMovement;

    private bool gamepadActive = false;

    private ControllerManager controllerManager;

    private void Awake()
    {
        CheckManagersInstance();

        playerInputMap = GetComponent<PlayerInput>();
        if (Gamepad.all.Count > 0)
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementGamepad");
            gamepadActive = true;
            if (controllerManager) controllerManager.SetControllerIsMouse(false);
        }
        else
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementMap");
            gamepadActive = false;
            if (controllerManager) controllerManager.SetControllerIsMouse(true);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        CheckManagersInstance();

        if (Gamepad.all.Count > 0)
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementGamepad");
            gamepadActive = true;
            if (controllerManager) controllerManager.SetControllerIsMouse(false);
        }
        else
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementMap");
            gamepadActive = false;
            if (controllerManager) controllerManager.SetControllerIsMouse(true);
        }

        /*if (Gamepad.all.Count > 0)
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementGamepad");
            gamepadActive = true;
        }
        else 
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementMap");
            gamepadActive=false;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        CheckManagersInstance();
    }

    public void OnDeviceLost()
    {
        playerInputMap.SwitchCurrentActionMap("PlayerMovementMap");
        gamepadActive = false;
        if (cameraMovement)
        {
            cameraMovement.SetYaw(cameraMovement.yawRotationalSpeedMouse);
            cameraMovement.SetPitch(cameraMovement.pitchRotationalSpeedMouse);
            if (controllerManager) controllerManager.SetControllerIsMouse(true);
        }
    }

    public void OnDeviceRegained()
    {
        playerInputMap.SwitchCurrentActionMap("PlayerMovementGamepad");
        gamepadActive = true;
        if (cameraMovement)
        {
            cameraMovement.SetYaw(cameraMovement.yawRotationalSpeedGamepad);
            cameraMovement.SetPitch(cameraMovement.pitchRotationalSpeedGamepad);
            if (controllerManager) controllerManager.SetControllerIsMouse(false);
        }
    }

    public void OnControlsChanged()
    {
        if (Gamepad.all.Count > 0)
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementGamepad");
            gamepadActive = true;
            if (cameraMovement)
            {
                cameraMovement.SetYaw(cameraMovement.yawRotationalSpeedGamepad);
                cameraMovement.SetPitch(cameraMovement.pitchRotationalSpeedGamepad);
            }
            if (controllerManager) controllerManager.SetControllerIsMouse(false);
        }
        else
        {
            playerInputMap.SwitchCurrentActionMap("PlayerMovementMap");
            gamepadActive = false;
            if (cameraMovement)
            {
                cameraMovement.SetYaw(cameraMovement.yawRotationalSpeedMouse);
                cameraMovement.SetPitch(cameraMovement.pitchRotationalSpeedMouse);
            }
            if (controllerManager) controllerManager.SetControllerIsMouse(true);
        }
    }

    void CheckManagersInstance()
    {
        if (controllerManager == null && GameManager.instance.GetControllerManager() != null) controllerManager = GameManager.instance.GetControllerManager();
    }

    public bool GetGamepadActive()
    {
        return gamepadActive;
    }
}
