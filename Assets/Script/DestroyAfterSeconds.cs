using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float delayBeforeDeath = 2;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, delayBeforeDeath);    
    }
}
