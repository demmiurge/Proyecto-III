using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWater : MonoBehaviour
{
    public int floaterNum;
    private bool waterJump = false;
    private bool floaterInWater = false;
    [SerializeField] Swimming swimComponent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bubble")
        {
            floaterInWater = true;
            if (floaterNum == 3)
            {
                waterJump = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Bubble")
        {
            floaterInWater = true;
            if (floaterNum == 3)
            {
                waterJump = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        floaterInWater = false;

        if(floaterNum == 3)
        {
            waterJump = true;
        }
    }

    public bool GetWaterState()
    {
        return floaterInWater;
    }

    public bool GetWaterJump()
    {
        return waterJump;
    }
}
