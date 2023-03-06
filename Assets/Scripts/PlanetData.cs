using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetData : MonoBehaviour
{
    [Header("Planet Parameters")]
    [Tooltip("The Radius of the planet in Km")]
    public string PlanetName = "";

    [Tooltip("The Mass In Earth mass (1 EarthMass = 5.97�10^24 kg)")]
    public float Mass = 1.0f;

    [Tooltip("The Radius of the planet in Km")]
    public float Radius = 6371.0f;

    [Tooltip("The Desnity of the planet in g/cm^3")]
    public float Density = 5.51f;

    [Tooltip("The planet Speed in km/s")]
    public float Speed = 30.0f;

    [Tooltip("The planet Gravity in m/s^2")]
    public float Gravity = 9.81f;

    public Material trajectoryMat;

    private LineRenderer LineDrawer;

    private LineRenderer TrajectoryDrawer;

    private CameraScript mCamera;

    public DVec3 Velocity = new DVec3();
    public DVec3 PhysicPosition = new DVec3();
    public DVec3 LerpedPosition = new DVec3();

    public bool IsData = false;
    public bool Placed = false;

    private DVec3[] path = new DVec3[256];
    private int pathSize = 0;

    private PlanetDataManager manager;

    // Start is called before the first frame update
    void Start()
    {
        var objToSpawn = new GameObject("Circle");
        objToSpawn.transform.parent = gameObject.transform;
        LineDrawer = objToSpawn.AddComponent<LineRenderer>();
        LineDrawer.startWidth = 0.01f;
        LineDrawer.endWidth = 0.01f;
        LineDrawer.material = new Material(Shader.Find("Unlit/Texture"));
        CreateCircle();
        mCamera = Camera.main.GetComponent<CameraScript>();

        manager = FindObjectOfType<PlanetDataManager>();
        if (!IsData)
        {
            manager.ReceivePlanet(this);
            PhysicPosition = new DVec3(transform.position);
            LerpedPosition = new DVec3(transform.position);
            TrajectoryDrawer = gameObject.AddComponent<LineRenderer>();
            TrajectoryDrawer.startWidth = 0.01f;
            TrajectoryDrawer.endWidth = 0.01f;
            TrajectoryDrawer.material = trajectoryMat;
            TrajectoryDrawer.startColor = Color.green;
            TrajectoryDrawer.endColor = Color.blue;
            TrajectoryDrawer.enabled = false;
            TrajectoryDrawer.positionCount = 0;
        }
    }

    public void BeginDrag()
    {
        gameObject.layer = 2; //Ignore Raycasts.
        IsData = false;
        Placed = false;
    }

    public void EndDrag()
    {
        gameObject.layer = 0;
        PlaceInSpace();
    }

    public void PlaceInSpace()
    {
        PhysicPosition = new DVec3(transform.position) + manager.GetFocusLerped();
        LerpedPosition = new DVec3(transform.position) + manager.GetFocusLerped();
        Placed = true;
        manager.ReceivePlanet(this);
    }

    public void DrawTrajectory()
    {
        TrajectoryDrawer.positionCount = Mathf.Min(pathSize, path.Length);
        int index = 0;
        for (int i = Mathf.Max(pathSize-path.Length, 0); i < pathSize; i++)
        {
            TrajectoryDrawer.SetPosition(index, (path[i % path.Length] - manager.GetFocusLerped()).AsVector());
            index++;
        }
    }

    public void PuchPositionToTrajectory()
    {
        path[pathSize % path.Length] = LerpedPosition;
        pathSize++;
    }

    public void SetPlanetName(string value)
    {
        PlanetName = value;
        GetComponentInChildren<PlanetNameScript>().Planet3DText.text = PlanetName;
    }

    private void OnDestroy()
    {
        manager.DeletePlanet(this);
    }

    void CreateCircle()
    {
        float Distance = Vector3.Distance(Camera.main.transform.position, transform.position);
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
    [Header("Circle Ui")]
    public float ThetaScale = 0.001f;
    public float CircleRadius;
    private float Theta = 0f;
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
        CircleRadius = Mathf.Max(Distance * 0.05f, 0.02f);
        var mouse = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(Distance);
        bool inside = IsInside(screenPoint.x, screenPoint.y, screenPoint.z, CircleRadius, mouse.x, mouse.y, mouse.z);
        if (inside)
        {
            if (mCamera.Hovered == null || (mCamera.Hovered != this && Distance < Vector3.Distance(Camera.main.transform.position, mCamera.Hovered.transform.position)))
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
        if (!inside && mCamera.Hovered == this || mCamera.Hovered != this && LineDrawer.gameObject.activeSelf)
        {
            if (mCamera.Hovered == this)
                mCamera.Hovered = null;
            LineDrawer.gameObject.SetActive(false);
        }
    }

    public void OnMouseDown()
    {
        TrajectoryDrawer.enabled = true;
        mCamera.SelectPlanet(this);
    }

    public void HideTrajectory()
    {
        TrajectoryDrawer.enabled = false;
    }
}
