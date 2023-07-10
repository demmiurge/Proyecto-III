using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueBehavior : MonoBehaviour
{
    [SerializeField] private SphereCollider surfaceToHook;

    public List<GameObject> connections;

    [SerializeField] private List<string> stickySurfaces;

    private Rigidbody rb;
    private bool imStuck { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        connections = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
        imStuck = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetStuck() { return imStuck; }

    public List<GameObject> GetConnections() { return connections; }

    void HookBubble() // Cuando enganchamos la bola a una superficie adherirle, detenemos totalmente su f�sica de movimiento
    {
        imStuck = true;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void ReleaseBubble() // Liberamos la bola de la congelaci�n en las f�sicas
    {
        imStuck = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
    }

    public void CollisionWithThisSurface(GameObject surface)
    {
        bool isClone = false;
        string surfaceTag = surface.tag;

        Debug.LogWarning("Need to refactor this section");

        // Compruebo si mi vecino es una burbuja y est� enganchado
        if (surfaceToHook.tag == surfaceTag)
            if (surface.transform.parent.gameObject.GetComponent<GlueBehavior>().GetStuck() == false)
                return;

        if (stickySurfaces.Contains(surfaceTag)) // Puede surgir alg�n problema, pero no deber�a de suceder
        {
            foreach (GameObject connection in connections)
                if (connection == surface)
                    isClone = true;

            if (isClone) return;

            connections.Add(surface);
            HookBubble();
        }
    }

    public void RemoveItemList(GameObject bubble)
    {
        connections.Remove(bubble);
    }

    public void CheckConnections()
    {
        bool isStucked = false;

        if (connections.Count > 0)
        {
            HookBubble();
        }
        else
        {
            ReleaseBubble();
        }
    }

    public void ReleaseSurfaceContact(GameObject surface)
    {
        // Detectamos si el objeto estaba entre nuestras conexiones
        for (int i = 0; i < connections.Count; i++)
            if (connections[i] == surface)
                connections.RemoveAt(i);

        //if (surface.tag == "Bubble")
        //{
        //    var test = surface;

        //    if (surface.transform.parent.gameObject.activeSelf == false)
        //    {
        //        Debug.Log("ReleaseBubble");
        //        ReleaseBubble();
        //    }

        //    if (surface.transform.parent.gameObject.activeSelf)
        //    {
        //        if (surface.transform.parent.gameObject.GetComponent<GlueBehavior>().GetStuck() == false)
        //        {
        //            Debug.Log("ReleaseBubble");
        //            ReleaseBubble();
        //        }
        //    }
        //}

        // Compruebo que no se haya roto el puente de burbujas
        // Primero me miro a m�
        //foreach (GameObject connection in connections)
        //{
        //    if (connection.tag == "Bubble")
        //        bubble = connection;

        //    if (connection.tag != "Bubble")
        //        solidSurface = true;
        //}

        //if (solidSurface == false)
        //{
        //    ReleaseBubble();
        //}
    }
}
