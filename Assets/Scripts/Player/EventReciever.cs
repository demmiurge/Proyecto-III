using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventReciever : MonoBehaviour
{
    public Animator playerAnimator;
    public UnityEvent jumpGrass;
    public UnityEvent jumpStone;
    public UnityEvent jumpWater;
    public UnityEvent landGrass;
    public UnityEvent landStone;
    public UnityEvent landWater;
    public UnityEvent sandStep;
    public UnityEvent stoneStep;
    public UnityEvent grassStep;
    public UnityEvent waterStep;
    public UnityEvent swim;
    public UnityEvent damage;

    public UnityEvent shoot;
    public UnityEvent destroy;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLandEvent(AnimationEvent animEvent)
    {
        if (playerMovement.IsTouchingWater())
        {
            landWater?.Invoke();
        }
        else
        {
            landGrass?.Invoke();
        }
    }


    public void OnStep(AnimationEvent animEvent)
    {
        if (playerMovement.IsTouchingWater())
        {
            waterStep?.Invoke();
        }
        else
        {
            grassStep?.Invoke();
        }
    }

    public void OnJumpAnim(AnimationEvent animEvent)
    {
        if (playerMovement.IsTouchingWater())
        {
            jumpWater?.Invoke();
        }
        else
        {
            jumpGrass?.Invoke();
        }
    }

    public void OnDamage(AnimationEvent animEvent)
    {
        damage?.Invoke();
    }

    public void OnShootAnim(AnimationEvent animEvent)
    {
        shoot?.Invoke();
    }

    public void OnDestroyAnim(AnimationEvent animEvent)
    {
        destroy?.Invoke();
    }
}
