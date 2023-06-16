using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    enum BossState
    {
        EnteringScene,
        FiringLasers,
        DroppingMines,
        FiringMissiles,
    }

    BossState _state = BossState.EnteringScene;

    [SerializeField] 
    float _enemySpeed = 10f;

    [SerializeField]
    int _health = 32;

    [SerializeField]
    bool _movingRight = true;

    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("In Animator?");
        if (this._animator?.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            AnimatorClipInfo animatorinfo = this._animator.GetCurrentAnimatorClipInfo(0)[0];

            if (animatorinfo.clip.name == "BossDescent_anim") { return; }
        } else if (_animator != null)  
        {
            Debug.Break();
            _animator.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        HandleMovement();        
    }

    private void HandleMovement()
    {
        if (_movingRight)
        {
            transform.Translate(Vector3.right * _enemySpeed * Time.deltaTime);
            Debug.Log("Right " + transform.position + _enemySpeed * Time.deltaTime);
            if (transform.position.x < -7f)
            {
                _movingRight = false;
            }
        } else
        {
            transform.Translate(Vector3.left * _enemySpeed * Time.deltaTime);

            Debug.Log("Left " + transform.position);
            if (transform.position.x > 7f)
            {
                _movingRight = true;
            }
        }
    }
}
