using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPlayer : MonoBehaviour {

    // Protect bullet to destroy more than 1 entity.
    private bool used = false;

    private void FixedUpdate() {
        transform.Translate(0, Game.parameter.BulletSpeed, 0);
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.layer == 9) {
            if (!used) {
                used = true;
                collision.gameObject.GetComponent<IDamageAble>().Damage();
            }
        }
        Destroy(gameObject);
    }
}
