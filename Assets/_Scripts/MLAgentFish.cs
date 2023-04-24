using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;


public class MLAgentFish : Agent
{

    Rigidbody rb;
    public Transform Target;

    Vector3 originPos;


    [Header("Collision Configuration")]
    public Material collisionMaterial;
    public RandomObstaclePlacer obstacleGenerator;

    #region cycleBehavior
    BehaviorParameters behaviorParameters;
    bool cycled;
    [Header("Heuristic")]
    int heuristicCycle = 25;
    int currentHeuristic;
    [Header("Inference")]
    int inferenceCycle = 1000;
    int currentInference;
    #endregion


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<MeshRenderer>().material = collisionMaterial;
            obstacleHit++; 
        }
    }



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originPos = transform.localPosition;      
    }

    public override void OnEpisodeBegin()
    {
        // Reset Pos/Vel
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        transform.localPosition = originPos;

        Target.localPosition = new Vector3(originPos.x + Random.Range(-10f, 10f), originPos.y + Random.Range(0, 5f), originPos.z + Random.Range(-10, 10f));

        obstacleGenerator.ResetAllObstacles();
        obstacleGenerator.GenerateRandomObstacleLayout(Target.position);

        obstacleHit = 0;
        rewardedNearZone = false;
        currentTimer = 0;
        reachedObjective = false;
        currentReward = 0;
        acumulativePunishOverTime = 0;

        #region Change BehaviorType

        //if (!cycled)
        //{
        //    if (currentHeuristic > heuristicCycle)
        //    {
        //        currentHeuristic = 0;
        //        behaviorParameters.BehaviorType = BehaviorType.InferenceOnly;
        //        cycled = true;
        //    }
        //    else
        //    {
        //        currentHeuristic++;
        //    }
        //}
        //else
        //{
        //    if (currentInference > inferenceCycle)
        //    {
        //        currentInference = 0;
        //        behaviorParameters.BehaviorType = BehaviorType.HeuristicOnly;
        //        cycled = false;
        //    }
        //    else
        //    {
        //        currentInference++;
        //    }
        //}
        #endregion

    }

    public float forceMultiplier = 10f;
    int obstacleHit = 0;
    bool rewardedNearZone;


    float currentTimer;
    float maxTimeReset = 15f;

    float acumulativePunishOverTime = 0;

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.y = actions.ContinuousActions[1];
        controlSignal.z = actions.ContinuousActions[2];

        rb.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        float distanceToOrigin = Vector3.Distance(transform.localPosition, originPos);

        // Reached target
        if (distanceToTarget < 2f)
        {
            AddReward(750);
            reachedObjective = true;
            OnEndEpisode();
        }
       
        if (distanceToTarget < 5f)
        {
            if (!rewardedNearZone)
            {
                rewardedNearZone = true;
                AddReward(50f);
            }

            AddReward(5f * Time.deltaTime / distanceToTarget);
        }
      

        if (distanceToOrigin > 15f)
        {
            SetReward(-250);
            OnEndEpisode();
        }

        if (currentTimer>50)
        {
            SetReward(-250);
            OnEndEpisode();
        }
        else
        {
            if (distanceToTarget > 5f)
            {
                currentTimer += Time.deltaTime;
                acumulativePunishOverTime += Time.deltaTime * 2.5f;
                if (acumulativePunishOverTime < 150f)
                {
                    //Debug.Log(-acumulativePunishOverTime * 5);
                    AddReward(-Time.deltaTime * 2.5f);
                }
            }
        }
    }
    int currentReward = 0;
    bool reachedObjective = false;
    public void OnEndEpisode()
    {
        currentReward = 0;
        for (int i = 0; i < obstacleHit; i++)
        {
            if (currentReward + 100 > 500)
            {
                break;
            }

            currentReward += 100;
            AddReward(-100);
        }
        

        obstacleHit = 0;
        EndEpisode();
    }

    public float rotationSpeed = 2.5f;
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rb.velocity), Time.deltaTime * 3f * rotationSpeed);
    }

   
    public Vector3 CheckDirectionTarget() { return Target.transform.position - transform.position; }
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(rb.velocity.z);
      
    }

    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    rb.velocity = CheckDirectionTarget().normalized * forceMultiplier;
    //}

}
