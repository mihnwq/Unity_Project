using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrbit : MonoBehaviour
{
    
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed = 7f;

    Transform current_orientation;

    public bool isLocked = false;

    public bool start = true;
     
    public void checkOrientation()
    {

        if (start)
        {
            current_orientation = orientation;
            start = false;
        }
         
        if (Input.GetKeyDown(KeyCode.Tab) && !isLocked)
        {
            current_orientation = playerObj;

            isLocked = true;
        }else if (Input.GetKeyDown(KeyCode.Tab) && isLocked)
        {
            current_orientation = orientation;
            isLocked = false;
        }
    }
     
    void Update()
    {
        checkOrientation();
        
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        
        current_orientation.forward = viewDir.normalized;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = current_orientation.forward * vertical + current_orientation.right * horizontal;
        
            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            


    }
}
