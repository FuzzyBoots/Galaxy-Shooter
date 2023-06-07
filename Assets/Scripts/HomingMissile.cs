using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private float _missileSpeed = 4f;
    [SerializeField]
    private float _missileDuration = 10f;
    [SerializeField]
    private float _turningRadius = 45f;

    private float _explosionTime = 0f;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField] GameObject _explosion;

    [SerializeField]
    private PlayerScript _playerRef;

    private Transform _target;
    private Rigidbody2D _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _explosionTime = Time.time + _missileDuration;

        _playerRef = GameObject.Find("Player").GetComponent<PlayerScript>();

        if (_playerRef == null )
        {
            Debug.LogError("Could not find Player.");
        }

        _enemyContainer = GameObject.Find("EnemyContainer");

        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (_target == null)
        {
            _target = GetClosestEnemy();

            _rigidBody.velocity = transform.up * _missileSpeed;
            return;
        }
        Vector2 direction = (Vector2)_target.position - _rigidBody.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _rigidBody.angularVelocity = -_turningRadius * rotateAmount;
        _rigidBody.velocity = transform.up * _missileSpeed;
    }

    private Transform GetClosestEnemy()
    {
        Transform _nearestEnemy = null;
        float closestEnemyDist = Mathf.Infinity;
        foreach (Transform enemy in _enemyContainer.transform.GetComponentsInChildren<Transform>())
        {
            float _dist = Vector3.Distance(transform.position, enemy.position);
            if (_dist < closestEnemyDist)
            {
                _nearestEnemy = enemy;
                closestEnemyDist = _dist;
            }
        }

        return _nearestEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _explosionTime)
        {
            GameObject explosion = Instantiate(_explosion, this.transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player":
                PlayerScript player = collision.GetComponent<PlayerScript>();
                if (player != null)
                {
                    player?.Damage();
                }
                else
                {
                    Debug.Log("Could not find player to damage.");
                }

                Destroy(this.gameObject);
                break;
            case "Enemy":
                StandardEnemyScript enemy = collision.GetComponent<StandardEnemyScript>();
                if (enemy != null)
                {
                    _playerRef.AddScore(15);
                    enemy.Die();
                }

                Destroy(this.gameObject);
                break;
        }
    }
}
