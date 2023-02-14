using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetNameScript : MonoBehaviour, IPointerDownHandler
{
    [System.NonSerialized] public TextMesh Planet3DText;
    // Start is called before the first frame update
    void Start()
    {
        Planet3DText = GetComponent<TextMesh>();
        Planet3DText.fontSize = 60;
        Planet3DText.text = gameObject.transform.parent.GetComponent<PlanetData>().PlanetName;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(Planet3DText.transform.position, Camera.main.transform.position) * 0.005f;

        Planet3DText.transform.localScale = Vector3.one;
        Planet3DText.transform.localScale = new Vector3(dist / Planet3DText.transform.lossyScale.x, dist / Planet3DText.transform.lossyScale.y, dist / Planet3DText.transform.lossyScale.z);
        Planet3DText.transform.rotation = Camera.main.transform.rotation;
    }

    private void OnMouseDown()
    {
        print("test");
        gameObject.transform.parent.GetComponent<PlanetData>().OnMouseDown();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("test");
    }
}
