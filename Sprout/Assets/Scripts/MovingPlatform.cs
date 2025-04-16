using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour, IActivatable
{
    public bool stopAfterReachEnd = true;
    public float stopAfterEndDelayAmount = 1.5f;
    public bool reverseResponse = false;

    public float speed = 1f;

    private bool canMove = true;

    private int startPointIndex = 0;
    private int endPointIndex = 1;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 previousPosition;
    private Vector3 deltaPosition;

    private float elapsedTime;
    private float timeToReach;

    public Transform targetParent;
    public Transform tolerancePoint;

    public List<GameObject> moveableGameObjects;
    private Transform[] targetLocations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Create new array of target locations
        targetLocations = new Transform[targetParent.childCount];

        //Assign target points to array
        for (int i = 0; i < targetParent.childCount; i++)
        {
            targetLocations[i] = targetParent.GetChild(i).GetComponent<Transform>();
        }

        //Handle initial target points at start
        startPoint = targetLocations[startPointIndex].position;
        endPoint = targetLocations[endPointIndex].position;
        timeToReach = Vector3.Distance(startPoint, endPoint) / speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            //$Moving Platform Move
            //MODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Moving Platform");
            //Increase elapsed time, elapsed time is a normalized value
            //Platform movement speed depends on speed variable
            elapsedTime += Time.deltaTime / timeToReach;

            //Interpolate between start and end point
            transform.position = Vector3.Lerp(startPoint, endPoint, elapsedTime);

            //Get delta between current and previous position
            deltaPosition = transform.position - previousPosition;

            //Handle platform reaching end destination
            if (elapsedTime > 1)
            {
                GetNewTargetPoints();

                //Update time to reach of new points
                timeToReach = Vector3.Distance(startPoint, endPoint) / speed;
                elapsedTime = 0;

                if (stopAfterReachEnd)
                {
                    canMove = false;
                    Invoke("ReactivateMovingPlatform", stopAfterEndDelayAmount);
                }
            }

            //Handle objects on top of moving platform
            foreach (GameObject moveable in moveableGameObjects)
            {
                moveable.transform.position += deltaPosition;
            }

            //Record current position as previous position for next update
            previousPosition = transform.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody collisionRb;

        //Add object with dynamic rigidbodies into movableGameObjects array
        if (collision.gameObject.TryGetComponent<Rigidbody>(out collisionRb))
        {
            if (collisionRb.isKinematic == false)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 contactPosition = contact.point;
                Vector3 otherColliderPosition = collision.transform.position;

                Vector3 contactToOtherVector = Vector3.Normalize(otherColliderPosition - contactPosition);

                float dot = Vector3.Dot(contactToOtherVector, Vector3.up);

                Debug.Log(dot);

                if(dot > 0.95f)
                {
                    if(moveableGameObjects.Contains(collision.gameObject) )
                    {
                        return;
                    }
                    else
                    {
                        moveableGameObjects.Add(collision.gameObject);
                    }
                }
                else
                {
                    moveableGameObjects.Remove(collision.gameObject);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Remove game object from array when exiting collision
        if (moveableGameObjects.Contains(collision.gameObject))
        {
            moveableGameObjects.Remove(collision.gameObject);
        }
    }

    //IActivatable interface implementation
    //Allow moving platform to move
    public void Activate()
    {
        //$Moving Platform Move

        canMove = true;

        if(reverseResponse)
        {
            canMove = false;
        }
    }

    //IActivatable interface implementation
    //Stop moving platfrom from moving
    public void Deactivate()
    {
        //$Moving Platform Stop Move

        canMove = false;

        if (reverseResponse)
        {
            canMove = true;
        }
    }

    //Get new starting and end points for interpolation
    private void GetNewTargetPoints()
    {
        //Increase index by 1
        startPointIndex++;
        endPointIndex++;

        //Loop back to 0 if index exceeds targetLocation array length
        if(startPointIndex > targetLocations.Length - 1) 
        {
            startPointIndex = 0;
        }

        if(endPointIndex > targetLocations.Length - 1)
        {
            endPointIndex = 0;
        }

        //Assign start and end point values

        startPoint = targetLocations[startPointIndex].position;
        endPoint = targetLocations[endPointIndex].position;
    }

    private void ReactivateMovingPlatform()
    {
        canMove = true;
    }
}
