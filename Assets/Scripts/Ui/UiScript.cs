using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiScript : MonoBehaviour
{
    enum Tab
    {
        NONE,
        PLANETS,
        STARS,
        OTHERS
    }

    [SerializeField] Button mPlanetsButton;
    [SerializeField] Button mStarsButton;

    [SerializeField] PlanetTab mPlanetTab;
    [SerializeField] StarTab mStarTab;
    [SerializeField] OtherTab mOtherTab;

    [SerializeField] GameObject mTextContainer;
    [SerializeField] GameObject mTextExample;
    [SerializeField] TextMeshProUGUI mTextZoom;

    Tab mCurrentTab = Tab.NONE;
    PlanetDataManager manager;
    Dictionary<PlanetData, TextMeshProUGUI> texts = new Dictionary<PlanetData, TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start()
    {
        ShowPlanetsSelection();
        manager = FindObjectOfType<PlanetDataManager>();
        mTextExample.SetActive(false);
    }

    public void QuitSimulation()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var text in texts)
        {
            var position = Camera.main.WorldToScreenPoint(text.Key.transform.position);
            if (position.z < 0)
            {
                text.Value.gameObject.SetActive(false);
                continue;
            }
            else
            {
                text.Value.gameObject.SetActive(true);
            }
            position.z = 0;
            text.Value.transform.position = position;
        }
        mTextZoom.text = $"Zoom : {(Camera.main.GetComponent<CameraScript>().Distance / 500000f).ToString("0.0000")}%";
    }

    public void AddText(PlanetData planet)
    {
        if (!texts.ContainsKey(planet))
        {
            var obj = Instantiate(mTextExample, mTextContainer.transform);
            obj.SetActive(true);
            obj.name = planet.PlanetName + "_text";
            var TextComponent = obj.GetComponent<TextMeshProUGUI>();
            TextComponent.text = planet.PlanetName;
            texts.Add(planet, TextComponent);
        }
    }

    public void RemoveText(PlanetData planet)
    {
        if (!planet)
            return;
        if (texts.ContainsKey(planet))
        {
            if (texts[planet])
            {
                var obj = texts[planet].gameObject;
                texts.Remove(planet);
                if (obj)
                {
                    Destroy(obj);
                }
            }
        }
    }

    public void ShowPlanetsSelection()
    {
        if (mCurrentTab == Tab.PLANETS)
            return;
        mStarTab.gameObject.SetActive(false);
        mOtherTab.gameObject.SetActive(false);
        mCurrentTab = Tab.PLANETS;
        mPlanetTab.gameObject.SetActive(true);
    }

    public void ShowStarsSelection()
    {
        if (mCurrentTab == Tab.STARS)
            return;
        mOtherTab.gameObject.SetActive(false);
        mPlanetTab.gameObject.SetActive(false);
        mCurrentTab = Tab.STARS;
        mStarTab.gameObject.SetActive(true);
    }

    public void ShowOthersSelection()
    {
        if (mCurrentTab == Tab.OTHERS)
            return;
        mPlanetTab.gameObject.SetActive(false);
        mStarTab.gameObject.SetActive(false);
        mCurrentTab = Tab.OTHERS;
        mOtherTab.gameObject.SetActive(true);
    }
}
