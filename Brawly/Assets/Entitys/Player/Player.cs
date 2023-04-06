using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Entity
{
    public float jumpCooldown;
    public bool readyToJump;

    public float moveSpeed = 2.5f;
    public float sprintSpeed = 3.5f;

    public float slideSpeed = 7f;

    public float dashSpeed = 10f;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed; 
    
    public float crouchSpeed = 1.5f;

    Jumping jump;
    Crouching cr;
    Sliding sd;

    public enum movementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        dash,
        air
    }

    public bool dashing = false;

    public movementState state = movementState.walking;

    public override void Start()
    {
        base.Start();
        
        jump = new Jumping(transform, rb);
        cr = new Crouching(transform, rb);
        sd = new Sliding(orientation, playerObj, rb);

        health = 100;

        speed = moveSpeed;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        groundDrag = 5f;

        Height = 1;

        cr.startYScale = transform.localScale.y;
        sd.startYScale = transform.localScale.y;
    }


    public override void Update()
    {
        checkGrounded();

        Move();

        checkCommands();

        base.Update();
        
        Debug.Log(desiredMoveSpeed);

    }

    public void Move()
    {
        updateAxis(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        entityMovement();

    }

    public void checkCommands()
    {
        StateHandler();
        checkJump();
        checkCrouch();
        checkSlide();
    }

  

    public void checkCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            cr.crouch();
        }
    }
    

    public void checkJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
                ChainVars.exitSlope = true;
            
                readyToJump = false;

                jump.Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
        }

    }
  
    public void ResetJump()
    {
        readyToJump = true;

        ChainVars.exitSlope = false;
    }

    public void checkSlide()
    {
        if (Input.GetKeyDown(KeyCode.F) && (horizontal != 0 || vertical != 0) && rb.velocity.magnitude > 3f)
        {
            sd.startSliding();
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            cr.normalize();
        }

        if (Input.GetKeyUp(KeyCode.F) && sd.sliding)
        {
            sd.stopSlide();
        }

        if (sd.sliding)
        {
            sd.slidingMovement();
        }
    }
    
    public IEnumerator smoothlyLerpSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed = moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        speed = desiredMoveSpeed;
    }

    private void StateHandler()
    {
        if (dashing)
        {
            desiredMoveSpeed = dashSpeed;
        } else if (sd.sliding)
        {
            state = movementState.sliding;

            if (onSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        } else if (Input.GetKey(KeyCode.LeftControl))
        {
            state = movementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        } else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = movementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = movementState.walking;
            desiredMoveSpeed = moveSpeed;
        }
        else
        {
            state = movementState.air;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && speed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(smoothlyLerpSpeed());
        }
        else
        {
            speed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        

    }

    public bool addDrag()
    {
        return state == movementState.walking || state == movementState.crouching || state == movementState.sprinting || state == movementState.sliding;
    }
    
 
}
