using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public static void SpawnExplosion(Vector3 location) {
        Instantiate(Game.i.Explosion,
            new Vector3(location.x, location.y, -0.01f),
            Quaternion.Euler(0, 0, Random.Range(0, 180)),
            Game.i.State.transform);
    }
    public void CallRemove() {
        Destroy(gameObject);
    }
}
