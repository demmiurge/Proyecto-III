using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class MobilePlatform : MonoBehaviour
{
    [SerializeField, Tooltip("Especificar que objeto se estar� moviendo.")]
    private Transform platform;

    [SerializeField, Tooltip("Listado de los puntos de control por los cuales se mover� la plataforma.")]
    private List<Transform> points;

    // NOTA DE ESCALABILIDAD: es posible tambi�n con una lista el tiempo de espera variable en cada punto.
    [SerializeField, Tooltip("Tiempo de espera de una plataforma al alcanzar un punto.")]
    private float waitTime = 5f;

    [SerializeField, Tooltip("Efecto de movimiento que tendr� la plataforma en el desplazamiento entre dos puntos, recomendable escoger entre los ya existentes.")]
    private AnimationCurve curve;

    [SerializeField, Tooltip("Tiempo que tarda la plataforma en completar todo un ciclo de movimiento, en caso de ponerlo en CERO, solamente completara el trayecto del punto CERO al punto UNO (aun existiendo m�s puntos), con lo que volver� autom�ticamente y sin animaciones intermedias al punto de partida.")]
    private float loopTime = 30f;

    // Velocidad de la plataforma en tiempo
    // El tiempo que tarda en llegar de punto A al punto B
    // NOTA DE ESCALABILIDAD: es posible hacer que cada punto tenga una velocidad particular mediante una lista de tiempos entre puntos.
    [SerializeField, Tooltip("El tiempo que tarda la plataforma en ir de un punto a otro, menor tiempo equivale a m�s velocidad y mayor tiempo equivale a menos velocidad.")]
    private float travelTime = 2f;

    [SerializeField, Tooltip("�La plataforma puede moverse al �nico? Al activarlo, siempre se estar� moviendo, aun as� se puede desactivar con alg�n activador externo.")]
    private bool motionActivated;

    private int currentPointIndex;
    private float currentTime;
    private float currentLerpTime;
    private bool waiting;
    private bool startSound;

    [Header("Scrolling")]
    [SerializeField] private UnityEvent OnScrolling;

    [Header("Scrolling")]
    [SerializeField] private UnityEvent OnPausingScrolling;

    public void ActivateMotion() => motionActivated = true;

    public void DisableMotion() => motionActivated = false;

    private void Update()
    {
        if (motionActivated)
        {
            if (!waiting)
            {
                if (startSound == false)
                {
                    startSound = true;
                    OnScrolling?.Invoke();
                }

                Vector3 startPoint = points[currentPointIndex].position;
                Vector3 endPoint = points[(currentPointIndex + 1) % points.Count].position;

                float duration = waitTime;

                currentLerpTime += Time.deltaTime / (travelTime);

                Vector3 currentPosition = Vector3.Lerp(startPoint, endPoint, curve.Evaluate(currentLerpTime));

                platform.position = currentPosition;

                if (currentLerpTime >= 1f)
                {
                    startSound = false;
                    OnPausingScrolling?.Invoke();
                    currentLerpTime = 0f;
                    StartCoroutine(WaitAtPoint(duration));
                }
            }
        }
    }

    private IEnumerator WaitAtPoint(float duration)
    {
        waiting = true;
        yield return new WaitForSeconds(duration);
        waiting = false;

        currentPointIndex = (currentPointIndex + 1) % points.Count;
        currentTime += duration;

        if (currentTime >= loopTime)
        {
            currentPointIndex = 0;
            currentTime = 0f;
        }
    }
}
