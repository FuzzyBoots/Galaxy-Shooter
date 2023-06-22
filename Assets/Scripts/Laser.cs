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
        Move(Vector3.up);
    }

    private void Move(Vector2 direction)
    {
        this.transform.Translate(direction * _laserSpeed * Time.deltaTime);

        if (Mathf.Abs(this.transform.position.x) > _destroyDistance ||
            Mathf.Abs(this.transform.position.y) > _destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        this.transform.Rotate(new Vector3(0, 0, 180));
        _isEnemyLaser= true;
    }

    public void SetSpeed(float speed)
    {
        _laserSpeed = speed;
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
        } else if (collision.tag == "Mine")
        {
            Mine mine = collision.GetComponent<Mine>();
            if (mine != null)
            {
                mine.Die();
            }
            else
            {
                Debug.Log("Could not find mine to kill");
            }

            Destroy(this.gameObject);
        } else if (_isEnemyLaser && collision.tag == "Powerup")
        {
            // Enemies can destroy power-ups
            Destroy(collision.gameObject);

            Destroy(this.gameObject);
        }
    }
}
