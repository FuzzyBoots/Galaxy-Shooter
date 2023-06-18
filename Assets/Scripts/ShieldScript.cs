using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    [SerializeField]
    int _shieldStrength = 0;

    [SerializeField]
    int _maxShieldStrength = 5;

    [SerializeField]
    SpriteRenderer _spriteRenderer;

    private void Start()
    {
        if (_spriteRenderer == null)
        {
            Debug.LogError("No sprite renderer is set!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_shieldStrength <= 0) { return; }

        if (collision.CompareTag("Laser") ||
            collision.CompareTag("Missile") ||
            collision.CompareTag("Lightning"))
        {
            _shieldStrength--;
            
            UpdateShield();
        }
    }

    private void UpdateShield()
    {
        float shieldFraction = (float)_shieldStrength / _maxShieldStrength;
        _spriteRenderer.color = new Color(1, 1, 1, shieldFraction);
    }

    public void ReviveShield(int amount)
    {
        _shieldStrength += amount;
        _shieldStrength = Math.Clamp(_shieldStrength, 0, _maxShieldStrength);
        UpdateShield();
    }
}
