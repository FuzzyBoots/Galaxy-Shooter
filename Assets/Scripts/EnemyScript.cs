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

    private ExplosionManager _explosionManager;

    private bool _isDead = false;

    [SerializeField]
    private GameObject _laserPrefab;

    private float _fireRate = 3f;
    private float _canFire = -1f;

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

        _explosionManager = GameObject.Find("AudioManager").GetComponent<ExplosionManager>();

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Could not find enemy animation");
        }

        if (_explosionManager == null)
        {
            Debug.LogError("No Explosion Manager found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            GameObject enemyLaser = Instantiate(_laserPrefab, this.transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            foreach (Laser laser in lasers)
            {
                laser.AssignEnemyLaser();
            }
        }
    }

    private void CalculateMovement()
    {
        this.transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (this.transform.position.y < _respawnDistance && !_isDead)
        {
            this.transform.position = new Vector3(Random.Range(-8f, 8f), 8f, 0f);
        }
    }

    private void Die()
    {
        _animator.SetTrigger("OnEnemyDeath");

        _explosionManager.PlayExplosion();

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.Log("No Collider2D found");
        }

        _isDead = true;

        Destroy(this.gameObject, 2.8f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _player?.AddScore(10);

            Destroy(other.gameObject);

            Die();
        }

        if (other.tag == "Player")
        {
            other.transform.GetComponent<PlayerScript>()?.Damage();

            Die();
        }
    }

    IEnumerator DelayedDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
