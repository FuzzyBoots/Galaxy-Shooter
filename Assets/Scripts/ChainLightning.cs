using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    private int _lightningSpawns = 4;

    SpriteRenderer _spriteRenderer;
    private bool _spawnsRight = true;

    public SpriteRenderer SpriteRenderer {
        get { 
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();

                if (SpriteRenderer == null)
                {
                    Debug.LogError("Couldn't find Sprite Renderer!");
                }
            }
            return _spriteRenderer;
        }
        set => _spriteRenderer = value; }



    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setSpawns(int lightningSpawns)
    {
        _lightningSpawns = lightningSpawns;
    }

    public void SwapDirection()
    {
        _spawnsRight = !_spawnsRight;

        SpriteRenderer.flipX = true;
    }

    private void SpawnLightning()
    {
        if (_lightningSpawns < 1) { return; }

        ChainLightning lightning;

        if (_spawnsRight)
        {
            lightning = Instantiate(this, transform.position + transform.right * SpriteRenderer.size.x * 1.3f, Quaternion.identity).GetComponent<ChainLightning>();
        } else
        {
            lightning = Instantiate(this, transform.position - transform.right * SpriteRenderer.size.x * 1.3f, Quaternion.identity).GetComponent<ChainLightning>();
            lightning.SwapDirection();
        }
        lightning.setSpawns(_lightningSpawns - 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Zap any enemy or mine
        switch (collision.tag)
        {
            case "Mine":
                Mine mine = collision.GetComponent<Mine>();
                if (mine != null)
                {
                    mine.Die();
                }
                else
                {
                    Debug.Log("Could not find mine to destroy.");
                }

                break;
            case "Enemy":
                StandardEnemyScript enemy = collision.GetComponent<StandardEnemyScript>();
                if (enemy != null)
                {
                    // _playerRef.AddScore(15);
                    enemy.Die();
                }

                Destroy(this.gameObject);
                break;
        }
    }
}
