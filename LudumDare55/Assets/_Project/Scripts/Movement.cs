﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 12;
    public float jumpForce = 22;
    public float slideSpeed = 1;
    public float wallJumpLerp = 5;
    public float dashSpeed = 40;
    public float coyoteTimeDuration = 0.2f;
    public float fastfallDelay = 0.4f;  // Time after jumping where its possible to fastfall

    [Space]
    [Header("Booleans")]
    public bool canWallGrab = false;
    public bool canWallSlide = false;
    public bool canWallJump = false;
    public bool canDash = false;
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    private float coyoteTimeCounter;
    private float fastfallDelayTimer;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);

        if(canWallGrab){
            if (coll.onWall && Input.GetButton("Fire3") && canMove)
            {
                if(side != coll.wallSide)
                    anim.Flip(side*-1);
                wallGrab = true;
                wallSlide = false;
            }

            if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
            {
                wallGrab = false;
                wallSlide = false;
            }
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        
        if (canWallGrab && canWallSlide && wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !canWallGrab){rb.velocity = new Vector2(0,rb.velocity.y);}

        if(canWallSlide){
            if(coll.onWall && !coll.onGround)
            {
                if (x != 0 && !wallGrab)
                {
                    wallSlide = true;
                    WallSlide();
                }
            }

            if (!coll.onWall || coll.onGround)
                wallSlide = false;
        }else{wallSlide = false;}
        

        if (!coll.onGround && coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        else if (coll.onGround)
        {
            coyoteTimeCounter = coyoteTimeDuration;
        }


        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");

            if (coll.onGround || coyoteTimeCounter > 0)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround && canWallJump)
                WallJump();
        }

        if(canDash){
            if (Input.GetButtonDown("Fire1") && !hasDashed)
            {
                if(xRaw != 0 || yRaw != 0)
                    Dash(xRaw, yRaw);
            }
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        if ((canWallGrab && wallGrab) || (canWallSlide && wallSlide) || !canMove)
            return;

        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }

        if(fastfallDelayTimer>0)fastfallDelayTimer-=Time.deltaTime;
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);

        hasDashed = true;

        anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if(canWallGrab && canWallSlide){
            if(coll.wallSide != side)
                anim.Flip(side * -1);

            if (!canMove)
                return;

            bool pushingWall = false;
            if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
            {
                pushingWall = true;
            }
            float push = pushingWall ? 0 : rb.velocity.x;

            rb.velocity = new Vector2(push, -slideSpeed);
        }
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (canWallGrab && wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    public void Jump(Vector2 dir, bool wall, float _jumpForce=0){
        if(_jumpForce==0){_jumpForce=jumpForce;}
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * _jumpForce;

        fastfallDelayTimer = fastfallDelay;

        particle.Play();
    }
    public void JumpUp(float _jumpForce=0){
        Jump(Vector2.up, false, _jumpForce);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    public bool CanFastfall(){
        return !coll.onGround && fastfallDelayTimer<=0;
    }
}
