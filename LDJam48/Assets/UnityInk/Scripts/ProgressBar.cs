using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    public string barName = "Generic";
    public GameObject barItemPrefab;
    public Transform parentTransform;
    public Transform currentSelectionObject;
    public TextMeshProUGUI barNameObject;
    public int numberOfBars;
    public int startingValue = 0;
    public int currentValue = 0;
    public List<GameObject> barItems = new List<GameObject> { };

    private float lerp = 0f;
    // Start is called before the first frame update
    void Start () {
        SpawnBars ();
        ActivateAll (false);
        SetValue (startingValue);
        currentValue = startingValue;
        //UIManager.instance?.AddProgressBar (this);
        if (barName != "") {
            InkWriter.main.story.ObserveVariable ((barName+"_value"), (string varName, object newValue) => {
                EventListener (varName, (int) newValue);
            });
            if (barNameObject != null){
                barNameObject.text = (string) InkWriter.main.story.variablesState[(barName+"_name")];
            }
        }
    }
    void OnEnable () {
        SetValueToInkValue ();
    }
    void SpawnBars () { // spawn the bars as necessary, unless already placed
        if (numberOfBars > 0 && barItems.Count < numberOfBars) {
            for (int i = barItems.Count; i < numberOfBars; i++) {
                GameObject newBar = Instantiate (barItemPrefab, parentTransform);
                barItems.Add (newBar);
            }
        };
    }
    void ActivateAll (bool activate) {
        string trigger = activate ? "Activate" : "Deactivate";
        foreach (GameObject barItem in barItems) {
            //barItem.GetComponent<Animator> ().SetTrigger (trigger);
        }
    }
    public void SetValue (int value) {
        Debug.Log ("Set value of " + barName + " to " + value);
        value = Mathf.Clamp (value, 0, barItems.Count);
        if (value < barItems.Count && value >= 0) {
            currentSelectionObject?.SetParent (barItems[value].transform, true);
            lerp = 0f;
        }
        currentValue = value;
    }
    public void SetValueToInkValue () {
        int currentValue = (int) InkWriter.main.story.variablesState[(barName+"_value")];
        SetValue (currentValue);
    }
    public void EventListener (string tag, int valuechange) { // from the tag listener
        SetValue (valuechange);
    }

    // Update is called once per frame
    void Update () {
        if (currentSelectionObject.localPosition != Vector3.zero) {
            currentSelectionObject.localPosition = Vector3.Lerp (currentSelectionObject.localPosition, Vector3.zero, lerp);
            lerp += Time.deltaTime * 0.05f;
        }
    }
}