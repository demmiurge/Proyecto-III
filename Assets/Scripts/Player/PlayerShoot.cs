using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Bubbles")]
    public Transform spawnBubbles;
    public float shootForce = 25f;
    public bool shootBubbles = true;
    public string playerName;
    private BubbleManagerV1 bubbleManager;
    [Header("Animator")]
    public Animator playerAnimator;
    public Animator gunAnimator;

    [Header("Camera")]
    public Camera camera;

    //Private Variables
    private GameObject bubbleAux;
    private bool isShooting = false;
    private PlayerMovement playerMovement;
    private Swimming swimming;

    [Header("Bullet")] 
    [SerializeField] private GameObject bullet;

    [Header("PreviewShoot")]
    [SerializeField] private PreviewShoot previewShoot;


    // Agregar un temporizador para volver a disparar
    [Header("Cooldown")]
    [SerializeField] private float fireRate = 1.0f;
    float nextFireTime;


    // Start is called before the first frame update
    void Start()
    {
        isShooting = false;
        playerMovement = GetComponent<PlayerMovement>();
        swimming = GetComponent<Swimming>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bubbleManager == null) bubbleManager = GameManager.instance.GetBubbleManager();
    }

    public void OnChangeWeapon(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            shootBubbles = !shootBubbles;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextFireTime)
        {
            if (playerMovement.GetStrafe() == true && playerMovement.GetZoom() == true && swimming.IsInWater() == false)
            {
                Vector3 shootDirection = previewShoot.GetDirection();

                GameObject bullet = Instantiate(this.bullet, spawnBubbles.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody>().AddForce(shootDirection * shootForce, ForceMode.Impulse);

                isShooting = false;
                bubbleAux = null;

                playerAnimator.SetTrigger("Shoot");
                gunAnimator.SetTrigger("Shoot");
            }

            nextFireTime = Time.time + fireRate;
        }
    }

    public void OnDestroyBubble(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (playerMovement.GetZoom())
            {
                Vector3 screeenCenter = new Vector3(0.5f, 0.5f, 0);
                Ray cameraCenter = camera.ViewportPointToRay(screeenCenter);
                Vector3 shootDirection = cameraCenter.direction;
                Vector3 direction = camera.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(camera.transform.position, direction, out hitInfo))
                {
                    if (hitInfo.transform.gameObject.tag == "Bubble")
                    {
                        bubbleManager.HideBubble(hitInfo.transform.gameObject);
                        playerAnimator.SetTrigger("Destroy");

                        Transform playerInBubble = transform.Find(playerName);
                        if (playerInBubble != null)
                        {
                            playerInBubble.SetParent(null);
                            swimming.SetWater(false);
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {

    }
}
