using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float     followSpeed = 0.1f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        if (followTarget != null)
        {
            Character character = followTarget.GetComponent<Character>();
            if (character != null)
            {
                if (character.isDead)
                {
                    return;
                }
            }

            Vector3 targetPos = followTarget.position;
            Vector3 error = targetPos - currentPos;

            Vector3 newPos = currentPos + error * followSpeed;

            currentPos = new Vector3(newPos.x, newPos.y + 3.0f, currentPos.z);
        }

        transform.position = currentPos;
    }
}
