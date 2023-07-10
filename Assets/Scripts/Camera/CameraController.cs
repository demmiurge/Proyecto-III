using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform lookAt;
    //float m_Yaw = 0.0f;
    float pitch = 0.0f;
    //public float m_Distance = 15.0f;
    public float yawRotationalSpeed = 720.0f;
    public float pitchRotationalSpeed = 360.0f;

    public float minPitch = 30.0f;
    public float maxPitch = 60.0f;

    public float minDistance = 15f;
    public float maxDistance = 15f;

    public LayerMask avoidObjectsLayerMask;
    public float offset = 0.1f;

    public KeyCode debugLockAngleKeyCode = KeyCode.I;
    public KeyCode debugLockKeyCode = KeyCode.O;

    bool angleLocked = false;
    bool aimLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        aimLocked = Cursor.lockState == CursorLockMode.Locked;
    }

#if UNITY_EDITOR
    void UpdateInputDebug()
    {
        if (Input.GetKeyDown(debugLockAngleKeyCode))
            angleLocked = !angleLocked;
        if (Input.GetKeyDown(debugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            aimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }
#endif

    void LateUpdate()
    {
#if UNITY_EDITOR
        UpdateInputDebug();
#endif
        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();

#if UNITY_EDITOR
        if (angleLocked)
        {
            mouseX = 0.0f;
            mouseY = 0.0f;
        }
#endif
        //Debug.Log("mouseX: " + mouseX + " | mouseY: " + mouseX);
        transform.LookAt(lookAt.position);

        float distance = Vector3.Distance(transform.position, lookAt.position);
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        float yaw = eulerAngles.y;

        yaw += mouseX * yawRotationalSpeed * Time.deltaTime;
        pitch += mouseY * pitchRotationalSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 forwardCamera = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad), Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        Vector3 desiredPosition = lookAt.transform.position - forwardCamera * distance;

        Ray ray = new Ray(lookAt.position, -forwardCamera);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, distance, avoidObjectsLayerMask.value))
        {
            desiredPosition = raycastHit.point + forwardCamera * offset;
        }
        transform.position = desiredPosition;
        transform.LookAt(lookAt.position);

    }

    // Update is called once per frame
    void Update()
    {

    }

}
