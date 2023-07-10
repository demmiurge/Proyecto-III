using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement2 : MonoBehaviour
{
    public Transform cameraPivot;
    public Transform cameraPosition;
    public Transform cameraWalls;
    public GameObject player;
    public float minDistance = 5f;
    public float maxDistance = 10f;
    public float minPitch = -20;
    public float maxPitch = 90;
    public float yawRotationalSpeedMouse = 40;
    public float pitchRotationalSpeedMouse = 40;
    public float yawRotationalSpeedGamepad = 60;
    public float pitchRotationalSpeedGamepad = 60;
    public LayerMask avoidObjectsLayerMask;
    public float offset = 0.1f;
    public PlayerInputManage playerInputMng;
    public bool performed = false;
    public PlayerMovement playerMovement;
    public bool hasToReset = false;
    public float maxResetTime = 7f;

    //Private Variables
    private Camera camera;
    private bool zoomIn = false;
    private bool change = false;
    private float pitch = 0.0f;
    float mouseXpos;
    float mouseYpos;
    private float yawRotationalSpeed;
    private float pitchRotationalSpeed;
    private float resetTimer = 0;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        if(playerInputMng.GetGamepadActive() == true)
        {
            yawRotationalSpeed = yawRotationalSpeedGamepad;
            pitchRotationalSpeed = pitchRotationalSpeedGamepad;
        }
        else
        {
            yawRotationalSpeed = yawRotationalSpeedMouse;
            pitchRotationalSpeed = pitchRotationalSpeedMouse;
        }

        transform.forward = player.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        mouseXpos = context.ReadValue<Vector2>().x;
        mouseYpos = context.ReadValue<Vector2>().y;
        resetTimer = 0;
        hasToReset = false;
    }

    void LateUpdate()
    {
        float mouseX = yawRotationalSpeed * mouseXpos * Time.deltaTime;
        float mouseY = pitchRotationalSpeed * mouseYpos * Time.deltaTime;

        Vector3 cameraRotation = camera.transform.rotation.eulerAngles;
        float yaw = cameraRotation.y;
        cameraRotation.x -= mouseY;
        cameraRotation.y += mouseX;
        pitch -= mouseY;
        yaw += mouseX;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.transform.rotation = Quaternion.Euler(pitch, yaw, cameraRotation.z);
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, cameraRotation.z);

        float distance = Vector3.Distance(transform.position, cameraPosition.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        Vector3 desiredPosition = cameraPosition.transform.position - camera.transform.forward * distance;

        float camDistance = Vector3.Distance(camera.transform.position, cameraWalls.position);
        camDistance = Mathf.Clamp(camDistance, 1f, 4.5f);
        Ray ray = new Ray(cameraWalls.position, -transform.forward);
        RaycastHit raycastHit;
        Debug.DrawRay(cameraWalls.position, -transform.forward, Color.red);

        if (Physics.Raycast(ray, out raycastHit, camDistance, avoidObjectsLayerMask.value) && playerMovement.GetZoom() == false)
        {
            desiredPosition = raycastHit.point - transform.forward * offset;
        }
        transform.position = desiredPosition;
    }


    public void SetRotation()
    {
        transform.forward = player.transform.forward;
    }

    //Setters
    public void SetYaw(float yaw)
    {
        yawRotationalSpeed = yaw;
    }

    public void SetPitch(float pitch)
    {
        pitchRotationalSpeed = pitch;
    }

    //Getter

    public float GetPitch()
    {
        float retPitch;
        retPitch = Mathf.InverseLerp(minPitch, maxPitch, pitch);
        return retPitch;
    }

}
