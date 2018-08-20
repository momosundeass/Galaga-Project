using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageAble, IGameState {
    const float MOVESPEED = 0.12f;
    const float BREAKSPEED = 0.015f;

    private Vector3 spawnPoint;
    private float xVelocity = 0;

    private void Start() {
        spawnPoint = transform.position + new Vector3(0, -2, 0);
        StartCoroutine(ReSpawn());
    }

    // Input
    private void Update() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            xVelocity = -MOVESPEED;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            xVelocity = MOVESPEED;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Game.i.EnemyControl.isAttackable) {
            Shoot();
        }
    }

    void Shoot() {
        if (Game.i.PlayerBulletTranform.childCount < Game.parameter.MaxBullet) {
            Instantiate(Game.i.PlayerBulletPrefab, transform.position + new Vector3(0, 1f), Quaternion.identity, Game.i.PlayerBulletTranform);
        }
    }

    // Movement Update
    private void FixedUpdate() { 
        transform.Translate(xVelocity, 0, 0);
        var currentPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(currentPos.x, -5.7f, 5.7f), currentPos.y, currentPos.z);
        if (xVelocity < 0) {
            xVelocity = Mathf.Min(0, xVelocity + BREAKSPEED);
        } else {
            xVelocity = Mathf.Max(0, xVelocity - BREAKSPEED);
        }
    }

    // Collider function
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 8) {
            Damage();
        }
    }

    #region IDamageAble
    public void Damage() {
        Explosion.SpawnExplosion(transform.position);
        if (Game.i.RemoveLive()) {
            Game.i.UIControl.UpdateGameReportText("Ship Lost <size=32> " + Game.i.spareLive + " To Go.</size>");
            StartCoroutine(ReSpawn());
        }
    }
    IEnumerator ReSpawn() {
        // Hold enemy attack
        Game.i.EnemyControl.isAttackable = false;

        transform.position = spawnPoint;

        // Turn off collider to make invincible
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(1.5f);


        // Spawn movement
        for (int i = 0; i < 120; i++) {
            yield return new WaitForFixedUpdate();
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, spawnPoint.y + 2, (1f / 120) * i), -0.01f);
        }

        // Turn enemy attack back
        Game.i.EnemyControl.isAttackable = true;
        Game.i.UIControl.UpdateGameReportText();

        // Wait another 1 sec then turn on collider
        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider2D>().enabled = true;
    }
    #endregion

    #region IGameState Interface
    public void GameOver() {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void GameComplete() {
        // Turn off collider to make invincible
        GetComponent<BoxCollider2D>().enabled = false;
        StopAllCoroutines();
        StartCoroutine(MoveMissionComplete());
    }

    IEnumerator MoveMissionComplete() {
        var step = 60;
        for (int i = 0; i < step; i++) {
            yield return new WaitForFixedUpdate();
            transform.Translate(Vector3.up * 0.5f);
        }

        StartCoroutine(ReSpawn());
    }
    #endregion
}
