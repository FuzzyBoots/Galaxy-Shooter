using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private float _missileSpeed = 4f;
    [SerializeField]
    private float _missileDuration = 10f;
    [SerializeField]
    private float _turningRadius = 45f;

    private float _explosionTime = 0f;

    [SerializeField]
    protected GameObject _enemyContainer;

    [SerializeField] GameObject _explosion;

    [SerializeField]
    protected PlayerScript _playerRef;

    private Transform _target;
    private Rigidbody2D _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _explosionTime = Time.time + _missileDuration;

        _playerRef = GameObject.Find("Player").GetComponent<PlayerScript>();

        if (_playerRef == null)
        {
            Debug.LogError("Could not find Player.");
        }

        _enemyContainer = GameObject.Find("EnemyContainer");

        if (_enemyContainer == null)
        {
            Debug.LogError("Could not find Enemy Container.");
        }

        _rigidBody = GetComponent<Rigidbody2D>();

        if (_rigidBody == null)
        {
            Debug.LogError("Could not find Rigidbidy.");        
        }
    }

    void FixedUpdate()
    {
        if (_target == null)
        {
            _target = GetClosestTarget();

            _rigidBody.velocity = transform.up * _missileSpeed;
            return;
        }
        Vector2 direction = (Vector2)_target.position - _rigidBody.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _rigidBody.angularVelocity = -_turningRadius * rotateAmount;
        _rigidBody.velocity = transform.up * _missileSpeed;
    }

    protected abstract Transform GetClosestTarget();    

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _explosionTime)
        {
            Instantiate(_explosion, this.transform.position, Quaternion.identity);

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

                Instantiate(_explosion, this.transform.position, Quaternion.identity);

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
