using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 10;

    [SerializeField]
    private float _destroyDistance = 10;

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (this.transform.position.y > _destroyDistance)
        {            
            Destroy(this.gameObject);
        }
    }
}
