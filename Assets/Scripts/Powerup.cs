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
    }

    [SerializeField]
    float _powerupSpeed = 3f;

    [SerializeField]
    float _powerupDuration = 3f;

    [SerializeField]
    private PowerupEnum _powerupIdent;

    [SerializeField] AudioClip _powerupClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);

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
                default:
                    Debug.LogError("Unexpected Powerup type");
                    break;
            }
            
            SpriteRenderer _renderer = gameObject.GetComponent<SpriteRenderer>();
            _renderer.enabled = false;
            
            Destroy(this.gameObject);
        }
    }
}
