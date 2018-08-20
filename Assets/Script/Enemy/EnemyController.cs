using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour, IGameState {

    const int ENEMY_PER_ROW = 9;
    const float XSPACE = 1.2f;
    const float YSPACE = 0.9f;
    const float MOVE_AMOUNT = 0.4f;
    const int MOVE_TIME = 6;
    const float ATTACK_WAVE_COOLDOWN = 1f;
    const float SPAWN_WIDTH = 12f;


    // Spawn line for spawn enemy
    [SerializeField] private Transform SpawnLine;
    public Vector3 getSpawnLocation { get { return SpawnLine.position; } }
    public Vector3 getRandomSpawnLocation { get { return getSpawnLocation + new Vector3(UnityEngine.Random.Range(SPAWN_WIDTH / 2f, SPAWN_WIDTH / 2f - SPAWN_WIDTH), 0, -0.01f); } }
    
    // EnemyHolder for store data of enemy by level
    public Dictionary<int, List<Enemy>> EnemyHolder = new Dictionary<int, List<Enemy>>() {
        { 1, new List<Enemy>() },
        { 2, new List<Enemy>() },
        { 3, new List<Enemy>() }
    };

    // Status : for hold attack of enemy and player
    public bool isAttackable = true;

    void Start() {
        // Send Enemy group to center of scene
        transform.localPosition = new Vector3(0, 0.95f, -0.01f);
        // Start Spawn and Move the group
        StartCoroutine(PackLoopMove());
        StartCoroutine(SpawnEnemy());
        StartCoroutine(LoopAttackWave());

        print(getStandbyPosition(0, 4));
    }

    #region Attack Order
    IEnumerator LoopAttackWave() {
        yield return new WaitForSeconds(3);

        while (transform.childCount != 0) {
            yield return new WaitForSeconds(ATTACK_WAVE_COOLDOWN);
            if (!isAttackable) continue;
            bool isAttacked = false;
            var rnd = new System.Random();
            var chance = rnd.Next(0, 100);

            if(chance < 20) {
                // Prevent same level of enemy attack twice at once
                if (EnemyHolder[3].Count != 0 && !EnemyHolder[3].Exists(x => x.getAttackStatus)) {
                    EnemyHolder[3].RandomPick().CallAttack();
                    isAttacked = true;
                }

            } else if(chance < 50) {
                // Prevent same level of enemy attack twice at once
                if (EnemyHolder[2].Count != 0 && !EnemyHolder[2].Exists(x => x.getAttackStatus)) {
                    EnemyHolder[2].RandomPick().CallAttack();
                    isAttacked = true;
                }
            } else {
                if (EnemyHolder[1].Count != 0) {
                    EnemyHolder[1].RandomPick().CallAttack();
                    isAttacked = true;
                }
            }
            // Attack never happen beforce just random pick
            if (!isAttacked) {
                // Force level 2 attack
                if (EnemyHolder[2].Count != 0) {
                    EnemyHolder[2].RandomPick().CallAttack();
                    isAttacked = true;
                } else if (EnemyHolder[3].Count != 0) {
                    EnemyHolder[3].RandomPick().CallAttack();
                    isAttacked = true;
                }
            }
        }


        // If enemy = 0 send mission complete
        Game.i.MissionComplete();

    }

    #endregion

    #region Movement Function
    IEnumerator PackLoopMove() {
        var rnd = new System.Random();
        var wait = new int[] { 7, 12 };
        // Move left first
        for (int i = 0; i < MOVE_TIME / 2; i++) {
            StartCoroutine(MoveGameOver(new Vector2(-MOVE_AMOUNT, 0)));
            yield return new WaitForSeconds(rnd.Next(wait[0], wait[1]) * 0.1f);
        }
        var vertical = -1; // -1 = Down , 1 = Up
        var horizental = 1; // -1 = Left , 1 = Right
        while (true) {
            // Move Left

            for (int time = 0; time < 2; time++) {
                // Move Horizental
                for (int i = 0; i < MOVE_TIME; i++) {
                    StartCoroutine(MoveGameOver(new Vector2(MOVE_AMOUNT * horizental, 0)));
                    yield return new WaitForSeconds(rnd.Next(wait[0], wait[1]) * 0.1f);
                }
                // Move Vertical
                StartCoroutine(MoveGameOver(new Vector2(0, MOVE_AMOUNT / 3f * vertical)));
                yield return new WaitForSeconds(rnd.Next(wait[0], wait[1]) * 0.1f);

                horizental *= -1;
            }



            vertical *= -1;
        }
    }

    IEnumerator MoveGameOver(Vector2 direction) {
        var step = 15;
        for (int i = 0; i < step; i++) {
            yield return new WaitForFixedUpdate();
            transform.Translate(direction / step);
        }
    }
    #endregion

    #region Enemy Spawn Function
    IEnumerator SpawnEnemy() {
        var SpawnColumn = new int[] { 4, 5, 3, 6, 2, 7, 1, 8, 0};
        var SpawnDelay = 0.12f;

        foreach (var column in SpawnColumn) {
            // Do random start location
            var spawnPosition = getRandomSpawnLocation;
            var offsetToMerge = getStandbyPosition(3, column);
            var spawnMoveType = new MoveType[] { MoveType.Line, MoveType.Curve, MoveType.Sin }.RandomPick();
            var mergePosition = new Vector3(UnityEngine.Random.Range(-1, 1) + offsetToMerge.x, offsetToMerge.y, -0.01f);

            for (int row = 0; row < 2; row++) {
                BuildEnemyObject(mergePosition, spawnPosition, spawnMoveType, row, column, Game.i.EnemyBlue);
                yield return new WaitForSeconds(SpawnDelay);
            }
            for (int row = 2; row < 4; row++) {
                BuildEnemyObject(mergePosition, spawnPosition, spawnMoveType, row, column, Game.i.EnemyRed);
                yield return new WaitForSeconds(SpawnDelay);
            }
            for (int row = 4; row < 5; row++) {
                BuildEnemyObject(mergePosition, spawnPosition, spawnMoveType, row, column, Game.i.EnemyGreen);
                yield return new WaitForSeconds(SpawnDelay);
            }
            yield return new WaitForSeconds(SpawnDelay * 10f);
        }
    }

    private void BuildEnemyObject(Vector3 mergePosition, Vector3 spawnPosition, MoveType moveTypeToSpawn, int row, int column, GameObject EnemyType) {
        Instantiate(EnemyType, spawnPosition,
            Quaternion.Euler(0, 0, 90), transform).
            gameObject.GetComponent<Enemy>().Spawn(getStandbyPosition(row, column), mergePosition, moveTypeToSpawn);
    }

    Vector3 getStandbyPosition(int row, int column) {
        return new Vector3(-((ENEMY_PER_ROW - 1) * XSPACE / 2f) + (column * XSPACE), row * YSPACE);
    }
    #endregion

    #region IGameState interface
    public void GameOver() {
        StopAllCoroutines();
        StartCoroutine(MoveGameOver());
    }

    IEnumerator MoveGameOver() {
        var step = 60;
        for (int i = 0; i < step; i++) {
            yield return new WaitForFixedUpdate();
            transform.Translate(Vector3.down * 0.3f);
        }
    }

    public void GameComplete() {
        StopAllCoroutines();
        StartCoroutine(WaitTheResetEnemy());
    }
    IEnumerator WaitTheResetEnemy() {
        yield return new WaitForSeconds(1.2f);
        Start();
    }
    #endregion


}
