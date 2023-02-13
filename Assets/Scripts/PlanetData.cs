using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetData : MonoBehaviour
{
    [Tooltip("The Mass In Earth mass (1 EarthMass = 5.97×10^24 kg)")]
    public float Mass = 1.0f;

    [Tooltip("The Radius of the planet in Km")]
    public float Radius = 6371;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
