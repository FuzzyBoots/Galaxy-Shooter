using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
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

        _audioSource.PlayOneShot(_explosionClip);

        Destroy(this.gameObject, 2.65f);
    }
}
