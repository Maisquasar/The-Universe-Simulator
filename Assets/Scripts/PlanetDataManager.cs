using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDataManager : MonoBehaviour
{
    const double GC = 6.67430e-11;

    [SerializeField] private List<PlanetData> Planets;
    [SerializeField] private float TimeScale = 1.0f;
    [SerializeField] private int TrajectoryUpdatePeriod = 10;
    private int currentFrame = 0;
    private float timeSinceLastFixedUpdate;
    private PlanetData focusedPlanet;
    private CameraScript mainCam;
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
            planet.IPosition += planet.Velocity * Time.fixedDeltaTime * TimeScale;
            planet.Velocity += GetAccelAtPoint(planet.transform.position, planet) * Time.fixedDeltaTime * TimeScale;
            if (updateTrajectory) planet.PuchPositionToTrajectory();
        }
        if (focusedPlanet)
        {
            focusedPlanet.DrawTrajectory();
        }
    }

    private void Update()
    {
        float delta = (Time.realtimeSinceStartup - timeSinceLastFixedUpdate) * TimeScale;
        foreach (var planet in Planets)
        {
            planet.transform.position = planet.IPosition + planet.Velocity * delta;
        }
    }

    public void ReceivePlanet(PlanetData planet)
    {
        Planets.Add(planet);
    }

    public void SetFocusedPlanet(PlanetData planet)
    {
        if (planet == focusedPlanet) return;
        if (focusedPlanet)
        {
            focusedPlanet.HideTrajectory();
        }
        focusedPlanet = planet;
        if (focusedPlanet)
        {
            mainCam.LerpCamera(focusedPlanet.gameObject);
        }
    }

    private Vector3 GetAccelAtPoint(Vector3 point, PlanetData self = null)
    {
        Vector3 result = Vector3.zero;
        foreach (var planet in Planets)
        {
            if (planet == self) continue; // no need to apply +inf acceleration to ourself, we are not BLJing
            Vector3 direction = planet.transform.position - point;
            float dist = direction.sqrMagnitude;
            result += direction.normalized * (float)(GC/dist);
        }
        return result;
    }
}
