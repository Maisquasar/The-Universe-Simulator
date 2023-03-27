using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PlanetTab : MonoBehaviour
{
    struct ImageTextGroup
    {
        public RawImage image;
        public Text text;
    }

    HorizontalLayoutGroup mLayout;
    [SerializeField] PlanetUiManager PlanetManager;
    List<ImageTextGroup> PlanetsImageTextGroup = new List<ImageTextGroup>();
    // Start is called before the first frame update
    public void Start()
    {
        mLayout = gameObject.GetComponentInChildren<HorizontalLayoutGroup>();
        foreach (var i in PlanetManager.transform.GetComponentsInChildren<PlanetData>())
        {
            i.gameObject.SetActive(false);
            i.Prefab = i.PlanetName;
            //Converts desired path into byte array
            byte[] pngBytes = System.IO.File.ReadAllBytes(Application.streamingAssetsPath + "/Resources/" + i.name + ".png");

            //Creates texture and loads byte array data to create image
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(pngBytes);
            //var icon = AssetDatabase.GetCachedIcon(path);
            Transform vertical = Instantiate(mLayout.transform.GetChild(0), mLayout.transform);
            vertical.gameObject.SetActive(true);
            vertical.GetComponentInChildren<PlanetImageUi>().PlanetRef = i;
            ImageTextGroup imageTextGroup = new ImageTextGroup();
            imageTextGroup.image = vertical.transform.GetComponentInChildren<RawImage>();
            imageTextGroup.text = vertical.transform.GetComponentInChildren<Text>();
            imageTextGroup.image.texture = tex;
            imageTextGroup.text.text = i.name;
            PlanetsImageTextGroup.Add(imageTextGroup);
        }
        mLayout.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
