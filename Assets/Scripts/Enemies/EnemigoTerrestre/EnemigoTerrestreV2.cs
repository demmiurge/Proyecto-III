using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class EnemigoTerrestreV2 : MonoBehaviour
{
    [Header("State Machine")]
    public TState m_State;
    public TState m_LastState;

    [Header("Movement Components")]
    public NavMeshAgent m_NavMeshAgent;
    public Rigidbody m_Rigidbody;

    [Header("Parameters")]
    public List<Transform> m_PatrolTargets;
    private int m_CurrentPatrolTargetId = 0;
    public float m_WalkSpeed = 3.0f;
    public float m_RunSpeed = 6.0f;
    public int damage = 1;
    public float m_HearRangeDistance = 6.0f;
    public float m_VisualConeAngle = 80f;
    public float m_SightDistance = 8.0f;
    public LayerMask m_SightLayerMask;

    public float m_AlertRotation = 360f;
    public float m_AlertRotationSpeed = 120f;

    public float m_MinDistanceToPlayer = 1f;
    public float m_MaxDistanceToPlayer = 8f;
    public float m_StunTime = 5f;

    [Header("Animation")]
    public Animator m_Animator;
    private SkinnedMeshRenderer m_Renderer;
    //public FadeObject m_Fader;

    [Header("Reset")]
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;

    [Header("Attack")]
    public Transform m_Player;
    public bool m_CanDealDamage = true;
    public float m_DamageDeactivationTime = 1.0f;
    public float m_AttackPositionOffset = 1.2f;
    public float m_WaitToAttackTime = 2.0f;
    public LayerMask m_AttackLayerMask;
    private Vector3 m_DashDirection;
    private Vector3 m_PlayerDashPosition;
    private Vector3 m_DashDestination;

    [Header("Stun")]
    public GameObject m_StunStars;
    public GameObject m_StunParticles;
    public float m_BounceForce = 2f;
    private Vector3 m_BounceDirection;
    public GameObject m_ParticlesStunSpawn;
    public Material m_DefaultMaterial;
    public Material m_StunMaterial;

    [Header("Audio")]
    public GameObject m_AllSounds;
    public GameObject m_BaseSoundEmitter;
    public GameObject m_DashSoundEmitter;
    public GameObject m_HitSoundEmitter;
    public GameObject m_DieSoundEmitter;
    //public GameObject m_AlertSoundEmitter;


    private bool m_IsBounced = false;
    private bool m_HasAttack = false;
    private bool m_AmIStunned = false;
    private bool m_PlayerNear = false;

    private Vector3 m_OffsetDie = new Vector3(0, 0.5f, 0);

    private float num = 0;

    private ParticleSystemManager particleSystemManager;

    public enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        ATTACK,
        STUN
    }

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_Animator = gameObject.GetComponent<Animator>();
        m_SightLayerMask = LayerMask.NameToLayer("Everything");
        m_AttackLayerMask = LayerMask.NameToLayer("Everything");
        m_Renderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnEnable()
    {
        //GameController.GetGameController().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        //m_BounceDirection = new Vector3(0, 0, 0);

        SetIdleState();

        //m_BounceCollider.enabled = false;
        SoundOff();
        StarsOff();
        StunParticlesOff();
    }

    void Start()
    {
        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();
    }

    private void Update()
    {
        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();

        CheckForPlayerNear();

        if (m_PlayerNear) { m_AllSounds.SetActive(true); }
        
        if (m_PlayerNear == false) { SoundOff(); }

        switch (m_State)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.STUN:
                UpdateStunState();
                break;
        }

    }

    void SetIdleState()
    {
        m_State = TState.IDLE;
    }

    void UpdateIdleState()
    {
        SetPatrolState();
    }

    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_LastState = m_State;
        m_NavMeshAgent.isStopped = false;
        //Debug.Log("State: " + m_State);
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetId].position;
        m_Animator.SetBool("Alert", false);
        m_Animator.SetBool("Patrol", true);
        ChangeSound(m_BaseSoundEmitter, /*m_AlertSoundEmitter,*/ m_DashSoundEmitter, m_HitSoundEmitter);
    }

    void UpdatePatrolState()
    {
        //CheckForPlayerNear();

        //if (m_PlayerNear) { m_AllSounds.SetActive(true); }

        //if (m_PlayerNear == false) { m_AllSounds.SetActive(false); }

        if (TargetPositionArrived())
        {
            MoveToNextPatrolPosition();
        }

        if (HearsPlayer() || SeesPlayer())
        {
            m_Animator.SetBool("Patrol", false);
            m_Animator.SetTrigger("Detect");
            SetAlertState();
        }
    }

    private bool TargetPositionArrived()
    {
        return !m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
    }

    private void MoveToNextPatrolPosition()
    {
        ++m_CurrentPatrolTargetId;
        if (m_CurrentPatrolTargetId >= m_PatrolTargets.Count)
        {
            m_CurrentPatrolTargetId = 0;
        }
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetId].position;
    }

    bool HearsPlayer()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_PlayerPosition = m_Player.transform.position;
        return Vector3.Distance(l_PlayerPosition, transform.position) <= m_HearRangeDistance;
    }

    bool SeesPlayer()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_PlayerPosition = m_Player.transform.position;
        Vector3 l_DirectionTargetXZ = l_PlayerPosition - transform.position;
        l_DirectionTargetXZ.y = 0.0f;
        l_DirectionTargetXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.y = 0.0f;
        l_ForwardXZ.Normalize();

        Ray l_Ray = new Ray(transform.position, l_DirectionTargetXZ);

        return Vector3.Distance(l_PlayerPosition, transform.position) < m_SightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionTargetXZ) > Mathf.Cos(m_VisualConeAngle * Mathf.Deg2Rad / 2.0f)
            && !Physics.Raycast(l_Ray, m_SightLayerMask);
    }

    void SetAlertState()
    {
        m_State = TState.ALERT;       
        //Debug.Log("State: " + m_State);
        m_NavMeshAgent.speed = m_WalkSpeed;
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Alert", true);
        //ChangeSound(m_AlertSoundEmitter, m_BaseSoundEmitter, m_DashSoundEmitter, m_HitSoundEmitter, m_DieSoundEmitter);
        m_NavMeshAgent.isStopped = true;
        m_LastState = m_State;
    }

    void UpdateAlertState()
    {
        float l_Rotation = m_AlertRotationSpeed * Time.deltaTime;
        if (m_AlertRotation > l_Rotation)
        {
            m_AlertRotation -= l_Rotation;
        }
        else //Si no ve a nadie
        {
            m_NavMeshAgent.isStopped = false;           
            SetPatrolState();
            m_AlertRotation = 360f;
        }
        transform.Rotate(0, l_Rotation, 0);

        if (SeesPlayer())
        {
            m_NavMeshAgent.isStopped = false;
            SetAttackState();
        }
    }

    bool PlayerBeyondMaxDistance()
    {
        return Vector3.Distance(m_Player.transform.position, transform.position) > m_MaxDistanceToPlayer;
        //return Vector3.Distance(GameController.GetGameController().GetPlayer().transform.position, transform.position) > m_MaxDistanceToPlayer;
    }


    void SetAttackState()
    {
        m_State = TState.ATTACK;
        m_Animator.SetBool("Alert", false);
        m_LastState = m_State;
        m_HasAttack = false;
        //Debug.Log("State: " + m_State);
        m_NavMeshAgent.speed = m_RunSpeed;
        SetNextAttackPosition();
    }

    void SetNextAttackPosition()
    {
        num++;
        //Debug.Log(num.ToString());

        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        m_PlayerDashPosition = m_Player.transform.position;
        m_DashDirection = m_PlayerDashPosition - transform.position;
        m_DashDirection.Normalize();
        m_DashDestination = m_PlayerDashPosition + (m_DashDirection * m_AttackPositionOffset);
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.SetDestination(m_DashDestination);
        //m_NavMeshAgent.isStopped = true;
        //m_NavMeshAgent.enabled = false;
        //Vector3 l_dash = m_DashDestination * m_RunSpeed * Time.deltaTime;
        //transform.position += l_dash;
        m_Animator.SetBool("Attack", true);
        ChangeSound(m_DashSoundEmitter, /*m_AlertSoundEmitter,*/ m_BaseSoundEmitter, m_HitSoundEmitter);

    }

    void UpdateAttackState()
    {
        num += Time.deltaTime;

        //StartCoroutine(EnemyFootStepsParticles(m_ParticlesFootStepSpawn.transform.position, m_ParticlesFootStepSpawn2.transform.position));
        //Debug.Log("Dash Destination = " + m_DashDestination.ToString());

        if ((Vector3.Distance(transform.position, m_DashDestination) < m_MinDistanceToPlayer) && !PlayerBeyondMaxDistance())
        {
            m_DashSoundEmitter.SetActive(false);
            m_NavMeshAgent.isStopped = true;
            m_Animator.SetBool("Attack", false);
            m_Animator.SetTrigger("Wait");

            if (m_HasAttack == false)
            {
                m_HasAttack = true;
                StartCoroutine(WaitToAttack());
            }

            //Debug.Log("Idle");
        }

        /*if (TargetPositionArrived() && !PlayerBeyondMaxDistance())
        {
            //m_Animator.SetTrigger("Detect");
            m_NavMeshAgent.isStopped = true;
            StartCoroutine(WaitToAttack());
        }*/

        if (PlayerBeyondMaxDistance() || num > 30f)
        {
            num = 0;
            m_NavMeshAgent.speed = m_WalkSpeed;
            SetAlertState();
        }
    }

    public IEnumerator WaitToAttack()
    {

        //num++;
        //Debug.Log(num.ToString());
        //m_Animator.SetTrigger("Idle");
        yield return new WaitForSeconds(m_WaitToAttackTime);
        m_HasAttack = false;
        SetNextAttackPosition();
        
        //m_NavMeshAgent.enabled = true;       
    }

    void SetStunState()
    {
        if (m_AmIStunned == false)
        {
            m_AmIStunned = true;
            gameObject.tag = tag.Replace("Enemy", "Untagged");
            m_State = TState.STUN;
            m_LastState = m_State;
            //Debug.Log("State: " + m_State);
            m_NavMeshAgent.speed = 0;
            m_NavMeshAgent.isStopped = true;
            //transform.position = transform.position;
            //Debug.Log("Stun");
            m_Animator.SetBool("Stun", true);
            m_Animator.SetBool("Patrol", false);
            m_Animator.SetBool("Alert", false);
            m_Animator.SetBool("Attack", false);
            StarsOn();
            StunParticlesOn();
            ChangeStunMaterial();
            ChangeSound(m_DieSoundEmitter, /*m_AlertSoundEmitter,*/ m_DashSoundEmitter, m_BaseSoundEmitter);
            StartCoroutine(EndStun());
        }
    }

    void UpdateStunState()
    {
    }

    private IEnumerator EndStun()
    {
        yield return new WaitForSeconds(m_StunTime);
        m_AmIStunned = false;

        //m_NavMeshAgent.enabled = true;
        gameObject.tag = tag.Replace("Untagged", "Enemy");
        m_Rigidbody.constraints = RigidbodyConstraints.None;
        m_NavMeshAgent.speed = m_WalkSpeed;
        m_NavMeshAgent.isStopped = false;
        m_Rigidbody.isKinematic = true;

        /*if (SeesPlayer())
        {
            SetAttackState();
        }*/
        m_DieSoundEmitter.SetActive(false);
        StarsOff();
        StunParticlesOff();
        m_Animator.SetBool("Stun", false);
        ChangeDefaultMaterial();
        m_IsBounced = false;
        SetAlertState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "BubbleBullet")
        {
            if (other.gameObject.tag == "Wall")
            {              
                RaycastHit hit;
                //m_NavMeshAgent.enabled = false;

                if (Physics.Raycast(transform.position, other.transform.position - transform.position, out hit) && !m_IsBounced)
                {
                    m_NavMeshAgent.speed = 0;
                    m_NavMeshAgent.isStopped = true;
                    m_Rigidbody.isKinematic = false;
                    //m_BounceCollider.enabled = true;

                   Vector3 collisionNormal = hit.normal;

                    //Calcular la direcci�n de rebote opuesta a la pared
                    m_BounceDirection = Vector3.Reflect(transform.forward, collisionNormal);
                    m_BounceDirection = m_BounceDirection + (Vector3.up * 2f);
                    //Debug.Log("Direction: " + m_BounceDirection);

                    //Agregar una fuerza de impulso en la direcci�n de rebote
                    m_Rigidbody.AddForce(m_BounceDirection * m_BounceForce, ForceMode.Impulse);                
                    Debug.Log("Rebote");
                    m_IsBounced = true;
                }              
            }

            ChangeSound(m_HitSoundEmitter, /*m_AlertSoundEmitter,*/ m_DashSoundEmitter, m_HitSoundEmitter);
            EnemyHurtParticles(transform.position);
            EnemyDieParticles(transform.position + m_OffsetDie);
            //m_NavMeshAgent.enabled = true;
            SetStunState();
        }
    }

    void EnemyHurtParticles(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesEnemyHurt(impactPosition);
    }

    void StunParticlesOn()
    {
        m_StunParticles.SetActive(true);
    }

    void StunParticlesOff()
    {
        m_StunParticles.SetActive(false);
    }

    void StarsOn()
    {
        m_StunStars.SetActive(true);
    }

    void StarsOff()
    {
        m_StunStars.SetActive(false);
    }
    
    public void ChangeStunMaterial()
    {
        if (m_Renderer != null && m_StunMaterial != null)
        {
            Material[] materials = m_Renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = m_StunMaterial;
            }
            m_Renderer.materials = materials;
        }
    }

    public void ChangeDefaultMaterial()
    {
        if (m_Renderer != null && m_DefaultMaterial != null)
        {
            Material[] materials = m_Renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = m_DefaultMaterial;
            }
            m_Renderer.materials = materials;
        }
    }

    public void ChangeSound(GameObject OnSound, GameObject OffSound1, GameObject OffSound2)//, GameObject OffSound4)
    {
        OnSound.SetActive(true);
        OffSound1.SetActive(false);
        OffSound2.SetActive(false);       
        //OffSound4.SetActive(false);
    }
    public void SoundOff()
    {
        m_AllSounds.SetActive(false);
        //m_BaseSoundEmitter.SetActive(false);
        m_DashSoundEmitter.SetActive(false);
        m_HitSoundEmitter.SetActive(false);
        m_DieSoundEmitter.SetActive(false);
        //m_AlertSoundEmitter.SetActive(false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_SightDistance * 1.75f);
    }

    private bool playerNearPrevious = false;
    private void CheckForPlayerNear()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, m_SightDistance * 1.75f);
        bool playerNearCurrent = false; //Variable auxiliar para el estado actual de playerNear

        foreach (Collider col in targets)
        {
            if (col.CompareTag("Player"))
            {
                playerNearCurrent = true;
                break;
            }
        }

        if (playerNearCurrent != playerNearPrevious)
        {
            if (playerNearCurrent)
            {
                Debug.Log("Player cerca");
                m_PlayerNear = true;
            }
            else
            {
                Debug.Log("Player lejos");
                m_PlayerNear = false;
            }
        }

        playerNearPrevious = playerNearCurrent;
        Array.Clear(targets, 0, targets.Length);
    }

    void RestartAnimationsStates()
    {
        m_Animator.SetBool("Alert", false);
        m_Animator.SetBool("Chase", false);
        m_Animator.SetBool("Wait", false);
        m_Animator.SetBool("Patrol", true);
    }

    public void RestartGameEnemy()
    {
        gameObject.SetActive(false);
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_NavMeshAgent.enabled = true;
        m_CurrentPatrolTargetId = 0;
        //m_NavMeshAgent.isStopped = false;
        RestartAnimationsStates();
        //SetIdleState();
        gameObject.SetActive(true);

    }

    public void EnemyDieParticles(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesEnemyDie(impactPosition);
    }

    public bool GetCanDealDamage()
    {
        return m_CanDealDamage;
    }

    public void SetCanDealDamage(bool CanDealDamage)
    {
        m_CanDealDamage = CanDealDamage;
        StartCoroutine(ResetCanDealDamage());
    }

    public IEnumerator ResetCanDealDamage()
    {
        yield return new WaitForSeconds(m_DamageDeactivationTime);
        m_CanDealDamage = true;
    }
    

}
