using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject objectWithTheParticleSystem;

    void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        Instantiate(objectWithTheParticleSystem, transform.position, Quaternion.identity, null);
    }
}
