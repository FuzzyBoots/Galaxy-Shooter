using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
}
