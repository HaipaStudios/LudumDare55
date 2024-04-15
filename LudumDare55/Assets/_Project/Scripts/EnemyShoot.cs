using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyShoot : MonoBehaviour{
    [AssetsOnly][SerializeField] GameObject btPrefab;
    [SerializeField] float btPersistTime=5f;
    [SerializeField] float btSpeed=4f;
    [SerializeField] float shootTime=2f;
    [SerializeField] float shootTimeStartOffset=0f;
    [DisableInEditorMode][SerializeField] float shootTimer;
    void Start(){
        shootTimer = shootTime + shootTimeStartOffset;
    }

    void Update(){
        if(shootTimer>0){shootTimer-=Time.deltaTime;}
        else{
            GameObject go = Instantiate(btPrefab, transform.position, Quaternion.identity);
            Bullet bt = go.GetComponent<Bullet>();

            bt.SetPersistTime(btPersistTime);

            Vector2 facingDirection = Vector2.left;
            if(transform.localScale.x < 0){
                facingDirection = Vector2.right;
            }
            bt.SetVelocity(facingDirection*btSpeed);
            shootTimer = shootTime;
        }
    }
}
