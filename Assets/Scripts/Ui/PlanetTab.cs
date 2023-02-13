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
    [SerializeField] PlanetDataManager PlanetManager;
    List<ImageTextGroup> PlanetsImageTextGroup = new List<ImageTextGroup>();
    // Start is called before the first frame update
    void Start()
    {
        mLayout = gameObject.GetComponentInChildren<HorizontalLayoutGroup>();
        foreach (var i in PlanetManager.transform.GetComponentsInChildren<PlanetData>())
        {
            var path = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Textures/PlanetsTexture/" + i.name + ".png");
            //var icon = AssetDatabase.GetCachedIcon(path);
            Transform vertical = Instantiate(mLayout.transform.GetChild(0), mLayout.transform);
            vertical.gameObject.SetActive(true);
            vertical.GetComponentInChildren<PlanetImageUi>().PlanetRef = i;
            ImageTextGroup imageTextGroup = new ImageTextGroup();
            imageTextGroup.image = vertical.transform.GetComponentInChildren<RawImage>();
            imageTextGroup.text = vertical.transform.GetComponentInChildren<Text>();
            imageTextGroup.image.texture = path;
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
