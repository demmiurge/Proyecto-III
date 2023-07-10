using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    bool controllerIsMouse;

    public void SetControllerIsMouse(bool isActive) => controllerIsMouse = isActive;
    public bool GetControllerIsMouse() => controllerIsMouse;

    public void ActiveCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DeactivateCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
