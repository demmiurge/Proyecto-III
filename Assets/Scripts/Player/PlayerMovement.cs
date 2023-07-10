using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Utilities;

public class PlayerMovement : MonoBehaviour
{

    [Header("Camera")]
    public Camera camera;
    public float lerpRotationPct = 0.1f;
    public LayerMask floorMask;
    public LayerMask noneFloorMask;
    public LayerMask waterLayermask;
    public Transform feetTransform;
    public Transform zoomPosition;
    public Transform cameraPosition;
    public Transform cameraPos;
    public Transform cameraIdle;
    public float cameraDefaultFOV = 60;
    public float cameraZoomFOV = 40f;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float zoomSpeed = 2f;
    public float jumpForce = 0.4f;
    public float currentJumpForce;
    public float jumpIncrement = 0.1f;
    public float waterJumpForce = 5f;
    public float waterMovementSpeed = 3f;
    public float swimmingSpeed = 1.5f;
    [Range(-2f, -0.0f)]
    public float fallDetection = -1.0f;
    public bool isInWater = false;
    public Transform capsule;
    public float fallForce = 10f;
    public float coyoteTime = 0.2f;
    public float jumpBuffer = 0.2f;
    public CheckWater checkFloater;

    [Header("Enemies")]
    public float knockbackForce = 3f;
    public float enableMoveTime = 0.5f;

    [Header("Animators")]
    public Animator playerAnimator;
    public Animator gunAnimator;

    [Header("Particles")]
    public ParticleSystem waterTrail;

    [Header("IK")]
    [Range(0.0f, 1.0f)]
    public float distanceToGround = 0f;
    public LayerMask detectionLayerMask;

    //Private Variables
    Rigidbody playerRigidbody;
    private PlayerInputManage playerInputMng;
    private PlayerLife playerLife;
    private Swimming swim;
    private Vector2 moveInput;
    private bool canMove = true;
    private bool isBouncing = false;
    private float currentBouncing = 0;
    private float currentTime = 0f;
    bool isFalling = false;
    bool jumping = false;
    bool coyoteJump = false;
    bool hasJumped = false;
    bool isInAir = false;
    bool jumpPressed = false;
    bool buffedJump = false;
    float currentBuffer = 0;
    private bool strafe = false;
    private bool zoom = false;
    private bool running = false;
    private bool swimmingUp = false;
    private bool swimmingDown = false;
    float movementSpeed = 0.0f;
    float speed = 0.0f;
    float weight = 0;
   

    private PreviewShoot previewShoot;
    private HUDManager hudManager;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerInputMng = GetComponent<PlayerInputManage>();
        playerLife = GetComponent<PlayerLife>();
        swim = GetComponent<Swimming>();
        previewShoot = GetComponent<PreviewShoot>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentJumpForce = jumpForce;
    }

    void Update()
    {
        if (hudManager == null) hudManager = GameManager.instance.GetHUDManager();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (swim.IsInWater() == false)
        {
            if (context.started)
            {
                strafe = true;
                zoom = true;
                previewShoot.AmingOn();
                hudManager.ActiveCrosshair();
                gunAnimator.SetBool("AimGun", true);
            }
        }
        if (context.canceled)
        {
            strafe = false;
            zoom = false;
            previewShoot.AmingOff();
            hudManager.DisableCrosshair();
            gunAnimator.SetBool("AimGun", false);
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsTouchingTheGround())
        {
            if (swim.IsInWater() == false)
            {
                if(context.started)
                {
                    jumping = true;
                    isFalling = false;
                    hasJumped = true;
                    playerAnimator.SetBool("Jump", true);
                    playerAnimator.SetBool("OnGround", false);
                }              
            }
        }
        if(isFalling == true && currentTime <= coyoteTime && hasJumped == false)
        {
            if (swim.IsInWater() == false)
            {
                if (context.started)
                {
                    playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                    jumping = true;
                    coyoteJump = true;
                    isFalling = false;
                    playerAnimator.SetBool("Jump", true);
                    playerAnimator.SetBool("Falling", false);
                    playerAnimator.SetBool("OnGround", false);
                }
            }
        }

        if(isFalling && swim.IsInWater() == false)
        {
            if (context.started)
                jumpPressed = true;
        } 

        if (context.performed)
        {
            jumping = false;
            coyoteJump = false;
            currentJumpForce = jumpForce;
            hasJumped = true;
            if(swim.IsInWater())
            {
                playerAnimator.SetBool("Jump", false);
            }
        }

        if (context.canceled)
        {
            jumping = false;
            coyoteJump = false;
            currentJumpForce = jumpForce;
            hasJumped = true;
            if (swim.IsInWater())
            {
                playerAnimator.SetBool("Jump", false);
            }
        }

    }

    public void OnWaterMoveUp(InputAction.CallbackContext context)
    {
        if(swim.IsInWater() && checkFloater.GetWaterJump() == false)
        {
            if (context.started)
            {
                swimmingUp = true;
            }
            if(context.canceled)
            {
                swimmingUp = false;
            }
        }
        else
        {
            swimmingUp = false;
        }
    }
    public void OnWaterMoveDown(InputAction.CallbackContext context)
    {
        if (swim.IsInWater())
        {
            if (context.started)
            {
                swimmingDown = true;           
            }
            if (context.canceled)
            {
                swimmingDown = false;
            }
        }
        else
        {
            swimmingDown = false;          
        }
    }

    public void OnWaterJump(InputAction.CallbackContext context)
    {
        if (swim.IsInWater() && transform.parent != null && checkFloater.GetWaterJump())
        {
            if (context.performed)
            {
                playerRigidbody.AddForce(transform.up * waterJumpForce, ForceMode.Impulse);
                playerAnimator.SetBool("Jump", true);
            }
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            running = true;
        }
        if (context.canceled)
        {
            running = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        bool hasMovement = false;

        Vector3 forwardCamera = camera.transform.forward;
        Vector3 rightCamera = camera.transform.right;

        forwardCamera.y = 0.0f;
        rightCamera.y = 0.0f;

        forwardCamera.Normalize();
        rightCamera.Normalize();

        Vector3 movement = Vector3.zero;

        //Input
        if (canMove)
        {
            if (moveInput.y != 0)
            {
                movement += moveInput.y * forwardCamera;
                hasMovement = true;

            }
            if (moveInput.x != 0)
            {
                movement += moveInput.x * rightCamera;
                hasMovement = true;
                playerAnimator.SetFloat("MovementX", moveInput.x);
            }
        }

        //Movement 
        if (hasMovement)
        {
            //Strafing
            if (strafe == false)
            {
                Quaternion lookAtRotation = Quaternion.LookRotation(movement);
                capsule.rotation = Quaternion.Lerp(capsule.rotation, lookAtRotation, lerpRotationPct);               
            }

            if(strafe == true)
            {
                Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(camera.transform.forward.x, movement.y, camera.transform.forward.z));
                capsule.rotation = Quaternion.Lerp(capsule.rotation, lookAtRotation, lerpRotationPct);                          
            }

            //Animation speed
            if (playerInputMng.GetGamepadActive() == false)
            {
                if (speed < 0.5f && running == false)
                {
                    speed += 0.05f;
                }
                else if (running == false)
                {
                    if (speed > 0.6f)
                    {
                        speed -= 0.1f;
                    }
                    else
                    {
                        speed = 0.5f;
                    }
                }
            }

            if (running == true && zoom == false)
            {
                movementSpeed = runSpeed;
                if (speed < 1f)
                {
                    speed += 0.05f;
                }
                else
                {
                    speed = 1f;
                }
            }
            else if(zoom == true)
            {
                movementSpeed = zoomSpeed;
                if (speed > 0.6f)
                {
                    speed -= 0.1f;
                }
                else
                {
                    speed = 0.5f;
                }
            }
            else
            {
                movementSpeed = walkSpeed;
            }

            //Animation speed for gamepad
            if(playerInputMng.GetGamepadActive() == true)
            {
                if (speed < 0.5f)
                {
                    speed += 0.05f;
                }

                if (((moveInput.x >= 0.5f || moveInput.y >= 0.5f) || (moveInput.x <= -0.5f || moveInput.y <= -0.5f)) && zoom == false)
                {
                    movementSpeed = runSpeed;
                    if (speed < 1f)
                    {
                        speed += 0.05f;
                    }
                    else
                    {
                        speed = 1f;
                    }
                }
                else
                {
                    if (speed > 0.6f)
                    {
                        speed -= 0.1f;
                    }
                    else
                    {
                        //speed = 0.5f;
                    }
                }
            }

            if(swim.IsInWater())
            {
                movementSpeed = swimmingSpeed;
            }

            if(jumping == true)
            {
                if (speed > 0)
                {
                    speed -= 0.1f;
                }
                else
                {
                    speed = 0;
                }
            }
        }
        else
        {
            movementSpeed = 0;
            if (speed > 0)
            {
                speed -= 0.1f;
            }
            else
            {
                speed = 0;
                playerAnimator.SetFloat("MovementX", 0);
            }
        }    

        //Movement in the water
        if (swimmingUp && swim.IsInWater() == true)
        {
            Vector3 waterMovement = new Vector3(0, playerRigidbody.transform.up.y, 0);
            waterMovement += waterMovement * waterMovementSpeed;
            playerRigidbody.velocity = new Vector3(0, waterMovement.y, 0);
            playerAnimator.SetLayerWeight(2, 1);
        }
        if (swimmingDown && swim.IsInWater() == true)
        {
            Vector3 waterMovement = new Vector3(0, -playerRigidbody.transform.up.y, 0);
            waterMovement += waterMovement * waterMovementSpeed;           
            playerRigidbody.velocity = new Vector3(0, waterMovement.y, 0);
            playerAnimator.SetLayerWeight(2, 1);
            playerAnimator.SetTrigger("SwimDowm");
        }
        if(swim.IsInWater() == false) { }
        {
            playerAnimator.SetLayerWeight(2, 0);
        }

        // AQUI
        if (IsTouchingDifferentGround() == false || IsTouchingTheGround()) 
        { 
            movement += movement * movementSpeed;
            playerRigidbody.velocity = new Vector3(movement.x, playerRigidbody.velocity.y, movement.z);
            if (swim.isInWater == false)
            {
                playerRigidbody.AddForce(Physics.gravity, ForceMode.Force);
            }
        }

        //Jumping
        if (jumping == true)
        {
            playerRigidbody.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            playerAnimator.SetBool("Jump", true);
            if (currentJumpForce > 0)
            {
                currentJumpForce -= jumpIncrement * Time.fixedDeltaTime;
            }
        }

        //Jump buffer
        if(isFalling && jumpPressed)
        {
            currentBuffer += Time.fixedDeltaTime;
            if(currentBuffer <= jumpBuffer)
            {
                buffedJump = true;
            }
        }

        if(buffedJump)
        {
            if (IsTouchingTheGround())
            {
                playerRigidbody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
                jumpPressed = false;
                StartCoroutine(BuffedJumpDisable(0.2f));
            }
        }

        //Strafing

        if (strafe == true && hasMovement == false)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(new Vector3(camera.transform.forward.x, movement.y, camera.transform.forward.z));
            capsule.rotation = Quaternion.Lerp(capsule.rotation, lookAtRotation, lerpRotationPct);
        }

        //Camera zoom
        if (zoom == true)
        {
            cameraPos.position = Vector3.Lerp(cameraPos.position, zoomPosition.position, Time.deltaTime * 4);
            weight += 0.1f;
            if(weight >= 1)
            {
                weight = 1;
            }
            playerAnimator.SetLayerWeight(3, weight);
            playerAnimator.SetFloat("DirectionY", camera.GetComponent<CameraMovement2>().GetPitch());

            if (moveInput.x != 0)
            {
                playerAnimator.SetBool("SideMovement", true);
            }
            else
            {
                playerAnimator.SetBool("SideMovement", false);
                playerAnimator.SetFloat("MovementX", 0);
            }
        }
        if(zoom == false)
        {
            weight -= 0.1f;
            if(weight <= 0)
            {
                weight = 0;
            }
            playerAnimator.SetFloat("DirectionY", camera.GetComponent<CameraMovement2>().GetPitch());
            playerAnimator.SetLayerWeight(3, weight);
            cameraPos.position = Vector3.Lerp(cameraPos.position, cameraPosition.transform.position, Time.deltaTime * 6);
            playerAnimator.SetBool("SideMovement", false);
        }

        //Camera reset 

        if(camera.GetComponent<CameraMovement2>().hasToReset)
        {
            cameraPos.position = Vector3.Lerp(cameraPos.position, cameraIdle.transform.position, Time.deltaTime);
            //camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, cameraIdle.transform.rotation, Time.deltaTime * 4);
            //Quaternion targetRot = Quaternion.Lerp(camera.transform.rotation, cameraIdle.transform.rotation, Time.deltaTime * 3);
            //camera.transform.rotation = Quaternion.Euler(camera.transform.rotation.x, (float)targetRot.y, 0);
            //camera.transform.rotation = Quaternion.Lerp(new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w), new Quaternion(cameraIdle.transform.rotation.x, cameraIdle.transform.rotation.y, 0, camera.transform.rotation.w), Time.deltaTime * 6);
            camera.transform.LookAt(capsule.transform);
        }

        if(isInAir && isFalling == false && swim.isInWater == false)
        {
            //playerAnimator.SetBool("Jump", true);
        }

        CheckFall();

        if (IsTouchingTheGround())
        {
            currentTime = 0;
            playerAnimator.SetBool("OnGround", true);
            playerAnimator.SetBool("Jump", false);
            hasJumped = false;
            isInAir = false;
            currentBuffer = 0;
        }
        else
        {
            isInAir = true;
        }

        playerAnimator.SetFloat("Speed", speed);

        if (isBouncing)
        {
            currentBouncing -= Time.deltaTime;
            playerRigidbody.AddForce(-capsule.transform.forward * knockbackForce, ForceMode.Impulse);
        }

    }

    //Check falling
    void CheckFall()
    {
        playerAnimator.SetBool("OnGround", IsTouchingTheGround());
        if (playerRigidbody.velocity.y < fallDetection && swim.IsInWater() == false && !IsTouchingTheGround())
        {
                isFalling = true;
                playerRigidbody.AddForce(Vector3.down * fallForce, ForceMode.Force);
                currentTime += Time.deltaTime;
                playerAnimator.SetBool("Jump", false);
                playerAnimator.SetBool("Falling", true);
        }
        else
        {
            isFalling = false;
            isInAir = false;
            playerAnimator.SetBool("Falling", false);         
        }
    }

    bool IsTouchingTheGround() => Physics.CheckSphere(feetTransform.position, 0.25f, floorMask);
    bool IsTouchingDifferentGround() => Physics.CheckSphere(feetTransform.position, 0.25f, noneFloorMask);
    public bool IsTouchingWater() => Physics.CheckSphere(feetTransform.position, 0.1f, waterLayermask);

    //Enemies
    private void OnCollisionEnter(Collision collision)
    {
        if (playerLife.canTakeDamage) //Vulnerable or not
        {
            if (collision.transform.tag == "Enemy")
            {
                playerLife.TakeDamage(1);
                playerRigidbody.AddForce(transform.up * knockbackForce, ForceMode.Impulse);
                isBouncing = true;
                currentBouncing = 2f;
                canMove = false;
                int num = Random.Range(0, 2);
                playerAnimator.SetInteger("HitNum", num);
                playerAnimator.SetBool("Hit", true);
                StartCoroutine(EnableMovement(enableMoveTime));
            }

            if (collision.transform.tag == "Spikes")
            {
                playerLife.TakeDamage(1);
                playerRigidbody.AddForce(transform.up * knockbackForce, ForceMode.Impulse);
                //playerRigidbody.velocity = new Vector3(0, 0, 0);
                isBouncing = true;
                currentBouncing = 2f;
                canMove = false;
                int num = Random.Range(0, 2);
                playerAnimator.SetInteger("HitNum", num);
                playerAnimator.SetBool("Hit", true);
                StartCoroutine(EnableMovement(enableMoveTime));
            }

            if(collision.transform.tag == "Death")
            {
                playerLife.Die();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Death")
        {
            playerLife.Die();
        }

        if(other.tag == "EnemyProjectile")
        {
            playerRigidbody.AddForce(transform.up * 4f, ForceMode.Impulse);
            isBouncing = true;
            currentBouncing = 2f;
            canMove = false;
            int num = Random.Range(0, 2);
            playerAnimator.SetInteger("HitNum", num);
            playerAnimator.SetBool("Hit", true);
            StartCoroutine(EnableMovement(enableMoveTime));
        }
    }

    /* void OnAnimatorIK(int layerIndex)
     {
         if (playerAnimator)
         {
             playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
             playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

             playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
             playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

             // Left foot
             RaycastHit hit;
             Ray ray = new Ray(playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot), Vector3.down);
             Debug.DrawRay(playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down, Color.red);
             if (Physics.Raycast(ray, out hit, distanceToGround + 1f, detectionLayerMask))
             {
                 if (hit.transform.tag == "Floor")
                 {
                     Vector3 footPosition = hit.point;
                     footPosition.y += distanceToGround;
                     playerAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot).x, footPosition.y, playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot).z));
                     playerAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                     //playerAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(new Vector3(playerAnimator.GetIKRotation(AvatarIKGoal.LeftFoot).x, playerAnimator.GetIKRotation(AvatarIKGoal.LeftFoot).y, playerAnimator.GetIKRotation(AvatarIKGoal.LeftFoot).z), hit.normal));

                 }
             }

             // Right foot
             ray = new Ray(playerAnimator.GetIKPosition(AvatarIKGoal.RightFoot), Vector3.down);
             if (Physics.Raycast(ray, out hit, distanceToGround + 1f, detectionLayerMask))
             {
                 if (hit.transform.tag == "Floor")
                 {
                     Vector3 footPosition = hit.point;
                     footPosition.y += distanceToGround;
                     playerAnimator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(playerAnimator.GetIKPosition(AvatarIKGoal.RightFoot).x, footPosition.y, playerAnimator.GetIKPosition(AvatarIKGoal.RightFoot).z));
                     playerAnimator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                 }
             }
         }
     }*/

    //Getters
    public bool GetStrafe()
    {
        return strafe;
    }

    public bool GetZoom()
    {
        return zoom;
    }

    public bool GetWaterState()
    {
        return isInWater;
    }

    public void SetMove(bool move)
    {
        canMove = move;
    }

    IEnumerator EnableMovement(float l_Time)
    {
        yield return new WaitForSeconds(l_Time);
        canMove = true;
        isBouncing = false;
        playerAnimator.SetBool("Hit", false);
    }

    IEnumerator BuffedJumpDisable(float time)
    {
        yield return new WaitForSeconds(time);
        buffedJump = false;
    }
}
