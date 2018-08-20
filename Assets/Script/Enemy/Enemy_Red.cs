using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Red : Enemy {
    public override byte Level {
        get {
            return 3;
        }
    }
    public override int getScore {
        get {
            return 200;
        }
    }

    protected override void Attack() {
        var target = getRandomLocationOfPlayer(3);
        var direction = Vector3.Normalize(transform.position - target);
        var offsetedTarget = target - (direction * 4);
        transform.LookAt2D(target);
        RandomMove(offsetedTarget, GotoStandbyLocation, MoveType.Line, MoveType.Curve);
        StartCoroutine(ShootBullet(target));
    }

    IEnumerator ShootBullet(Vector3 bulletTarget) {
        yield return new WaitForSeconds(0.4f / Game.parameter.EnemyBulletCount);

        // Shoot rouge bullet first
        var rougeDirection = transform.position.ToVector2() - getRandomLocationOfPlayer(0.5f).ToVector2();
        var rougeAngle = Mathf.Atan2(rougeDirection.y, rougeDirection.x) * Mathf.Rad2Deg + 180;
        Instantiate(Game.i.EnemyRocket,
                // +z bring it to front
                transform.position + new Vector3(0, 0, -0.01f),
                Quaternion.AngleAxis(rougeAngle, Vector3.forward),
                Game.i.State.transform);

        for (int i = 0; i < Game.parameter.EnemyBulletCount - 1; i++) {
            yield return new WaitForSeconds(0.4f / Game.parameter.EnemyBulletCount);
            // Random target for bullet
            // More bullet it shoot more spred it get
            var range = 1.5f + Game.parameter.EnemyBulletCount / 7f;
            var randomedTarget = new Vector3(bulletTarget.x + UnityEngine.Random.Range(-range, range), bulletTarget.y, 0);

            // Rotate bullet before shoot
            var dir = transform.position.ToVector2() - randomedTarget.ToVector2();
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;

            // Build bullet
            Instantiate(Game.i.EnemyRocket, 
                // +z bring it to front
                transform.position + new Vector3(0,0, -0.01f),
                Quaternion.AngleAxis(angle, Vector3.forward),
                Game.i.State.transform);
            
        }
    }
}
