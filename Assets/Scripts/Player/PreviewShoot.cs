using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;

public class PreviewShoot : MonoBehaviour
{
    [SerializeField] private PlayerShoot playerShoot;
    private GameObject bubbleDummy;
    [SerializeField] private LayerMask validSurfacesLayer;
    [SerializeField] private List<string> surfacesTags;
    [SerializeField] private float offsetDistance = 1.75f;
    [SerializeField] private Camera camera;

    private Vector3 direction;

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void AmingOn() => isPreviewActive = true;

    public void AmingOff() => isPreviewActive = false;

    private bool isPreviewActive = false;


    private void Update()
    {
        if (bubbleDummy == null) bubbleDummy = GameManager.instance.GetBubbleDummy();

        if (bubbleDummy.activeSelf)
            bubbleDummy.SetActive(false);

        if (isPreviewActive)
        {
            Ray raySeekTargetPoint = camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            RaycastHit hitTargetPoint;
            Vector3 targetPoint = Vector3.forward;

            if (Physics.Raycast(raySeekTargetPoint, out hitTargetPoint, Mathf.Infinity, validSurfacesLayer))
                targetPoint = hitTargetPoint.point;

            Debug.DrawLine(camera.transform.position, targetPoint, Color.magenta);

            Vector3 direction;

            if (targetPoint == Vector3.forward)
                direction = camera.transform.forward;
            else
                direction = (targetPoint - playerShoot.spawnBubbles.position).normalized;

            this.direction = direction;

            Ray ray = new Ray(playerShoot.spawnBubbles.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, validSurfacesLayer))
            {
                foreach (string surfaceTag in surfacesTags)
                {
                    if (surfaceTag == hit.transform.tag)
                    {
                        Debug.DrawLine(playerShoot.spawnBubbles.position, hit.point, Color.blue);
                        Vector3 offset = hit.normal * offsetDistance;

                        bubbleDummy.transform.position = hit.point + offset;

                        Vector3 direccion = Camera.main.transform.position - bubbleDummy.transform.position;
                        Quaternion rotacion = Quaternion.LookRotation(direccion);
                        rotacion *= Quaternion.Euler(90, 0, 0);
                        bubbleDummy.transform.rotation = rotacion;

                        if (!bubbleDummy.activeSelf)
                            bubbleDummy.SetActive(true);
                    }
                }
            }
            else
                if (bubbleDummy.activeSelf)
                    bubbleDummy.SetActive(false);
        }
        else
            if (bubbleDummy.activeSelf)
                bubbleDummy.SetActive(false);
    }
}
