using System;
using UnityEngine;

/// <summary>
/// temporally disabled
/// 
/// </summary>
public class Dashing : MonoBehaviour
{
    
    public Transform playerObj;
    public Rigidbody rb;
    private Player pl;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        pl = GetComponent<Player>();
    }

    public float dashForce = 50f;

    public float dashDuration = 0.25f;
    
    public float dashCd = 1f;
    public float dashCdTimer = 0f;

    public void dashUpdate()
    {
        if(Input.GetKeyDown(KeyCode.C))
            Dash();

        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    public void Dash()
    {
        if (dashCdTimer > 0)
        {
            return;
        }
        else
        {
            dashCdTimer = dashCd;
        }
        
        pl.dashing = true;
        
        Vector3 dashDirection = playerObj.forward * dashForce;

        delayedForce = dashDirection;

        Invoke(nameof(addDelayedForce),0.025f);
        Invoke(nameof(ResetDash),dashDuration);
    }

    private Vector3 delayedForce;

    private void addDelayedForce()
    {
        rb.AddForce(delayedForce,ForceMode.Impulse);
    }

    public void ResetDash()
    {
        pl.dashing = false;
    }
}
