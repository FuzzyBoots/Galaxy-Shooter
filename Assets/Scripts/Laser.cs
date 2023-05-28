using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 10;

    [SerializeField]
    private float _destroyDistance = 10;

    [SerializeField]
    private bool _isEnemyLaser;

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser)
        {
            MoveDown();
        }
        else
        {
            MoveUp();
        }
    }

    private void MoveUp()
    {
        this.transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (this.transform.position.y > _destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveDown()
    {
        this.transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if (this.transform.position.y < -_destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser= true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && _isEnemyLaser)
        {
            PlayerScript player = collision.GetComponent<PlayerScript>();
            if (player != null)
            {
                player?.Damage();
            }
            else
            {
                Debug.Log("Could not find player to damage.");
            }

            GameObject.Destroy(this.gameObject);
        }
    }
}
