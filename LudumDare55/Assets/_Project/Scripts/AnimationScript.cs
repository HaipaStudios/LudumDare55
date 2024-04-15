using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] bool reverseFlip = true;
    private Animator anim;
    private Movement move;
    private Collision coll;
    [HideInInspector]
    public SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        move = GetComponentInParent<Movement>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(anim!=null){
            anim.SetBool("onGround", coll.onGround);
            anim.SetBool("onWall", coll.onWall);
            anim.SetBool("onRightWall", coll.onRightWall);
            anim.SetBool("wallGrab", move.wallGrab);
            anim.SetBool("wallSlide", move.wallSlide);
            anim.SetBool("canMove", move.canMove);
            anim.SetBool("isDashing", move.isDashing);
        }

    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        if(anim!=null){
            anim.SetFloat("HorizontalAxis", x);
            anim.SetFloat("VerticalAxis", y);
            anim.SetFloat("VerticalVelocity", yVel);
        }
    }

    public void SetTrigger(string trigger)
    {
        if(anim!=null){
            anim.SetTrigger(trigger);
        }
    }

    public void Flip(int side)
    {
        if(reverseFlip)side *= -1;
        
        if (move.wallGrab || move.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }
}
