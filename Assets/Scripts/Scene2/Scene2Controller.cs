﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DigitalRuby.RainMaker;

public class Scene2Controller : MonoBehaviour
{
    [SerializeField] private Button cowButton, garbageButton, treesButton;
    [SerializeField] private GameObject rainPrefab, cowObject, garbageObject, 
        treesObject;
    [SerializeField] private AudioController audioController;
    [SerializeField] private SpritesS2Controller spritesController;
    [SerializeField] private CanvasS2Controller canvasController;
    [SerializeField] private Scene2NarratorController narratorController;
    private RainScript2D rainScript;
    private enum Tcontroller { CANVAS, SPRITE, SELF }

    public void quitScene() // Called by Button (btQuit)
    {
        AplicationModel.isFirstTimeScene2 = true;
        SceneManager.LoadScene("Main Menu");
    }

    private void loadPlayersForest() // Invoked in Start()
    {
        AplicationModel.scenesCompleted++;
        SceneManager.LoadScene("Players Forest");
    }

    public void showHelp()
    {
        playANarratorAudio("playHelpAudio", "leaveHelpInterface", 9f);
        canvasController.changeToHelpInterface();
    }

    private void startRaining() // Invoked by in Start()
    {
        rainScript =
            ( Instantiate(rainPrefab) ).GetComponent<RainScript2D>();
        rainScript.RainHeightMultiplier = 0f;
        rainScript.RainWidthMultiplier = 1.12f;
        Destroy(rainScript.gameObject, 8f);
    }

    private void playANarratorAudio(
        string functionToInvoke, string sceneFunction, float length,
        Tcontroller controller = Tcontroller.CANVAS, float awaitTime = 0f
    )
    {
        narratorController.Invoke(functionToInvoke, awaitTime + 1f);
        if(sceneFunction != null)
        {
            length += awaitTime;
            switch (controller)
            {
                case Tcontroller.CANVAS:
                    canvasController.Invoke(sceneFunction, length); break;
                case Tcontroller.SPRITE:
                    spritesController.Invoke(sceneFunction, length); break;
                
                default: Invoke(sceneFunction, length); break;
            }
        }
    }

    void Start() // Start is called before the first frame update
    {
        if(AplicationModel.isFirstTimeScene2)
        {
            AplicationModel.isFirstTimeScene2 = false;
            canvasController.showBackgroundCover();
            playANarratorAudio(
                "playIntroductionAudio", "hideBackgroundCover", 27.66f
            );
        }

        Action treesClicked = () => {

            playANarratorAudio(
                "playTreesSelectedAudio", "showTreesRoots", 8.74f,
                Tcontroller.SPRITE
            );
            playANarratorAudio(
                "playAboutRootsAudio", null, 9.89f,
                Tcontroller.SELF, 8.74f
            );
            playANarratorAudio(
                "playAboutEvaporationAudio", "startRaining", 16.56f,
                Tcontroller.SELF, 18.63f
            );
            playANarratorAudio(
                "playAboutRainAudio", "turnSceneGreenAndAnimalsHappy", 8.96f,
                Tcontroller.SPRITE, 35.19f
            );
            playANarratorAudio(
                "playSceneCompletedAudio", "loadPlayersForest", 9f,
                Tcontroller.SELF, 44.15f
            );
        };

        Action<GameObject, Button> buttonClicked = (gameObject, button) => {
            Button[] buttons = {cowButton, treesButton, garbageButton};

            canvasController.showBackgroundCover();
            foreach (Button btn in buttons) btn.onClick.RemoveAllListeners();
            Destroy(button.gameObject);
            gameObject.SetActive(true);
        };

        cowButton.onClick.AddListener( () => {
            buttonClicked(cowObject, cowButton);
            playANarratorAudio(
                "playCowSelectedAudio", "changeToTryAgainInterface", 7.76f
            );
        });

        garbageButton.onClick.AddListener( () => {
            buttonClicked(garbageObject, garbageButton);
            playANarratorAudio(
                "playTrashSelectedAudio", "changeToTryAgainInterface", 8.49f
            );
        });

        treesButton.onClick.AddListener( () => {
            buttonClicked(treesObject, treesButton);
            treesClicked();
        });
    }
}
