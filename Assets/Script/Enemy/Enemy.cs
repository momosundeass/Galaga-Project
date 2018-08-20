using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageAble {
    /// <summary>
    /// Score that player get when kill
    /// </summary>
    public abstract int getScore { get; }
    /// <summary>
    /// Level of enemy
    /// </summary>
    public abstract byte Level { get; }

    [HideInInspector]
    protected Vector2 relativePosition;
    protected Vector3 getStanbyLocation { get { return Game.i.EnemyControl.transform.position.MergeVector2(relativePosition); } }

    // Status : Attacking going on?
    protected bool isAttacking = true;
    public bool getAttackStatus { get { return isAttacking; } }

    /// <summary>
    /// This function call when build the object
    /// </summary>
    public void Spawn(Vector2 relativePosition, Vector3 mergePosition, MoveType moveTypeToSpawn) {
        this.relativePosition = relativePosition;
        transform.LookAt2D(mergePosition);
        RandomMove(mergePosition, GotoStandbyLocationFromCurrentPosition, moveTypeToSpawn);
        // Add to enemy holder
        Game.i.EnemyControl.EnemyHolder[Level].Add(this);
    }

    #region Attack Function
    /// <summary>
    /// Call attack by Enemy Controller.
    /// </summary>
    public void CallAttack() {
        if (!isAttacking) {
            isAttacking = true;
            Attack();
        }
    }

    /// <summary>
    /// Override to do custom Attack.
    /// </summary>
    protected abstract void Attack();
    #endregion

    #region Movement Function
    protected void GotoStandbyLocation() {
        transform.position = Game.i.EnemyControl.getRandomSpawnLocation;
        StartCoroutine(moveToStandbyLocation(() => isAttacking = false));
    }
    protected void GotoStandbyLocationFromCurrentPosition() {
        StartCoroutine(moveToStandbyLocation(() => isAttacking = false));
    }
    /// <summary>
    /// Random select move from parameter typesToMove
    /// </summary>
    protected void RandomMove(Vector3 moveTo, System.Action callOnMoveComplete, params MoveType[] typesToMove) {
        switch (typesToMove.RandomPick()) {
            case MoveType.Curve:
                StartCoroutine(CurveMove(moveTo, callOnMoveComplete));
                break;
            case MoveType.Sin:
                StartCoroutine(SinMove(moveTo, callOnMoveComplete));
                break;
            default:
                StartCoroutine(LineMove(moveTo, callOnMoveComplete));
                break;
        }
    }

    #region Movement IEnumerator
    protected IEnumerator LineMove(Vector3 moveTo, System.Action callOnMoveComplete) {
        var startPos = transform.position;
        var distance = Vector3.Distance(moveTo, startPos);
        var time = distance / (Game.parameter.EnemySpeed / 100f);

        for (int i = 0; i < time; i++) {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.Lerp(startPos, moveTo, (1f / time) * i);
        }
        callOnMoveComplete();
    }

    protected IEnumerator CurveMove(Vector3 moveTo, System.Action callOnMoveComplete) {
        var startPos = transform.position;
        var distance = Vector3.Distance(moveTo, startPos);
        var time = distance / (Game.parameter.EnemySpeed / 100f);
        var isInvert = Random.Range(0, 2) == 0?1: -1; 
        setInCurveAnchon(moveTo);
        for (int i = 0; i < time; i++) {
            yield return new WaitForFixedUpdate();
            var x = Mathf.Lerp(0, distance, (1f / time) * i);
            var targetToMove = transform.parent.getLocalPositionOfTransform(new Vector3(x, ExtendedTool.SinFomular(distance, x, 2.5f * isInvert)));
            transform.LookAt2D(targetToMove);
            transform.position = targetToMove;
        }
        RemoveOutCurveAnchon();
        callOnMoveComplete();
    }

    protected IEnumerator SinMove(Vector3 moveTo, System.Action callOnMoveComplete) {
        var startPos = transform.position;
        var distance = Vector3.Distance(moveTo, startPos);
        var time = distance / (Game.parameter.EnemySpeed / 100f);
        var isInvert = Random.Range(0, 2) == 0 ? 1 : -1;
        setInCurveAnchon(moveTo);
        for (int i = 0; i < time; i++) {
            yield return new WaitForFixedUpdate();
            var x = Mathf.Lerp(0, distance, (1f / time) * i);
            var targetToMove = transform.parent.getLocalPositionOfTransform(new Vector3(x, ExtendedTool.SinFomular(distance / 2f, x, 1 * isInvert)));
            transform.LookAt2D(targetToMove);
            transform.position = targetToMove;
        }
        RemoveOutCurveAnchon();
        callOnMoveComplete();
    }

    protected IEnumerator moveToStandbyLocation(System.Action callOnMoveComplete) {
        var startPos = transform.position;
        var distance = Vector3.Distance(getStanbyLocation, startPos);
        var time = distance / (Game.parameter.EnemySpeed / 100f);
        // Move to standy Location
        for (int i = 0; i < time; i++) {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.Lerp(startPos, getStanbyLocation, (1f / time) * i);
            transform.LookAt2D(getStanbyLocation);
        }
        // Make sure object get back to standby location
        transform.position = getStanbyLocation;
        
        // Rotate to standy angle
        var currentRotate = transform.rotation;
        time = 15;
        for (int i = 0; i < time; i++) {
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(currentRotate, Quaternion.Euler(0, 0, 90), (1f / time) * i);
        }
        // Make sure object get back to standby angle
        transform.rotation = Quaternion.Euler(0, 0, 90);
        callOnMoveComplete();
    }
    #endregion

    protected void setInCurveAnchon(Vector3 moveTo) {
        var direction = transform.position.ToVector2() - moveTo.ToVector2();
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        var anchon = Instantiate(Game.i.CurveAnchon, transform.position, Quaternion.AngleAxis(angle, Vector3.forward), Game.i.EnemyControl.transform) as GameObject;
        transform.parent = anchon.transform;
    }
    protected void RemoveOutCurveAnchon() {
        if (transform.parent.tag == "MovementAnchon") {
            Destroy(transform.parent.gameObject);
        }
        transform.parent = Game.i.EnemyControl.transform;
        transform.GetComponent<BoxCollider2D>().enabled = true;

    }
    #endregion

    #region interface IDamageAble

    public void Damage() {
        // Remove from enemy holder
        Game.i.EnemyControl.EnemyHolder[Level].Remove(this);
        Game.i.AddScore(getScore);
        if(transform.parent.tag == "MovementAnchon") {
            Destroy(transform.parent.gameObject);
        }else {
            Destroy(gameObject);
        }
        Explosion.SpawnExplosion(transform.position);

    }

    #endregion

    /// <summary>
    /// Get position of the player via random between -xSwing and xSwing
    /// </summary>
    /// <param name="xSwing"></param>
    /// <param name="yOffset"> Offset position of vertical plane </param>
    /// <returns></returns>
    protected Vector3 getRandomLocationOfPlayer(float xSwing = 1.5f, float yOffset = 0) {
        var playerPos = Game.i.CurrentPlayer.transform.position;
        return new Vector3(playerPos.x + Random.Range(-xSwing, xSwing), playerPos.y + yOffset, transform.position.z);
    }

}
