using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSecondsRealtime(5);
        Destroy(gameObject);      
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag  == "Snake" || other.tag == "Snake'")
        {
            Destroy(gameObject);            
        }
    }

}
