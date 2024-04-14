using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumping : MonoBehaviour
{
    private Rigidbody2D rb;
    public float fallMultiplier = 3f;
    public float fastFallMultiplier = 14f;
    public float lowJumpMultiplier = 8f;
    Movement mv;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        mv = GetComponent<Movement>();
    }

    void Update(){
        if(Input.GetAxis("Vertical")<0 && mv.CanFastfall()){  // Fastfall
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fastFallMultiplier - 1) * Time.deltaTime;
        }
        else{
            if(rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }
}
