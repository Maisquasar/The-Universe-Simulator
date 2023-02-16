using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetData : MonoBehaviour
{
    [Tooltip("The Mass In Earth mass (1 EarthMass = 5.97�10^24 kg)")]
    public float Mass = 1.0f;

    [Tooltip("The Radius of the planet in Km")]
    public float Radius = 6371;

    [Tooltip("The Radius of the planet in Km")]
    public string PlanetName = "";

    private LineRenderer LineDrawer;

    private CameraScript mCamera;

    // Start is called before the first frame update
    void Start()
    {
        var objToSpawn = new GameObject("Circle");
        objToSpawn.transform.parent = gameObject.transform;
        LineDrawer = objToSpawn.AddComponent<LineRenderer>();
        LineDrawer.startWidth = 0.01f;
        LineDrawer.endWidth = 0.01f;
        LineDrawer.material = new Material(Shader.Find("Standard"));
        CreateCircle();
        mCamera = Camera.main.GetComponent<CameraScript>();
    }

    private void OnDestroy()
    {
    }

    void CreateCircle()
    {
        float Distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        //float factor = 0.6f * (transform.lossyScale.x / 0.6f);
        CircleRadius = Mathf.Max(Distance * 0.05f, transform.lossyScale.x + 0.002f);
        float width = Mathf.Max(0.004f * Distance, 0.004f);
        LineDrawer.startWidth = width;
        LineDrawer.endWidth = width;
        Theta = 0f;
        int Size = (int)((1f / ThetaScale) + 1f);
        LineDrawer.positionCount = (Size);
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = CircleRadius * Mathf.Cos(Theta);
            float y = CircleRadius * Mathf.Sin(Theta);
            LineDrawer.SetPosition(i, (transform.position + Camera.main.transform.rotation * new Vector3(x, y, 0)));
        }
    }

    public float ThetaScale = 0.01f;
    private float Theta = 0f;
    public float CircleRadius;

    bool IsInside(float circle_x, float circle_y, float circle_z,
                              float rad, float x, float y, float z)
    {
        if ((x - circle_x) * (x - circle_x) +
            (y - circle_y) * (y - circle_y) +
            (z - circle_z) * (z - circle_z) <= rad * rad)
            return true;
        else
            return false;
    }

    void Update()
    {
        var screenPoint = transform.position;
        var Distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        var mouse = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(Distance);
        bool inside = IsInside(screenPoint.x, screenPoint.y, screenPoint.z, CircleRadius, mouse.x, mouse.y, mouse.z);
        if (inside)
        {
            if (!mCamera.Hovered || (mCamera.Hovered != this && Distance < Vector3.Distance(Camera.main.transform.position, mCamera.Hovered.transform.position)))
            {
                mCamera.Hovered = this;
                LineDrawer.gameObject.SetActive(true);
            }
            if (mCamera.Hovered == this)
            {
                CreateCircle();
                if (Input.GetMouseButtonDown(0))
                {
                    this.OnMouseDown();
                }
            }

        }
        if (!inside || (inside && mCamera.Hovered != this))
        {
            mCamera.Hovered = null;
            LineDrawer.gameObject.SetActive(false);
        }
    }

    public void OnMouseDown()
    {
        var camera = Camera.main.GetComponent<CameraScript>();
        camera.LerpCamera(gameObject);
    }
}
