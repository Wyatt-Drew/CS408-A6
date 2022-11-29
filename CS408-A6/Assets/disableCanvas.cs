using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This module handles particle system and canvas commands

public class disableCanvas : MonoBehaviour
{
    private bool isPaused = false;
    private bool isMenu = true;
    private bool isCredits = false;
    private miscLogic miscLogic;
    public GameObject panel;
    void Start()
    {
        miscLogic = FindObjectOfType<miscLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMenu)
        {
            if (Input.inputString != "")
            {
                GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
                isMenu = false;
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

    //Toggle credits
    public void toggleCredits()
    {
        isCredits = !isCredits;
        Debug.Log(isCredits);
        panel.SetActive(isCredits);
    }
}
