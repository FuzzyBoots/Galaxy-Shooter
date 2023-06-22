using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{
    [SerializeField]
    RawImage _healthImage;

    [SerializeField]
    int _maxHealth;

    [SerializeField]
    int _curHealth;
    private float _origWidth;

    // Start is called before the first frame update
    void Start()
    {
        if (_healthImage == null)
        {
            Debug.LogError("No health image specified for boss health bar.");
        }
        _origWidth = _healthImage.rectTransform.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int value)
    {
        _curHealth = value;
        UpdateBar();
    }

    private void UpdateBar()
    {
        Debug.Log($"Boss Health: {_curHealth} / {_maxHealth}");
        _healthImage.rectTransform.sizeDelta = new Vector2((float)_curHealth / _maxHealth
            * _origWidth, _healthImage.rectTransform.sizeDelta.y);
    }

    public void SetMaxHealth(int value)
    {
        _maxHealth = value;
        UpdateBar();
    }
}
