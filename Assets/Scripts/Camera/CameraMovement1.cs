using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement1 : MonoBehaviour
{
    public Transform cameraPosition;
    public Transform cameraPivot;
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

    //Private Variables
    private Camera camera;
    private float pitch = 0.0f;
    float mouseXpos;
    float mouseYpos;
    private float yawRotationalSpeed;
    private float pitchRotationalSpeed;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        mouseXpos = context.ReadValue<Vector2>().x;
        mouseYpos = context.ReadValue<Vector2>().y;    
    }

    void LateUpdate()
    {
        float mouseX = yawRotationalSpeed * mouseXpos * Time.deltaTime;
        float mouseY = pitchRotationalSpeed * mouseYpos * Time.deltaTime;

        float distance = Vector3.Distance(camera.transform.position, cameraPosition.position);
        Vector3 cameraRotation = camera.transform.rotation.eulerAngles;
        float yaw = cameraRotation.y;
        cameraRotation.x -= mouseY;
        cameraRotation.y += mouseX;
        pitch -= mouseY;
        yaw += mouseX;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.transform.rotation = Quaternion.Euler(pitch, yaw, cameraRotation.z);
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, cameraRotation.z);

        /*cameraRotation.x = Mathf.Clamp(cameraRotation.x, minPitch, maxPitch);
        camera.transform.rotation = Quaternion.Euler(cameraRotation);*/
   
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        Vector3 desiredPosition = cameraPosition.transform.position - camera.transform.forward * distance;

        Debug.DrawRay(cameraPivot.position, (cameraPosition.transform.forward * -1), Color.red);
        Ray ray = new Ray(cameraPivot.position, (camera.transform.forward * -1));
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, distance, avoidObjectsLayerMask.value))
        {
            desiredPosition = raycastHit.point - camera.transform.forward * offset;
            
        }
        transform.position = desiredPosition;

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

}
