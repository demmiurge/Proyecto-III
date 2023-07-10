using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleBehaviour : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private List<string> stickySurfaces;
    [SerializeField] private List<string> stickyMobileSurfaces;

    private List<GameObject> connections;

    public bool isStucked = false; // Variable que me permite comunicarme con mis compañeras burbujas

    public bool checkingSystem;

    private SphereCollider sphereCollider;

    void Awake()
    {
        //
        sphereCollider = GetComponent<SphereCollider>();
    }

    public List<string> GetStickySurfaces() => stickySurfaces;

    // Start is called before the first frame update
    void Start()
    {
        // Inicializamos la lista de conexiones
        connections = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
        checkingSystem = true;
    }

    public IEnumerator Winton(float banana)
    {
        yield return new WaitForSeconds(banana);
        sphereCollider.enabled = true;
    }

    public void OnEnable()
    {
        sphereCollider.enabled = false;
        StartCoroutine(Winton(0.005f));
    }

    public List<GameObject> GetConnections() => connections;

    // Update is called once per frame
    void Update()
    {
        // Testeo de los comportamientos básicos
        if (Input.GetKeyDown(KeyCode.Keypad1)) // Simulación para engancharse
            Adhere();

        if (Input.GetKeyDown(KeyCode.Keypad2)) // Simulación para el desacople
            Release();
    }

    void LateUpdate()
    {
        CheckConnections();
    }

    private void CheckConnections()
    {
        if (connections.Count == 0)
        {
            CanIFreeMyself();
        }
    }

    public void ResetStates()
    {
        transform.parent = null;
        tag = "Bubble";
        connections.Clear();
        Release();
    }

    void Adhere()
    {
        isStucked = true;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void Release()
    {
        isStucked = false;

        rb.isKinematic = false;

        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkingSystem == false) return;

        // Comprobamos que sea una superficie válida a la que me pueda enganchar
        // También miro si ya me había pegado antes a esta superficie
        if (CanJoin(other.tag, stickySurfaces) && AlreadyHave(other.gameObject, connections))
        {
            connections.Add(other.gameObject);
        }

        // Si la conexión es móvil, no solo nos adherimos, sino que también nos hacemos hijos de ella
        if (CanJoin(other.tag, stickyMobileSurfaces) && AlreadyHave(other.gameObject, connections))
        {
            Debug.LogWarning("GUILLE ARREGLARLO");
            //connections.Add(other.gameObject);
            //transform.SetParent(other.gameObject.transform);
            //gameObject.tag = "MobileBubble";
        }

        // Compruebo si debo engancharme
        CheckToMerge();
    }

    private void OnTriggerExit(Collider other)
    {
        if (checkingSystem == false) return;

        // No hago ningún tipo de comprobación, simplemente compruebo que lo tenga en la lista para borrarlo
        // RemoveFromMyConnections(other.gameObject);
        RemoveComplexedRestForThePhysicsEngine(0f, other);

        // Compruebo si debo liberarme o engancharme
        CheckConnectionsRestForThePhysicsEngine(0f);
        // CanIFreeMyself();
    }

    private bool CanJoin(string tag, List<string> surfaces)
    {
        foreach (string stickySurface in surfaces)
            if (stickySurface == tag) return true;

        return false;
    }

    private bool AlreadyHave(GameObject surface, List<GameObject> listConnections)
    {
        foreach (GameObject connection in listConnections)
            if (connection == surface)
                return false;

        return true;
    }

    private void RemoveFromMyConnections(GameObject surface)
    {
        // Comprobamos que exista en nuestra lista
        for (int i = 0; i < connections.Count; i++)
        {
            // Si existe lo borramos, si no existe, no haremos nada
            if (connections[i] == surface)
            {
                // Debug.Log("Size list: " + connections.Count + " | Remove: " + surface.name);
                connections.RemoveAt(i);
                // Debug.Log("Size list before remove item: " + connections.Count);
            }
        }
    }

    IEnumerator CheckConnectionsRestForThePhysicsEngine(float time)
    {
        yield return new WaitForSeconds(time);
        CanIFreeMyself();
    }

    IEnumerator RemoveComplexedRestForThePhysicsEngine(float time, Collider other)
    {
        yield return new WaitForSeconds(time);
        RemoveFromMyConnections(other.gameObject);
    }

    // Dejo actuar al motor de físicas por sus cálculos y luego hago una comprobación
    public void CanIFreeMyself()
    {
        // Me preparo un variable del tipo semáforo, para saber si me tengo que desacoplar
        bool solidSurface = false;

        foreach (GameObject connection in connections)
        {
            if (connection.tag != tag)
            {
                solidSurface = true;
            }

            // Si colisiono con una compañera le pregunto si está pegada a una superficie
            if (solidSurface == false && connection.tag == tag)
            {
                // Comprobamos que esté activa en la escena
                if (connection.activeSelf)
                {
                    // Obtenemos su componente de comportamiento
                    BubbleBehaviour bubbleCompanion = connection.GetComponent<BubbleBehaviour>();

                    // Si está bloqueada, pues nos engancharemos a ella
                    if (bubbleCompanion.isStucked)
                    {
                        solidSurface = true;
                    }
                }
            }
        }

        if (solidSurface == false)
        {
            Release();
        }
    }

    // Me encargo de revisar mis conexiones, para ver si me puedo desenganchar
    public void CheckToMerge()
    {
        // Me preparo un variable del tipo semáforo, para saber si me tengo que fusionar
        bool solidSurface = false;

        // Me preocupo por las otras superficies, las que son burbujas, las ignoro por el momento
        foreach (GameObject connection in connections)
        {
            if (connection.tag != tag)
            {
                solidSurface = true;
            }

            // Si colisiono con una compañera le pregunto si está pegada a una superficie
            if (solidSurface == false && connection.tag == tag)
            {
                // Comprobamos que esté activa en la escena
                if (connection.activeSelf)
                {
                    // Obtenemos su componente de comportamiento
                    BubbleBehaviour bubbleCompanion = connection.GetComponent<BubbleBehaviour>();

                    // Si está bloqueada, pues nos engancharemos a ella
                    if (bubbleCompanion.isStucked)
                    {
                        solidSurface = true;
                    }
                }
            }
        }

        // Si no tengo ninguna superficie sólida, pues me desengancho
        // Pero si tengo una de válida, pues me engancho
        if (solidSurface)
        {
            Adhere();
        }
    }
}
