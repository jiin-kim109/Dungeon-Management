using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Hero>())
        {
           
        }
    }
}
