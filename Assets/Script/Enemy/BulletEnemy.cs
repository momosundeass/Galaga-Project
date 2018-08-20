using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour {
	void Start () {
        StartCoroutine(waitAndDestroy());
	}

    private void FixedUpdate() {
        transform.Translate(Game.parameter.BulletSpeed / 1.6f, 0, 0);
    }

    // Destroy self after 3 second
    IEnumerator waitAndDestroy() {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
