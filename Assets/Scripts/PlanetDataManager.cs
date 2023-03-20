using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetDataManager : MonoBehaviour
{
    const double GC = 6.67430e-11;

    [SerializeField] private List<PlanetData> Planets;
    [SerializeField] public double TimeScale = 1.0f;
    [SerializeField] private int TrajectoryUpdatePeriod = 10;
    [SerializeField] private float CameraLerpTime = 1.0f;
    private int currentFrame = 0;
    private float timeSinceLastFixedUpdate;

    private PlanetData focusedPlanet;
    private CameraScript mainCam;
    [SerializeField] private float lerpTime = 0.0f;
    [SerializeField] private DVec3 lastLerpedPos = new DVec3();
    [SerializeField] public bool ShowAllTrajectories = false;
    private bool TrajectoriesState = false;

    public ref List<PlanetData> GetAllPlanets() { return ref Planets; }


    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastFixedUpdate = Time.realtimeSinceStartup;
        mainCam = Camera.main.GetComponent<CameraScript>();
        CameraLerpTime = mainCam.CameraLerpTime;
    }

    private UiScript mCanvas;
    private void Awake()
    {
        mCanvas = FindObjectOfType<UiScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentFrame++;
        bool updateTrajectory = false;
        if (currentFrame >= TrajectoryUpdatePeriod)
        {
            currentFrame -= TrajectoryUpdatePeriod;
            updateTrajectory = true;
        }
        timeSinceLastFixedUpdate = Time.realtimeSinceStartup;
        double dt = (Time.fixedDeltaTime * TimeScale);
        for (int i = 0; i < 10; i++)
        {
            List<DVec3> tempPos = new List<DVec3>();
            foreach (var planet in Planets)
            {
                if (!planet.Placed) continue;
                double dt2 = dt / 10;
                DVec3 newPos = planet.PhysicPosition + planet.Velocity * dt2 + planet.Acceleration * (dt2 * dt2 * 0.5);
                DVec3 newAcc = GetAccelAtPoint(planet.PhysicPosition, planet);
                DVec3 newVel = planet.Velocity + (planet.Acceleration + newAcc) * (dt2 * 0.5);
                tempPos.Add(newPos);
                planet.Velocity = newVel;
                planet.Acceleration = newAcc;
                if (i == 9 && updateTrajectory) planet.PuchPositionToTrajectory();
            }
            int counter = 0;
            foreach (var planet in Planets)
            {
                if (!planet.Placed) continue;
                planet.PhysicPosition = tempPos[counter];
                counter++;
            }
        }
    }

    private void Update()
    {
        if (TrajectoriesState != ShowAllTrajectories)
        {
            if (!ShowAllTrajectories) foreach (var planet in Planets)
            {
                if (!planet.Placed || planet == focusedPlanet) continue;
                planet.HideTrajectory();
            }
            TrajectoriesState = ShowAllTrajectories;
        }
        if (lerpTime > 0)
        {
            lerpTime -= Time.deltaTime;
        }
        double delta = (Time.realtimeSinceStartup - timeSinceLastFixedUpdate) * TimeScale;
        if (focusedPlanet && focusedPlanet.Placed)
        {
            focusedPlanet.LerpedPosition = focusedPlanet.PhysicPosition + focusedPlanet.Velocity * delta;
            focusedPlanet.transform.position = (focusedPlanet.LerpedPosition - GetFocusLerped()).AsVector();
        }
        foreach (var planet in Planets)
        {
            if (!planet.Placed || planet == focusedPlanet) continue;
            planet.LerpedPosition = planet.PhysicPosition + planet.Velocity * delta;
            planet.transform.position = (planet.LerpedPosition - GetFocusLerped()).AsVector();
            if (ShowAllTrajectories) planet.DrawTrajectory();
        }
        if (focusedPlanet)
        {
            focusedPlanet.DrawTrajectory();
        }
    }

    public void ReceivePlanet(PlanetData planet)
    {
        if (!Planets.Contains(planet))
            Planets.Add(planet);
        mCanvas.AddText(planet);
    }

    public void DeletePlanet(PlanetData planet)
    {
        mCanvas.RemoveText(planet);
        Planets.Remove(planet);
    }

    public void SetFocusedPlanet(PlanetData planet)
    {
        if (focusedPlanet == planet) return;
        if (focusedPlanet)
        {
            if (!ShowAllTrajectories) focusedPlanet.HideTrajectory();
            lastLerpedPos = focusedPlanet.LerpedPosition;
        }
        else
        {
            lastLerpedPos = new DVec3();
        }
        lerpTime = CameraLerpTime;
        focusedPlanet = planet;
    }

    public DVec3 GetFocus()
    {
        if (focusedPlanet) return focusedPlanet.LerpedPosition;
        else return new DVec3();
    }

    public DVec3 GetFocusLerped()
    {
        if (lerpTime > 0.0f) return lastLerpedPos + (GetFocus() - lastLerpedPos) * ((CameraLerpTime - lerpTime) / CameraLerpTime);
        else return GetFocus();
    }

    public DVec3 GetAccelAtPoint(DVec3 point, PlanetData self = null, bool lerped = false, double boxSize = 0)
    {
        DVec3 result = new DVec3();
        foreach (var planet in Planets)
        {
            if (planet == self) continue; // no need to apply +inf acceleration to ourself
            if (boxSize > 0 && (Math.Abs(planet.transform.position.x) > boxSize || Math.Abs(planet.transform.position.y) > boxSize || Math.Abs(planet.transform.position.z) > boxSize)) continue;
            DVec3 direction = (lerped ? planet.LerpedPosition : planet.PhysicPosition) - point;
            double dist = direction.Length() * 1e7; // one unit is 10 000 Km, so we need to multiply by 1e7 to go in meters
            if (dist < 0.000001) continue; // also no need to apply +inf acceleration at all
            result += direction.Normalized() * (GC*planet.Mass*5.97e24/(dist*dist));
        }
        return result / 1e7; // dont forget to rescale up to our unit system
    }

    public double GetAccelForceAtPoint(DVec3 point, PlanetData self = null, double boxSize = 0)
    {
        double result = 0.0;
        foreach (var planet in Planets)
        {
            if (planet == self) continue; // no need to apply +inf acceleration to ourself
            if (boxSize > 0 && (Math.Abs(planet.transform.position.x) > boxSize || Math.Abs(planet.transform.position.y) > boxSize || Math.Abs(planet.transform.position.z) > boxSize)) continue;
            DVec3 direction = planet.LerpedPosition - point;
            double dist = direction.Length() * 1e7; // one unit is 10 000 Km, so we need to multiply by 1e7 to go in meters
            if (dist < 0.000001) continue; // also no need to apply +inf acceleration at all
            result += GC * planet.Mass * 5.97e24 / (dist * dist);
        }
        return result / 1e7; // dont forget to rescale up to our unit system
    }
    public bool IsPosInsideSomething(Vector3 pos)
    {
        foreach (var planet in Planets)
        {
            float dist = (planet.transform.position - pos).magnitude;
            if (planet.Radius / 10000 >= dist) return true;
        }
        return false;
    }
}
