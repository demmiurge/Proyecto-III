using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GraphicSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModesDropdown;
    [SerializeField] private TextMeshProUGUI screenResolution;

    //private List<string> screenResolutions = new List<string>();
    //private int currentIndex;

    private List<string> availableResolutions;
    private int currentResolutionIndex;

    private void Start()
    {
        availableResolutions = new List<string>();

        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            if (!availableResolutions.Contains(option))
                availableResolutions.Add(option);
        }

        UpdateCurrentIndexOfScreenResolution();
    }

    public void UpdateCurrentIndexOfScreenResolution()
    {
        //Resolution currentRes = Screen.currentResolution;
        //currentResolutionIndex = availableResolutions.FindIndex(res => res.width == currentRes.width && res.height == currentRes.height);
        //Resolution resolution = availableResolutions[currentResolutionIndex];
        //screenResolution.SetText(resolution.width + " x " + resolution.height);

        Resolution currentRes = Screen.currentResolution;

        for (var i = 0; i < availableResolutions.Count; i++)
        {
            string availableResolution = availableResolutions[i];
            if (availableResolution.Contains(currentRes.width.ToString()) && availableResolution.Contains(currentRes.height.ToString()))
            {
                currentResolutionIndex = i;
                screenResolution.SetText(availableResolution);
            }
        }
    }

    public void ApplyResolution(bool next)
    {
        if (next)
            currentResolutionIndex++;
        else
            currentResolutionIndex--;

        if (currentResolutionIndex < 0)
            currentResolutionIndex = availableResolutions.Count - 1;
        else if (currentResolutionIndex >= availableResolutions.Count)
            currentResolutionIndex = 0;

        //Resolution resolution = availableResolutions[currentResolutionIndex];
        //screenResolution.SetText(resolution.width + " x " + resolution.height);
        //Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        string selectedOption = availableResolutions[currentResolutionIndex];
        string[] resolutionValues = selectedOption.Split('x');
        int width = int.Parse(resolutionValues[0].Trim());
        int height = int.Parse(resolutionValues[1].Trim());

        screenResolution.SetText(selectedOption);
        Screen.SetResolution(width, height, Screen.fullScreen);
    }
}
