using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BubbleBehaviourV1 : MonoBehaviour
{
    // Para superficies estáticas y móviles que me pueda adherir
    [SerializeField] private List<string> stickySurfaces;

    // Superficies que me pueden eliminar
    [SerializeField] private List<string> harmfulSurfaces;

    [SerializeField] private float sequentialHideTimer = 2.5f;

    //[SerializeField] private float timeOfLife = 10.0f;
    //private float restTime;

    // La lista que hará la burbuja de las colisiones con las que mantiene contacto
    private List<GameObject> connections;

    private Rigidbody rb;

    // Nos sirve para saber si está enganchada y no permitimos otra conexión nueva.
    private bool iAlreadyStuck;

    // Establecemos un semáforo para definir que esta burbuja se ha operado, para evitar el "stack overflow"
    private bool hasOperated;

    public bool GetIAlreadyStuck() => iAlreadyStuck;

    public bool GetHasOperated() => hasOperated;

    [Header("Bubble size")] 
    [SerializeField] private Vector3 smallScale;
    [SerializeField] private Vector3 largeScale;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private Transform targetTransform;
    private float elapsedTime = 0f;
    private bool desiredScaleAchieved;

    public List<string> GetStickySurfaces() => stickySurfaces;

    private ParticleSystemManager particleSystemManager;

    [Header("Events")] 
    [SerializeField] private UnityEvent createBubble;
    [SerializeField] private UnityEvent destroyBubble;

    void Awake()
    {
        // Inicializamos lo básico
        rb = GetComponent<Rigidbody>();
        connections = new List<GameObject>();
        iAlreadyStuck = false;
    }

    public void OnEnable()
    {
        NormalizeBubble();

        // restTime = timeOfLife;

        if (targetTransform) 
        {
            desiredScaleAchieved = false;

            targetTransform.gameObject.SetActive(true);
            targetTransform.parent = transform;
            targetTransform.localPosition = Vector3.zero;
            targetTransform.localScale = smallScale;

            elapsedTime = 0f; // Reiniciar el tiempo transcurrido
        }
    }

    public void OnDisable()
    {
        targetTransform.gameObject.SetActive(false);
    }

    private void IncreaseSize()
    {
        // Incrementar el tiempo transcurrido
        elapsedTime += Time.deltaTime;

        float animationProgress = elapsedTime / animationTime;

        float curveValue = scaleCurve.Evaluate(animationProgress);

        float scaleMultiplier = Mathf.Lerp(1f, curveValue, animationProgress);

        Vector3 newScale = Vector3.Lerp(smallScale, largeScale, animationProgress) * scaleMultiplier;

        targetTransform.localScale = newScale;

        if (animationProgress >= 1f)
            desiredScaleAchieved = true;
    }

    void Update()
    {
        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();

        if (desiredScaleAchieved == false) IncreaseSize();

        /*
        if (restTime >= 0)
            restTime -= Time.deltaTime;
        else
        {
            NotifyNeighboringBubbles();

            foreach (Transform child in gameObject.transform)
            {
                if (child.tag == "Player")
                {
                    child.parent = null;
                    child.GetComponent<Swimming>().ExitWater();
                }
            }

            destroyBubble?.Invoke();
            particleSystemManager.GenerateDestructionParticlesForTheLargeBubble(transform.position);
            gameObject.SetActive(false);
        }
        */
    }

    public void DestroyBubble()
    {
        destroyBubble?.Invoke();
        particleSystemManager.GenerateDestructionParticlesForTheLargeBubble(transform.position);
        gameObject.SetActive(false);
    }

    public void GetRidOfYourChildren()
    {
        foreach (Transform child in gameObject.transform)
            if (child.gameObject.layer != 9)
                child.parent = null;
    }

    // Desactivo el generador si lo tengo como compañero
    public void DisableGenerator()
    {
        foreach (GameObject connection in connections)
        {
            if (connection?.GetComponent<ElectricityGenerator>())
            {
                ElectricityGenerator electricityGenerator = connection.GetComponent<ElectricityGenerator>();
                electricityGenerator.ShutDownGenerator();
            }
        }
    }

    public void NotifyNeighboringBubbles()
    {
        // El "ToList()" evita llamadas innecesarias, que pueden evocar a un error
        foreach (GameObject connection in connections.ToList())
        {
            // Solo nos interesan las burbujas
            if (connection.tag == gameObject.tag)
            {
                // Obtenemos la burbuja vecina
                BubbleBehaviourV1 bubbleBehaviourV1 = connection.GetComponent<BubbleBehaviourV1>();

                // Le decimos que nos borre de su lista
                bubbleBehaviourV1.RemoveConnection(gameObject);

                // Solicitamos que comprueben nuevamente sus conexiones y refresquen su estado a ser necesario
                bubbleBehaviourV1.UpdateStatus(gameObject);
            }
        }
    }

    public void RemoveConnection(GameObject connection)
    {
        connections.Remove(connection);
    }

    public void UpdateStatus(GameObject bubble)
    {
        hasOperated = true;
        bool continueIterating = true;

        foreach (GameObject connection in connections)
            if (connection.tag != gameObject.tag)
                continueIterating = false;

        if (continueIterating)
        {
            foreach (GameObject connection in connections)
            {
                if (connection.tag == gameObject.tag && connection != bubble && connection.GetComponent<BubbleBehaviourV1>().GetHasOperated() == false)
                {
                    BubbleBehaviourV1 bubbleBehaviourV1 = connection.GetComponent<BubbleBehaviourV1>();
                    bubbleBehaviourV1.UpdateStatus(gameObject);
                }
            }
        }

        if (continueIterating)
            NormalizeBubbleBlow(true);
        else
            NormalizeBubble(true);
    }

    private void NormalizeBubble(bool setParentNull = false)
    {
        if (setParentNull) transform.SetParent(null);

        connections.Clear();

        iAlreadyStuck = false;

        rb.velocity = Vector3.zero;

        rb.isKinematic = false;
    }

    private void NormalizeBubbleBlow(bool setParentNull = false)
    {
        if (setParentNull) transform.SetParent(null);

        connections.Clear();

        iAlreadyStuck = false;

        rb.velocity = Vector3.zero;

        rb.isKinematic = false;

        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == "Player")
            {
                child.parent = null;
                child.GetComponent<Swimming>().ExitWater();
            }
        }

        destroyBubble?.Invoke();
        particleSystemManager.GenerateDestructionParticlesForTheLargeBubble(transform.position);
        gameObject.SetActive(false);
    }

    // En que momento hacemos el contacto, lo comprobamos
    private void OnTriggerEnter(Collider other)
    {
        // Primero compruebo la colisión, si es una superficie que pueda explotar la burbuja
        foreach (string harmfulSurface in harmfulSurfaces)
        {
            if (harmfulSurface == other.tag)
            {
                NotifyNeighboringBubbles();

                foreach (Transform child in gameObject.transform)
                {
                    if (child.tag == "Player")
                    {
                        child.parent = null;
                        child.GetComponent<Swimming>().ExitWater();
                    }
                }

                destroyBubble?.Invoke();
                particleSystemManager.GenerateDestructionParticlesForTheLargeBubble(transform.position);
                gameObject.SetActive(false);
            }
        }

        // Comprobamos que sea una superficie válida a la que me pueda enganchar
        // También miro si ya me había pegado antes a esta superficie
        foreach (string stickySurface in stickySurfaces)
        {
            // En caso de que la superficie sea válida, proseguimos con la validación
            if (stickySurface == other.tag)
            {
                // Comprobamos si ya la teníamos en la lista, para evitar la duplicidad
                foreach (GameObject connection in connections)
                    if (connection == other.gameObject)
                        return;

                // Si la validación previa es correcta y no la tenemos en nuestras conexiones, procederemos a agregarla
                // Dependiendo de si ya estoy parado y si es una burbuja la que me colisiona, casos posibles
                if (iAlreadyStuck && tag == other.tag) // 1.- Estoy pegado y me colisiona una burbuja
                {
                    // Compruebo si estando quieta me colisiona una burbuja amiga
                    connections.Add(other.gameObject);

                    // Sonido
                    createBubble?.Invoke();
                }
                if (iAlreadyStuck && tag != other.tag) // 2.- Estoy pegado y me colisiona una superficie adherible
                {
                    return;
                }
                if (iAlreadyStuck == false && tag == other.tag) // 3.- Estoy flotando y me colisiona una burbuja amiga
                {
                    // Le pregunto a la burbuja amiga si está enganchada
                    // Si enganchada, puedo adherirme a ella
                    BubbleBehaviourV1 bubbleBehaviourV1 = other.GetComponent<BubbleBehaviourV1>();

                    if (bubbleBehaviourV1.GetIAlreadyStuck())
                    {
                        hasOperated = false;

                        // Congelo las físicas del movimiento que tenía como burbuja
                        FreezeMyMovementAndPhysics();
                        // Congelo mi movimiento y me engancho a la superficie
                        HookMeOnSurface(other.gameObject);
                    }

                    // Sonido
                    createBubble?.Invoke();
                }
                if (iAlreadyStuck == false && tag != other.tag) // 4.- Estoy flotando y colisiono contra una superficie adherible
                {
                    hasOperated = false;

                    // Congelo las físicas del movimiento que tenía como burbuja
                    FreezeMyMovementAndPhysics();
                    // Congelo mi movimiento y me engancho a la superficie
                    HookMeOnSurface(other.gameObject);

                    // Sonido
                    createBubble?.Invoke();
                }
            }
        }
    }

    private void HookMeOnSurface(GameObject surface)
    {
        iAlreadyStuck = true;
        connections.Add(surface);
        transform.SetParent(surface.transform);
    }

    private void FreezeMyMovementAndPhysics()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }
}
