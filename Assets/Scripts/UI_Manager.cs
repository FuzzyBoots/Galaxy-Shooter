using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    TMP_Text _scoreText;

    [SerializeField]
    private Image _livesImg;

    [SerializeField]
    Sprite [] _livesSprites;

    [SerializeField]
    TMP_Text _gameOverText;

    [SerializeField]
    TMP_Text _reloadText;

    [SerializeField]
    float _flickerTime = 5;
    [SerializeField]
    float _flickerFrequency = 5;

    [SerializeField]
    private float _thrustPercentage = 1.0f;
    
    [SerializeField]
    TMP_Text _ammoText;

    [SerializeField]
    Image _thrusterImage;

    [SerializeField]
    TMP_Text _overheatText;

    [SerializeField]
    TMP_Text _waveText;

    [SerializeField]
    TMP_Text _winText;

    [SerializeField]
    RawImage _healthBarImage;
    
    [SerializeField]
    GameObject _healthBar;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _reloadText.gameObject.SetActive(false);
        SetBossHealthEnabled(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameOver && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.GameOver = false;
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) { 
            #if (UNITY_EDITOR)
                 UnityEditor.EditorApplication.isPlaying = false;
            #elif (UNITY_STANDALONE) 
                Application.Quit();
            #elif (UNITY_WEBGL)
                // Doesn't actually fix the problem... and apparently closing the 
                // tab in code was eliminated due to security issues.
                Application.OpenURL("about:blank");
            #endif
        }

        float effectivePercentage = 1f - Mathf.Clamp(_thrustPercentage, 0.0f, 1.0f);
        _thrusterImage.rectTransform.anchoredPosition = Vector3.left * effectivePercentage 
            * _thrusterImage.rectTransform.rect.width;
    }

    public void SetScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void SetLives(int lives)
    {
        _livesImg.sprite = _livesSprites[lives];
    }

    public void DisplayGameOver(bool winning = false)
    {
        TMP_Text endingText;
        GameManager.GameOver = true;
        
        if (!winning)
        {
            endingText = _gameOverText;
        }
        else
        {
            endingText = _winText;
        }
        endingText.gameObject.SetActive(true);
        _reloadText.gameObject.SetActive(true);
        StartCoroutine(FlickerText(endingText));
    }

    IEnumerator FlickerText(TMP_Text text)
    {
        float interval = _flickerFrequency > 0 ? 1f / _flickerFrequency : 0.5f;
        float endTime = Time.time + _flickerTime;
        while (Time.time < endTime)
        {
            text.gameObject.SetActive(!_gameOverText.gameObject.activeSelf);
            yield return new WaitForSeconds(interval);
        }
        text.gameObject.SetActive(true);
        yield break;
    }

    public void ShowWaveText(int wave)
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = $"WAVE {wave}";
        StartCoroutine(PulseText(_waveText));
    }

    IEnumerator PulseText(TMP_Text text)
    {
        float startTime = Time.time;
        float endTime = Time.time + 1f;
        Color origColor = text.color;
        while (Time.time < endTime)
        {
            text.color = new Color(origColor.r, origColor.g, origColor.g, (Mathf.Cos(6.28f * (Time.time - startTime) * 2) + 1) / 2);
            yield return new WaitForSeconds(0.1f);
        }
        text.color = origColor;
        text.gameObject.SetActive(false);
        yield break;
    }

    public void SetAmmo(int ammoCount, int maxAmmoCount)
    {
        _ammoText.text = $"Ammo: {ammoCount} / {maxAmmoCount}";
    }

    public void SetThrusterPower(float thrusterPower)
    {
        _thrustPercentage = thrusterPower;
    }

    public void SetOverheatVisible(bool visible)
    {
        _overheatText.gameObject.SetActive(visible);
    }

    public void SetBossHealthEnabled(bool value)
    {
        _healthBar.SetActive(value);
    }

    public void SetBossMaxHealth(int value)
    {
        _healthBar.GetComponent<HealthBar>().SetMaxHealth(value);
    }

    public void SetBossHealth(int value)
    {
        _healthBar.GetComponent<HealthBar>().SetHealth(value);
    }
}
