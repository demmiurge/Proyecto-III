using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotaryPlatform : MonoBehaviour
{
    private enum MOVEMENT_TYPE
    {
        Y,
        X,
        Z,
    }

    [SerializeField] private MOVEMENT_TYPE movementType;
    [SerializeField] private float rotationAngle = 90f; // Ángulo de rotación en grados
    [SerializeField] private float rotationTime = 1f; // Tiempo en segundos para completar la rotación
    [SerializeField] private float waitTime = 1f; // Tiempo en segundos para esperar después de la rotación
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private Transform platform;

    [SerializeField, Tooltip("¿La plataforma puede moverse al único? Al activarlo, siempre se estará moviendo, aun así se puede desactivar con algún activador externo.")]
    private bool motionActivated;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isRotating;
    private float rotationProgress;
    private float waitTimer;
    private bool startSound;

    [Header("Scrolling")]
    [SerializeField] private UnityEvent OnScrolling;

    [Header("Scrolling")]
    [SerializeField] private UnityEvent OnPausingScrolling;

    public void ActivateMotion() => motionActivated = true;

    public void DisableMotion() => motionActivated = false;

    private void Start()
    {
        initialRotation = platform.rotation;

        switch (movementType)
        {
            case MOVEMENT_TYPE.Y:
                targetRotation = initialRotation * Quaternion.Euler(0f, rotationAngle, 0f);
                break;
            case MOVEMENT_TYPE.X:
                targetRotation = initialRotation * Quaternion.Euler(rotationAngle, 0f, 0f);
                break;
            case MOVEMENT_TYPE.Z:
                targetRotation = initialRotation * Quaternion.Euler(0f, 0f, rotationAngle);
                break;
        }
    }

    private void Update()
    {
        if (motionActivated)
        {
            if (isRotating)
            {
                if (rotationProgress < rotationTime)
                {
                    if (startSound == false)
                    {
                        startSound = true;
                        OnScrolling?.Invoke();
                    }

                    rotationProgress += Time.deltaTime;
                    float curveProgress = rotationCurve.Evaluate(rotationProgress / rotationTime);
                    platform.rotation = Quaternion.Slerp(initialRotation, targetRotation, curveProgress);
                }
                else
                {
                    startSound = false;
                    OnPausingScrolling?.Invoke();

                    platform.rotation = targetRotation;
                    waitTimer += Time.deltaTime;

                    if (waitTimer >= waitTime)
                    {
                        initialRotation = targetRotation;

                        switch (movementType)
                        {
                            case MOVEMENT_TYPE.Y:
                                targetRotation *= Quaternion.Euler(0f, rotationAngle, 0f);
                                break;
                            case MOVEMENT_TYPE.X:
                                targetRotation *= Quaternion.Euler(rotationAngle, 0f, 0f);
                                break;
                            case MOVEMENT_TYPE.Z:
                                targetRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                                break;
                        }

                        rotationProgress = 0f;
                        waitTimer = 0f;
                    }
                }
            }
            else
            {
                isRotating = true;
            }
        }
    }
}
