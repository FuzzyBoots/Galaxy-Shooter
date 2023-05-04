using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    [SerializeField]
    private float RBound = 8f;
    [SerializeField]
    private float LBound = -8f;
    [SerializeField]
    private float UBound = 8f;
    [SerializeField]
    private float DBound = -8f;

    [SerializeField]
    private GameObject laserPrefab;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        HandleLaser();
    }

    private void HandleLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject laser = Instantiate(laserPrefab, this.transform.position, Quaternion.identity);
        }
    }

    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        this.transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        this.transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, LBound, RBound), Mathf.Clamp(this.transform.position.y, DBound, UBound), this.transform.position.z);
    }
}
