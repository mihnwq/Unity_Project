using UnityEngine;

public class Crouching
{
    HitboxChange hc;

     Transform tf;
     CapsuleCollider hitbox;

     public Crouching(Transform tf,CapsuleCollider hitbox)
     {
         this.tf = tf;
        this.hitbox = hitbox;

        hc = new HitboxChange(hitbox, tf);
     }

    public void crouch()
    {
        /* tf.localScale = new Vector3(tf.localScale.x, crouchYScale, tf.localScale.z);
         rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);*/

        hc.changeColliderCrouch(2);
    }

    public void normalize()
    {
        //  tf.localScale = new Vector3(tf.localScale.x, startYScale, tf.localScale.z);
        hc.normalize();
    }
        
}
