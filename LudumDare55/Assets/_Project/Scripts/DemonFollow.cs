using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DemonFollow : MonoBehaviour{
    [SerializeField] float moveSpeed=5;
    [SerializeField] float dashSpeed=5;
    [SerializeField] float dashDistMult=2.2f;
    [SerializeField] float distanceToAddNewPoint=2;
    [SerializeField] float distanceToRemoveOldPoint=10;
    [SerializeField] float distanceToFollowPlayer=5;
    [SerializeField] float maxPointListCount=5;
    [DisableInEditorMode][SerializeField] List<Vector2> pointsList;
    [DisableInEditorMode][SerializeField] float distanceToPlayer;
    [DisableInEditorMode][SerializeField] public bool dashCalled;
    [SerializeField] public float dashTime;
    [DisableInEditorMode][SerializeField] public float dashTimer;
    Transform playerTransform;
    Vector2 dashDirection;
    void Start(){
        playerTransform = Player.INSTANCE.transform;
    }
    void Update(){
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanceToNextPoint = 0;
        if(pointsList.Count > 0) distanceToNextPoint = Vector2.Distance(playerTransform.position, pointsList[pointsList.Count - 1]);

        if(pointsList.Count == 0 || distanceToPlayer >= distanceToNextPoint && distanceToNextPoint >= distanceToAddNewPoint){
            AddNewPoint();
        }
        if(distanceToPlayer < distanceToFollowPlayer){
            float _step = Time.deltaTime * moveSpeed;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, _step);
        }else{
            MoveToNextPoint();
        }


        //
        for(var i=0;i<pointsList.Count;i++){
            float distanceToPlayerFromPoint = Vector2.Distance(pointsList[i], playerTransform.position);
            if(distanceToPlayerFromPoint > distanceToRemoveOldPoint)pointsList.RemoveAt(i);
        }


        if(Input.GetKeyDown(KeyCode.Z)){
            CallDash();
        }
        if(dashTimer>0){
            dashTimer -= Time.deltaTime;
            Dash();
        }else{}
            
    }
    void AddNewPoint(){
        if(pointsList.Count>maxPointListCount){
            pointsList.RemoveAt(0);
        }
        pointsList.Add(playerTransform.position);
    }
    void MoveToNextPoint(){
        if (pointsList.Count > 0)
        {
            Vector2 nextPoint = pointsList[0];
            float step = Time.deltaTime * moveSpeed;
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, step);
            
            if (Vector2.Distance(transform.position, nextPoint) < 0.1f)
            {
                pointsList.RemoveAt(0);
            }
        }
    }
    public void CallDash(){
        if(dashTimer<=0){
            dashTimer = dashTime;
            // dashDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            // Vector2 mousePos = AssetsManager.INSTANCE.ClampToWindow(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashDirection = (mousePos - (Vector2)transform.position).normalized;
        }
    }
    void Dash(){
        float step = Time.deltaTime * dashSpeed;
        Vector2 dashDest;

        // if (pointsList.Count > 0){
        //     dashDest = pointsList[0];
        // }else{
            // dashDest = playerTransform.position;
        // }
        // dashDest = dashDest * dashDistMult;
        // Vector2 dashDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        dashDest = (Vector2)transform.position + dashDirection * dashDistMult;

        transform.position = Vector2.MoveTowards(transform.position, dashDest, step);
        if (Vector2.Distance(transform.position, dashDest) < 0.1f){
            dashTimer = 0;
        }
    }
}
