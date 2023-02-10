using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    enum Tab
    {
        PLANETS,
        STARS
    }

    [SerializeField] Button mPlanetsButton;
    [SerializeField] Button mStarsButton;
    [SerializeField] PlanetTab mPlanetTab;

    Tab mCurrentTab = Tab.PLANETS;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPlanetsSelection()
    {
        if (mCurrentTab == Tab.PLANETS)
            return;

        mCurrentTab = Tab.PLANETS;
        mPlanetTab.gameObject.SetActive(true);
    }

    public void ShowStarsSelection()
    {
        if (mCurrentTab == Tab.STARS)
            return;
        mCurrentTab = Tab.STARS;
        mPlanetTab.gameObject.SetActive(false);
    }
}
