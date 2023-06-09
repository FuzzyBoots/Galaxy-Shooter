using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mine : MonoBehaviour
{
    Rigidbody2D _rigidbody;

    [SerializeField] GameObject _explosion;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody= GetComponent<Rigidbody2D>();

        _rigidbody.velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-1f, -3f));
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.velocity += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime;
    }

    public void Die()
    {
        Instantiate(_explosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.Damage();
            }
            else
            {
                Debug.Log("Could not find player to damage.");
            }

            Instantiate(_explosion, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
