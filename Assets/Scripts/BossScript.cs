using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossScript : MonoBehaviour
{
    enum BossStateEnum
    {
        Shielding,
        FiringLasers,
        DroppingMines,
        FiringMissiles,
    }


    [Serializable]
    class BossState
    {
        public BossStateEnum state;
        public float duration;
    }

    [SerializeField]
    BossState[] _bossBehavior;

    bool _enteringScene = true;

    [SerializeField]
    int _stateIndex = 0;

    [SerializeField] 
    float _enemySpeed = 10f;

    [SerializeField]
    int _health = 32;

    [SerializeField]
    bool _movingRight = true;

    [SerializeField]
    Vector3 _initialEnteringPosition = new Vector3(0f, 5.5f, -10f);

    [SerializeField]
    Vector3 _finalEnteringPosition = new Vector3(0f, 2.5f, 0f);

    [SerializeField]
    float _enteringTime = 3f;

    [SerializeField]
    float _shieldingIncreasePeriod = 1f;

    float _shieldTime;

    float _startTime;

    private float _mineTime;

    [SerializeField]
    private float _mineInterval;

    [Header("Prefabs")]
    [SerializeField]
    GameObject _shield;

    [SerializeField]
    private GameObject _minePrefab;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _missilePrefab;
    
    private bool _mineRight;

    [SerializeField]
    private int _initialBehaviorIndex;

    public float StartTime { get { return _startTime; } set { 
            _startTime = value;
        } }

    // Start is called before the first frame update
    void Start()
    {
        // Initial position
        transform.position = _initialEnteringPosition;
        StartTime = Time.time;

        if (_shield == null )
        {
            Debug.LogError("No shield script set!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        HandleBehavior();
    }

    private void HandleBehavior()
    {
        if (_enteringScene) { return; }

        BossState _curState = _bossBehavior[_stateIndex];
        
        if (Time.time - StartTime > _curState.duration)
        {
            _stateIndex = (_stateIndex + 1) % _bossBehavior.Length;
            Debug.Log("Behavior switch " + _stateIndex);
            StartTime = Time.time;
        }
        
        switch (_curState.state)
        { 
            case BossStateEnum.Shielding:
                HandleShield();
                break;
            case BossStateEnum.FiringLasers:
                HandleLasers();
                break;
            case BossStateEnum.DroppingMines:
                HandleMines();
                break;
            case BossStateEnum.FiringMissiles:
                HandleMissiles();
                break;
        }
    }

    private void HandleMissiles()
    {
        Debug.Log("Missiles!");

        // Usual logic for delaying firing
        // Multiple missiles?
    }

    private void HandleMines()
    {
        // Set out a bunch of mines
        if (Time.time > _mineTime)
        {
            // Alternate left and right
            GameObject mine = Instantiate(_minePrefab, transform.position + 
                new Vector3(_mineRight ? -5f : 5f, -1f, 0), Quaternion.identity);

            Vector2 velocity = new Vector2((_mineRight ? -1 : 1), -0.1f);
            if (_mineRight == _movingRight)
            {
                velocity.x += _enemySpeed;
            }
            mine.GetComponent<Rigidbody2D>().velocity = velocity;
            
            _mineRight = !_mineRight;

            _mineTime = Time.time + _mineInterval;
        }
    }

    private void HandleLasers()
    {
        Debug.Log("Lasers!");

        // Usual logic for delaying firing
        // Multiple lasers?
    }

    private void HandleShield()
    {
        if (Time.time > _shieldTime)
        {
            _shield.GetComponent<ShieldScript>()?.ReviveShield(1);
            _shieldTime = Time.time + _shieldingIncreasePeriod;
        }
        // Sound cue?
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with:" + collision.gameObject.name);
    }

    private void HandleMovement()
    {
        if (_enteringScene)
        {
            transform.position = Vector3.Slerp(_initialEnteringPosition, _finalEnteringPosition, (Time.time - StartTime) / _enteringTime);

            if (Time.time - StartTime > _enteringTime)
            {
                _enteringScene = false;
                _stateIndex = /*Random.Range(0, _bossBehavior.Length)*/ _initialBehaviorIndex;
                StartTime = Time.time;
            }
            return;
        }

        if (_movingRight)
        {
            transform.Translate(Vector3.right * _enemySpeed * Time.deltaTime);
            if (transform.position.x < -7f)
            {
                _movingRight = false;
            }
        } else
        {
            transform.Translate(Vector3.left * _enemySpeed * Time.deltaTime);

            if (transform.position.x > 7f)
            {
                _movingRight = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_enteringScene) { return; }

        if (other.tag == "Laser" || other.tag == "Missile")
        {
            Destroy(other.gameObject);

            Damage();
        }

        if (other.tag == "Player")
        {
            other.transform.GetComponent<PlayerScript>()?.Damage();
        }
    }

    public void Damage()
    {
        _health--;

        // Change appearance?
        // Update boss healthbar

        if (_health < 1)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
