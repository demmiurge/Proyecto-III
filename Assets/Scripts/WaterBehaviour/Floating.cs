using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Floating : MonoBehaviour
{
    public Transform[] floatingPoints;

    public float underwaterLinearDrag = 3f;
    public float underwaterAngularDrag = 1f;

    public float airLinearDrag = 0f;
    public float airAngularDrag = 0.05f;

    public float floatingPower = 15f;

    public float waterHeight = 0f;

    Rigidbody rigidbody;
    bool underwater = false;
    int floatersUnderwater;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        floatersUnderwater = 0;
        for(int i = 0; i < floatingPoints.Length; i++)
        {
            float difference = floatingPoints[i].position.y - waterHeight;

            if (difference < 0)
            {
                rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floatingPoints[i].position, ForceMode.Force);
                floatersUnderwater += 1;
                if (!underwater)
                {
                    underwater = true;
                    SwitchState(true);
                }
            }
        }
        
        if(underwater && floatersUnderwater == 0)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if(isUnderwater)
        {
            rigidbody.drag = underwaterLinearDrag;
            rigidbody.angularDrag = underwaterAngularDrag;
        }
        else
        {
            rigidbody.drag = airLinearDrag;
            rigidbody.angularDrag= airAngularDrag;
        }
    }
}
