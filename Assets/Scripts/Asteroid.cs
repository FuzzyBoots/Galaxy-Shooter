using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 3f;

    [SerializeField] GameObject _explosion;
    
    private ExplosionManager _explosionManager;

    [SerializeField] SpawnManager _spawnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        _explosionManager = GameObject.Find("AudioManager").GetComponent<ExplosionManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("No Spawn Manager found!");
        }

        if (_explosionManager == null)
        {
            Debug.LogError("No Explosion Manager found!");
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
            Instantiate(_explosion, this.transform.position, Quaternion.identity);

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            else
            {
                Debug.Log("No Collider2D found, could not disable");
            }

            _explosionManager.PlayExplosion();

            _spawnManager.StartSpawning();

            Destroy(collision.gameObject);

            Destroy(this.gameObject, 0.3f);
        }
    }
}
