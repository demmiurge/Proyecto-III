using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers;

    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (gameObject.GetComponent<Renderer>())
            gameObject.GetComponent<Renderer>().material = onMaterial;
    }

    public void Deactivate()
    {
        if (gameObject.GetComponent<Renderer>())
            gameObject.GetComponent<Renderer>().material = offMaterial;
    }
}
