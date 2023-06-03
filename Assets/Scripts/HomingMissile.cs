using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        _explosionTime = Time.time + _missileDuration;

        _playerRef = GameObject.Find("Player").GetComponent<PlayerScript>();

        _enemyContainer = GameObject.Find("EnemyContainer");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _explosionTime)
        {
            GameObject explosion = Instantiate(_explosion, this.transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }

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

        if (_nearestEnemy == null) { return; }

        Vector3 direction = (Vector3)_nearestEnemy.position - transform.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        float rotation = Mathf.Clamp(_turningRadius * Time.deltaTime, 0f, rotateAmount);
        this.transform.Rotate(0,0, rotation);
        Vector3 displacement = _missileSpeed * transform.up* Time.deltaTime;
        this.transform.Translate(displacement);
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

                GameObject.Destroy(this.gameObject);
                break;
            case "Enemy":
                EnemyScript enemy = collision.GetComponent<EnemyScript>();
                if (enemy != null)
                {
                    _playerRef.AddScore(15);
                    enemy.Die();
                }
                break;
        }

        Destroy(this.gameObject, 2.65f);
    }
}
