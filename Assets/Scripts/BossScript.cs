using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
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

    [SerializeField]
    public int Health { 
        get { return _health; }
        set { 
            _health = value; 
            _uiManager.SetBossHealth(value); 
        } 
    }

    float _startTime;

    float _shieldTime;
    float _mineTime;
    private float _missileTime;
    private float _laserTime;

    private bool _mineRight;

    UI_Manager _uiManager; 


    [Header("Behavior Parameters")]
    [SerializeField]
    float _shieldingIncreasePeriod = 1f;

    [SerializeField]
    float _mineInterval;

    [SerializeField]
    float _missileInterval;

    [Header("Lasers")]
    [SerializeField]
    private float _laserInterval = 1;
    [SerializeField]
    private float _laserArc = 90;
    [SerializeField]
    private int _laserNumber = 4;
    [SerializeField]
    private float _laserDelay = 0.1f;

    [Header("Prefabs")]
    [SerializeField]
    GameObject _shield;

    [SerializeField]
    GameObject _minePrefab;

    [SerializeField]
    GameObject _laserPrefab;

    [SerializeField]
    GameObject _missilePrefab;
    
    private Coroutine _laserCoroutine;

    [SerializeField]
    private float _rotationOffset;


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

        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
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
            if (_curState.state == BossStateEnum.FiringLasers)
            {
                // Cancel the coroutine
                StopCoroutine(_laserCoroutine);
            }
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
        if (Time.time <= _missileTime) { return; }

        GameObject missileL = Instantiate(_missilePrefab, transform.position +
            new Vector3(5.6f, 1.7f, 0), Quaternion.Euler(0,0,-45f));
            
        GameObject missileR = Instantiate(_missilePrefab, transform.position +
            new Vector3(-5.6f, 1.7f, 0), Quaternion.Euler(0, 0, 45f));

        _missileTime = Time.time + _missileInterval;
    }

    private void HandleMines()
    {
        // Set out a bunch of mines
        if (Time.time <= _mineTime) { return; }
        
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

    private void HandleLasers()
    {
        // Usual logic for delaying firing
        if (Time.time <= _laserTime) { return; }

        // Let's start a co-routine to fire in an arc.
        _laserCoroutine = StartCoroutine(FireLaserSpray());

        _laserTime = Time.time + _laserInterval;
    }

    IEnumerator FireLaserSpray()
    {
        if (_laserNumber <= 1)
        {
            GameObject laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, -5, 0), Quaternion.identity);
            laser.GetComponent<Laser>().AssignEnemyLaser();
        }
        else
        {
            float angleSlice = _laserArc / (_laserNumber - 1);
            for (int index = 0; index < _laserNumber; ++index)
            {
                float angle = _rotationOffset + index * angleSlice - _laserArc / 2;

                GameObject laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, -4.75f, 0), Quaternion.Euler(0, 0, angle));
                Laser laserScript = laser.GetComponent<Laser>();
                laserScript.AssignEnemyLaser();
                laserScript.SetSpeed(5);

                yield return new WaitForSeconds(_laserDelay);
            }
        }

        yield return null;
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
                _stateIndex = Random.Range(0, _bossBehavior.Length);
                _startTime = Time.time;

                _uiManager.SetBossHealthEnabled(true);
                _uiManager.SetBossMaxHealth(_health);
                _uiManager.SetBossHealth(_health);
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
        Debug.Log("Damaged: " + _health);

        // Change appearance?
        // Update boss healthbar
        _uiManager?.SetBossHealth(_health);

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
