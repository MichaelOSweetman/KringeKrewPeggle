using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    File name: Rotator.cs
    Summary: Rotates a gameobject indefinitely at a set rate
    Creation Date: 22/04/2024
    Last Modified: 22/04/2024
*/
public class Rotator : MonoBehaviour
{
    public float m_degreesPerSecond = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, m_degreesPerSecond * Time.deltaTime);
    }
}
