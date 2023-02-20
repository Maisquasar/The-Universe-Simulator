using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolUi : MonoBehaviour
{
    public Button SelectionButton;
    public Button TransformButton;
    public Button GarbageButton;
    public Color Selected;
    public Color Default;

    private CameraScript mCamera;
    // Start is called before the first frame update
    void Start()
    {
        mCamera = Camera.main.GetComponent<CameraScript>();
        SelectionClicked();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectionClicked()
    {
        mCamera.CurrentTool = CameraScript.Tool.SELECTION;
        SelectionButton.GetComponent<Image>().color = Selected;
        TransformButton.GetComponent<Image>().color = Default;
        GarbageButton.GetComponent<Image>().color = Default;
    }

    public void TransformClicked()
    {
        mCamera.CurrentTool = CameraScript.Tool.TRANSFORM;
        SelectionButton.GetComponent<Image>().color = Default;
        TransformButton.GetComponent<Image>().color = Selected;
        GarbageButton.GetComponent<Image>().color = Default;

    }

    public void GarbageClick()
    {
        mCamera.CurrentTool = CameraScript.Tool.DELETE;
        SelectionButton.GetComponent<Image>().color = Default;
        TransformButton.GetComponent<Image>().color = Default;
        GarbageButton.GetComponent<Image>().color = Selected;

    }
}
