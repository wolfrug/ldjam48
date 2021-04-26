using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {
    // Start is called before the first frame update
    public Button startGameButton;
    public Button restartGame;
    public Button quitGame;
    public string startGameScene;
    public string continueGameScene;
    private string sceneToLoad;
    void Start () {
        bool hasSaved = ES3.KeyExists ("LDJam48_HasSaved");
        if (hasSaved && startGameButton != null && restartGame != null) {
            startGameButton.GetComponentInChildren<TMPro.TextMeshProUGUI> ().text = "Continue";
            restartGame.gameObject.SetActive (true);
        }
        if (startGameButton != null) {
            if (hasSaved) {
                startGameButton.onClick.AddListener (() => LoadScene (startGameScene));
            } else {
                startGameButton.onClick.AddListener (() => LoadScene (continueGameScene));
            }

        };
        if (restartGame != null) {
            restartGame.onClick.AddListener (ResetGame);
        };
        if (quitGame != null) {
            quitGame.onClick.AddListener (QuitGame);
        };
    }

    void ResetGame () {
        ES3.DeleteKey ("LDJam48_HasSaved");
        ES3.DeleteKey ("default_savedInkStory");
        SceneManager.LoadScene (startGameScene);
    }

    void LoadScene (string scene) {
        Doozy.Engine.GameEventMessage.SendEvent ("HideScene");
        sceneToLoad = scene;
        Invoke ("LateLoadScene", 0.5f);
    }
    void LateLoadScene () {
        SceneManager.LoadScene (sceneToLoad);
    }
    void QuitGame () {
        Application.Quit ();
    }
    public void InvokeStartButton () {
        startGameButton.onClick.Invoke ();
    }

    // Update is called once per frame
    void Update () {

    }
}