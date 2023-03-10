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
        STARS,
        OTHERS
    }

    [SerializeField] Button mPlanetsButton;
    [SerializeField] Button mStarsButton;

    [SerializeField] PlanetTab mPlanetTab;
    [SerializeField] StarTab mStarTab;
    [SerializeField] OtherTab mOtherTab;

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
