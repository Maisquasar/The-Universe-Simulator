using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    public enum Tool
    {
        SELECTION,
        TRANSFORM,
        DELETE,
    };

    public Vector3 Center;
    public float Distance = 5.0f;
    public float ScrollSensibility = 10.0f;
    public float MoveSensibility = 0.5f;
    public float CameraSpeed = 1f;
    private float initalSpeed = 0.0f;
    public float CameraTransitionTime = 0.5f;
    private Vector2 delta;
    private Vector2 lastPos;
    private Inspector mInspector;
    private bool hasFinishedLerp = false;
    private PlanetDataManager mPlanetDataManager;
    [System.NonSerialized] public Tool CurrentTool = Tool.SELECTION;

    [System.NonSerialized] public PlanetData Selected;
    [System.NonSerialized] public PlanetData Focused;
    [System.NonSerialized] public PlanetData Hovered;

    // Start is called before the first frame update
    void Start()
    {
        ScrollSensibility = 0.025f;
        initalSpeed = CameraSpeed;
        mInspector = FindObjectOfType<Inspector>();
        mPlanetDataManager = FindObjectOfType<PlanetDataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraMovements();
    }

    void UpdateCameraMovements()
    {
        if (hasFinishedLerp && Focused)
        {
            Center = Focused.transform.position;
        }
        // Return if inside an Ui Component
        if (mInspector.InputFileSelected) return;
        else
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            if (raycastResults.Count > 0)
            {
                return;
            }
        }
        Distance -= (Distance * 5) * Input.mouseScrollDelta.y * ScrollSensibility;
        Distance = Mathf.Min(Distance, 500000);
        float min = GetMin();
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

        //Inputs Update 
        if (Input.GetKey(KeyCode.LeftShift) && CameraSpeed == initalSpeed)
        {
            CameraSpeed *= 2;
        }
        else
        {
            CameraSpeed = initalSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (Focused)
                DisableFocus();
            Center += transform.forward * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (Focused)
                DisableFocus();
            Center -= transform.forward * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (Focused)
                DisableFocus();
            Center += transform.right * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (Focused)
                DisableFocus();
            Center -= transform.right * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (Focused)
                DisableFocus();
            Center += transform.up * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (Focused)
                DisableFocus();
            Center -= transform.up * CameraSpeed;
        }

        if (Input.GetMouseButtonDown(0) && Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)).Length == 0)
        {
            if (Selected)
                Deselect();
        }
        delta = new Vector2();
        Vector3 cameraDir = -transform.forward;
        transform.position = Center + cameraDir.normalized * Distance;
    }

    public void DisableFocus()
    {
        Focused = null;
    }

    public void Deselect()
    {
        SelectPlanet(null);
    }

    float GetMin()
    {
        if (Selected) return Mathf.Max(Selected.transform.lossyScale.x * 2, 0.0f);
        else return 0.0f;
    }

    public void SelectPlanet(PlanetData planet)
    {
        switch (CurrentTool)
        {
            case Tool.SELECTION:
                {
                    if (planet)
                    {
                        Selected = planet.GetComponent<PlanetData>();
                    }
                    else
                    {
                        Selected = null;
                        StopAllCoroutines();
                    }
                    Focused = Selected;
                    mInspector.NewSelected(Selected);
                    mPlanetDataManager.SetFocusedPlanet(Focused);
                    break;
                }
            case Tool.TRANSFORM:
                {
                    break;
                }
            case Tool.DELETE:
                {
                    if (!planet) return;
                    Destroy(planet.gameObject);
                    break;
                }
        }

    }


    public void LerpCamera(GameObject planet)
    {
        StopAllCoroutines();
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
        hasFinishedLerp = false;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            Center = Vector3.Lerp(initial, Selected.transform.position, t / duration);
            yield return 0;
        }
        Center = Selected.transform.position;
        hasFinishedLerp = true;
    }
}
