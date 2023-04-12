using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileThrowing : MonoBehaviour
{

    public Transform throwDirection;
    public Transform attackPoint;
    public GameObject objectToThrow;

    public int totalThrows;
    public int throwCooldown;

    public float throwForce;
    public float throwUpwardForce;

    public bool readyToThrow;
    
    Quaternion angle;
    Quaternion transformPos;
    Quaternion finalAngle;

    public float mouseSensitivity = 3.0f;

    public void Start()
    {
        readyToThrow = true;
        
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.F) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    public void Throw()
    {
        readyToThrow = false;

       
      //  GameObject projectile = Instantiate(objectToThrow, attackPoint.position + positionMultiplier, throwDirection.rotation);


      Vector3 position_multiplier = new Vector3(0.5f,0,0.5f);
      
      GameObject projectile = Instantiate(objectToThrow, attackPoint.position + position_multiplier, attackPoint.rotation);
      
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

      /*  Vector3 forceDirection = throwDirection.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(throwDirection.position, throwDirection.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 foreceToAdd = forceDirection * throwForce + transform.forward * throwForce;
    */

      Vector3 foreceToAdd = attackPoint.forward * throwForce;
        
        projectileRb.AddForce(foreceToAdd,ForceMode.Impulse);

        totalThrows--;
        
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    public void ResetThrow()
    {
        readyToThrow = true;
    }
}
