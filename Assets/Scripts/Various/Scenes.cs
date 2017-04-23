﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour {
    public static readonly int MainMenu = 0;
    public static readonly int GameOver = 1;
    public static readonly int Level1 = 2;

    [SerializeField] GameObject cameras;
	[SerializeField] GameObject gameoverPanel;
    bool b = false;

	void Update () {
		if(Input.GetKeyDown(KeyCode.V))
        {
            b = !b;
            cameras.SetActive(b);
        }
	}

    static int currentScene = 0;

    public static void LoadScene(int sceneIndex)
    {
        currentScene = sceneIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public static int CurrentScene
    {
        get
        {
            return currentScene;
        }
    }

    public void BackToMenu()
	{
        LoadScene(MainMenu);
	}

    


    
}