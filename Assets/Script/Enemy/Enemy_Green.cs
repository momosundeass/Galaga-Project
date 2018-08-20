using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Green : Enemy {
    public override byte Level {
        get {
            return 3;
        }
    }
    public override int getScore {
        get {
            return 300;
        }
    }

    protected override void Attack() {
        var aboveTarget = getRandomLocationOfPlayer(0.7f, 2f);
        transform.LookAt2D(aboveTarget);
        RandomMove(aboveTarget, () => StartCoroutine(ShootBeam()), MoveType.Line, MoveType.Sin);
    }

    IEnumerator ShootBeam() {
        var shootBeamLocation = transform.position;
        var currentRotate = transform.rotation;
        // Rotate beforce shoot
        var time = 15;
        for (int i = 0; i < time; i++) {
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(currentRotate, Quaternion.Euler(0, 0, 90), (1f / time) * i);
            // Keep object from moving with it parent while beaming
            transform.position = shootBeamLocation;
        }
        transform.rotation = Quaternion.Euler(0, 0, 90);
        // End rotate

        // Beam Shooting
        var beam = Instantiate(Game.i.EnemyBeam, transform.position + new Vector3(0, -1.5f), Quaternion.identity, transform) as GameObject;
        for (int i = 0; i < 120; i++) {
            transform.position = shootBeamLocation;
            yield return new WaitForFixedUpdate();
        }
        Destroy(beam);
            
        GotoStandbyLocationFromCurrentPosition();
    }
}

