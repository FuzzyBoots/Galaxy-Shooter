using System.Collections;
using System.IO;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public enum MovementStyles
    {
        StraightDown,
        CaromDownL,
        CaromDownR
    }

    [SerializeField] 
    private MovementStyles _movementStyle = MovementStyles.StraightDown;

    [SerializeField]
    private float _enemySpeed = 4f;

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

            Laser enemyLaser_L = Instantiate(_laserPrefab, transform.position + new Vector3(-0.23f, -1.8f, 0f), Quaternion.identity).GetComponent<Laser>();
            Laser enemyLaser_R = Instantiate(_laserPrefab, transform.position + new Vector3(0.23f, -1.8f, 0f), Quaternion.identity).GetComponent<Laser>();
            
            enemyLaser_L.AssignEnemyLaser();
            enemyLaser_R.AssignEnemyLaser();
        }
    }

    private void CalculateMovement()
    {
        switch (_movementStyle)
        {
            case MovementStyles.StraightDown:
                transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
                break;
            case MovementStyles.CaromDownL:
                transform.Translate(new Vector3(-0.7f, -0.7f, 0f) * _enemySpeed * Time.deltaTime);
                if (transform.position.x < GameManager.lBound)
                {
                    _movementStyle = MovementStyles.CaromDownR;
                }
                break;
            case MovementStyles.CaromDownR:
                transform.Translate(new Vector3(0.7f, -0.7f, 0f) * _enemySpeed * Time.deltaTime);
                if (transform.position.x > GameManager.rBound)
                {
                    _movementStyle = MovementStyles.CaromDownL;
                }
                break;
        }        

        if (transform.position.y < GameManager.dBound && !_isDead)
        {
            transform.position = new Vector3(Random.Range(GameManager.lBound, GameManager.rBound), GameManager.uBound, 0f);
        }
    }

    public void Die()
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

        Destroy(gameObject, 2.8f);
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
        Destroy(gameObject);
    }

    internal void SetMovementStyle(MovementStyles movement)
    {
        _movementStyle = movement;
    }
}
