using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private int secondsToDestroy;

    void Start()
    {
        Destroy(gameObject, secondsToDestroy);
    }
}
