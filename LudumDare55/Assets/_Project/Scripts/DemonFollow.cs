using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DemonFollow : MonoBehaviour{
    [SerializeField] float moveSpeed=5;
    [SerializeField] float distanceToAddNewPoint=2;
    [SerializeField] float distanceToRemoveOldPoint=10;
    [SerializeField] float distanceToFollowPlayer=5;
    [SerializeField] float maxPointListCount=5;
    [DisableInEditorMode][SerializeField] List<Vector2> pointsList;
    [DisableInEditorMode][SerializeField] float distanceToPlayer;
    Transform playerTransform;
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
    }
    void AddNewPoint(){
        if(pointsList.Count>maxPointListCount){
            pointsList.RemoveAt(0);
        }
        pointsList.Add(playerTransform.position);
    }
    void MoveToNextPoint(){
        // float _step = Time.deltaTime * moveSpeed;
        // if(pointsList.Count > 0){
        //     transform.position = Vector2.MoveTowards(transform.position, pointsList[0], _step);
        //     float distanceToPoint = Vector2.Distance(transform.position, pointsList[0]);
        //     if(distanceToPoint < 0.1f){
        //         pointsList.RemoveAt(0);
        //     }
        // }
        if (pointsList.Count > 0)
        {
            Vector2 nextPoint = pointsList[0];
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, step);
            
            if (Vector2.Distance(transform.position, nextPoint) < 0.1f)
            {
                pointsList.RemoveAt(0);
            }
        }
    }
}
