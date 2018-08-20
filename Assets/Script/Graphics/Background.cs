using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	void Update () {
        if (transform.localPosition.y < -25.5f) {
            transform.localPosition = new Vector3(0, 25.5f, 1);
        }
        transform.position += Vector3.down * 0.15f;
        
	}
    
}
