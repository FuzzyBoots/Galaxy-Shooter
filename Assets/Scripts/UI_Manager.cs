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
    private bool _gameOver = false;

    [SerializeField]
    private float _thrustPercentage = 1.0f;
    
    [SerializeField]
    TMP_Text _ammoText;

    [SerializeField]
    Image _thrusterImage;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _reloadText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.R))
        {
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
        _thrusterImage.rectTransform.anchoredPosition = Vector3.left * effectivePercentage * _thrusterImage.rectTransform.rect.width;
    }

    public void SetScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void SetLives(int lives)
    {
        _livesImg.sprite = _livesSprites[lives];
    }

    public void DisplayGameOver()
    {
        _gameOver = true;
        _gameOverText.gameObject.SetActive(true);
        _reloadText.gameObject.SetActive(true);
        StartCoroutine(FlickerGameOver());
    }

    IEnumerator FlickerGameOver()
    {
        float interval = _flickerFrequency > 0 ? 1f / _flickerFrequency : 0.5f;
        float endTime = Time.time + _flickerTime;
        while (Time.time < endTime)
        {
            _gameOverText.gameObject.SetActive(!_gameOverText.gameObject.activeSelf);
            yield return new WaitForSeconds(interval);
        }
        _gameOverText.gameObject.SetActive(true);
        yield break;
    }

    internal void SetAmmo(int ammoCount)
    {
        _ammoText.text = "Ammo: " + ammoCount;
    }

    internal void setThrusterPower(float thrusterPower)
    {
        _thrustPercentage = thrusterPower;
    }
}
