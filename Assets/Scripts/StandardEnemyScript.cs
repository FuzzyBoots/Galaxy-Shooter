using System;
using System.Collections;
using System.IO;
using System.Linq;
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
        Ram
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
    }

    public static AttackStyle GetRandomAttackStyle()
    {
        Array values = Enum.GetValues(typeof(AttackStyle));
        AttackStyle randomAttack = (AttackStyle)values.GetValue(Random.Range(0, values.Length));
        Debug.Log(randomAttack.ToString());
        return randomAttack;
    }

    [SerializeField] 
    private MovementStyle _movementStyle = MovementStyle.StraightDown;

    [SerializeField]
    private AttackStyle _attackStyle = AttackStyle.FireLaser;

    [SerializeField]
    private float _enemySpeed = 4f;

    [SerializeField]
    private float _enemyRamSpeed = 12f;

    [SerializeField]
    private static PlayerScript _player;

    private Animator _animator;

    private ExplosionManager _explosionManager;

    private bool _isDead = false;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _fireRate = 3f;

    private float _canFire = -1f;

    [SerializeField]
    private float _ramDistance = 4f;

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

        HandleAttack();
    }

    private void HandleAttack()
    {
        if (_isDead) return;

        switch (_attackStyle)
        {
            case AttackStyle.FireLaser:
                HandleLaser();
                break;
            case AttackStyle.Ram:
                HandleRam();
                break;
        }
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
                Debug.Log($"Distance: {hit.distance} RB: {hit.rigidbody} Collider: {hit.collider}");
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
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

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
                transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
                break;
            case MovementStyle.CaromDownL:
                transform.Translate(new Vector3(-0.7f, -0.7f, 0f) * _enemySpeed * Time.deltaTime);
                if (transform.position.x < GameManager.lBound)
                {
                    _movementStyle = MovementStyle.CaromDownR;
                }
                break;
            case MovementStyle.CaromDownR:
                transform.Translate(new Vector3(0.7f, -0.7f, 0f) * _enemySpeed * Time.deltaTime);
                if (transform.position.x > GameManager.rBound)
                {
                    _movementStyle = MovementStyle.CaromDownL;
                }
                break;
            case MovementStyle.Ram:
                transform.Translate(Vector3.down * _enemyRamSpeed * Time.deltaTime);
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
        
    internal void SetMovementStyle(MovementStyle movement)
    {
        _movementStyle = movement;
    }

    internal void SetAttackStyle(AttackStyle attack)
    {
        _attackStyle = attack;
    }
}
