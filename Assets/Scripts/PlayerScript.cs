using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedBoost = 1.8f;
    [SerializeField]
    private float _thrusterBoost = 1.5f;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    [SerializeField]
    private GameObject _firePrefab;
    [SerializeField]
    private GameObject _homingPrefab;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private GameObject _lightningPrefab;


    [Header("Timing")]
    [SerializeField]
    private float _fireRate = 0.5f;  // half a second
    private float _canFire = 0;
    private float _tripleShotTime = 0;
    private float _speedTime = 0;
    private float _homingTime = 0;
    private float _reverseTime;
    private float _lightningTime;

    [Header("Player Variables")]
    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private int _shieldPower = 0;
    [SerializeField]
    private int _maxShieldPower = 3;

    [SerializeField]
    private int _ammoCount = 15;
    [SerializeField]
    private int _maxAmmoCount = 50;

    [SerializeField]
    private int _score = 0;

    [Header("Visualizers")]
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _thrusterVisualizer;

    private Transform _laserContainer;
    private Transform _fireContainer;

    private bool _thrustersActive = false;

    UI_Manager _uiManager;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private AudioClip _missileClip;
    [SerializeField]
    private AudioClip _lightningClip;
    [SerializeField]
    private AudioClip _clickClip;

    private AudioSource _audioSource;

    [Header("Truster variables")]    
    [SerializeField]
    private float _thrusterPower = 1;
    [SerializeField]
    private float _thrusterDepletion = 0.75f;
    [SerializeField]
    private float _thrusterRegeneration = 0.25f;
    private float _thrusterTimeout = 0;
    [SerializeField]
    private float _thrusterOverheatTimeout = 3f;

    [Header("Camera")]
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _shakeTime = 0.5f;
    [SerializeField]
    private float _shakeAmount = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0f, GameManager.dBound, 0f);

        _laserContainer = GameObject.Find("LaserContainer").transform;

        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();

        _audioSource = GetComponent<AudioSource>();

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

        if (_fireContainer == null)
        {
            Debug.LogError("No Fire Container found!");
        }

        // Initialize UI
        _uiManager.SetScore(_score);
        _uiManager.SetAmmo(_ammoCount, _maxAmmoCount);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        AttractPowerups();

        HandleLaser();
    }

    private void AttractPowerups()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Powerup.SetAttraction(true);
        } else if (Input.GetKeyUp(KeyCode.C))
        {
            Powerup.SetAttraction(false);
        }
    }

    private void HandleLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_ammoCount > 0)
            {
                bool alreadyFired = false;
                if (Time.time < _homingTime)
                {
                    _audioSource.PlayOneShot(_missileClip);
                    GameObject missile = Instantiate(_homingPrefab, this.transform.position + Vector3.up * 2f, Quaternion.identity);

                    missile.transform.parent = _laserContainer;
                    alreadyFired = true;
                }
                
                if (Time.time < _lightningTime)
                {
                    _audioSource.PlayOneShot(_lightningClip);
                    GameObject lightningR = Instantiate(_lightningPrefab, this.transform.position + Vector3.right * 2f, Quaternion.identity);
                    GameObject lightningL = Instantiate(_lightningPrefab, this.transform.position + Vector3.left * 2f, Quaternion.identity);
                    lightningL.GetComponent<ChainLightning>()?.SwapDirection();

                    lightningL.transform.parent = _laserContainer;
                    lightningR.transform.parent = _laserContainer;
                    alreadyFired = true;
                }
                
                if (Time.time < _tripleShotTime)
                {
                    _audioSource.PlayOneShot(_laserClip);
                    GameObject laser = Instantiate(_tripleLaserPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);

                    laser.transform.parent = _laserContainer;
                    alreadyFired = true;
                }
                
                if (!alreadyFired)
                {
                    _audioSource.PlayOneShot(_laserClip);
                    GameObject laser = Instantiate(_laserPrefab, this.transform.position + Vector3.up * 0.9f, Quaternion.identity);

                    laser.transform.parent = _laserContainer;
                }

                _canFire = Time.time + _fireRate;

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
        _thrustersActive = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
            && _thrusterPower > 0f;
        _thrusterVisualizer.SetActive(_thrustersActive);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Time.time < _reverseTime)
        {
            horizontalInput *= -1;
            verticalInput *= -1;
        }
        
        float effectiveSpeed = GetEffectiveSpeed();

        if (_thrustersActive)
        {
            _thrusterPower = Mathf.Clamp(_thrusterPower - Time.deltaTime * _thrusterDepletion, 0f, 1f);
            if (_thrusterPower < 0.05f)
            {
                // Overheat
                _thrusterPower = 0f;
                _thrusterTimeout = Time.time + _thrusterOverheatTimeout;
            }
        } else if (_thrusterTimeout <= Time.time)
        {
            _thrusterPower = Mathf.Clamp(_thrusterPower + Time.deltaTime * _thrusterRegeneration, 0f, 1f);
        }

        _uiManager.SetOverheatVisible(_thrusterTimeout > Time.time);

        _uiManager.SetThrusterPower(_thrusterPower);

        transform.Translate(Vector3.right * horizontalInput * effectiveSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * effectiveSpeed * Time.deltaTime);

        transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, GameManager.lBound, GameManager.rBound), Mathf.Clamp(this.transform.position.y, GameManager.dBound, GameManager.uBound), this.transform.position.z);
    }

    private float GetEffectiveSpeed()
    {
        float effectiveSpeed = _speed;
        if (Time.time < _speedTime)
        {
            effectiveSpeed *= _speedBoost;
        }

        if (_thrustersActive)
        {
            effectiveSpeed *= _thrusterBoost;
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

        StartCoroutine(ShakeCamera());
        AdjustHealth(-1);        
    }

    IEnumerator ShakeCamera()
    {
        Vector3 origPos = _camera.transform.position;
        float endShake = Time.time + _shakeTime;
        while (Time.time < endShake)
        {
            _camera.transform.position = origPos + new Vector3(Random.Range(-_shakeAmount, _shakeAmount), Random.Range(-_shakeAmount, _shakeAmount), 0);
            yield return new WaitForSeconds(0.1f);
        }
        _camera.transform.position = origPos;
    }

    public void AddScore(int score)
    {
        _score += score;
        _uiManager.SetScore(_score);
    }

    public void AdjustAmmo(int amount)
    {
        _ammoCount += amount;
        _ammoCount = Math.Clamp(_ammoCount, 0, _maxAmmoCount);
        _uiManager.SetAmmo(_ammoCount, _maxAmmoCount);
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
                        Destroy(fire);
                    }
                }
            }
        }

        if (_lives < 1)
        {
            SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            spawnManager.StopSpawningEnemies();
            spawnManager.StopSpawningPowerups();

            Instantiate(_explosion, this.transform.position, Quaternion.identity);
            
            _uiManager.DisplayGameOver();

            Destroy(this.gameObject, 0.3f);
        }
    }

    internal void TurnOnMissile(float powerupDuration)
    {
        _homingTime = Time.time + powerupDuration;
    }

    internal void TurnOnReverse(float powerupDuration)
    {
        _reverseTime = Time.time + powerupDuration;
    }

    internal void TurnOnLightning(float powerupDuration)
    {
        _lightningTime = Time.time + powerupDuration;
    }
}
