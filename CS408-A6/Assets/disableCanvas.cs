using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableCanvas : MonoBehaviour
{
    private bool isPaused = false;
    private bool isMenu = true;
    private bool isCredits = false;
    private miscLogic miscLogic;
    public GameObject panel;
    private terrainManager terrainManager;
    void Start()
    {
        miscLogic = FindObjectOfType<miscLogic>();
        terrainManager = FindObjectOfType<terrainManager>();
    }
    void Update()
    {
        //Creative feature (Instructions)
        if (isMenu)
        {
            if (Input.inputString != "")
            {
                GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
                isMenu = false;
                terrainManager.updateIsMenu(false);
                miscLogic.toggleEmission(!isPaused);
                if (isCredits)
                    toggleCredits();
                return;
            }
        }
        foreach (char c in Input.inputString.ToLower())
        {
            switch (c)
            {
                case 'i'://instructions
                    {
                        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
                        isMenu = true;
                        terrainManager.updateIsMenu(true);
                        miscLogic.toggleEmission(false);
                        return;
                        //break;
                    }
                case 'p'://pause Emission
                    {
                        isPaused = !isPaused;
                        miscLogic.toggleEmission(!isPaused);
                        break;
                    }
            }
        }
    }

    //Creative feature (Credits)
    public void toggleCredits()
    {
        isCredits = !isCredits;
        panel.SetActive(isCredits);
    }
}
