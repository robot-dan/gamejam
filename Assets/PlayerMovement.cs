using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float upwardForce = 1000f;
    public float sideForce = 1000f;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            rb.AddForce(0,upwardForce,0, ForceMode.Impulse);
        }

        if (Input.GetKey("a"))
        {
            rb.AddForce(-sideForce, 0, 0);
        }

        if (Input.GetKey("d"))
        {
            rb.AddForce(sideForce, 0, 0);
        }
        if (Input.GetKey("space"))
        {
            rb.drag = 3;
        }
        if (Input.GetKeyUp("space"))
        {
            rb.drag = 0;
        }
        if (Input.GetKey("z"))
        {
            rb.AddTorque(new Vector3(0f, 0f, 1f),ForceMode.Acceleration);
            Debug.Log("torque addded");
        }
    }

}
