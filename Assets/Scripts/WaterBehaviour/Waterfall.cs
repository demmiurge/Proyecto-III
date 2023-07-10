using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Waterfall : MonoBehaviour
{
    public UnityEvent water;
    public UnityEvent waterfall;

    bool invoked = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WaterfallStart();
    }

    void WaterfallStart()
    {
        if (invoked == false)
        {
            water?.Invoke();
            waterfall?.Invoke();
            Debug.Log("water");
            invoked = true;
        }
        else
            return;
    }
}
