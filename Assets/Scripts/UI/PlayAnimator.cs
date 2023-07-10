using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string showTriggerParameterName;
    [SerializeField] private string hideTriggerParameterName;

    void OnEnable()
    {
        animator.SetTrigger(showTriggerParameterName);
    }

    public void CallHideAnimation()
    {
        animator.SetTrigger(hideTriggerParameterName);
    }

    //public void GetInitialAnimationTime()
    //{
    //    GetAnimationDuration(showTriggerParameterName);
    //}

    //public void GetFinalAnimationTime()
    //{
    //    GetAnimationDuration(hideTriggerParameterName);
    //}

    //private float GetAnimationDuration(string animationName)
    //{
    //    float duration = 0f;

    //    if (animator != null && !string.IsNullOrEmpty(animationName))
    //    {
    //        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

    //        foreach (AnimatorClipInfo clip in clipInfo)
    //        {
    //            if (clip.clip.name == animationName)
    //            {
    //                duration = clip.clip.length;
    //                break;
    //            }
    //        }
    //    }

    //    return duration;
    //}
}
