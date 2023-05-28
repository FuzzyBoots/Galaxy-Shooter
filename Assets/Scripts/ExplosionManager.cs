using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    AudioSource _audioSource;

    [SerializeField] private AudioClip _explosionClip;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("No Audio Source found!");
        }
    }

    public void PlayExplosion()
    {
        _audioSource.PlayOneShot(_explosionClip);
    }
}
