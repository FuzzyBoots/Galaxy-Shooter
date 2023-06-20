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

    [Header("Behavior")]
    [SerializeField]
    private int _initialBehaviorIndex;

    [SerializeField]
    BossState[] _bossBehavior;

    [SerializeField]
    int _stateIndex = 0;

    [SerializeField]
    bool _movingRight = true;

    bool _enteringScene = true;

    Vector3 _initialEnteringPosition = new Vector3(0f, 5.5f, -10f);
    Vector3 _finalEnteringPosition = new Vector3(0f, 2.5f, 0f);

    [SerializeField]
    float _enteringTime = 3f;

    [Header("Parameters")]
    [SerializeField] 
    float _enemySpeed = 10f;

    [SerializeField]
    int _health = 32;

    float _startTime;

    float _shieldTime;
    float _mineTime;
    private float _missileTime;

    private bool _mineRight;


    [SerializeField]
    float _shieldingIncreasePeriod = 1f;

    [SerializeField]
    float _mineInterval;

    [SerializeField]
    float _missileInterval;
    
    [Header("Prefabs")]
    [SerializeField]
    GameObject _shield;

    [SerializeField]
    GameObject _minePrefab;

    [SerializeField]
    GameObject _laserPrefab;

    [SerializeField]
    GameObject _missilePrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initial position
        transform.position = _initialEnteringPosition;
        _startTime = Time.time;

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
        
        if (Time.time - _startTime > _curState.duration)
        {
            _stateIndex = (_stateIndex + 1) % _bossBehavior.Length;
            Debug.Log("Behavior switch " + _stateIndex);
            _startTime = Time.time;
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
        if (Time.time > _missileTime)
        {
            GameObject missileL = Instantiate(_missilePrefab, transform.position +
                new Vector3(5.6f, 1.7f, 0), Quaternion.Euler(0,0,-45f));
            
            GameObject missileR = Instantiate(_missilePrefab, transform.position +
                new Vector3(-5.6f, 1.7f, 0), Quaternion.Euler(0, 0, 45f));

            _missileTime = Time.time + _missileInterval;
        }
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

    private void HandleMovement()
    {
        if (_enteringScene)
        {
            transform.position = Vector3.Slerp(_initialEnteringPosition, _finalEnteringPosition, (Time.time - _startTime) / _enteringTime);

            if (Time.time - _startTime > _enteringTime)
            {
                _enteringScene = false;
                _stateIndex = /*Random.Range(0, _bossBehavior.Length)*/ _initialBehaviorIndex;
                _startTime = Time.time;
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
