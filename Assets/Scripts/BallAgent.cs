using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    public GameObject Target;
    private Rigidbody rb;
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0, 1f, 0);
        Target.transform.localPosition = new Vector3(Random.value * 8 - 4, 1f, Random.value * 8 - 4);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);   // Agent position
        sensor.AddObservation(Target.transform.localPosition); // Target position
        sensor.AddObservation(rb.velocity);                    // Agent velocity
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 agentMovement = Vector3.zero;
        agentMovement.x = actionBuffers.ContinuousActions[0];
        agentMovement.z = actionBuffers.ContinuousActions[1];

        rb.AddForce(agentMovement * speed);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.transform.localPosition);

        if (distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (this.transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
