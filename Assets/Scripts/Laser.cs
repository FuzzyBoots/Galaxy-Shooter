using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 10;

    [SerializeField]
    private float _destroyDistance = 10;
    private Vector2 _directionVector = Vector2.up;
    [SerializeField]
    private bool _isEnemyLaser;

    // Update is called once per frame
    void Update()
    {
        Move(_directionVector);
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
        _directionVector = Vector2.down;
        _isEnemyLaser= true;
    }

    public void SetVector(Vector2 directionVector)
    {
        this.transform.Rotate(Vector3.forward * Vector2.Angle(Vector2.up, directionVector));
        _directionVector = directionVector;
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
        }
    }
}
