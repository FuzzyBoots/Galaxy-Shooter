using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissilePlayer : HomingMissile
{
    protected override Transform GetClosestTarget()
    {
        Transform _nearestEnemy = null;
        float closestEnemyDist = Mathf.Infinity;
        foreach (Transform enemy in _enemyContainer.transform.GetComponentsInChildren<Transform>())
        {
            float _dist = Vector3.Distance(transform.position, enemy.position);
            if (_dist < closestEnemyDist)
            {
                _nearestEnemy = enemy;
                closestEnemyDist = _dist;
            }
        }

        return _nearestEnemy;
    }
}
