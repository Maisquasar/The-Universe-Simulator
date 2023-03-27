using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlanetImageUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public PlanetData PlanetRef;
    private GameObject mDragedPlanet;
    public PlanetData mDragedPlanetData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mDragedPlanet = Instantiate(PlanetRef, GameObject.Find("All Planets").transform).gameObject;
        mDragedPlanetData = mDragedPlanet.GetComponent<PlanetData>();
        mDragedPlanet.SetActive(true);
        mDragedPlanet.transform.localScale = PlanetRef.transform.localScale;
        UpdateDraggedPlanet(mDragedPlanet);
        mDragedPlanet.layer = 2; //Ignore Raycasts.
        mDragedPlanetData.IsData = false;
        mDragedPlanetData.Placed = false;
    }

    static public void UpdateDraggedPlanet(GameObject planet)
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        planet.transform.position = GetPoint(ray, planet);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mDragedPlanet.layer = 0;
        mDragedPlanetData.PlaceInSpace();
        mDragedPlanet = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragedPlanet)
            return; 
        UpdateDraggedPlanet(mDragedPlanet);
    }

    static private Vector3 GetPoint(Ray ray, GameObject planet)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var radius = planet.transform.localScale.x;
            return hit.point + hit.normal * radius;
        }
        else
        {
            float dist = Vector3.Distance(Camera.main.GetComponent<CameraScript>().Center, Camera.main.transform.position);
            return ray.GetPoint(dist);
        }
    }
}
