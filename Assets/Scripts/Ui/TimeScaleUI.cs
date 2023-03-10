using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScaleUI : MonoBehaviour
{
    [SerializeField] Sprite mPlayImage;
    [SerializeField] Sprite mPauseImage;
    [SerializeField] Button mPlayButton;
    [SerializeField] TMP_InputField InputField;

    private PlanetDataManager manager;
    float mTimeScale = 1f;
    public float TimeScale { set { manager.TimeScale = value; } }
    bool play = false;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PlanetDataManager>();
        InputField.text = manager.TimeScale.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButtonClick()
    {
        play = !play;
        if (play)
        {
            TimeScale = 0;
            mPlayButton.image.sprite = mPlayImage;
        }
        else
        {
            TimeScale = mTimeScale;
            mPlayButton.image.sprite = mPauseImage;
        }
    }

    public void OnInputEnter()
    {
        float value;
        if (float.TryParse(InputField.text, out value))
        {
            TimeScale = value;
            mTimeScale = value;
        }
        else
        {
            InputField.text = mTimeScale.ToString();
        }
    }

}
