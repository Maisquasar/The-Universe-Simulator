using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetData : MonoBehaviour
{
    [Tooltip("The Mass In Earth mass (1 EarthMass = 5.97�10^24 kg)")]
    public float Mass = 1.0f;

    [Tooltip("The Radius of the planet in Km")]
    public float Radius = 6371;

    [Tooltip("The Radius of the planet in Km")]
    public string PlanetName = "";

    //private LineRenderer LineDrawer;

    [Tooltip("The Radius of the planet in Km")]
    // Start is called before the first frame update
    void Start()
    {
        //LineDrawer = gameObject.AddComponent<LineRenderer>();
        //LineDrawer.startWidth = 0.01f;
        //LineDrawer.endWidth = 0.01f;
    }

    //public float ThetaScale = 0.01f;
    //private float Theta = 0f;
    //private int Size;
    //public float radius = 3f;
    // Update is called once per frame
    void Update()
    {
        /*
        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        LineDrawer.SetVertexCount(Size);
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            LineDrawer.SetPosition(i,  transform.rotation * (transform.position + new Vector3(x, y, 0)));
        }
        */
    }

    public void OnMouseDown()
    {
        var camera = Camera.main.GetComponent<CameraScript>();
        camera.LerpCamera(gameObject);
    }
}
