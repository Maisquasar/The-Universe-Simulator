using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetImageUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public PlanetData PlanetRef;
    private GameObject mDragedPlanet;

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
        mDragedPlanet.SetActive(true);
        mDragedPlanet.transform.localScale = PlanetRef.transform.localScale;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mDragedPlanet.transform.position = GetPoint(ray);
        mDragedPlanet.layer = 2; //Ignore Raycasts.
        mDragedPlanet.GetComponent<PlanetData>().IsData = false;
        mDragedPlanet.GetComponent<PlanetData>().Placed = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mDragedPlanet.layer = 0;
        mDragedPlanet.GetComponent<PlanetData>().Placed = true;
        mDragedPlanet.GetComponent<PlanetData>().LerpedPosition = new DVec3(mDragedPlanet.transform.position);
        mDragedPlanet = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragedPlanet)
            return;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mDragedPlanet.transform.position = GetPoint(ray);

    }

    private Vector3 GetPoint(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var radius = mDragedPlanet.transform.localScale.x;
            return hit.point + hit.normal * radius;
        }
        else
        {
            float dist = Vector3.Distance(Camera.main.GetComponent<CameraScript>().Center, Camera.main.transform.position);
            return ray.GetPoint(dist);
        }
    }
}
