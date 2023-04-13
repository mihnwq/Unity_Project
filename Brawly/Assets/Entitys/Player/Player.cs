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

    public float dashSpeedChangeFacotr = 50f;

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
        
        
      
    }

    public void Move()
    {
        updateAxis(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(!dashing)
        entityMovement(true);

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
        if (Input.GetKeyDown(KeyCode.F) && (horizontal != 0 || vertical != 0) && rb.velocity.magnitude > 3f && state != movementState.air)
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
    
    public IEnumerator smoothlyLerpSlopeSpeed()
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

    private float speedChangeFacotr;
    public IEnumerator smoothlyLerpDashSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed = moveSpeed);
        float startValue = moveSpeed;

        float boastFactor = speedChangeFacotr;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boastFactor;
            yield return null;
        }

        speed = desiredMoveSpeed;
        speedChangeFacotr = 1f;
        keepMomentum = false;
    }

    private bool keepMomentum = false;
    private movementState lastState;
    
    private void StateHandler()
    {
        if (dashing)
        {
            state = movementState.dash;
            desiredMoveSpeed = dashSpeed;
            speedChangeFacotr = dashSpeedChangeFacotr;
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

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = moveSpeed;
            else desiredMoveSpeed = sprintSpeed;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && speed != 0 && onSlope())
        {
            StopAllCoroutines();
            StartCoroutine(smoothlyLerpSlopeSpeed());
        }
        else
        {
            bool hasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
            if (lastState == movementState.dash) keepMomentum = true;


            if (hasChanged)
            {
                if (keepMomentum)
                {
                    StopAllCoroutines();
                    StartCoroutine(smoothlyLerpDashSpeed());
                }
                else
                {
                    StopAllCoroutines();
                    speed = desiredMoveSpeed;
                }
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

    }

    public bool addDrag()
    {
        return state == movementState.walking || state == movementState.crouching || state == movementState.sprinting || state == movementState.sliding;
    }
    
 
}
