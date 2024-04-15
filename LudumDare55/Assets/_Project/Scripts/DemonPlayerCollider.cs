using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonPlayerCollider : MonoBehaviour{
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")){
            Player.INSTANCE.dead = true;
        }
    }
}
