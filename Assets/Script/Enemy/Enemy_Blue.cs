using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Blue : Enemy {
    public override byte Level {
        get {
            return 1; 
        }
    }
    public override int getScore {
        get {
            return 100;
        }
    }

    protected override void Attack() {
        var target = getRandomLocationOfPlayer();
        var direction = Vector3.Normalize(transform.position - target);
        var offsetedTarget = target - (direction * 4);
        transform.LookAt2D(target);
        StartCoroutine(LineMove(offsetedTarget, GotoStandbyLocation));
    }
}
