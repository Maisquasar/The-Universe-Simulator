using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    public Vector3 Center;
    public float Distance = 5.0f;
    public float ScrollSensibility = 10.0f;
    public float MoveSensibility = 0.5f;
    public float CameraSpeed = 1.0f;
    public float CameraTransitionTime = 0.5f;
    private Vector2 delta;
    private Vector2 lastPos;

    bool mFirst = true;

    // Start is called before the first frame update
    void Start()
    {
        ScrollSensibility = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        Distance -= Input.mouseScrollDelta.y * ScrollSensibility;
        float min = 0.1f;
        if (Distance < min) Distance = min;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            var newPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            delta = (newPos - lastPos) * MoveSensibility;
            lastPos = newPos;
        }
        else
        {
            lastPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        transform.RotateAround(Center, transform.up, delta.x);
        transform.RotateAround(Center, transform.right, -delta.y);

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
        Vector3 cameraDir = -transform.forward;
        transform.position = Center + cameraDir.normalized * Distance;
    }

    public void LerpCamera(GameObject planet)
    {
        StartCoroutine(LerpCameraFromTo(Center, planet.transform.position, CameraTransitionTime));
        StartCoroutine(LerpDistanceFromTo(Distance, planet.transform.localScale.x * 3, CameraTransitionTime));
    }

    IEnumerator LerpDistanceFromTo(float initial, float goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            Distance = Mathf.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        Distance = goTo;
    }

    IEnumerator LerpCameraFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            Center = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        Center = goTo;
    }
}
