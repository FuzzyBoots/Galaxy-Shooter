using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Destroy in 2.65s");
        Destroy(this.gameObject, 2.65f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
