using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class Powerup : MonoBehaviour
{
    enum PowerupEnum
    {
        TripleShot,
        Speed,
        Shield,
        Ammo,
        Health,
        Homing,
        Reverse,
        Lightning,
    }

    [SerializeField]
    float _powerupSpeed = 3f;

    [SerializeField]
    float _powerupDuration = 3f;

    [SerializeField]
    private PowerupEnum _powerupIdent;

    [SerializeField] AudioClip _powerupClip;
    static bool _attractionOn;

    [SerializeField]
    float _attractionSpeed = 5f;

    private Transform _playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (_playerTransform == null )
        {
            Debug.LogError("Could not find Player transform");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.down * _powerupSpeed;

        if (_attractionOn )
        {
            Vector3 attractionVector = _playerTransform.position - transform.position;
            movement += attractionVector.normalized * _attractionSpeed;
        }

        this.transform.Translate(movement * Time.deltaTime);

        if (this.transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerScript player = collision.transform.GetComponent<PlayerScript>();

            AudioSource.PlayClipAtPoint(_powerupClip, transform.position);
            
            switch (_powerupIdent)
            {
                case PowerupEnum.TripleShot:
                    player?.TurnOnTripleShot(_powerupDuration);
                    break;
                case PowerupEnum.Speed:
                    player?.TurnOnSpeed(_powerupDuration);
                    break;
                case PowerupEnum.Shield:
                    player?.AdjustShield(3);
                    break;
                case PowerupEnum.Ammo:
                    player?.AdjustAmmo(10);
                    break;
                case PowerupEnum.Health:
                    player?.AdjustHealth(1);
                    break;
                case PowerupEnum.Homing:
                    player?.TurnOnMissile(_powerupDuration);
                    break;
                case PowerupEnum.Reverse:
                    player?.TurnOnReverse(_powerupDuration);
                    break;
                case PowerupEnum.Lightning:
                    player?.TurnOnLightning(_powerupDuration);
                    break;
                default:
                    Debug.LogError("Unexpected Powerup type");
                    break;
            }
            
            SpriteRenderer _renderer = gameObject.GetComponent<SpriteRenderer>();
            _renderer.enabled = false;
            
            Destroy(this.gameObject);
        }
    }
    internal static void SetAttraction(bool attractionOn)
    {
        _attractionOn = attractionOn;
    }
}
