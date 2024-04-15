using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyGoomba : MonoBehaviour{
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float patrolDistance = 5f;
    [SerializeField] float waitTime = 0.2f;
    [SerializeField] bool startToLeft;

    [DisableInEditorMode][SerializeField] Vector2 spawnPos;
    [DisableInEditorMode][SerializeField] Vector2 leftPatrolPoint;
    [DisableInEditorMode][SerializeField] Vector2 rightPatrolPoint;
    [DisableInEditorMode][SerializeField] bool goingLeft;
    [DisableInEditorMode][SerializeField] bool waiting;

    void Start(){
        spawnPos = transform.position;
        goingLeft = startToLeft;
        leftPatrolPoint = spawnPos + Vector2.left * patrolDistance;
        rightPatrolPoint = spawnPos + Vector2.right * patrolDistance;
    }

    void Update(){
        if (!waiting){
            MoveTowardsPatrolPoint();
        }
    }

    void MoveTowardsPatrolPoint(){
        Vector2 targetPos = goingLeft ? leftPatrolPoint : rightPatrolPoint;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.01f){
            waiting = true;
            Invoke("ChangeDirection", waitTime);
        }
    }

    void ChangeDirection(){
        goingLeft = !goingLeft;
        waiting = false;
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            if(other.transform.position.y > this.transform.position.y){
                Player.INSTANCE.GetComponent<Movement>().JumpUp(50);
                Destroy(gameObject);
            }
        }
    }
}
