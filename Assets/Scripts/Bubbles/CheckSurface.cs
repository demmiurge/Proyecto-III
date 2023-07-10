using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSurface : MonoBehaviour
{
    [SerializeField] private GlueBehavior glueBehavior;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        glueBehavior.CollisionWithThisSurface(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        glueBehavior.ReleaseSurfaceContact(other.gameObject);
    }
}
