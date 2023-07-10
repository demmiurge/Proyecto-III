using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    //[SerializeField] private BubbleManagerV1 bubbleManager;

    //HOTFIX: NEW CODE!
    public Vector3 direction;

    public Transform target;
    public GameObject m_Particles;
    public float attractionForce = 10f;
    private float velocidadRotacion = 20f;

    private ParticleSystemManager particleSystemManager;

    //public GameObject enemy;

    private void Start()
    {       

        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();
    }

    void Update()
    {
        transform.Rotate(new Vector3(velocidadRotacion, velocidadRotacion, velocidadRotacion) * Time.deltaTime);

        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();

        if (target != null)
        {
            //Calcular la dirección hacia el objetivo

            //HOTFIX: COMMENTED!
            //Vector3 direction = (target.position - transform.position).normalized;

            //Aplicar una fuerza para atraer el proyectil hacia el objetivo
            GetComponent<Rigidbody>().AddForce(direction * attractionForce, ForceMode.Force);
        }
        m_Particles.SetActive(true);
        Destroy(gameObject, 2f);
    }


    private void OnTriggerEnter(Collider other)
    {
        m_Particles.SetActive(false);

        if (other.CompareTag("Bubble"))
        {
            //Destruir la burbuja
            //bubbleManager.HideBubble(gameObject);
            //other.gameObject.SetActive(false);
            //enemy.GetComponent<EnemigoAereoV2>().NullTarget();
            //Debug.Log("Null target by proyectile");

            DestroyProjectile(0);
        }

        if (other.CompareTag("Player"))
        {
            // Causar daño al jugador
            PlayerLife playerLife = target.GetComponent<PlayerLife>();

            if (playerLife.canTakeDamage)
            {
                //Debug.Log("Damage: " + damage);
                playerLife.TakeDamage(damage);
            }

            DestroyProjectile(0);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            DestroyProjectile(0);
        }
    }
    public void EnemyHurtParticles(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesEnemyHurt(impactPosition);
    }

    private void DestroyProjectile(float wait)
    {
        EnemyHurtParticles(transform.position);
        Destroy(gameObject, wait);
    }
}

