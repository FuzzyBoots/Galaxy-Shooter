using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;



public class Powerup : MonoBehaviour
{    enum PowerupEnum
    {
        TripleShot,
        Speed,
        Shield
    }

    [SerializeField]
    float _powerupSpeed = 3f;

    [SerializeField]
    private PowerupEnum _powerupIdent;

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
            
            switch (_powerupIdent)
            {
                case PowerupEnum.TripleShot:
                    player?.TurnOnTripleShot();
                    break;
                case PowerupEnum.Speed:
                    Debug.Log("Speed!");
                    break;
                case PowerupEnum.Shield:
                    Debug.Log("Shield!");
                    break;
                default:
                    Debug.Log("Unexpected!");
                    break;
            }
            
            
            Destroy(this.gameObject);
        }
    }
}
