using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    //This is a reference of the target's transform position
    [SerializeField] private Transform targetTransform;
    //Add some qualities to make it easier to see the trial
    //    results of the multiple AI
    //One for win condition, lose condition, and a floormesh
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer; 


    public override void OnEpisodeBegin()
    {
        //Reset the ai's position after it runs the trial
        //transform.position = Vector3.zero;

        //Update the position since we have multiple
        //    ai's running at the same time but in different
        //    global positions
        transform.localPosition = Vector3.zero; 


        //New updated stuff: Random Position of ai and target
        //May comment out since the yaml file did not work, python could not find the config folder
        //transform.localPosition = new Vector3(Random.Range(-1.3f, +0.5f), 0f, Random.Range(-1.5f, +1f));
        //targetTransform.localPosition = new Vector3(Random.Range(1.6f, 3.2f), 0f, Random.Range(-1.4f, +1f)); 
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //We need to know the position of the character 
        //    and the position of our target. 
        //These two transforms will allow the ai to complete
        //    it's task
        //Passing in 2 Vector3's that contain 3 values each
        //    which are the x, y, and z values
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Continuous actions = uses a number from -1 to 1 
        //Debug.Log(actions.ContinuousActions[0]);
        //Discrete actions = selects specific things in the arrays
        //Debug.Log(actions.DiscreteActions[0]); 

        //We are getting two continuous vales that will help
        //    the ai reach its goal
        //Recieve one for the x movement and one for the z
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 1f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    //This is to test where we are driving the actions ourselves
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //Using the input to move the character with the arrow keys
        //    by modifying the actions values
        //continuousActions[0] = Input.GetAxisRaw("Horizontal");
        //continuousActions[1] = Input.GetAxisRaw("Vertical"); 

    }
    private void OnTriggerEnter(Collider other)
    {
        //Setting a reward when hitting the reward collider
        if (other.CompareTag("Goal"))
        {
            //If our ai gets to the goal it gets a positive reward
            //    hence why it is +1 
            SetReward(+1f);
            floorMeshRenderer.material = winMaterial; 
            EndEpisode();
        }

        if(other.CompareTag("Walls"))
        {
            //If our ai hits the walls it gets a negative reward
            //  hence why it is -1
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial; 
            EndEpisode(); 
        }
    }
}
