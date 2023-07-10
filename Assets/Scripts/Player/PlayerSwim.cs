using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwim : MonoBehaviour
{
    [Header("Points")]
    public Transform[] floatingPoints;
    [Header("Underwater Drag")]
    public float underwaterLinearDrag = 4f;
    public float underwaterAngularDrag = 1.5f;
    [Header("Surface Drag")]
    public float airLinearDrag = 0f;
    public float airAngularDrag = 0.05f;
    [Header("Floating")]
    public float floatingPower = 15f;
    public float waterHeight = 0f;
    public bool isInWater = false;

    //Private variables
    Rigidbody playerRigidbody;
    private bool underwater = false;
    private bool diving = false;
    private int floatersUnderwater;
    SphereCollider sphereCollider;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
