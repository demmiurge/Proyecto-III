using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;

public class Swimming : MonoBehaviour
{
    public GameObject[] floatingPoints;
    public float underwaterLinearDrag = 3f;
    public float underwaterAngularDrag = 1f;

    public float airLinearDrag = 0f;
    public float airAngularDrag = 0.05f;

    public float floatingPower = 15f;

    public float waterHeight = 0f;

    public bool isInWater = false;

    [Header("Animator")]
    public Animator playerAnimator;

    [Header("Particles")]
    public ParticleSystem splash;
    public VisualEffect splashPrefab;
    public Transform body;

    public UnityEvent swim;
    public UnityEvent stopSwim;

    Rigidbody rigidbody;
    bool underwater = false;
    int floatersUnderwater;
    SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        floatersUnderwater = 0;
        if (isInWater == true)
        {
            /*for (int i = 0; i < floatingPoints.Length; i++)
            {
                float difference = floatingPoints[i].transform.position.y - waterHeight;

                if (difference < 0)
                {
                    //rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floatingPoints[i].position, ForceMode.Force);
                    //floatersUnderwater += 1;
                    if (!underwater)
                    {
                        underwater = true;
                        SwitchState(true);
                    }
                }
            }*/
            underwater = true;
            SwitchState(true);
        }

        if (isInWater == false)
        {
            if (underwater)
            {
                underwater = false;
                SwitchState(false);
            }
        }

        CheckFloaters();

    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            rigidbody.drag = underwaterLinearDrag;
            rigidbody.angularDrag = underwaterAngularDrag;
            rigidbody.useGravity = false;
            playerAnimator.SetBool("InWater", true);
            playerAnimator.SetBool("OnGround", false);
        }
        else
        {
            rigidbody.drag = airLinearDrag;
            rigidbody.angularDrag = airAngularDrag;
            rigidbody.useGravity = true;
            playerAnimator.SetBool("InWater", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bubble")
        {            
            waterHeight = other.GetComponent<SphereCollider>().bounds.ClosestPoint(transform.position).y;
            InstantiateParticles(-transform.forward);
            //splash.Emit(transform.position, -splash.transform.forward * 5f, splash.startSize, splash.startLifetime, Color.white);
            if (floatersUnderwater >= 2)
            {
                isInWater = true;
                rigidbody.useGravity = false;
                underwater = true;
            }
            swim?.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Bubble")
        {
            waterHeight = other.GetComponent<SphereCollider>().bounds.ClosestPoint(transform.position).y;
           
            if (floatersUnderwater >= 2)
            {
                isInWater = true;
                rigidbody.useGravity = false;
                underwater = true;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        //StartCoroutine(Exit());
        if (other.tag == "Bubble")
        {
            InstantiateParticles(transform.forward);
            swim?.Invoke();
        }
        
        StartCoroutine(Exit());
        //ExitWater();
    }

    void CheckFloaters()
    { 
        foreach (GameObject floater in floatingPoints)
        {
            if(floater.gameObject.GetComponent<CheckWater>().GetWaterState() == true)
            {
                floatersUnderwater += 1;
            }
        }
    }

    void InstantiateParticles(Vector3 vec)
    {
        VisualEffect effect = Instantiate(splashPrefab, new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z), body.rotation);
        //effect.SetVector3(3, vec);
        effect.Play();
        Destroy(effect.gameObject, 0.5f);
    }

    public IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.2f);
        ExitWater();
    }

    public void ExitWater()
    {
        isInWater = false;
        rigidbody.useGravity = true;
        playerAnimator.SetBool("InWater", false);
        floatersUnderwater = 0;
       
    }

    public bool IsInWater()
    {
        return isInWater;
    }

    public void SetWater(bool set)
    {
        isInWater = set;
    }
}
