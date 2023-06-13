using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileEnemy : HomingMissile
{
    protected override Transform GetClosestTarget()
    {
        return _playerRef.transform;
    }
}
