using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    [SerializeField]
    private float _speedBoost = 1.8f;

    [SerializeField]
    private float _rBound = 8f;
    [SerializeField]
    private float _lBound = -8f;
    [SerializeField]
    private float _uBound = 8f;
    [SerializeField]
    private float _dBound = -8f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;

    [SerializeField]
    private float _fireRate = 0.5f;  // half a second
    private float _canFire = 0;
    
    [SerializeField]
    private int _lives = 3;

    private float _tripleShotTime = 0;
    private float _speedTime = 0;

    [SerializeField]
    private bool _shieldActive = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    
    private Transform _laserContainer;

    [SerializeField]
    private int _score = 0;

    [SerializeField]
    UI_Manager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0f, -6f, 0f);

        _laserContainer = GameObject.Find("LaserContainer").transform;

        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();

        if (_laserContainer == null)
        {
            Debug.LogError("No Laser Container found!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("No UI Manager found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        HandleLaser();
    }

    private void HandleLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            GameObject laser;
            if (Time.time < _tripleShotTime)
            {
                Debug.Log("Triple Laser");
                laser = Instantiate(_tripleLaserPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            } else
            {
                laser = Instantiate(_laserPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            }
            laser.transform.parent = _laserContainer;
            _canFire= Time.time + _fireRate;
        }
    }

    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float effectiveSpeed = GetEffectiveSpeed();

        transform.Translate(Vector3.right * horizontalInput * effectiveSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * effectiveSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, _lBound, _rBound), Mathf.Clamp(this.transform.position.y, _dBound, _uBound), this.transform.position.z);
    }

    private float GetEffectiveSpeed()
    {
        return (Time.time < _speedTime) ? _speed * _speedBoost : _speed;
    }

    public void TurnOnTripleShot(float _powerupDuration)
    {
        _tripleShotTime = Time.time + _powerupDuration;
    }

    public void TurnOnSpeed(float _powerupDuration)
    {
        _speedTime = Time.time + _powerupDuration;
    }

    public void SetShield(bool active)
    {
        _shieldActive = active;
        _shieldVisualizer.SetActive(_shieldActive);
    }

    public void Damage()
    {
        Debug.Log($"Shield active: {_shieldActive}");
        if (_shieldActive)
        {
            // Tank the hit
            Debug.Log("Bonk!");
            SetShield(false);
            return;
        }

        _lives--;
        _uiManager.SetLives(_lives);

        if (_lives < 1)
        {
            SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            spawnManager.StopSpawningEnemies();
            spawnManager.StopSpawningPowerups();

            _uiManager.DisplayGameOver();

            Destroy(this.gameObject);
        }
    }

    public void AddScore(int score)
    {
        _score += score;
        _uiManager.SetScore(_score);
    }
}
