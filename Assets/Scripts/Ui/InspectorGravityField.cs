using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorGravityField : MonoBehaviour
{
    public Button HideButton;
    public GameObject Background;
    public TMP_InputField IPointCount;
    public TMP_InputField IPointSize;
    public TMP_InputField IScaleParameter;
    public Toggle IIncludeAllPlanets;
    public Toggle IDrawAllTrajectories;
    public Toggle IDrawGField;
    public TMP_Dropdown IType;

    [NonSerialized] public bool InputFileSelected = false;
    private GravityFieldRenderer mGField;
    private PlanetDataManager mData;
    bool hide = false;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
        mGField = FindObjectOfType<GravityFieldRenderer>();
        mData = FindObjectOfType<PlanetDataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputFileSelected) return;
        IPointCount.text = mGField.PointCount.ToString();
        IPointSize.text = mGField.PointSize.ToString();
        IScaleParameter.text = mGField.ScaleParameter.ToString();
        IIncludeAllPlanets.isOn = mGField.IncludeAllPlanets;
        IType.value = (int)mGField.type;
    }

    public void Hide()
    {
        if (hide)
            return;
        hide = true;
        Background.SetActive(!hide);
    }

    public void Show()
    {
        if (!hide)
            return;
        hide = false;
        Background.SetActive(!hide);
    }

    public void EnterInputField()
    {
        InputFileSelected = true;
    }

    public void ExitInputField()
    {
        InputFileSelected = false;
    }

    public void HideShow()
    {
        hide = !hide;
        Background.SetActive(!hide);
    }

    public void PointCountEnter()
    {
        int result;
        if (int.TryParse(IPointCount.text, out result))
        {
            mGField.PointCount = result;
        }
        IPointCount.text = mGField.PointCount.ToString();
    }
    public void PointSizeEnter()
    {
        int result;
        if (int.TryParse(IPointSize.text, out result))
        {
            mGField.PointSize = result;
        }
        IPointSize.text = mGField.PointSize.ToString();
    }
    public void ScaleParameterEnter()
    {
        double result;
        if (double.TryParse(IScaleParameter.text, out result))
        {
            mGField.ScaleParameter = result;
        }
        IScaleParameter.text = mGField.ScaleParameter.ToString();
    }

    public void IncludeAllPlanetsCheck()
    {
        mGField.IncludeAllPlanets = IIncludeAllPlanets.isOn;
    }

    public void DrawAllTrajectories()
    {
        mData.ShowAllTrajectories = IDrawAllTrajectories.isOn;
    }

    public void DrawGField()
    {
        mGField.ShouldDraw = IDrawGField.isOn;
    }

    public void TypeChanged()
    {
        if (IType.value >= Enum.GetNames(typeof(RenderType)).Length || IType.value < 0) IType.value = 0;
        mGField.type = (RenderType)IType.value;
    }
}
