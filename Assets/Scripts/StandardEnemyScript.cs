using System;
using System.Collections;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Random = UnityEngine.Random;

public class StandardEnemyScript : MonoBehaviour
{
    public enum MovementStyle
    {
        StraightDown,
        CaromDownL,
        CaromDownR,
        END_STANDARD,
        Ram,
        NONE,
    }

    public static MovementStyle GetRandomMovementStyle()
    {
        Array values = Enum.GetValues(typeof(MovementStyle));
        MovementStyle randomMovement = (MovementStyle)values.GetValue(Random.Range(0, ((int)MovementStyle.END_STANDARD)));
        return randomMovement;
    }

    public enum AttackStyle
    {
        FireLaser,
        Ram,
        DropMines,
        FireFromRear,
    }

    public static AttackStyle GetRandomAttackStyle()
    {
        Array values = Enum.GetValues(typeof(AttackStyle));
        AttackStyle randomAttack = (AttackStyle)values.GetValue(Random.Range(0, values.Length));
        Debug.Log(randomAttack.ToString());
        return randomAttack;
    }

    [SerializeField]
    private GameObject _turret;

    [SerializeField]
    private GameObject _shieldVisualizer;

    [SerializeField] 
    private MovementStyle _movementStyle = MovementStyle.StraightDown;

    [SerializeField]
    private AttackStyle _attackStyle = AttackStyle.FireLaser;

    [SerializeField]
    private float _enemySpeed = 4f;

    [SerializeField]
    private float _enemyRamSpeed = 12f;

    [SerializeField]
    private float _mineFrequency = 2.0f;

    [SerializeField]
    private static PlayerScript _player;

    private Animator _animator;

    private bool _isDead = false;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _fireRate = 3f;

    private float _canFire = -1f;

    [SerializeField]
    private float _ramDistance = 4f;

    [SerializeField]
    private float _laserDistance = 8f;

    private int _shieldPower;
    
    private float _canMine;

    [SerializeField]
    private GameObject _minePrefab;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private float _rotationSpeed = 4f;
    private bool _shootingBack = false;

    private Coroutine _turnAndShootCoroutine;

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

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Could not find enemy animation");
        }

        // _turret = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        HandleAttack();
    }

    private void HandleAttack()
    {
        if (_isDead || GameManager.GameOver) return;

        switch (_attackStyle)
        {
            case AttackStyle.FireLaser:
                HandleLaser();
                break;
            case AttackStyle.Ram:
                HandleRam();
                break;
            case AttackStyle.DropMines:
                HandleMine();
                break;
            case AttackStyle.FireFromRear:
                HandleAttackFromRear();
                break;
        }
    }

    private void HandleAttackFromRear()
    {
        if (_player == null)
        {
            Debug.Log("Player is null during HandleAttackFromRear");
        }

        if (_player.transform.position.y > transform.position.y && !_shootingBack)
        {
            // Wheel around and shoot?
            _turnAndShootCoroutine = StartCoroutine(TurnAroundAndShoot());
        }
    }

    IEnumerator TurnAroundAndShoot()
    {
        _shootingBack = true;
        bool sweepingRight = true;

        Debug.Log("Entering turn around");
        // Turn until reversed
        while (_turret.transform.eulerAngles.z < 180f && transform.position.y > GameManager.dBound)
        {
            _turret.transform.Rotate(Vector3.forward * Time.deltaTime * _rotationSpeed);
            Debug.Log($"Rotation: {_turret.transform.eulerAngles.z}");

            Ray2D ramRay = new Ray2D(_turret.transform.position, -_turret.transform.up * _laserDistance);
            Debug.DrawRay(ramRay.origin, ramRay.direction * _laserDistance);

            if (Time.time <= _canFire)
            {
                // yield return new WaitForSeconds(_canFire - Time.time);
                yield return new WaitForSeconds(0.1f);
            }
            float _delay = Random.Range(0f, 2f);
            _canFire = Time.time + _fireRate + _delay;

            RaycastHit2D[] hitData = Physics2D.RaycastAll(ramRay.origin, ramRay.direction, _laserDistance);

            foreach (RaycastHit2D hit in hitData)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Vector3 laserVector = new Vector3(0, -1.8f, 0f);
                    laserVector = Vector3.RotateTowards(laserVector, _turret.transform.up, 5, 0);

                    Laser enemyLaser = Instantiate(_laserPrefab, transform.position + laserVector, Quaternion.identity).GetComponent<Laser>();
                    enemyLaser.AssignEnemyLaser();
                    enemyLaser.SetVector(laserVector.normalized);
                    Debug.Break();
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Exiting");
        
        // Rotate back
        this.transform.rotation = Quaternion.identity;
        Debug.Log(transform.rotation.eulerAngles);
        _shootingBack = false;

        yield break;
    }

    private void HandleMine()
    {
        if (Time.time <= _canMine)
        {
            return;
        }

        float _delay = Random.Range(0f, 2f);
        _canMine = Time.time + _mineFrequency + _delay;

        Instantiate(_minePrefab, transform.position + Vector3.up * 2, Quaternion.identity);
    }

    private void HandleRam()
    {
        Ray2D ramRay = new Ray2D(transform.position + Vector3.down, Vector3.down);
        Debug.DrawRay(ramRay.origin, ramRay.direction * _ramDistance);

        RaycastHit2D[] hitData = Physics2D.RaycastAll(ramRay.origin, ramRay.direction, _ramDistance);

        foreach (RaycastHit2D hit in hitData)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                // Start ram
                _movementStyle = MovementStyle.Ram;
            }
        }
    }

    private void HandleLaser()
    {
        if (Time.time <= _canFire)
        {
            return;
        }
        float _delay = Random.Range(0f, 2f);
        _canFire = Time.time + _fireRate + _delay;

        Laser enemyLaser_L = Instantiate(_laserPrefab, transform.position + new Vector3(-0.23f, -1.8f, 0f), Quaternion.identity).GetComponent<Laser>();
        Laser enemyLaser_R = Instantiate(_laserPrefab, transform.position + new Vector3(0.23f, -1.8f, 0f), Quaternion.identity).GetComponent<Laser>();
        
        enemyLaser_L.AssignEnemyLaser();
        enemyLaser_R.AssignEnemyLaser();
    }

    private void CalculateMovement()
    {
        switch (_movementStyle)
        {
            case MovementStyle.StraightDown:
                transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime, Space.World);
                break;
            case MovementStyle.CaromDownL:
                transform.Translate(new Vector3(-0.7f, -0.7f, 0f) * _enemySpeed * Time.deltaTime, Space.World);
                if (transform.position.x < GameManager.lBound)
                {
                    _movementStyle = MovementStyle.CaromDownR;
                }
                break;
            case MovementStyle.CaromDownR:
                transform.Translate(new Vector3(0.7f, -0.7f, 0f) * _enemySpeed * Time.deltaTime, Space.World);
                if (transform.position.x > GameManager.rBound)
                {
                    _movementStyle = MovementStyle.CaromDownL;
                }
                break;
            case MovementStyle.Ram:
                transform.Translate(Vector3.down * _enemyRamSpeed * Time.deltaTime, Space.World);
                break;
            case MovementStyle.NONE:
                // Do nothing
                transform.Translate(Vector3.up * 0f);
                break;
        }

        if (transform.position.y < GameManager.dBound && !_isDead)
        {
            if (_movementStyle == MovementStyle.Ram)
            {
                _movementStyle = GetRandomMovementStyle();
            }
            transform.position = new Vector3(Random.Range(GameManager.lBound, GameManager.rBound), GameManager.uBound, 0f);
        }
    }

    public void AddShield()
    {
        _shieldPower = 1;
        ChangeShield();
    }

    private void ChangeShield()
    {
        SpriteRenderer spriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();

        spriteRenderer.color = new Color(1, 0, 0, _shieldPower);
    }

    public void Damage()
    {
        if (_shieldPower > 0)
        {
            _shieldPower--;
            ChangeShield();
            return;
        }
        _player?.AddScore(10);

        Die();
    }

    public void Die()
    {
        _animator.SetTrigger("OnEnemyDeath");
        
        GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
        explosion.transform.SetParent(transform);

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
            Destroy(other.gameObject);

            Damage();
        }

        if (other.tag == "Player")
        {
            other.transform.GetComponent<PlayerScript>()?.Damage();

            Die();
        }
    }

    internal void SetMovementStyle(MovementStyle movement)
    {
        _movementStyle = movement;
    }

    internal void SetAttackStyle(AttackStyle attack)
    {
        _attackStyle = attack;
    }
}
