using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [Header("Basic settings of life")]
    public int maxLife = 3;
    public int minLife = 1;
    public int currentLife;

    [Header("Settings when hit")]
    [SerializeField] private string healingPackTag = "Heart";
    [SerializeField] private float visualInputDuration = 1.0f;
    [SerializeField] public float invulnerableTime = 1.0f;

    [Header("Other settings")]
    public bool canTakeDamage = true;

    private LifeSystemUI lifeUI;
    private PlayerResetter playerResetter;
    private PlayerMovement playerMovement;
    private PlayerUpdateMaterial playerUpdateMaterial;
    private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = maxLife;
        playerUpdateMaterial = GetComponent<PlayerUpdateMaterial>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (lifeUI == null) lifeUI = GameManager.instance.GetLifeSystemUI();
        if (playerResetter == null) playerResetter = GameManager.instance.GetPlayerResetter();
        if(currentLife == 0) playerMovement.SetMove(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == healingPackTag)
        {
            AddLife();
            other.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentLife -= damage;

        // Llamamos al script que realiza el input visual
        playerUpdateMaterial.StartInterpolation(visualInputDuration);

        if (currentLife < minLife)
        {
            Die();
        }
        else
        {
            lifeUI.SetDamage(damage);
            canTakeDamage = false;

            StartCoroutine(ActivateVulnerability());
        }
    }

    public void AddLife(int life = 1)
    {
        currentLife += life;

        if(currentLife > maxLife)
            currentLife = maxLife;
        
        lifeUI.AddHearts(life);
    }

    public void Die()
    {
        // Aplicamos los cambios y agregamos la animación que sea necesaria
        currentLife = 0;
        lifeUI.RemoveAllHearts();
        // TODO Agregar animación de muerte del jugador
        playerAnimator.SetTrigger("Death");
        playerAnimator.SetBool("Dead", true);
        // Notificamos que el jugador ha muerto, para que se realicen todos los pasos necesarios
        playerResetter.PlayerDeath();
    }

    void ResetLives()
    {
        currentLife = maxLife;
    }

    public void RestartLives()
    {
        currentLife = maxLife;
        lifeUI.RestarHearts();
    }

    IEnumerator ActivateVulnerability()
    {
        yield return new WaitForSeconds(invulnerableTime);
        canTakeDamage = true;
    }
}
