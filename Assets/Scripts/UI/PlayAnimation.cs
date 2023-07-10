using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] private string animationName;
    [SerializeField] private string disappearAnimationName;
    [SerializeField] private string actionWhenInteracting;

    [SerializeField] private Animation animation;

    [SerializeField] private bool deactivateObject = true;

    void Awake()
    {
        if (animation == null && GetComponent<Animation>()) animation = GetComponent<Animation>();
    }

    void OnEnable()
    {
        if (animation == null) animation = GetComponent<Animation>();
        animation.Play(animationName);
    }

    public void ActivateSpecialEvent()
    {
        animation.Play(actionWhenInteracting);
    }

    public void OnCallDisappearHear()
    {
        if (animation == null) animation = GetComponent<Animation>();
        animation.Play(disappearAnimationName);

        if (deactivateObject) Invoke("DeactivateGameObject", animation.GetClip(disappearAnimationName).length);
    }

    private void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }
}
