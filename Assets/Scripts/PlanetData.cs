using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlanetData : MonoBehaviour
{
    [Header("Planet Parameters")]
    [Tooltip("The Radius of the planet in Km")]
    public string PlanetName = "";

    [Tooltip("The Mass In Earth mass (1 EarthMass = 5.97�10^24 kg)")]
    public float Mass = 1.0f;

    [SerializeField]
    [Tooltip("The Radius of the planet in Km")]
    public float radius = 6371.0f;

    [Tooltip("The Desnity of the planet in g/cm^3")]
    public float Density = 5.51f;

    [Tooltip("The planet Speed in km/s")]
    public float Speed = 30.0f;

    [Tooltip("The planet Gravity in m/s^2")]
    public float Gravity = 9.81f;

    public Material trajectoryMat;

    [SerializeField] GameObject Particles;

    public bool IsBlackHole = false;

    public string Prefab;

    bool mDestroy = false;

    public float Radius {
        set
        {
            radius = value;
            var scale = value / 10000f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        get
        {
            return radius;
        }
    }

    private LineRenderer TrajectoryDrawer;

    private CameraScript mCamera;

    public DVec3 Velocity = new DVec3();
    public DVec3 Acceleration = new DVec3();
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
        Radius = radius;
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
            TrajectoryDrawer.endColor = Color.green;
            TrajectoryDrawer.enabled = false;
            TrajectoryDrawer.positionCount = 0;
        }
        if (Placed || IsData)
        {
            Prefab = PlanetName;
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
        if (!TrajectoryDrawer)
            return;
        if (!TrajectoryDrawer.enabled) TrajectoryDrawer.enabled = true;
        TrajectoryDrawer.positionCount = Mathf.Min(pathSize, path.Length) + 1;
        int index = 0;
        for (int i = Mathf.Max(pathSize - path.Length, 0); i < pathSize; i++)
        {
            TrajectoryDrawer.SetPosition(index, (path[i % path.Length] - manager.GetFocusLerped()).AsVector());
            index++;
        }
        TrajectoryDrawer.SetPosition(index, (LerpedPosition - manager.GetFocusLerped()).AsVector());
        TrajectoryDrawer.startWidth = Vector3.Distance(transform.position, Camera.main.transform.position) * 0.005f;
        TrajectoryDrawer.endWidth = TrajectoryDrawer.startWidth;
    }

    public void PuchPositionToTrajectory()
    {
        path[pathSize % path.Length] = LerpedPosition;
        pathSize++;
    }

    public void SetPlanetName(string value)
    {
        PlanetName = value;
    }

    private void OnDestroy()
    {
        if (manager)
            manager.DeletePlanet(this);
    }

    void Update()
    {
        var mouse = Mouse.current.position.ReadValue();
        var Distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        var screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        bool inside = mCamera.IsInside(screenPoint.x, screenPoint.y, mCamera.CircleRadius, mouse.x, mouse.y);
        Vector3 camToObject = transform.position - Camera.main.transform.position;
        if (inside && Vector3.Dot(Camera.main.transform.forward, camToObject) > 0)
        {
            if (mCamera.Hovered == null || (mCamera.Hovered != this && Distance < Vector3.Distance(Camera.main.transform.position, mCamera.Hovered.transform.position)))
            {
                mCamera.SetHovered(this);
            }
            if (mCamera.Hovered == this)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    this.OnClick();
                }
            }

        }
        if (!inside && mCamera.Hovered == this || mCamera.Hovered != this)
        {
            if (mCamera.Hovered == this)
                mCamera.SetHovered(null);
        }
    }

    public void OnClick()
    {
        TrajectoryDrawer.enabled = true;
        mCamera.SelectPlanet(this);
    }

    public void HideTrajectory()
    {
        TrajectoryDrawer.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (manager.TimeScale == 0) return;
        var planet = other.gameObject.GetComponent<PlanetData>();
        if (planet && !mDestroy && !planet.mDestroy)
        {
            if (!planet.Placed || !this.Placed)
                return;
            if (planet.Mass > Mass)
            {
                print($"Planet {other.name} hit {name}; {name} Destroyed");
                planet.Mass += Mass;
                planet.radius += Radius;
                mDestroy = true;
                if (!IsBlackHole && !planet.IsBlackHole)
                {
                    var part = Instantiate(Particles);
                    part.transform.position = this.transform.position;
                    var main = part.GetComponent<ParticleSystem>().main;
                    main.startSize = new ParticleSystem.MinMaxCurve(this.transform.lossyScale.x, this.transform.lossyScale.x);
                }
                Destroy(this.gameObject);
            }
            else
            {
                print($"Planet {other.name} hit {name}; {other.name} Destroyed");
                Mass += planet.Mass;
                Radius += planet.Radius;
                other.isTrigger = false;
                planet.mDestroy = true;
                if (!IsBlackHole && !planet.IsBlackHole)
                {
                    var part = Instantiate(Particles);
                    part.transform.position = planet.transform.position;
                    var main = part.GetComponent<ParticleSystem>().main;
                    main.startSize = new ParticleSystem.MinMaxCurve(this.transform.lossyScale.x, this.transform.lossyScale.x);
                }
                Destroy(planet.gameObject);
            }
        }
    }
}
