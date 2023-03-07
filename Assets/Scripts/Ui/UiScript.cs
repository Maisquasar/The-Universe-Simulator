using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    enum Tab
    {
        NONE,
        PLANETS,
        STARS
    }

    [SerializeField] Button mPlanetsButton;
    [SerializeField] Button mStarsButton;
    [SerializeField] PlanetTab mPlanetTab;
    [SerializeField] StarTab mStarTab;

    Tab mCurrentTab = Tab.NONE;

    // Start is called before the first frame update
    void Start()
    {
        ShowPlanetsSelection();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPlanetsSelection()
    {
        if (mCurrentTab == Tab.PLANETS)
            return;
        mStarTab.gameObject.SetActive(false);
        mCurrentTab = Tab.PLANETS;
        mPlanetTab.gameObject.SetActive(true);
    }

    public void ShowStarsSelection()
    {
        if (mCurrentTab == Tab.STARS)
            return;
        mStarTab.gameObject.SetActive(true);
        mCurrentTab = Tab.STARS;
        mPlanetTab.gameObject.SetActive(false);
    }
}
