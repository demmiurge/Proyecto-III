using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float fadeSpeed = 0.5f;  //Velocidad de FadeOut
    public SkinnedMeshRenderer skinnedMeshRenderer; 
    public EnemigoAereoV2 enemyAereoScript;

    private void Start()
    {
        //skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        foreach (Material material in skinnedMeshRenderer.materials)
        {
            Color color = material.color;
            color.a -= fadeSpeed * Time.deltaTime;
            material.color = color;
        }

        if (skinnedMeshRenderer.material.color.a <= 0f) //FadeOut completo
        {
            gameObject.SetActive(false);
            enemyAereoScript.DropItem();
        }
    }
}
