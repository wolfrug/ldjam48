using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class WriterStarted : UnityEvent<TypeWriter> { }

[System.Serializable]
public class WriterStopped : UnityEvent<TypeWriter> { }

public class TypeWriter : MonoBehaviour {

    public TextMeshProUGUI self_;
    public float textSpeed_ = 25f;
    public float punctuationPause = 0.1f;
    public Button skipButton_;
    public bool writeTextOnStart_ = false;
    public bool useFade_ = true;
    public bool clearOldTextWithFade = true;
    public bool isWriting_;

    public WriterStarted startedEvent_;
    public WriterStopped stoppedEvent_;
    private string textToWrite_;
    private bool skipWrite_;
    private float startAlpha_ = 1f;

    private void Start () {
        if (skipButton_ != null) {
            skipButton_.onClick.AddListener (() => SkipWrite ());
        };
        startAlpha_ = self_.alpha;
        if (writeTextOnStart_) {
            if (clearOldTextWithFade) {
                StartCoroutine (FadeOutTextAndWrite (self_.text));
            } else {
                Write (self_.text);
            };
        } else {
            if (clearOldTextWithFade) {
                StartCoroutine (FadeOutText ());
            } else {
                ClearWriter ();
            };
        }
    }
    void OnDestroy () {
        StopAllCoroutines ();
    }

    public void WriteSimple (string text) {
        Write (text, true, false);
    }
    public void Write (string Text, bool clearText = true, bool showEncounterButton = true) {
        // Remove own text if set
        string theText = Text;
        if (clearText) {
            if (clearOldTextWithFade) {
              //  Debug.Log ("Clearing old text with fade and writing - return");
                StartCoroutine (FadeOutTextAndWrite (theText));
                return;
            } else {
            //    Debug.Log ("Clearing old text, no fade");
                ClearWriter ();
            }
        } else {
          //  Debug.Log ("Adding new text to old text, no clear.");
            theText = self_.text + " " + Text;
        }
        textToWrite_ = theText;
        StartCoroutine (Writer (theText));
    }
    public void SkipWrite () {
        skipWrite_ = true;
    }

    IEnumerator FadeOutTextAndWrite (string theText) {
        yield return StartCoroutine (FadeOutText ());
        textToWrite_ = theText;
        StartCoroutine (Writer (theText));
    }
    IEnumerator FadeOutText () {
        isWriting_ = true; // this is part of the writing!
        startedEvent_.Invoke (this);
        while (self_.alpha > 0f) {
            self_.alpha -= Time.deltaTime;
            yield return null;
        }
        ClearWriter ();
        self_.alpha = startAlpha_;
    }

    IEnumerator Writer (string theText) {

        if (!isWriting_) {
            isWriting_ = true;
            startedEvent_.Invoke (this);
        };
      //  Debug.Log ("Writing text: " + theText);
        if (!useFade_) {
            // Write the text out one letter at a time
            int stringLength = theText.Length;
            string writtenText = "";
            skipWrite_ = false;
            for (int i = 0; i < stringLength; i++) {
                writtenText += theText[i];
                // Punctuation!
                if (IsPunctuation (theText[i])) {
                    yield return new WaitForSeconds (punctuationPause);
                }
                self_.text = writtenText;
                yield return new WaitForSeconds (1f / textSpeed_);
                if (skipWrite_) {
                    break;
                }
            }
        } else { // Use fade > full text is added, but with an alpha of 00
            string alphaText = "<alpha=#00>" + theText;
            self_.text = alphaText;
            int stringLength = theText.Length;
            string writtenText = "";
            string textLeft = theText;
            skipWrite_ = false;
            //StartCoroutine (CharacterFader ());
            for (int i = 0; i < stringLength; i++) {
                textLeft = textLeft.Remove (0, 1);
                self_.text = writtenText + "<alpha=#22>" + theText[i] + "<alpha=#00>" + textLeft;
                //CharacterFader ();
                writtenText += "<alpha=#22>" + theText[i];
                writtenText = CharacterFader (writtenText);
                // Punctuation!
                if (IsPunctuation (theText[i])) {
                    yield return new WaitForSeconds (punctuationPause);
                }
                yield return new WaitForSeconds (1f / textSpeed_);
                if (skipWrite_) {
                    break;
                }
            }
            while (self_.text != theText) {
                writtenText = CharacterFader (writtenText);
                self_.text = writtenText;
                yield return new WaitForSeconds (1f / textSpeed_);
            }
        }
        self_.text = textToWrite_;
        isWriting_ = false;
        stoppedEvent_.Invoke (this);
    }
    public bool IsPunctuation (char character) {
        switch (character) {
            case '.':
                return true;
            case ',':
                return true;
            case ';':
                return true;
            case ':':
                return true;
            case '-':
                return true;
        }
        return false;
    }
    string CharacterFader (string targetText) {
        if (isWriting_) {
            string selfText = targetText;
            selfText = selfText.Replace ("<alpha=#FF>", "");
            selfText = selfText.Replace ("<alpha=#EE>", "<alpha=#FF>");
            selfText = selfText.Replace ("<alpha=#DD>", "<alpha=#EE>");
            selfText = selfText.Replace ("<alpha=#CC>", "<alpha=#DD>");
            selfText = selfText.Replace ("<alpha=#BB>", "<alpha=#CC>");
            selfText = selfText.Replace ("<alpha=#AA>", "<alpha=#BB>");
            selfText = selfText.Replace ("<alpha=#99>", "<alpha=#AA>");
            selfText = selfText.Replace ("<alpha=#88>", "<alpha=#99>");
            selfText = selfText.Replace ("<alpha=#77>", "<alpha=#88>");
            selfText = selfText.Replace ("<alpha=#66>", "<alpha=#77>");
            selfText = selfText.Replace ("<alpha=#55>", "<alpha=#66>");
            selfText = selfText.Replace ("<alpha=#44>", "<alpha=#55>");
            selfText = selfText.Replace ("<alpha=#22>", "<alpha=#44>");
            //selfText.Replace ("<alpha=#00>", "<alpha=#22>");
            return selfText;
            //yield return new WaitForEndOfFrame ();
        }
        return "";
    }
    public void ClearWriter () {
      //  Debug.Log ("Clearing writer");
        self_.text = "";
    }

}