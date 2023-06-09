using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 3f;

    [SerializeField] GameObject _explosion;

    [SerializeField] SpawnManager _spawnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("No Spawn Manager found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(Vector3.forward, _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            GameObject explosion = Instantiate(_explosion, this.transform.position, Quaternion.identity);

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            else
            {
                Debug.Log("No Collider2D found, could not disable");
            }

            _spawnManager.StartSpawning();

            Destroy(collision.gameObject);

            Destroy(this.gameObject, 0.3f);
        }
    }
}
