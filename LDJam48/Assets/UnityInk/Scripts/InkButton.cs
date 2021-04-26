using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class InkButtonClicked : UnityEvent<Choice, Button> { }

public class InkButton : MonoBehaviour // can be used instead of a button to create alternate onclick conditions
{
    public InkButtonClicked clickedEvent;
    public bool removeButtonsOnClick = true;
    public bool disableButtonsOnClick = false;

    private Choice currentChoice;
    private Button currentButton;
    private InkWriter currentInkWriter;

    public void onClicked (Choice choice, Button button, InkWriter targetWriter) { // Bypasses the regular ol' clickety click
        currentChoice = choice;
        currentButton = button;
        currentInkWriter = targetWriter;
        clickedEvent.Invoke (choice, button);
    }
    public void CompleteClickWithDelay (float delay) {
        Invoke ("CompleteClick", delay);
    }
    public void CompleteClick () {
        if (currentChoice != null && currentButton != null && currentInkWriter != null) {
            Debug.Log ("Completing click!", gameObject);
            currentInkWriter.OnClickChoiceButton (currentChoice, currentButton);
            currentChoice = null;
            currentButton = null;
            currentInkWriter = null;
        } else {
            Debug.LogWarning ("Null value detected for click. currentChoice: " + currentChoice + " currentButton: " + currentButton + " currentInkWriter: " + currentInkWriter, gameObject);
        }
    }
}