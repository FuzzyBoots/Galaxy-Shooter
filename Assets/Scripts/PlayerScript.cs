using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    [SerializeField]
    private float _speedBoost = 1.8f;
    [SerializeField]
    private float _thrusterBoost = 1.5f;

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
    private GameObject _firePrefab;
    [SerializeField]
    private GameObject _homingPrefab;

    [SerializeField]
    private float _fireRate = 0.5f;  // half a second
    private float _canFire = 0;
    
    [SerializeField]
    private int _lives = 3;

    private float _tripleShotTime = 0;
    private float _speedTime = 0;

    [SerializeField]
    private int _shieldPower = 0;

    [SerializeField]
    private int _maxShieldPower = 3;

    [SerializeField]
    private int _ammoCount = 15;

    [SerializeField]
    private GameObject _shieldVisualizer;
    
    private Transform _laserContainer;

    private Transform _fireContainer;

    [SerializeField]
    private int _score = 0;

    private bool _thrustersActive = false;

    UI_Manager _uiManager;

    [SerializeField]
    private AudioClip _laserClip;

    private AudioSource _audioSource;

    private ExplosionManager _explosionManager;
    
    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private AudioClip _clickClip;
    private float _homingTime;

    [SerializeField]
    private float _thrusterPower = 1;

    [SerializeField]
    private float _thrusterDepletion = 0.75f;
    [SerializeField]
    private float _thrusterRegeneration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0f, -6f, 0f);

        _laserContainer = GameObject.Find("LaserContainer").transform;

        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();

        _audioSource = GetComponent<AudioSource>();

        _explosionManager = GameObject.Find("AudioManager").GetComponent<ExplosionManager>();

        _fireContainer = GameObject.Find("FireContainer").transform;

        if (_laserContainer == null)
        {
            Debug.LogError("No Laser Container found!");
        }

        if (_uiManager == null)
        {
            Debug.LogError("No UI Manager found!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("No Audio Source found!");
        }

        if (_explosionManager == null)
        {
            Debug.LogError("No Explosion Manager found!");
        }

        if (_fireContainer == null)
        {
            Debug.LogError("No Fire Container found!");
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
            if (_ammoCount > 0)
            {
                GameObject laser;
                if (Time.time < _homingTime)
                {
                    Debug.Log("Missile?");
                    laser = Instantiate(_homingPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);
                }
                else if (Time.time < _tripleShotTime)
                {
                    laser = Instantiate(_tripleLaserPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);
                }
                else
                {
                    laser = Instantiate(_laserPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);
                }
                laser.transform.parent = _laserContainer;
                _canFire = Time.time + _fireRate;

                _audioSource.PlayOneShot(_laserClip);
                AdjustAmmo(-1);
            }
            else
            {
                _audioSource.PlayOneShot(_clickClip);
            }
        }
    }

    private void CalculateMovement()
    {
        _thrustersActive = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && _thrusterPower > 0.0f;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        float effectiveSpeed = GetEffectiveSpeed(Time.deltaTime);

        _thrusterPower = Mathf.Clamp(_thrusterPower + Time.deltaTime * _thrusterRegeneration, 0f, 1f);
        _uiManager.setThrusterPower(_thrusterPower);

        transform.Translate(Vector3.right * horizontalInput * effectiveSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * effectiveSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, _lBound, _rBound), Mathf.Clamp(this.transform.position.y, _dBound, _uBound), this.transform.position.z);
    }

    private float GetEffectiveSpeed(float deltaTime)
    {
        float effectiveSpeed = _speed;
        if (Time.time < _speedTime)
        {
            effectiveSpeed *= _speedBoost;
        }

        if (_thrustersActive)
        {
            effectiveSpeed *= _thrusterBoost;
            _thrusterPower = Mathf.Clamp(_thrusterPower - deltaTime * _thrusterDepletion, 0f, 1f);
        }

        return effectiveSpeed;
    }

    public void TurnOnTripleShot(float _powerupDuration)
    {
        _tripleShotTime = Time.time + _powerupDuration;
    }

    public void TurnOnSpeed(float _powerupDuration)
    {
        _speedTime = Time.time + _powerupDuration;
    }

    public void AdjustShield(int charge)
    {
        _shieldPower = Math.Clamp(_shieldPower + charge, 0, _maxShieldPower);
        ChangeShield();
    }

    private void ChangeShield()
    {
        SpriteRenderer spriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();

        float gradient = (_shieldPower / (float)_maxShieldPower);
        spriteRenderer.color = new Color(1, gradient, gradient, Mathf.Sqrt(gradient));
    }

    public void Damage()
    {
        if (_shieldPower > 0)
        {
            // Tank the hit
            AdjustShield(-1);
            return;
        }

        AdjustHealth(-1);        
    }

    public void AddScore(int score)
    {
        _score += score;
        _uiManager.SetScore(_score);
    }

    public void AdjustAmmo(int amount)
    {
        _ammoCount += amount;
        _uiManager.SetAmmo(_ammoCount);
    }

    internal void AdjustHealth(int amount)
    {
        if (amount == 0) { return; }
        _lives = Mathf.Clamp(_lives + amount, 0, 3);

        _uiManager.SetLives(_lives);

        if (amount < 0)
        {
            for (int i = amount; i < 0; ++i)
            {
                GameObject fire = Instantiate(_firePrefab, transform.position, Quaternion.identity);
                fire.transform.SetParent(_fireContainer.transform);
                float posX = UnityEngine.Random.Range(-0.62f, 0.62f);
                fire.transform.Translate(new Vector3(posX, -1.7f, 0f));
            }
        }

        if (amount > 0)
        {
            if (_fireContainer.childCount > 0)
            {
                for (int i = 0; i < amount; ++i)
                {
                    GameObject fire = _fireContainer.GetChild(0).gameObject;
                    if (fire != null)
                    {
                        Debug.Log("Destroying fire at X pos " + fire.transform.position.x);
                        Destroy(fire);
                    }
                }
            }
            else
            {
                Debug.Log("Could not find a fire to heal.");
            }
        }

        if (_lives < 1)
        {
            SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            spawnManager.StopSpawningEnemies();
            spawnManager.StopSpawningPowerups();

            Instantiate(_explosion, this.transform.position, Quaternion.identity);
            _explosionManager.PlayExplosion();

            _uiManager.DisplayGameOver();

            Destroy(this.gameObject, 0.3f);
        }
    }

    internal void TurnOnMissile(float powerupDuration)
    {
        Debug.Log("Missile turned on");
        _homingTime = Time.time + powerupDuration;
    }
}
