using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroduce : MonoBehaviour
{
    public GameObject introducePanel;
    private bool isFirstShow = false;
    void Start()
    {
        if (introducePanel != null)
        {
            introducePanel.SetActive(false);
        }
    }
    public void ShowIntroduce()
    {
        
        if (introducePanel != null)
        {
            if (isFirstShow) { 
             introducePanel.SetActive(false);
                isFirstShow = false;
                return;
            }
            introducePanel.SetActive(true);
            isFirstShow = true;
        }
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
