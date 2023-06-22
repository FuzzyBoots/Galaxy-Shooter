using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileEnemy : HomingMissile
{
    protected override Transform GetClosestTarget()
    {
        if (_playerRef != null)
        {
            return _playerRef.transform;
        }
        else { 
            return null; 
        }
    }
}
