using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private float _enemySpeed = 4f;
    private float _respawnDistance = -4f;

    [SerializeField]
    private PlayerScript _player;

    // Start is called before the first frame update
    void Start()
    {
        if (_player == null) { 
            _player = GameObject.Find("Player").GetComponent<PlayerScript>();
        }
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
            _player?.AddScore(10);

            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        if (other.tag == "Player")
        {
            other.transform.GetComponent<PlayerScript>()?.Damage();
            Destroy(this.gameObject);
        }
    }
}
