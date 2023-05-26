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
        Shield
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

            Debug.Log("Playing Sound");
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
                    player?.SetShield(true);
                    break;
                default:
                    Debug.Log("Unexpected!");
                    break;
            }
            
            SpriteRenderer _renderer = gameObject.GetComponent<SpriteRenderer>();
            _renderer.enabled = false;
            
            Destroy(this.gameObject);
        }
    }
}
