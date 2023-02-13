using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotScript : MonoBehaviour
{
    public string PictureName;
    // Start is called before the first frame update
    void Start()
    {
        ScreenCapture.CaptureScreenshot("Assets/Textures/PlanetsTexture/" + PictureName + ".png");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
