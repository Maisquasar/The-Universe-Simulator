using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    public float ScrollSensibility = .001f;
    public float MoveSensibility = 0.5f;
    public float CameraSpeed = 1f;
    private float initalSpeed = 0.0f;
    public float CameraLerpTime = 1.0f;
    private Vector2 delta;
    private Vector2 lastPos;
    private Inspector mInspector;
    private bool hasFinishedLerp = false;
    private PlanetDataManager mPlanetDataManager;
    private Vector3 cameraMov = Vector3.zero;
    [System.NonSerialized] public Tool CurrentTool = Tool.SELECTION;

    public PlanetData Selected;
    public PlanetData Focused;
    public PlanetData Hovered;
    public PlanetData Dragged;

    public bool HasFinishedLerp {get => hasFinishedLerp; }

    private Camera mCamera;

    [SerializeField] GameObject mCircle;

    private UiScript mCanvas;
    private void Awake()
    {
        mCanvas = FindObjectOfType<UiScript>();
        mCircle.SetActive(false);
    }

    public void SetHovered(PlanetData planet)
    {
        Hovered = planet;
        if (planet)
            mCircle.SetActive(true);
        else
            mCircle.SetActive(false);

    }

    public void OnMoveH(InputAction.CallbackContext context)
    {
        Vector2 val = context.ReadValue<Vector2>();
        cameraMov.x = val.x;
        cameraMov.y = val.y;
    }

    public void OnMoveV(InputAction.CallbackContext context)
    {
        cameraMov.z = context.ReadValue<float>();
    }

    public void SetCircleRadius(float radius)
    {
        CircleRadius = radius;
        var rect = mCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(100 * radius / 75f, 100 * radius / 75f);
    }

    // Start is called before the first frame update
    void Start()
    {
        initalSpeed = CameraSpeed;
        mInspector = FindObjectOfType<Inspector>();
        mPlanetDataManager = FindObjectOfType<PlanetDataManager>();
        mCamera = GetComponent<Camera>();
        SetCircleRadius(50);
    }

    public float CircleRadius = 1000.0f;
    public bool IsInside(float circle_x, float circle_y,
                              float rad, float x, float y)
    {
        if ((x - circle_x) * (x - circle_x) +
            (y - circle_y) * (y - circle_y) <= rad * rad)
            return true;
        else
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraMovements();
        mCircle.SetActive(Hovered);
        if (Mouse.current.leftButton.ReadValue() != 0 && Dragged)
        {
            PlanetImageUi.UpdateDraggedPlanet(Dragged.gameObject);
        }
        else if (Dragged && Input.GetMouseButtonUp(0))
        {
            Dragged.EndDrag();
            Dragged = null;
        }
        if (Hovered && mCircle.activeSelf)
        {
            var position = Camera.main.WorldToScreenPoint(Hovered.transform.position);
            if (position.z < 0)
            {
                mCircle.SetActive(false);
            }
            mCircle.transform.position = new Vector3(position.x, position.y, 0);
        }
    }

    void UpdateCameraMovements()
    {
        // Return if inside an Ui Component
        if (mInspector.InputFileSelected || ClickOnUI()) return;
        mCamera.nearClipPlane = Mathf.Max(Distance * 0.000001f, 0.01f);
        Distance -= (Distance * 5) * (Mouse.current.scroll.ReadValue().y * ScrollSensibility);
        Distance = Mathf.Min(Distance, 500000);
        CameraSpeed = Distance * 0.01f;
        float min = GetMin();
        if (Distance < min) Distance = min;
        if (Mouse.current.rightButton.ReadValue() != 0)
        {
            var newPos = Mouse.current.position.ReadValue();
            delta = (newPos - lastPos) * MoveSensibility;
            lastPos = newPos;
        }
        else
        {
            lastPos = Mouse.current.position.ReadValue();
        }
        transform.RotateAround(Center, transform.up, delta.x);
        transform.RotateAround(Center, transform.right, -delta.y);

        if (Focused && cameraMov.sqrMagnitude > 0) DisableFocus();
        Center += transform.rotation * cameraMov * CameraSpeed;


        if (Mouse.current.leftButton.ReadValue() != 0 && Physics.RaycastAll(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue())).Length == 0)
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
        //SelectPlanet(null);
    }

    float GetMin()
    {
        if (Selected) return Mathf.Max(Selected.transform.lossyScale.x * 2, 0.0f);
        else return 0.0f;
    }

    public bool ClickOnUI()
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Mouse.current.position.ReadValue();
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            foreach (var r in raycastResults)
            {
                if (r.gameObject.layer == 5/*UI tag*/)
                    return true;
            }
        }
        return false;
    }

    public void SelectPlanet(PlanetData planet)
    {
        if (mInspector.InputFileSelected || ClickOnUI()) return;
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
                    mPlanetDataManager.SetFocusedPlanet(planet);
                    Focused = planet;
                    mInspector.NewSelected(Selected);
                    LerpCamera(planet.gameObject, CameraLerpTime);
                    break;
                }
            case Tool.TRANSFORM:
                {
                    Focused = null;
                    Selected = null;
                    Dragged = planet;
                    planet.BeginDrag();
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


    public void LerpCamera(GameObject planet, float lerpTime)
    {
        StartCoroutine(LerpCameraFromTo(Center, Vector3.zero, lerpTime));
        StartCoroutine(LerpDistanceFromTo(Distance, planet.transform.localScale.x * 3, lerpTime));
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
