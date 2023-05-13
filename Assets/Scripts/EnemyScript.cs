using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private float _enemySpeed = 4f;
    private float _respawnDistance = -4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (this.transform.position.y < _respawnDistance)
        {
            this.transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collided with {other.tag}");
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        if (other.tag == "Player")
        {
            Debug.Log("Collided with Player");
            other.transform.GetComponent<PlayerScript>()?.Damage();
            Destroy(this.gameObject);
        }
    }
}
