using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteBubbleDebug : MonoBehaviour
{
    [SerializeField] private KeyCode keyHide;
    [SerializeField] private BubbleManager bubbleManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyHide))
        {
            bubbleManager.HideBubble(gameObject);
        }
    }
}
