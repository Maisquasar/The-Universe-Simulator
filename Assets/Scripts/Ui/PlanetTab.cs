using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlanetTab : MonoBehaviour
{
    HorizontalLayoutGroup mLayout;
    [SerializeField] PlanetDataManager PlanetManager;
    List<GameObject> PlanetDatas = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        mLayout = gameObject.GetComponentInChildren<HorizontalLayoutGroup>();
        foreach (var i in PlanetManager.transform.GetComponentsInChildren<PlanetData>())
        {
            var image = Instantiate(mLayout.transform.GetChild(0), mLayout.transform);
            PlanetDatas.Add(Instantiate(i.gameObject, image.transform));
            PlanetDatas.Last().transform.localScale = new Vector3(25, 25, 25);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
