using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    float _powerupSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);

        if (this.transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerScript player = collision.transform.GetComponent<PlayerScript>();
            player?.TurnOnTripleShot();
            Destroy(this.gameObject);
        }
    }
}
