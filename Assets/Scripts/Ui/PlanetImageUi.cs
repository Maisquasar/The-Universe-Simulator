using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetImageUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public PlanetData PlanetRef;
    public GameObject mDragedPlanet;

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
        mDragedPlanet.transform.localScale = new Vector3(100f, 100f, 100f);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mDragedPlanet.transform.position = ray.GetPoint(1000);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mDragedPlanet = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragedPlanet)
            return;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mDragedPlanet.transform.position = ray.GetPoint(1000);
    }
}
