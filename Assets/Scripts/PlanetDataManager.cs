using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDataManager : MonoBehaviour
{
    const double GC = 6.67430e-11;

    [SerializeField] private List<PlanetData> Planets;
    [SerializeField] private double TimeScale = 1.0f;
    [SerializeField] private int TrajectoryUpdatePeriod = 10;
    [SerializeField] private float CameraLerpTime = 1.0f;
    private int currentFrame = 0;
    private float timeSinceLastFixedUpdate;
    private PlanetData focusedPlanet;
    private CameraScript mainCam;
    [SerializeField] private float lerpTime = 0.0f;
    [SerializeField] private DVec3 lastLerpedPos = new DVec3();
    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastFixedUpdate = Time.realtimeSinceStartup;
        mainCam = Camera.main.GetComponent<CameraScript>();
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
        foreach (var planet in Planets)
        {
            if (!planet.Placed) continue;
            planet.PhysicPosition += planet.Velocity * (Time.fixedDeltaTime * TimeScale);
            planet.Velocity += GetAccelAtPoint(planet.PhysicPosition, planet) * Time.fixedDeltaTime * TimeScale;
            if (updateTrajectory) planet.PuchPositionToTrajectory();
        }
    }

    private void Update()
    {
        if (lerpTime > 0) lerpTime -= Time.deltaTime;
        double delta = (Time.realtimeSinceStartup - timeSinceLastFixedUpdate) * TimeScale;
        foreach (var planet in Planets)
        {
            if (!planet.Placed) continue;
            planet.LerpedPosition = planet.PhysicPosition + planet.Velocity * delta;
            planet.transform.position = (planet.LerpedPosition - GetFocusLerped()).AsVector();
        }
        if (focusedPlanet)
        {
            focusedPlanet.DrawTrajectory();
        }
    }

    public void ReceivePlanet(PlanetData planet)
    {
        Planets.Add(planet);
    }

    public void DeletePlanet(PlanetData planet)
    {
        Planets.Remove(planet);
    }

    public void SetFocusedPlanet(PlanetData planet)
    {
        if (planet == focusedPlanet) return;
        if (focusedPlanet)
        {
            focusedPlanet.HideTrajectory();
            lastLerpedPos = focusedPlanet.LerpedPosition;
        }
        else
        {
            lastLerpedPos = new DVec3();
        }
        lerpTime = CameraLerpTime;
        focusedPlanet = planet;
        if (focusedPlanet)
        {
            //mainCam.LerpCamera(focusedPlanet.gameObject);
        }
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

    private DVec3 GetAccelAtPoint(DVec3 point, PlanetData self = null)
    {
        DVec3 result = new DVec3();
        foreach (var planet in Planets)
        {
            if (planet == self) continue; // no need to apply +inf acceleration to ourself
            DVec3 direction = planet.PhysicPosition - point;
            double dist = direction.LengthSquared();
            if (dist < 0.000001) continue; // also no need to apply +inf acceleration at all
            result += direction.Normalized() * (GC*planet.Mass/dist);
        }
        return result;
    }
}
