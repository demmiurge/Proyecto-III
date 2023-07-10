using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatforms : MonoBehaviour
{
    public Transform cameraPivot;
    private GameObject supposedParent;
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();    
        rb = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bubble")
        {
            transform.SetParent(other.transform, true);
            supposedParent = other.gameObject;
            transform.up = Vector3.up;
        }

        if (other.tag == "DecorativeWater")
        {
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Bubble")
        {
            // EN UN TRIGGER STAY NUNCA FORZAR A REPETIR COSAS SI NO SON DE MOVIMIENTO
            transform.SetParent(other.transform, true);
            supposedParent = other.gameObject;
            transform.up = Vector3.up;
        }

        /*if(other.tag == "DecorativeWater" && (rb.velocity.x != 0 || rb.velocity.z != 0) && Time.renderedFrameCount % 5 == 0)
        {
            int y = (int)rb.rotation.y;
            playerMovement.WaterTrail(y - 90, y + 90, 3, 5, 0.3f, 0.5f);
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        transform.SetParent(null);
        supposedParent = null;
        //transform.rotation = Quaternion.Euler(rotation.x, transform.rotation.y, rotation.z);
        //cameraPivot.rotation = Quaternion.Euler(pivotRotation.x, pivotRotation.y, pivotRotation.z);

        if (other.tag == "DecorativeWater")
        {
           
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckSurface(collision);
        transform.up = Vector3.up;
    }

    void OnCollisionStay(Collision collision)
    {
        CheckSurface(collision);
        transform.up = Vector3.up;
    }

    private void CheckSurface(Collision collision)
    {
        if (supposedParent != null && supposedParent.tag == "Bubble") return;

        if (collision.transform.tag == "MobilePlatform" && supposedParent?.tag != "Bubble")
        {
            if (supposedParent == null)
            {
                transform.SetParent(collision.transform, true);
                supposedParent = collision.gameObject;
            }
            if (supposedParent != collision.gameObject)
            {
                transform.SetParent(collision.transform, true);
                supposedParent = collision.gameObject;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (supposedParent?.tag == "Bubble") return;
        transform.SetParent(null);
        supposedParent = null;
        //transform.rotation = Quaternion.Euler(rotation.x, transform.rotation.y, rotation.z);
        //cameraPivot.rotation = Quaternion.Euler(pivotRotation.x, pivotRotation.y, pivotRotation.z);
    }
}
