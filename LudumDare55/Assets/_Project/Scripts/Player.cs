using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour{
    public static Player INSTANCE;
    [DisableInEditorMode]public bool dead;
}