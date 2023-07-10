using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStates : MonoBehaviour
{
    // El script nos permitirá inyectar información
    [SerializeField] private List<string> bubbleStates;
    private BubbleBehaviourV1 bubbleBehaviour;

    private List<string> stickySurfaces;

    // Start is called before the first frame update
    void Start()
    {
        stickySurfaces = new List<string>();
        bubbleBehaviour = GetComponent<BubbleBehaviourV1>();

        stickySurfaces = bubbleBehaviour.GetStickySurfaces();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == tag)
        {

        }
    }
}
