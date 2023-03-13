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
    public TMP_InputField IPlanetPositionX;
    public TMP_InputField IPlanetPositionY;
    public TMP_InputField IPlanetPositionZ;
    public TMP_InputField IPlanetVelocityX;
    public TMP_InputField IPlanetVelocityY;
    public TMP_InputField IPlanetVelocityZ;

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
        if (!mCamera.Selected || InputFileSelected) return;
        IPlanetPositionX.text = mCamera.Selected.PhysicPosition.x.ToString();
        IPlanetPositionY.text = mCamera.Selected.PhysicPosition.y.ToString();
        IPlanetPositionZ.text = mCamera.Selected.PhysicPosition.z.ToString();
        IPlanetVelocityX.text = mCamera.Selected.Velocity.x.ToString();
        IPlanetVelocityY.text = mCamera.Selected.Velocity.y.ToString();
        IPlanetVelocityZ.text = mCamera.Selected.Velocity.z.ToString();
        IPlanetRadius.text = mCamera.Selected.Radius.ToString();
        IPlanetMass.text = mCamera.Selected.Mass.ToString();
    }

    public void NewSelected(PlanetData selected)
    {
        if (selected == null)
        {
            IPlanetName.text = null;
            PlanetName.text = null;
            IPlanetRadius.text = null;
            IPlanetMass.text = null;
            return;
        }
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
        {
            mCamera.Selected.Radius = result;
        }
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
        if (float.TryParse(IPlanetMass.text, out result))
        {
            mCamera.Selected.Mass = result;
        }
    }

    private void SetValueD(TMP_InputField field, ref double value)
    {
        double result;
        if (double.TryParse(field.text, out result))
        {
           value = result;
        }
        else
        {
            field.text = value.ToString();
        }
    }

    private void SetValueF(TMP_InputField field, ref float value)
    {
        float result;
        if (float.TryParse(field.text, out result))
        {
            value = result;
        }
        else
        {
            field.text = value.ToString();
        }
    }

    public void PlanetPositionEnter()
    {
        if (!mCamera.Selected) return;

        SetValueD(IPlanetPositionX, ref mCamera.Selected.PhysicPosition.x);
        SetValueD(IPlanetPositionY, ref mCamera.Selected.PhysicPosition.y);
        SetValueD(IPlanetPositionZ, ref mCamera.Selected.PhysicPosition.z);
    }


    public void PlanetVelocityEnter()
    {
        if (!mCamera.Selected) return;

        SetValueD(IPlanetVelocityX, ref mCamera.Selected.Velocity.x);
        SetValueD(IPlanetVelocityY, ref mCamera.Selected.Velocity.y);
        SetValueD(IPlanetVelocityZ, ref mCamera.Selected.Velocity.z);
    }
}
