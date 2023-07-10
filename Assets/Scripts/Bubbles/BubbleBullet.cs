using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

// Una vez creado, tengo que comprobar que haya un total de tres burbujas
// 1.- Comprobar la existencia de tres burbujas en escena, obviando que estén las tres activas

public class BubbleBullet : MonoBehaviour
{
    // Para superficies estáticas y móviles que me pueda adherir
    [SerializeField] private List<string> stickySurfaces;

    // Esto no debería de estar, pero es un parche al planteamiento de los enemigos
    [SerializeField] private List<string> immersionCollisionTags;

    private BubbleManagerV1 bubbleManager;

    [SerializeField] private float offsetDistance = 1.75f;

    private Rigidbody rb;

    private SphereCollider sphereCollider;

    [SerializeField] private float timeToStartDetectingCollisions = 0.01f;
    [SerializeField] private float timeToSelfDestruct = 2.5f;

    private ParticleSystemManager particleSystemManager;

    [Header("Events")] 
    [SerializeField] private UnityEvent colliderEvent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (bubbleManager == null) bubbleManager = GameManager.instance.GetBubbleManager();

        if (bubbleManager.AreAllTheBubblesActive())
        {
            // En caso de que estén todas las burbujas activas, te pediré que elimines la más antigua
            bubbleManager.HideTheOldestBubble();
        }
    }

    void Update()
    {
        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();
    }

    public void OnEnable()
    {
        sphereCollider.enabled = false;
        StartCoroutine(AvoidCollision(timeToStartDetectingCollisions));
        StartCoroutine(SelfDestructTimer(timeToSelfDestruct));
    }

    public IEnumerator AvoidCollision(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sphereCollider.enabled = true;
    }

    public IEnumerator SelfDestructTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        bool isValidSurface = false;

        // Comprobar que la superficie con la que me colisiono sea válida
        // En caso de no ser válida explotaré
        foreach (string stickySurface in stickySurfaces)
        {
            if (stickySurface == collision.gameObject.tag)
            {
                ContactPoint contact = collision.contacts[0];

                Vector3 collisionPoint = contact.point;
                Vector3 collisionNormal = contact.normal;

                Vector3 spawnPosition = collisionPoint + (collisionNormal * offsetDistance);

                // Para que no entre en la resolución externa
                isValidSurface = true;
                ValidSurface(collisionPoint, collisionNormal);
                
                CreateBubble(spawnPosition);
            }
        }

        if (isValidSurface == false)
            InvalidSurface(collision.contacts[0].point, collision.contacts[0].normal);

        colliderEvent?.Invoke();

        // En caso de ser una superficie válida o no, explotaré de todas formas
        Destroy(gameObject);
    }

    // Esto no debería de estar, pero es un parche al planteamiento de los enemigos
    private void OnTriggerEnter(Collider other)
    {
        foreach (string immersionCollisionTag in immersionCollisionTags)
        {
            if (immersionCollisionTag == other.tag)
            {
                DetectionOfAnEnemyCollider(transform.position);
                colliderEvent?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    void ValidSurface(Vector3 impactPosition, Vector3 surfaceNormal)
    {
        particleSystemManager.GenerateParticlesForTheBigBubble(impactPosition, surfaceNormal);
    }

    void InvalidSurface(Vector3 impactPosition, Vector3 surfaceNormal)
    {
        particleSystemManager.GenerateParticlesForInvalidSurface(impactPosition, surfaceNormal);
    }

    void DetectionOfAnEnemyCollider(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesForHitAnEnemy(impactPosition);
    }

    void CreateBubble(Vector3 spawnPosition)
    {
        GameObject bubble = bubbleManager.GetBubble();

        Rigidbody rbBubble = bubble.GetComponent<Rigidbody>();

        rbBubble.isKinematic = true;

        bubble.transform.position = spawnPosition;

        rbBubble.isKinematic = false;
    }
}
