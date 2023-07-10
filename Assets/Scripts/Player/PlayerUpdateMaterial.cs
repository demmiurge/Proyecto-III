using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdateMaterial : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private string propertyName;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private AnimationCurve interpolationCurve;

    private float duration;

    private float currentTime = 0f;
    private bool isInterpolating = false;

    public void StartInterpolation(float duration)
    {
        if (isInterpolating)
            return;

        this.duration = duration;
        currentTime = 0f;
        isInterpolating = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartInterpolation(1);
        }

        if (isInterpolating)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;

            float curveValue = interpolationCurve.Evaluate(t);

            Color currentColor = Color.Lerp(startColor, endColor, curveValue);

            foreach (Renderer renderer in renderers)
                renderer.materials[0].SetColor(propertyName, currentColor);

            if (currentTime >= duration)
                isInterpolating = false;
        }
    }
}
