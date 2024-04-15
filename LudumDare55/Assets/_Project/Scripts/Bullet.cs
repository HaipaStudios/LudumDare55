using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Bullet : MonoBehaviour{
    [DisableInEditorMode][SerializeField] float persistTime;
    [DisableInEditorMode][SerializeField] float persistTimer;
    [DisableInEditorMode][SerializeField] bool isSetup;
    Rigidbody2D rb;
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    void Update(){
        if(persistTimer>0){
            persistTimer -= Time.deltaTime;
        }
        else{
            if(isSetup){
                Destroy(gameObject);
            }
        }
    }
    public void SetPersistTime(float _persistTime){
        persistTime = _persistTime;
        persistTimer = persistTime;
        isSetup = true;
    }
    public void SetVelocity(Vector2 _velocity){
        rb.velocity = _velocity;
    }
}
