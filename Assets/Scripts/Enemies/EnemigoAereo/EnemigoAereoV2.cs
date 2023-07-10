using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using FMODUnity;
using Random = UnityEngine.Random;

public class EnemigoAereoV2 : MonoBehaviour
{

    [Header("State Machine")]
    public TState m_State;
    public TState m_LastState;

    [Header("Nav Agent")]
    public NavMeshAgent m_NavMeshAgent;

    [Header("Parameters")]
    //public GameObject m_Target;
    public GameObject m_Player;
    public GameObject m_Projectile;
    public Transform m_FirePoint;
    public float m_ProjectileSpeed = 10.0f;
    public float m_Life = 1.0f;
    public float m_MaxLife = 1.0f;
    public float m_Damage = 1.0f;
    public float m_ShootCooldown = 2f;
    public List<Transform> m_PatrolTargets;
    private int m_CurrentPatrolTargetId = 0;

    public float m_VisualConeAngle = 80f;
    public float m_HearRangeDistance = 8.0f;
    public float m_SightDistance = 15.0f;
    public LayerMask m_SightLayerMask;
    public LayerMask m_FloorLayer;

    public float m_AlertRotation = 360f;
    public float m_AlertRotationSpeed = 120f;

    public float m_MinDistanceToTarget = 6f;
    public float m_MaxDistanceToTarget = 18f;
    public float m_MaxShootDistance = 15f;
    public float m_SpeedLookAt = 3f;

    private bool m_Shooting = false;

    public GameObject m_LifeItemPrefab;
    public Collider m_FallCollider;
    public GameObject m_ParticlesSpawn;

    [Header("Animation")]
    public Animator m_Animator;

    public AnimationClip m_ShootClip;
    public AnimationClip m_DieClip;
    public AnimationClip m_DisappearClip;

    public AnimationEvent m_ShootEvent;
    public AnimationEvent m_DieEvent;
    public AnimationEvent m_DisappearEvent;

    public float m_FallSpeed = 5f;

    private bool m_ShadoFall = false;
    
    private bool m_OnGround = false;
    private bool m_PlayerNear = false;

    private Vector3 m_OffsetShoot = new Vector3(0, 1.5f, 0);
    private Vector3 m_OffsetDie = new Vector3(0, 0.5f, 0);


    [Header("Heart spawn settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField][Range(0, 1)] private float probabilityFactor = 0.75f;
    [SerializeField] private float offsetFromGround = 0.0f;

    /*public AnimationClip m_HitAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public FadeObject m_DroneFader;
    public GameObject m_ParticleEmitter;*/

    /*[Header("VFX")]
    public ParticleSystem m_MuzzleFlashParticles;*/

    [Header("Audio")]
    public GameObject m_AllSounds;
    public GameObject m_BaseSoundEmitter;
    public GameObject m_ShootSoundEmitter;
    public GameObject m_HitSoundEmitter;
    public GameObject m_DieSoundEmitter;
    public GameObject m_AlertSoundEmitter;

    [Header("Reset")]
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;

    private ParticleSystemManager particleSystemManager;

    // Parche temporal al doble disparo
    private bool hasShoot;
    //private Quaternion m_DieRotation;

    public enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        CHASE,
        ATTACK,
        DIE
    }

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Animator = gameObject.GetComponent<Animator>();
        m_Animator = gameObject.GetComponent<Animator>();
        m_SightLayerMask = LayerMask.NameToLayer("Everything");
        m_FloorLayer = LayerMask.NameToLayer("Floor");
}

    private void Start()
    {
        if (particleSystemManager == null) particleSystemManager = GameManager.instance.GetParticleSystemManager();
    }

    private void OnEnable()
    {
        
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        SetIdleState();

        SoundOff();
        m_ParticlesSpawn.SetActive(true);

        //m_ShootClip = m_Animator.runtimeAnimatorController.animationClips[0];
        m_ShootEvent.functionName = "ShadoShoot";
        m_ShootEvent.time = m_ShootClip.length;
        m_ShootClip.AddEvent(m_ShootEvent);

        //m_DeathClip = m_Animator.runtimeAnimatorController.animationClips[1];
        m_DieEvent.functionName = "ShadoFall";
        m_DieEvent.time = m_DieClip.length;
        m_DieClip.AddEvent(m_DieEvent);

        m_DisappearEvent.functionName = "ShadoDisappear";
        m_DisappearEvent.time = m_DieClip.length;
        m_DisappearClip.AddEvent(m_DieEvent);

        m_ShadoFall = false;
        m_FallCollider.enabled = false;

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
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.DIE:
                UpdateDieState();
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
        //NullTarget();
        m_State = TState.PATROL;
        m_LastState = m_State;
        m_NavMeshAgent.enabled = true;
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetId].position;
        m_Animator.SetBool("Alert", false);
        m_Animator.SetBool("Patrol", true);
        ChangeSound(m_BaseSoundEmitter, m_AlertSoundEmitter, m_HitSoundEmitter, m_DieSoundEmitter);
    }

    void UpdatePatrolState()
    {
        //CheckForBubbles();

        /*if (m_Target == null)
        {*/
        CheckForPlayerNear();

        if (m_PlayerNear) { m_AllSounds.SetActive(true); }

        if (m_PlayerNear == false) { m_AllSounds.SetActive(false); }

        if (PatrolTargetPositionArrived())
        {
            MoveToNextPatrolPosition();
        }

        //}

        /*
        else if (HearsBubble() || m_Target.tag == "Bubble")
        {
            SetAlertState();
        }
        */

        if (HearsPlayer() || SeesPlayer())
        {
            m_Animator.SetBool("Patrol", false);
            m_Animator.SetTrigger("Detect");
            SetAlertState();
        }
    }

    private bool PatrolTargetPositionArrived()
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

    /*
    private void CheckForBubbles()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, m_SightDistance);
        foreach (Collider col in targets)
        {
            if (col.CompareTag("Bubble"))
            {
                m_Target = col.gameObject;
                Debug.Log("Target = " + m_Target.tag);
                break;
            }

            if (targets.Length == 0)
            {
                Debug.Log("No hay burbujas cerca");
                NullTarget();
                break;
            }
        }
    }
    */


    /*
    bool HearsBubble()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_TargetPosition = m_Target.transform.position;
        return Vector3.Distance(l_TargetPosition, transform.position) <= m_HearRangeDistance;
    }
    
    bool SeesTarget()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        if (m_Target != null)
        {
            Vector3 l_TargetPosition = m_Target.transform.position;
            Vector3 l_DirectionTargetXZ = l_TargetPosition - transform.position;
            l_DirectionTargetXZ.y = 0.0f;
            l_DirectionTargetXZ.Normalize();
            Vector3 l_ForwardXZ = transform.forward;
            l_ForwardXZ.y = 0.0f;
            l_ForwardXZ.Normalize();

            Ray l_Ray = new Ray(transform.position, l_DirectionTargetXZ);

            return Vector3.Distance(l_TargetPosition, transform.position) < m_SightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionTargetXZ) > Mathf.Cos(m_VisualConeAngle * Mathf.Deg2Rad / 2.0f)
                && !Physics.Raycast(l_Ray, m_SightLayerMask);
        }

        return false;
    }
    */

    bool HearsPlayer()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_TargetPosition = m_Player.transform.position;
        return Vector3.Distance(l_TargetPosition, transform.position) <= m_HearRangeDistance;
    }

    bool SeesPlayer()
    {
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_TargetPosition = m_Player.transform.position;
        Vector3 l_DirectionTargetXZ = l_TargetPosition - transform.position;
        l_DirectionTargetXZ.y = 0.0f;
        l_DirectionTargetXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.y = 0.0f;
        l_ForwardXZ.Normalize();

        Ray l_Ray = new Ray(transform.position, l_DirectionTargetXZ);

        return Vector3.Distance(l_TargetPosition, transform.position) < m_SightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionTargetXZ) > Mathf.Cos(m_VisualConeAngle * Mathf.Deg2Rad / 2.0f)
            && !Physics.Raycast(l_Ray, m_SightLayerMask);
    }

    void SetAlertState()
    {
        m_State = TState.ALERT;
        m_Animator.SetBool("Alert", true);
        ChangeSound(m_AlertSoundEmitter, m_DieSoundEmitter, m_HitSoundEmitter, m_BaseSoundEmitter);
        m_LastState = m_State;
        m_NavMeshAgent.enabled = true;
        m_NavMeshAgent.isStopped = true;
    }

    void UpdateAlertState()
    {
        //m_Animator.SetTrigger("Alert");
        float l_Rotation = m_AlertRotationSpeed * Time.deltaTime;
        if (m_AlertRotation > l_Rotation)
        {
            m_AlertRotation -= l_Rotation;
        }
        else
        {
            m_NavMeshAgent.isStopped = false;
            SetPatrolState();
            m_AlertRotation = 360f;
        }

        transform.Rotate(0, l_Rotation, 0);

        //CheckForBubbles();

        //if (SeesTarget() || (m_Target != null && m_Target.activeSelf))
        //{
        //    Debug.Log("TARGET SEEN WHILE ON ALERT");
        //    m_NavMeshAgent.isStopped = false;
        //    SetChaseState();
        //}

        if (SeesPlayer())
        {
            //LookTarget(m_Player.transform);
            //Debug.Log("TARGET SEEN WHILE ON ALERT");
            
            //m_NavMeshAgent.isStopped = false;
            SetAttackState();
        }
    }

    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_LastState = m_State;
        m_NavMeshAgent.enabled = true;
        m_Animator.SetBool("Chase", true);
        SetNextChasePosition();
    }

    void UpdateChaseState()
    {
        if (TargetWithinShootDistance())
        {
            SetAttackState();
        }

        if (TargetBeyondMinDistance() && !TargetBeyondMaxDistance())
        {
            SetNextChasePosition();
        }

        if (TargetBeyondMaxDistance())
        {
            //NullTarget();
            SetPatrolState();
        }
    }
    bool TargetBeyondMaxDistance()
    {
        return Vector3.Distance(m_Player.transform.position, transform.position) > m_MaxDistanceToTarget;
        //return Vector3.Distance(GameController.GetGameController().GetPlayer().transform.position, transform.position) > m_MaxDistanceToPlayer;
    }

    bool TargetBeyondMinDistance()
    {
        return Vector3.Distance(m_Player.transform.position, transform.position) > m_MinDistanceToTarget;
        //return Vector3.Distance(GameController.GetGameController().GetPlayer().transform.position, transform.position) > m_MinDistanceToPlayer;
    }

    bool TargetWithinShootDistance()
    {
        return Vector3.Distance(m_Player.transform.position, transform.position) < m_MaxShootDistance;
        //return Vector3.Distance(GameController.GetGameController().GetPlayer().transform.position, transform.position) < m_MaxShootDistance;
    }

    void SetNextChasePosition()
    {
        m_NavMeshAgent.enabled = true;
        //Vector3 l_PlayerPosition = GameController.GetGameController().GetTarget().transform.position;
        Vector3 l_TargetPosition = m_Player.transform.position;
        Vector3 l_Direction = l_TargetPosition - transform.position;
        l_Direction.Normalize();
        float l_Distance = Vector3.Distance(transform.position, l_TargetPosition);
        Vector3 l_NextPosition = transform.position + l_Direction * (l_Distance - m_MinDistanceToTarget);

        m_NavMeshAgent.destination = l_NextPosition;
    }

    void SetAttackState()
    {
        m_State = TState.ATTACK;
        m_LastState = m_State;
        m_Animator.SetBool("Alert", false);
        m_Animator.SetBool("Chase", false);
        //m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.enabled = false;
    }

    void UpdateAttackState()
    {
        /*if (m_Target == null || !m_Target.activeSelf)
        {
            SetAlertState();
        }     

        else
        {

        CheckForAttackBubbles();
        */

        //StartCoroutine(LookTargetRoutine(m_Player.transform));

        LookTarget(m_Player.transform);

        if (CanShoot())
        {           
            m_Shooting = true;
            SetAttackAnimation();
        }

        if (!TargetWithinShootDistance())
        {
            //NullTarget();
            m_NavMeshAgent.enabled = true;
            //m_NavMeshAgent.isStopped = false;
            SetChaseState();
        }

    }

    /*
    private void CheckForAttackBubbles()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, m_SightDistance);
        foreach (Collider col in targets)
        {
            if (col.CompareTag("Bubble"))
            {
                if(m_Target.tag != col.gameObject.tag)
                {
                    m_Target = col.gameObject;
                    Debug.Log("Target = " + m_Target.tag);
                    break;
                }

                if (targets.Length == 0)
                {
                    Debug.Log("No hay burbujas cerca");
                    NullTarget();
                    break;
                }
            }
        }
    }*/

    public void ShadoShoot()
    {
        if (hasShoot == false && TargetWithinShootDistance()) // Shoot performed
        {
            hasShoot = true;
            //PlayerController l_Player = GameController.GetGameController().GetPlayer();  
            EnemyShootParticles(m_FirePoint.position);
            ChangeSound(m_ShootSoundEmitter, m_AlertSoundEmitter, m_HitSoundEmitter, m_BaseSoundEmitter);
            GameObject projectile = Instantiate(m_Projectile, m_FirePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            Vector3 direction = ((m_Player.transform.position + m_OffsetShoot) - m_FirePoint.position).normalized;
            rb.velocity = direction * m_ProjectileSpeed;

            Projectile projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.direction = direction;

            projectileScript.target = m_Player.transform;
            //projectileScript.enemy = gameObject;
  
            StartCoroutine(EndShoot());
        };
    }

    bool CanShoot()
    {
        return !m_Shooting;
    }

    void SetAttackAnimation()
    {
        //Debug.Log("Attack Animation");
        m_Animator.SetBool("Wait", false);
        m_Animator.SetTrigger("Attack");
    }

    /*public void NullTarget()
    {
        m_Target = null;
        Debug.Log("Target = null");
    }*/

    void SetDieState()
    {
        m_State = TState.DIE;
        m_LastState = m_State;
        SetDieAnimation();
        //m_DieRotation = transform.rotation;
    }

    void UpdateDieState()
    {
        if(m_ShadoFall)
        {
            float l_elapsedTime = new float();
            l_elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(l_elapsedTime / m_FallSpeed);
            Vector3 fallGround = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 0f, t), transform.position.z);

            transform.position = fallGround;

            if (m_OnGround)
            {
                ChangeSound(m_DieSoundEmitter, m_AlertSoundEmitter, m_HitSoundEmitter, m_BaseSoundEmitter);
                m_ShadoFall = false;
                Debug.Log("Tocando suelo");
                m_Animator.SetBool("Disappear", true);               
            }
        }
    }

    void SetDieAnimation()
    {
        m_Animator.SetTrigger("Hit");
        m_NavMeshAgent.enabled = false;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        gameObject.GetComponent<Oscillate>().enabled = false;
        m_ParticlesSpawn.SetActive(false);
        transform.position = new Vector3(transform.position.x, m_StartPosition.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bubble" || other.gameObject.tag == "BubbleBullet")
        {
            EnemyHurtParticles(transform.position);
            ChangeSound(m_HitSoundEmitter, m_AlertSoundEmitter, m_DieSoundEmitter, m_BaseSoundEmitter);
            SetDieState();
            m_FallCollider.enabled = true;
            //other.gameObject.SetActive(false);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_OnGround = true;
            EnemyHurtParticles(transform.position);
        }
    }

    public IEnumerator EndShoot()
    {
        //m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Wait", true);
        //yield return new WaitForSeconds(m_DieAnimationClip.length);
        yield return new WaitForSeconds(m_ShootCooldown);
        m_ShootSoundEmitter.SetActive(false);
        hasShoot = false;
        m_Shooting = false;
    }

    public void DropItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, targetLayer))
        {
            if (Random.Range(0, 1) <= probabilityFactor)
            {
                Vector3 collisionPoint = hit.point;

                collisionPoint += Vector3.up * offsetFromGround;

                Instantiate(m_LifeItemPrefab, collisionPoint, Quaternion.identity);
            }
        }
    }

    public void ChangeSound(GameObject OnSound, GameObject OffSound1, GameObject OffSound2, GameObject OffSound3)
    {
        OnSound.SetActive(true);
        OffSound1.SetActive(false);
        OffSound2.SetActive(false);
        OffSound3.SetActive(false);
    }

    public void SoundOff()
    {
        m_AllSounds.SetActive(false);
        //m_BaseSoundEmitter.SetActive(false);
        m_ShootSoundEmitter.SetActive(false);
        m_HitSoundEmitter.SetActive(false);
        m_DieSoundEmitter.SetActive(false);
        m_AlertSoundEmitter.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_SightDistance * 1.5f);
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

    public void EnemyHurtParticles(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesEnemyHurt(impactPosition);
    }

    void EnemyShootParticles(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesEnemyShoot(impactPosition);
    }

    public void EnemyDieParticles(Vector3 impactPosition)
    {
        particleSystemManager.GenerateParticlesEnemyDie(impactPosition);
    }

    public void ShadoFall()
    {
        m_ShadoFall = true;       
    }

    public void ShadoDisappear()
    {
        //StartCoroutine(DisappearCoroutine());
        //m_NavMeshAgent.enabled = true;
        //m_FadeOutScript.enabled = true;
        EnemyDieParticles(transform.position + m_OffsetDie);
        gameObject.SetActive(false);
        DropItem();
    }

    void LookTarget(Transform target)
    {
        Vector3 targetDirection = (target.position + m_OffsetShoot) - transform.position;
        //targetDirection.z = 0f; // Establecer el componente Z a cero para limitar la rotación en el eje Z
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Interpolar la rotación suavemente
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_SpeedLookAt * Time.deltaTime);
    }

    void RestartAnimationsStates()
    {
        m_Animator.SetBool("Alert", false);
        m_Animator.SetBool("Chase", false);
        m_Animator.SetBool("Wait", false);
        m_Animator.SetBool("Disappear", false);
        m_Animator.SetBool("Patrol", true);
    }

    public void RestartGameEnemy()
    {
        Debug.Log("Restart enemies");
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_NavMeshAgent.enabled = true;
        m_CurrentPatrolTargetId = 0;
        //m_NavMeshAgent.isStopped = false;
        RestartAnimationsStates();
        //SetIdleState();
        gameObject.SetActive(true);

    }
}
