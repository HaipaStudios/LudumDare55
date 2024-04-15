using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour{
    public static Player INSTANCE;
    [DisableInEditorMode]public bool dead;
    [DisableInEditorMode]List<MonoBehaviour> componentsList;
    void Awake(){INSTANCE=this;}
    void Start(){
        componentsList.Add(GetComponent<Movement>());
        componentsList.Add(GetComponent<Collision>());
        componentsList.Add(GetComponent<BetterJumping>());
    }
    void Update(){
        foreach(MonoBehaviour cmp in componentsList){
            cmp.enabled = !dead;
        }
    }
}