using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotScript : MonoBehaviour
{
    public string PictureName;
    // Start is called before the first frame update
    void Start()
    {
        SaveScreenshotToFile("Assets/Textures/PlanetsTexture/" + PictureName + ".png");
    }

    public Texture2D TakeScreenShot()
    {
        return Screenshot();
    }

    Texture2D Screenshot()
    {
        Camera camera = GetComponent<Camera>();
        int resWidth = camera.pixelWidth;
        int resHeight = camera.pixelHeight;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        return screenShot;
    }

    public Texture2D SaveScreenshotToFile(string fileName)
    {
        Texture2D screenShot = Screenshot();
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fileName, bytes);
        return screenShot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
