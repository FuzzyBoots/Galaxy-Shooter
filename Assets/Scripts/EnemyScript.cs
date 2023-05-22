using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private float _enemySpeed = 4f;
    private float _respawnDistance = -4f;

    [SerializeField]
    private static PlayerScript _player;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        if (_player == null)
        {
            _player = GameObject.Find("Player").GetComponent<PlayerScript>();

            if (_player == null)
            {
                Debug.LogError("Could not find the player!");
            }
        }

        _animator = GetComponent<Animator>();

        if (_animator == null ) {
            Debug.LogError("Could not find enemy animation");
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

            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0f;

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            else
            {
                Debug.Log("No Collider2D found");
            }

            Destroy(other.gameObject);

            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "Player")
        {
            other.transform.GetComponent<PlayerScript>()?.Damage();
            Destroy(this.gameObject);
        }
    }

    IEnumerator DelayedDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
