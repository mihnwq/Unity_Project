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

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        
        //shit needs tuning
        Vector3 positionMultiplier = new Vector3(1f, 0.5f, 0.5f);

        //the creation of the object and it's rotation and position
        //can be made to be spawned in the player and have the direction of the mouse
        //all to be added for later parts of the game
      //  GameObject projectile = Instantiate(objectToThrow, attackPoint.position + positionMultiplier, throwDirection.rotation);

      GameObject projectile = Instantiate(objectToThrow, attackPoint.position + positionMultiplier, attackPoint.rotation);
      
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = throwDirection.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(throwDirection.position, throwDirection.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 foreceToAdd = forceDirection * throwForce + transform.up * throwForce;
        
        projectileRb.AddForce(foreceToAdd,ForceMode.Impulse);

        totalThrows--;
        
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    public void ResetThrow()
    {
        readyToThrow = true;
    }
}
