using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkSwing : MonoBehaviour
{   
    private LineRenderer lr;
    private Rigidbody playerRigidBody, gameObjectsRigidBody;
    private Vector3 swingPoint, jointDirection, offset;
    public LayerMask isSwingable;
    public bool fixedDistance = true;
    public Transform trunkBase, trunkEnd, player;
    private float maxDistance = 8f;
    private ConfigurableJoint joint, joint2;
    private GameObject myGameObject;
    
    

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        playerRigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.DrawRay(trunkBase.position, (trunkEnd.position - trunkBase.position)*100f,Color.red);
        if (Input.GetMouseButtonDown(0))
        {
            StartSwing();
        }else if (Input.GetMouseButtonUp(0))
        {
            StopSwing();
        }
        if (isGrappling())
        {
 
            jointDirection = joint.connectedAnchor - trunkBase.position;
            //player.rotation = Quaternion.LookRotation(jointDirection) * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            Vector3 torque = Vector3.Cross(jointDirection, player.transform.up);
            Debug.Log(torque);
            playerRigidBody.AddTorque(-0.75f*torque, ForceMode.Acceleration);
            //Try to reduce wobbliness
            playerRigidBody.AddTorque(-0.5f*playerRigidBody.angularVelocity);

            Vector3 maxPoint = joint.connectedAnchor + (joint.linearLimit.limit+ 4f) * (player.transform.TransformPoint(joint.anchor) - joint.connectedAnchor).normalized;
            myGameObject.transform.position = maxPoint;

        }


    }
    void FixedUpdate()
    {
      
        if (Input.GetKey("e"))
        {
            if (joint != null)
            {
                SoftJointLimit limit = joint.linearLimit;
                if(limit.limit < maxDistance-0.3f)
                {
                    limit.limit = limit.limit + 0.3f;
                    joint.linearLimit = limit;
                    if (fixedDistance)
                    {
                        //SoftJointLimit limit2 = joint.linearLimit;
                        //limit2.limit =  2f;
                        //joint2.linearLimit = limit2;
                    }
                    playerRigidBody.velocity = 0.999f * playerRigidBody.velocity;
                }
                
            }

        }
        else if (Input.GetKey("q"))
        {
            if (joint != null)
            {
                SoftJointLimit limit = joint.linearLimit;
                if (limit.limit> 0.3f){
                    limit.limit = limit.limit - 0.2f;

                    joint.linearLimit = limit;
                    if (fixedDistance)
                    {
                        //SoftJointLimit limit2 = joint.linearLimit;
                        //limit2.limit = 2f;
                        //joint2.linearLimit = limit2;
                    }
                    playerRigidBody.velocity = 1.01f * playerRigidBody.velocity;
                }
                
            }

        }
    }
    void LateUpdate()
    {
        drawRope();   
    }
    void StartSwing()
    {
        RaycastHit hit;
        if (Physics.Raycast(origin: trunkBase.position, direction: (trunkEnd.position-trunkBase.position), out hit, maxDistance, isSwingable))
        {
            swingPoint = hit.point;
            //joint = player.gameObject.AddComponent<SpringJoint>();
            joint = player.gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;
            offset = new Vector3(0f, 0.9f, 0f);
            joint.anchor = offset;

            float distanceFromPoint = Vector3.Distance(a: transform.TransformPoint(joint.anchor), b: swingPoint);

            SoftJointLimit limit = joint.linearLimit;
            limit.limit = 1f * distanceFromPoint;
            
            /*SoftJointLimitSpring springLimit = joint.linearLimitSpring;
            springLimit.spring = 50f;
            springLimit.damper = 10f;*/
            

            joint.configuredInWorldSpace = false;

            joint.axis = new Vector3(0f, 0f, 1f);

            joint.linearLimit = limit;
            //joint.linearLimitSpring = springLimit;

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;

            if (fixedDistance){
                myGameObject = new GameObject("Test Object"); // Make a new GO.
                gameObjectsRigidBody = myGameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
                gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
                gameObjectsRigidBody.isKinematic = true;
                Vector3 maxPoint = joint.connectedAnchor + (joint.linearLimit.limit + 4f) * (player.transform.TransformPoint(joint.anchor) - joint.connectedAnchor).normalized;
                myGameObject.transform.position = maxPoint;

                





                 joint2 = myGameObject.AddComponent<ConfigurableJoint>();
                 joint2.connectedBody = playerRigidBody;
                 joint2.autoConfigureConnectedAnchor = false;
                 joint2.connectedAnchor = -offset;

                 //joint2.anchor = joint.anchor;


                 joint2.configuredInWorldSpace = false;

                 joint2.axis = new Vector3(0f, 0f, 1f);

                SoftJointLimit limit2 = joint.linearLimit;
                limit2.limit = 0.5f;
                joint2.linearLimit = limit2;

                SoftJointLimitSpring springLimit = joint2.linearLimitSpring;
                //Higher number is stronger spring
                springLimit.spring = 150f;
                //Higher damper means it returns to normal quicker
                springLimit.damper = 1f;
                joint2.linearLimitSpring = springLimit;

                joint2.xMotion = ConfigurableJointMotion.Limited;
                 joint2.yMotion = ConfigurableJointMotion.Limited;
                 joint2.zMotion = ConfigurableJointMotion.Limited;
                 joint2.angularXMotion = ConfigurableJointMotion.Free;
                 joint2.angularYMotion = ConfigurableJointMotion.Free;
                 joint2.angularZMotion = ConfigurableJointMotion.Free;
                //joint2.swapBodies = true;
            }


            //Can't do it this way
            /*SoftJointLimit low_angular_x_limit = joint.lowAngularXLimit;
            low_angular_x_limit.limit = 90f;

            SoftJointLimit high_angular_x_limit = joint.highAngularXLimit;
            high_angular_x_limit.limit = 90f;

            SoftJointLimitSpring angular_x_spring = joint.angularXLimitSpring;
            angular_x_spring.spring = 100f;
            angular_x_spring.damper = 1f;

            
            jointDirection = joint.connectedAnchor - trunkBase.position;
            joint.secondaryAxis = jointDirection;

            joint.highAngularXLimit = high_angular_x_limit;
            joint.lowAngularXLimit = low_angular_x_limit;
            joint.angularXLimitSpring = angular_x_spring;

            
            

            joint.maxDistance = 1f * distanceFromPoint;
            joint.minDistance = 0.9f * distanceFromPoint;

            joint.spring = 50f;
            joint.damper = 0f;
            joint.massScale = 10f;
            */
            lr.positionCount = 2;
        }
    }

    void drawRope()
    {
        if (!joint) return;
        lr.SetPosition(index: 0, player.transform.TransformPoint(joint.anchor));
        lr.SetPosition(index: 1, swingPoint);
    }
    void StopSwing()
    {
        lr.positionCount = 0;
        Destroy(joint);
        if (fixedDistance)
        {
            Destroy(myGameObject);
            Destroy(joint2);
        }
    }
    public bool isGrappling()
    {
        return joint != null;
    }
    public Vector3 getSwingPoint()
    {
        return swingPoint;
    }
}
