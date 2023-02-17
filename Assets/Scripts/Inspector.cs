using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour
{
    public Button HideButton;
    public GameObject Background;
    public Text PlanetName;
    public TMP_InputField IPlanetName;
    public TMP_InputField IPlanetRadius;
    public TMP_InputField IPlanetMass;

    [System.NonSerialized] public bool InputFileSelected = false;
    private CameraScript mCamera;
    bool hide = false;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
        mCamera = Camera.main.GetComponent<CameraScript>();
        PlanetName.text = "";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NewSelected(PlanetData selected)
    {
        IPlanetName.text = selected.PlanetName;
        PlanetName.text = IPlanetName.text;
        IPlanetRadius.text = selected.Radius.ToString();
        IPlanetMass.text = selected.Mass.ToString();
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

    public void PlanetRadiusEnter()
    {
        float result;
        if (mCamera.Selected && float.TryParse(IPlanetRadius.text, out result))
            mCamera.Selected.Radius = result;
        else if (mCamera.Selected)
            IPlanetRadius.text = mCamera.Selected.Radius.ToString();
    }


    public void PlanetNameEnter()
    {
        if (!mCamera.Selected) return;

        mCamera.Selected.SetPlanetName(IPlanetName.text);
        PlanetName.text = IPlanetName.text;
    }

    public void PlanetMassEnter()
    {
        if (!mCamera.Selected) return;

        float result;
        if (float.TryParse(IPlanetRadius.text, out result))
        {
            mCamera.Selected.Mass = result;
        }
    }
}
