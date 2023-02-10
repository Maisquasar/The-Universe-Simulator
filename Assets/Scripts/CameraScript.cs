using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Vector3 Center;
    public float Distance = 5.0f;
    public float ScrollSensibility = 10.0f;
    public float MoveSensibility = 0.5f;
    public float CameraSpeed = 1.0f;
    private Vector2 delta;
    private Vector2 lastPos;

    bool mFirst = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Mouse1))
        {
            var newPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            delta = (newPos - lastPos) * MoveSensibility;
            lastPos = newPos;
        }
        else
        {
            lastPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (mFirst)
            {
                mFirst = false;
            }
            else
            {
                return;
            }
        }
        transform.RotateAround(Center, transform.up, delta.x);
        transform.RotateAround(Center, transform.right, -delta.y);
        if (Input.GetKey(KeyCode.LeftShift) && CameraSpeed == 1)
        {
            CameraSpeed *= 2;
        }
        else
        {
            CameraSpeed = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Center += transform.forward * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Center -= transform.forward * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Center += transform.right * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Center -= transform.right * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Center += transform.up * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Center -= transform.up * CameraSpeed;
        }

        delta = new Vector2();
        Vector3 cameraDir = transform.position - Center;
        transform.position = Center + cameraDir.normalized * Distance;
    }
}
