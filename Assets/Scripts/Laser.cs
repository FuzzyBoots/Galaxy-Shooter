using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 10;

    [SerializeField]
    private float _destroyDistance = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the laser
        this.transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (this.transform.position.y > _destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }
}
