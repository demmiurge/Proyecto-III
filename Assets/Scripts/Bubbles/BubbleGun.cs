using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BubbleGun : MonoBehaviour
{
    [SerializeField] private Transform spawnBubbles;
    [SerializeField] private Transform rayCast;
    [SerializeField] private Camera camera;
    [SerializeField] private BubbleManager bubbleManager;

    private GameObject bubbleAux;

    [SerializeField] private KeyCode keyShoot = KeyCode.F;
    [SerializeField] private float shootHeight = 10f;
    private bool isShooting = false;

    // Start is called before the first frame update
    void Start()
    {
        isShooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 origin = rayCast.position;
        Vector3 direction = camera.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(origin, direction, out hitInfo))
        {
            if (Input.GetKeyDown(KeyCode.K) && hitInfo.transform.gameObject.tag == "Bubble")
            {
                bubbleManager.HideBubble(hitInfo.transform.gameObject);
            }
        }

        Debug.DrawRay(origin, direction * 10, Color.green);

        if (Input.GetKeyDown(keyShoot))
        {
            OnShoot();
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        GameObject bubble = bubbleManager.GetBubble();

        if (bubble == null) Debug.LogError("There was a problem in the BubbleManager, it didn't give me a bubble"); return;
    }

    void FixedUpdate()
    {
        if (isShooting)
        {
            //Vector3 forwardShot = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * camera.transform.forward;

            //GetComponent<Rigidbody>().AddForce(forwardShot, ForceMode.Impulse);
            //isShooting = false;


            // Obtener la posición y la rotación de la cámara
            Vector3 posicionCamara = camera.transform.position;
            Quaternion rotacionCamara = camera.transform.rotation;

            // Calcular la dirección en la que se está apuntando
            Vector3 shostDirection = rotacionCamara * Vector3.forward;

            // Disparar la bola en la dirección calculada
            bubbleAux.GetComponent<Rigidbody>().AddForce(shostDirection * 10f + Vector3.up * shootHeight, ForceMode.Impulse);

            isShooting = false;
            bubbleAux = null;
        }
    }

    public void OnShoot()
    {
        GameObject bubble = bubbleManager.GetBubble();

        bubbleManager.RefreshBubbleConnections(bubble);

        if (bubble == null)
        {
            Debug.LogError("There was a problem in the BubbleManager, it didn't give me a bubble"); 
            return;
        }

        Rigidbody rbBubble = bubble.GetComponent<Rigidbody>();

        rbBubble.isKinematic = true;

        bubble.transform.position = spawnBubbles.transform.position;

        rbBubble.isKinematic = false;

        bubbleAux = bubble;

        isShooting = true;
    }
}
