using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DemonFollow : MonoBehaviour{
    [SerializeField] float moveSpeed=5;
    [SerializeField] float dashSpeed=25;
    [SerializeField] float whenFarAwayMoveSpeed=25;
    [SerializeField] float whenFarAwayDistance=20;
    // [SerializeField] float dashDistMult=2.2f;
    [SerializeField] float distanceToAddNewPoint=2;
    [SerializeField] float distanceToRemoveOldPoint=10;
    [SerializeField] float distanceToFollowPlayer=5;
    [SerializeField] float maxPointListCount=5;
    [DisableInEditorMode][SerializeField] List<Vector2> pointsList;
    [DisableInEditorMode][SerializeField] float distanceToPlayer;
    [DisableInEditorMode][SerializeField] public bool dashCalled;
    [SerializeField] public float dashTime=1;
    [DisableInEditorMode][SerializeField] public float dashTimer;
    [SerializeField] public float dashCooldown=5;
    [DisableInEditorMode][SerializeField] public float dashCooldownTimer;
    [DisableInEditorMode][SerializeField] float moveSpeedCurrent;
    Transform playerTransform;
    Vector2 dashDirection;
    void Start(){
        playerTransform = Player.INSTANCE.transform;
    }
    void Update(){
        if(!Player.INSTANCE.dead){
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            //if(dashTimer > 0){
                //moveSpeedCurrent = dashSpeed;
            //}else{
                if(distanceToPlayer > whenFarAwayDistance){
                    moveSpeedCurrent = whenFarAwayMoveSpeed;
                }else{
                    moveSpeedCurrent =  moveSpeed;
                }
            //}
            

            float distanceToNextPoint = 0;
            if(pointsList.Count > 0) distanceToNextPoint = Vector2.Distance(playerTransform.position, pointsList[pointsList.Count - 1]);

            if(pointsList.Count == 0 || distanceToPlayer >= distanceToNextPoint && distanceToNextPoint >= distanceToAddNewPoint){
                AddNewPoint();
            }
            if(distanceToPlayer < distanceToFollowPlayer){
                float _step = Time.deltaTime * moveSpeedCurrent;
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, _step);
            }else{
                MoveToNextPoint();
            }


            //
            for(var i=0;i<pointsList.Count;i++){
                float distanceToPlayerFromPoint = Vector2.Distance(pointsList[i], playerTransform.position);
                if(distanceToPlayerFromPoint > distanceToRemoveOldPoint)pointsList.RemoveAt(i);
            }

            if(Input.GetMouseButtonDown(0)){
                CallDash();
            }
            if(dashTimer>0){
                dashTimer -= Time.deltaTime;
                Dash();
            }
            if(dashCooldownTimer>0){dashCooldownTimer -= Time.deltaTime;}
        }
    }
    void AddNewPoint(){
        if(pointsList.Count>maxPointListCount){
            pointsList.RemoveAt(0);
        }
        pointsList.Add(playerTransform.position);
    }
    void MoveToNextPoint(){
        if (pointsList.Count > 0){
            Vector2 nextPoint = pointsList[0];
            float step = Time.deltaTime * moveSpeedCurrent;
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, step);
            
            if (Vector2.Distance(transform.position, nextPoint) < 0.1f){
                pointsList.RemoveAt(0);
            }
        }
    }
    public void CallDash(){
        if(dashTimer<=0 && dashCooldownTimer<=0){
            dashTimer = dashTime;
            dashCooldownTimer = dashCooldown;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashDirection = (mousePos - (Vector2)transform.position).normalized;
        }
    }
    void Dash(){
        float _step = Time.deltaTime * dashSpeed;
        Vector2 dashDest;

        dashDest = (Vector2)transform.position + dashDirection * 2f;// * dashDistMult;

        transform.position = Vector2.MoveTowards(transform.position, dashDest, _step);
        if (Vector2.Distance(transform.position, dashDest) < 0.1f){
            dashTimer = 0;
        }
    }
}
