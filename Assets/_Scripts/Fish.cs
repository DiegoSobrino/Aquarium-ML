using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 0.25f;
    public float maxSpeed = 2f;
    public float rotationSpeed = 0.25f;
    public float minDistance = 1;
    public Transform target;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (CheckDistanceTarget() < minDistance)
        {
            target.position = FindRandomPoint();
        }
    }

    private void FixedUpdate()
    {
        GoTarget();
        RotateToTarget();

     
    }

  
    public Vector3 FindRandomPoint()
    {
        return new Vector3(Random.Range(-5, 5), Random.Range(2, 5), Random.Range(-5, 5));
    }


    void RotateToTarget()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CheckDirectionTarget()), Time.deltaTime * 3f * rotationSpeed);
    }

    public float rotationScaleMovement  = 0.1f;
    void GoTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 dir= target.transform.position - transform.position;

        //if (rb.velocity.magnitude > maxSpeed)
        //{
        //    float counterSpeed = rb.velocity.magnitude;
        //    rb.AddForce(-dir.normalized * (speed/ counterSpeed) * Time.fixedDeltaTime, ForceMode.Force);
        //}
        //else
        //{
        //    
        //}

        rb.AddForce(dir.normalized * speed * Time.fixedDeltaTime, ForceMode.Force);

        Debug.DrawRay(transform.position, dir.normalized * speed * Time.fixedDeltaTime + Vector3.one * rotationScaleMovement * Vector3.Angle(transform.forward, CheckDirectionTarget()), Color.red);

     

    }

    void GoForward()
    {
        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void OnDrawGizmosSelected()
    {
       
        Gizmos.color = Color.red;


        Gizmos.color = Color.green;
        if (rb != null) Gizmos.DrawRay(transform.position, rb.velocity);
       
       

    }


    /// <summary>
    /// Get distance between self and a position
    /// </summary>
    public float GetDistance(Vector3 position) { return Vector3.Distance(transform.position, position); }
    /// <summary>
    /// Get direction between self and a position
    /// </summary>
    public Vector3 GetDirection(Vector3 target) { return target - transform.position; }

    /// <summary>
    /// Get distance between self and a target
    /// </summary>
    public float CheckDistanceTarget() { return Vector3.Distance(transform.position, target.transform.position); }
    /// <summary>
    /// Get direction between self and a target
    /// </summary>
    public Vector3 CheckDirectionTarget() { return target.transform.position - transform.position; }

    public Vector3 CheckDirectionAnyTarget(Vector3 target)
    {
        return target - transform.position;
    }

    Vector3 horizontalDir;
    /// <summary>
    /// Get horizontal direction between self and target (dir.Y = 0)
    /// </summary>
    public Vector3 CheckHorizontalDirectionTarget()
    {
        horizontalDir = target.transform.position - transform.position;
        horizontalDir.y = 0;
        return horizontalDir;
    }

    public Vector3 CheckHorizontalDirectionToTarget(Vector3 original, Vector3 target)
    {
        horizontalDir = target - original;
        horizontalDir.y = 0;
        return horizontalDir;
    }

    /// <summary>
    /// Get horizontal direction between self and position (dir.Y = 0)
    /// </summary>
    public Vector3 CheckHorizontalDirectionTarget(Vector3 position)
    {
        horizontalDir = position - transform.position;
        horizontalDir.y = 0;
        return horizontalDir;
    }

    /// <summary>
    /// Get direction between 2 positions
    /// </summary>
    public Vector3 GetDirection2Points(Vector3 pos1, Vector3 pos2) { return pos2 - pos1; }


}
