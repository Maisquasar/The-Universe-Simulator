using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour
{
    Button HideButton;
    public GameObject Background;
    public Text PlanetName;
    private CameraScript mCamera;
    bool hide = false;
    // Start is called before the first frame update
    void Start()
    {
        HideButton = GameObject.Find("HideButton").GetComponent<Button>();
        HideShow();
        mCamera = Camera.main.GetComponent<CameraScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mCamera.Selected)
        {
            PlanetName.text = mCamera.Selected.PlanetName;
        }
    }

    public void HideShow()
    {
        hide = !hide;
        Background.SetActive(!hide);
    }
}
