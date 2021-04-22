using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Doozy.Engine;
using Ink.Runtime;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// This is a super bare bones example of how to play and display a ink story in Unity.

[System.Serializable]
public class TagFoundEvent : UnityEvent<string> { }

[System.Serializable]
public class TextTagFoundEvent : UnityEvent<string> { }

[System.Serializable]
public class WriterOpenEvent : UnityEvent<InkWriter> { };

[System.Serializable]
public class WriterCloseEvent : UnityEvent<InkWriter> { };

/// <summary>
/// Main purpose for this is to have a wrapper for all those million ink variables...
/// </summary>

public class InkWriter : MonoBehaviour {

    [SerializeField]
    private bool m_startStory = false;
    public static InkWriter main;
    public bool mainWriter = true; // only one inkwriter should be main
    [SerializeField]
    private InkStoryObject inkStoryObject;
    public Story story;

    [SerializeField]
    private Canvas writerCanvas;
    [SerializeField]
    private CanvasGroup writerCanvasGroup;

    [SerializeField]
    private GameObject textArea;
    [SerializeField]
    private GameObject optionsArea;

    // UI Prefabs
    [SerializeField]
    private TextMeshProUGUI textPrefab;
    [SerializeField]
    private Button buttonPrefab;

    public ScrollRect scrollView;
    private bool autoScroll = false;

    public Image portrait;
    public Image background;
    public Button continueButton;
    public bool writerVisible = false;
    public bool clickToContinue = false;
    public bool hideWhenFinished = true;

    public bool clearOnNewStory = true;
    public bool clearAfterEveryChoice = false;
    private bool continueStory = true;
    private bool autoContinueNext = false;

    private Coroutine refreshCoroutine;
    public TagFoundEvent tagEvent;

    public TextTagFoundEvent textTagEvent;

    public TMPLinkClickedEvent linkClickedEvent;

    // literally only used once during loading :P
    public WriterOpenEvent onWriterOpen;
    public WriterCloseEvent onWriterClose;

    private List<Button> cachedButtons = new List<Button> { };
    private Button pickedButton;
    private string lastText;
    private string lastSaveableTags;

    // Make this array larger as more tags are added; currently portrait is saved in 0 and background in 1
    private string[] tagsToSave = new string[2] { "", "" };
    char delimiterLeft = '<';
    char delimiterRight = '>';
    private bool loading;

    void Awake () {

        if (main == null && mainWriter) {
            main = this;
            InitMain ();
        } else if (main != null && mainWriter) {
            Debug.LogWarning ("Cannot have two main inkWriters!");
        }
    }

    void InitMain () {
        story = new Story (inkStoryObject.inkJsonAsset.text);
        inkStoryObject.Init ();
        InkWriter.main.story.variablesState["debug"] = false;
    }

    void Start () {
        RemoveChildren ();
        if (m_startStory) {
            Invoke (nameof (StartStory), 0.1f);
        }

        if (!clickToContinue) {
            ActivateContinueButton (false);
        }
        continueButton.onClick.AddListener (OnClickContinueButton);
        // Find canvas group of writer canvas
        if (writerCanvas != null) {
            writerCanvasGroup = writerCanvas.GetComponent<CanvasGroup> ();
            writerCanvas.gameObject.SetActive (true);
        }
        //HideCanvas (true);
    }

    // Creates a new Story object with the compiled story which we can then play!
    public void StartStory () {
        HideCanvas (false);

        if (clearOnNewStory) {
            ClearChildren ();
        }

        lastText = "";
        lastSaveableTags = "";

        //string savedJson = PlayerPrefs.GetString(inkStoryObject.storyName + "savedInkStory");
        /*string savedJson = "";
        if (ES3.KeyExists (inkStoryObject.storyName + "_hasSaved")) {
            if (ES3.KeyExists (inkStoryObject.storyName + "savedInkStory")) {
                savedJson = ES3.Load<string> (inkStoryObject.storyName + "savedInkStory");
            };
            if (savedJson != "") {
                InkWriter.main.story.state.LoadJson (savedJson);
                Debug.Log ("Loading story");
                lastText = (string) InkWriter.main.story.variablesState["lastSavedString"];
                lastSaveableTags = (string) InkWriter.main.story.variablesState["lastSavedTags"];
                Debug.Log ("Tags at load point: " + lastSaveableTags);
                loading = true;
            } else { // no saved json -> go to "start" knot
                InkWriter.main.story.variablesState["debug"] = false;
                if (mainWriter) { GoToKnot ("start"); };
            }
        } else { // no saved json -> go to "start" knot
            ES3.DeleteKey (inkStoryObject.storyName + "savedInkStory");
            InkWriter.main.story.variablesState["debug"] = false;
            if (mainWriter) { GoToKnot ("start"); };
        }*/
        // ADDED THIS FOR TEST
        InkWriter.main.story.variablesState["debug"] = false;
        if (mainWriter) { GoToKnot ("start"); };
        // END TEST

        RefreshView ();
    }

    [NaughtyAttributes.Button]
    public void ResetStory () {
        lastText = "";
        for (int i = 0; i < tagsToSave.Length; i++) {
            tagsToSave[i] = "";
        }
        ES3.DeleteKey (inkStoryObject.storyName + "_hasSaved");
        if (!Application.isPlaying) {
            //PlayerPrefs.SetString(inkStoryObject.storyName + "savedInkStory", "");
            ES3.DeleteKey (inkStoryObject.storyName + "savedInkStory");
        } else {
            int childCount = textArea.transform.childCount;
            for (int i = childCount - 1; i >= 0; --i) {
                GameObject.Destroy (textArea.transform.GetChild (i).gameObject);
            }
            //PlayerPrefs.SetString(inkStoryObject.storyName + "savedInkStory", "");
            ES3.DeleteKey (inkStoryObject.storyName + "savedInkStory");

            DisablePortraits ();
            DisableBackgrounds ();
            StartStory ();
        }
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    void RefreshView () {
        // Remove all the UI on screen
        RemoveChildren ();
        if (clearAfterEveryChoice) {
            ClearChildren ();
        }
        continueStory = true;
        if (clickToContinue) { ActivateContinueButton (true); };
        if (refreshCoroutine == null) {
            refreshCoroutine = StartCoroutine (RefreshCoroutine ());
        } else {
            StopCoroutine (refreshCoroutine);
            refreshCoroutine = StartCoroutine (RefreshCoroutine ());
        }
    }

    IEnumerator RefreshCoroutine () {

        // if we loaded, display the last text
        if (loading) {
            LoadTags ();
            CreateContentView (lastText, textArea.transform);
            loading = false;
            lastText = "";
            lastSaveableTags = "";
        }

        // Read all the content until we can't continue any more
        while (InkWriter.main.story.canContinue) {
            // Continue gets the next line of the story
            yield return new WaitUntil (() => continueStory);
            CreateText ();
        }

        // continue click after last text
        yield return new WaitUntil (() => continueStory);
        // Disable continue button
        if (clickToContinue) { ActivateContinueButton (false); };
        // Display all the choices, if there are any!
        if (InkWriter.main.story.currentChoices.Count > 0) {
            for (int i = 0; i < InkWriter.main.story.currentChoices.Count; i++) {
                Choice choice = InkWriter.main.story.currentChoices[i];
                Button button = CreateChoiceView (choice.text.Trim ());
                // Tell the button what to do when we press it - also delete potential previous listeners
                button.onClick.RemoveAllListeners ();
                button.onClick.AddListener (delegate {
                    OnClickChoiceButton (choice, button);
                });
                // add button to cached button dictionary [nope -> added in CreateChoiceView instead]
                //cachedButtons.Add(button);
            }
            // Select the first -active- cached button to allow for navigation with keyboard
            foreach (Button button in cachedButtons) {
                if (button.interactable) {
                    button.Select ();
                    break;
                }
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else {
            if (mainWriter) {
                //SaveStory ();
                if (hideWhenFinished) {
                    HideCanvas (true);
                } else { // PROBABLY CHANGE THIS LOL.
                    Button choice = CreateChoiceView ("This is a terrible bug. Everything is broken. Click here to restart.");
                    cachedButtons.Add (choice);
                    choice.onClick.AddListener (delegate {
                        ResetStory ();
                    });
                }
            } else {
                if (hideWhenFinished) {
                    HideCanvas (true);
                }
            }
        }
        /*Button choice2 = CreateChoiceView ("Reset story");
		choice2.onClick.AddListener (delegate {
			ResetStory ();
		});
		cachedButtons.Add (choice2);*/
        // Move scrollbar
        MoveScrollBar (true);
    }

    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton (Choice choice, Button button) {

        lastText = "";
        InkWriter.main.story.ChooseChoiceIndex (choice.index);
        pickedButton = button;
        RefreshView ();

    }
    void OnClickContinueButton () {

        continueStory = true;

    }

    void CreateText () {
        string text = InkWriter.main.story.Continue ();
        // This removes any white space from the text.
        text = text.Trim ();
        if (text != "") {
            if (!autoContinueNext) // On auto-continues, we don't add the extra linebreaks, as these are generally 'system messages'
            {
                text += "\n\n";
            };
            // Display the text on screen!
            if (InkWriter.main.story.currentTags.Count > 0) {
                SaveTags (InkWriter.main.story.currentTags);
                if (TagParser (InkWriter.main.story.currentTags)) {
                    CreateContentView (text, textArea.transform);
                }
            } else {
                CreateContentView (text, textArea.transform);
            };
            lastText += text;
            if (clickToContinue) {
                if (!autoContinueNext) // Means we skip the click-to-continue for this one singular spot
                {
                    continueStory = false;
                    ActivateContinueButton (true);
                } else if (autoContinueNext && story.canContinue) {
                    autoContinueNext = false;
                } else {
                    continueStory = false;
                    ActivateContinueButton (true);
                }
            }
        }
    }
    void SaveTags (List<string> tags) {
        foreach (string tag in tags) {
            if (tag.Contains ("portrait")) {
                tagsToSave[0] = tag;
                //Debug.Log ("Saving tag: " + tag);
            }
            if (tag.Contains ("background")) {
                tagsToSave[1] = tag;
                //Debug.Log ("Saving tag: " + tag);
            }
        }
    }
    void SerializeSavedTags () { // save the tags that need to be saved/loaded, e.g. character portrait showing, background.
        // clear tags, we only want the latest

        lastSaveableTags = "";
        if (tagsToSave.Length > 0) {
            foreach (string tag in tagsToSave) {
                if (tag.Contains ("portrait")) {
                    lastSaveableTags += delimiterLeft;
                    lastSaveableTags += tag;
                    lastSaveableTags += delimiterRight;
                }
                if (tag.Contains ("background")) {
                    lastSaveableTags += delimiterLeft;
                    lastSaveableTags += tag;
                    lastSaveableTags += delimiterRight;
                }
            }
        };
        //Debug.Log ("Saving tags: " + lastSaveableTags);
    }
    void LoadTags () {
        // These are the symbols that delimit each individual string, by default <> - these can't be used in the body of the string itself! (you can't use [] btw, unless you do some clever regex-canceling magic)
        //Debug.Log ("Loading tags: " + lastSaveableTags);
        List<string> checkString = new List<string> { };

        // Checks for anything between delimiter brackets and then sends the first match onward.
        Regex brackets = new Regex (delimiterLeft + ".*?" + delimiterRight);
        MatchCollection matches = brackets.Matches (lastSaveableTags);
        if (matches.Count > 0) {
            for (int i = 0; i < matches.Count; i++) {
                // Trim out the actual string
                string ReqText = matches[i].Value.Trim (new Char[] { delimiterLeft, delimiterRight, ' ' });
                checkString.Add (ReqText);
                //Debug.Log ("Adding to parser list: " + ReqText);
                // Also re-add to saved tags!
            }
        };
        //Debug.Log ("Final parser list: " + checkString[0] + checkString[1]);
        SaveTags (checkString);
        TagParser (checkString);
    }

    void DisablePortraits () {
        int childCount = InkWriter.main.portrait.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            InkWriter.main.portrait.transform.GetChild (i).gameObject.SetActive (false);
        }
        //       Debug.Log("Disabled portraits");
    }
    void DisableBackgrounds () {
        int childCount = InkWriter.main.background.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            InkWriter.main.background.transform.GetChild (i).gameObject.SetActive (false);
        }
        //Debug.Log ("Disabled backgrounds");
    }

    void ActivateContinueButton (bool activate) {
        continueButton.gameObject.SetActive (activate);
        if (activate) {
            continueButton.Select ();
        }
    }

    [NaughtyAttributes.Button]
    public void SaveStory () {

        InkWriter.main.story.variablesState["lastSavedString"] = lastText;
        SerializeSavedTags ();
        InkWriter.main.story.variablesState["lastSavedTags"] = lastSaveableTags;
        //PlayerPrefs.SetString(inkStoryObject.storyName + "savedInkStory", InkWriter.main.story.state.ToJson());
        ES3.Save<string> (inkStoryObject.storyName + "savedInkStory", InkWriter.main.story.state.ToJson ());
        Debug.Log ("Tags at save point: " + lastSaveableTags);
    }

    [NaughtyAttributes.Button]
    public void LoadStory () {
        StartStory ();
    }
    void PauseStory () {
        SaveStory ();
        //RemoveChildren ();
        Button choice = CreateChoiceView ("Continue Story");
        choice.onClick.AddListener (delegate {
            StartStory ();
        });
    }

    public void GoToKnot (string knot) {
        if (GameManager.instance.GameState == GameStates.GAME) {
            if (clearOnNewStory) {
                ClearChildren ();
            }
            lastText = "";
            InkWriter.main.story.ChoosePathString (knot);
            HideCanvas (false);
            RefreshView ();
        };
    }
    public void GoToKnotUnsafe (string knot) {
        if (clearOnNewStory) {
            ClearChildren ();
        }
        lastText = "";
        InkWriter.main.story.ChoosePathString (knot, false);
        HideCanvas (false);
        RefreshView ();
    }
    public void ContinueStory () {
        Debug.Log ("Continue story at " + gameObject.name);
        if (clearOnNewStory) {
            //ClearChildren();
        }
        //InkWriter.main.story.Continue();
        if (!writerVisible) {
            Debug.LogWarning ("Writer is not visible for " + gameObject, gameObject);
            HideCanvas (false);
        };
        RefreshView ();
    }

    void ClearChildren () {
        int childCount = textArea.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            GameObject.Destroy (textArea.transform.GetChild (i).gameObject);
        }
        if (textArea != optionsArea) {
            childCount = optionsArea.transform.childCount;
            for (int i = childCount - 1; i >= 0; --i) {
                GameObject.Destroy (optionsArea.transform.GetChild (i).gameObject);
            }
        }
    }

    // Creates a piece of text
    void CreateContentView (string text, Transform parent) {
        TextMeshProUGUI storyText = FindExistingText (text);
        if (storyText == null) {
            storyText = Instantiate (textPrefab) as TextMeshProUGUI;
            storyText.text = text;
            storyText.transform.SetParent (parent, false);
            // Find the text link listener and listen to it
        };
        TMP_LinkWatcher linkWatcher = storyText.gameObject.GetComponent<TMP_LinkWatcher> ();
        if (linkWatcher != null) {
            linkWatcher.linkClickedEvent.RemoveAllListeners ();
            linkWatcher.linkClickedEvent.AddListener ((arg) => linkClickedEvent.Invoke (arg));
        };
    }
    public void DebugAThing (string debugtext) {
        Debug.LogWarning (debugtext);
    }
    // Creates a button showing the choice text
    Button CreateChoiceView (string text) {
        Button choice = FindExistingButton (text); // Finds an existing button
        if (choice == null) {
            // Creates the button from a prefab
            choice = Instantiate (buttonPrefab) as Button;
            choice.transform.SetParent (optionsArea.transform, false);
            // Gets the text from the button prefab
            TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI> ();
            choiceText.text = text;
            // Add it to the list of created buttons for later deletion - found buttons are not added!
            cachedButtons.Add (choice);
        };
        choice.interactable = ChoiceIsInteractable (text);
        // Make the button expand to fit the text
        //HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup> ();
        //layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    [NaughtyAttributes.Button]
    void DebugRegexStuff () {
        FindExistingButton ("[0/107 stuff] La la la [useButton.TestButton]");
    }

    TextMeshProUGUI FindExistingText (string text) { // Attempts to find an existing text component already in the scene based on the text given
        TextMeshProUGUI returnValue = null;
        GameObject returnObj = null;
        // Checks for anything between delimiter brackets and then sends the first match onward.
        Regex brackets = new Regex (@"(?<=\[).+?(?=\])");
        MatchCollection matches = brackets.Matches (text);
        if (matches.Count > 0) {
            for (int i = 0; i < matches.Count; i++) { // does it contain the text 'useButton'?
                //                Debug.Log(matches[i].Value);
                if (matches[i].Value.Contains ("useText.")) {
                    // If so, remove the useButton part, then try to find the button named what's left
                    string searchTerm = matches[i].Value.Replace ("useText.", "");
                    returnObj = GameObject.Find (searchTerm);
                    if (returnObj != null) {
                        returnValue = returnObj.GetComponent<TextMeshProUGUI> ();
                        if (returnValue != null) {
                            // Neat, let's clean the text and set it! (note that we do not care if it's interactable or not)
                            string newText = text.Replace (matches[i].Value, "").Replace ("[]", "");
                            returnValue.text = newText;
                            //                           Debug.Log("Found text and changed its text to " + newText);
                            break;
                        }
                    }
                }
                // The match did not contain anything - bad news!
                //                Debug.LogWarning("Got match: " + matches[i].Value + " but no text object was found!");
            }
        };
        return returnValue;
    }

    Button FindExistingButton (string text) { // Attempts to find an existing button already in the scene based on the text on the button (if there is a text on the button)
        Button returnValue = null;
        GameObject returnObj = null;
        // Checks for anything between delimiter brackets and then sends the first match onward.
        Regex brackets = new Regex (@"(?<=\[).+?(?=\])");
        MatchCollection matches = brackets.Matches (text);
        if (matches.Count > 0) {
            for (int i = 0; i < matches.Count; i++) { // does it contain the text 'useButton'?
                //               Debug.Log(matches[i].Value);
                if (matches[i].Value.Contains ("useButton.")) {
                    // If so, remove the useButton part, then try to find the button named what's left
                    string searchTerm = matches[i].Value.Replace ("useButton.", "");
                    returnObj = GameObject.Find (searchTerm);
                    if (returnObj != null) {
                        returnValue = returnObj.GetComponentInChildren<Button> ();
                        if (returnValue != null) {
                            // Neat, let's clean the text and set it! (note that we do not care if it's interactable or not)
                            string newText = text.Replace (matches[i].Value, "").Replace ("[]", "");
                            returnValue.GetComponentInChildren<TextMeshProUGUI> ().text = newText;
                            //                        Debug.Log("Found button and changed its text to " + newText);
                            break;
                        }
                    }
                }
                // The match did not contain anything - bad news!
                //            Debug.LogWarning("Got match: " + matches[i].Value + " but no button was found!");
            }
        };
        return returnValue;
    }

    bool ChoiceIsInteractable (string text) { // Checks to see if the choice should be interactable or not by reading a (x out of x) text in it

        bool returnValue = true;
        // Checks for anything between delimiter brackets and then sends the first match onward.
        Regex brackets = new Regex (@"(?<=\[).+?(?=\])");
        MatchCollection matches = brackets.Matches (text);
        if (matches.Count > 0) {
            for (int i = 0; i < matches.Count; i++) {
                // Split by '/' into two
                string[] parts = matches[i].Value.Split ('/');
                if (parts.Length < 2) {
                    Debug.Log ("Could not split into two with /, presumably not what we're looking for. Quitting.");
                    return true;
                }
                for (int z = 0; z < 2; z++) { // remove everything after spaces...
                    string[] newParts = parts[z].Split (' ');
                    parts[z] = newParts[0];
                }
                // Create new intParts array to store the stringParts - we only care about the first two no matter what
                int[] intParts = new int[2];
                for (int y = 0; y < 2; y++) { // Create 
                    //  Debug.Log("parted string: " + parts[y]);
                    int tryParsed = -1;
                    int.TryParse (parts[y], out tryParsed);
                    intParts[y] = tryParsed;
                    //  Debug.Log("parted int: " + intParts[y]);
                }
                // Now compare intsParts [0] and [1] - if it's smaller, return false!
                if (intParts[0] < intParts[1]) {
                    returnValue = false;
                }
            }
        };
        return returnValue;
    }

    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren () {
        /*int childCount = textArea.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			GameObject.Destroy (textArea.transform.GetChild (i).gameObject);
		}*/
        foreach (Button btn in cachedButtons) {
            if (btn != pickedButton) {
                GameObject.Destroy (btn.gameObject);
            };
        }

        if (pickedButton != null) {
            pickedButton.targetGraphic.color = pickedButton.colors.highlightedColor;
            pickedButton.interactable = false;
        };
        cachedButtons.Clear ();
        pickedButton = null;
    }

    void MoveScrollBar (bool move) {
        autoScroll = move;
    }

    public void HideCanvasInvoke () {
        HideCanvas ();
    }
    public void HideCanvas (bool hide = true, bool switchToGame = true) {
        writerVisible = !hide;
        //GameManager.instance.PauseGame (!hide);
        //allow switching between Game and Narrative, but not between other states
        if (mainWriter) {
            if (hide && switchToGame) {
                Debug.Log ("Returning to gamestate.", gameObject);
                GameEventMessage.SendEvent ("EndNarrative");
                GameManager.instance.SetState (GameStates.GAME);
            } else if (!hide) {
                Debug.Log ("Starting narrative.", gameObject);
                GameEventMessage.SendEvent ("StartNarrative");
                GameManager.instance.SetState (GameStates.NARRATIVE);
            } else {
                Debug.LogWarning ("Something went wrong with the narrative/game state switch!");
            }
        } else {
            // Non-main canvases cannot switch out of narrative - only the mainWriter can
            if (!hide) {
                Debug.Log ("Showing non-main writer");
            } else {
                Debug.Log ("Hiding non-main writer.");
            }
        }
        /* 
        writerCanvasGroup.alpha = hide ? 0f : 1f;
        writerCanvasGroup.interactable = !hide;
        writerCanvasGroup.blocksRaycasts = !hide;
        */
        if (hide) {
            GameEventMessage.SendEvent ("CloseInkWriter");
            onWriterClose.Invoke (this);
        } else {
            GameEventMessage.SendEvent ("OpenInkWriter");
            onWriterOpen.Invoke (this);
        }
        if (hide && mainWriter) {
            DisableBackgrounds ();
            DisablePortraits ();
        }
    }

    void Update () {
        if (scrollView != null) {
            if (scrollView.verticalNormalizedPosition > 0f && autoScroll) {
                scrollView.verticalNormalizedPosition = -0.01f;
            } else if (scrollView.verticalNormalizedPosition <= 0f && autoScroll) {
                autoScroll = false;
            }
        }
    }

    bool TagParser (List<string> tags) {
        /*foreach (string tag in tags) {
			Debug.Log ("Tagparser content " + tag);
		}*/
        //Debug.Log (tags[0]);
        // parse tags, and return true or false if the story should continue
        bool returnValue = true;
        if (tags.Contains ("endStory")) {
            PauseStory ();
            return false;
        }
        if (tags.Contains ("saveStory")) {
            SaveStory ();
        }
        if (tags.Contains ("changeportrait")) {
            DisablePortraits ();
        }
        if (tags.Contains ("changebackground")) {
            DisableBackgrounds ();
        }
        if (tags.Contains ("startSay")) {
            Debug.Log ("Starting say.");
            StopCoroutine (refreshCoroutine);
            //HideCanvas(true);
            returnValue = false;
            GameEventMessage.SendEvent ("OpenDialogueWriter");
        }
        if (tags.Contains ("endSay")) {
            Debug.Log ("Ending say.");
            StopCoroutine (refreshCoroutine);
            HideCanvas (true);
            returnValue = false;
            // Add the said text to the main content window!
            InkWriter.main.CreateContentView (lastText.Replace ("\n", "") + "\n\n", InkWriter.main.textArea.transform);
            lastText = "";
        }
        if (tags.Contains ("characterCreationStart")) { // do not continue here, continue elsewhere!
            Debug.Log ("Character creation check received!");
            StopCoroutine (refreshCoroutine);
            HideCanvas (true, false);
            returnValue = false;
            GameEventMessage.SendEvent ("OpenCharacterCreatorWriter");
        }
        if (tags.Contains ("characterCreationEnd")) {
            Debug.Log ("Character creation check end received!");
            StopCoroutine (refreshCoroutine);
            HideCanvas (true);
            returnValue = false;
        }
        if (tags.Contains ("openJournal")) { // do not continue here, continue elsewhere!
            Debug.Log ("Opening journal!");
            StopCoroutine (refreshCoroutine);
            HideCanvas (true, false);
            returnValue = false;
            GameEventMessage.SendEvent ("OpenJournalWriter");
        }
        if (tags.Contains ("closeJournal")) {
            Debug.Log ("Closing journal!");
            StopCoroutine (refreshCoroutine);
            HideCanvas (true, false);
            //Invoke ("HideCanvasInvoke", 0.5f);
            returnValue = false;
        }
        if (tags.Contains ("autoContinue")) // Automatically continue, bypassing the continue button!
        {
            Debug.Log ("Auto-continuing!");
            if (clickToContinue) {
                autoContinueNext = true;
            }
        }
        // invoke all the tags!
        foreach (string tag in tags) {
            //            Debug.Log("Invoking tag: " + tag);
            InkWriter.main.tagEvent.Invoke (tag);
        }

        return returnValue;
    }
}