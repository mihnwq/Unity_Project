using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HitboxChange
{

    CapsuleCollider hitbox;


    Transform position;


    float originalSize;
    Vector3 originalPoint;
     Vector3 originalPosition;

    public HitboxChange(CapsuleCollider hitbox,  Transform position)
    {
        this.hitbox = hitbox;


        this.position = position;

        originalPoint = hitbox.center;

        originalPosition = position.position;

        originalSize = hitbox.height;
    }


    public void changeColliderCrouch(float division)
    {

        float newSize = originalSize / division;

        float yOffset = newSize / 2;

        Vector3 newPoint = originalPoint;

        originalPoint.y -= yOffset;


        float offset = 0.01f;
        Vector3 newPosition = originalPosition;

        newPosition.y += offset;

        hitbox.height = newSize;

        hitbox.center = newPoint;

        position.position = newPosition;

    }

    public void normalize()
    {
        hitbox.height = originalSize;

        hitbox.center = originalPoint;

        position.position = originalPosition;
    }

 }

