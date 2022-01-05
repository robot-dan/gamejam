using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{

    
    public TrunkSwing ts;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ts.isGrappling())
        {
            this.transform.position = ts.getSwingPoint();
        }
        else if (Input.GetMouseButton(1))
        {
           
            var mousePos = Input.mousePosition;
            mousePos.z = 10;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            this.transform.position = new Vector3(mousePos.x, mousePos.y, 0);

        }
        if (Input.GetKey("p"))
        {
            Debug.Break();
        }


    }
}
