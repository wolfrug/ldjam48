using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVisualInventory : MonoBehaviour { // hook up to an action watcher and let it go wild
    public Transform parent;
    public InventoryController targetController;
    public List<GameObject> shelfObjects = new List<GameObject> { };
    private List<GameObject> pickedObjects = new List<GameObject> { };
    public bool pickRandomly = true;
    // Start is called before the first frame update
    IEnumerator Start () {
        yield return new WaitForSeconds (0.5f); // wait for inventories to initialize for sure
        for (int i = 0; i < parent.childCount; i++) {
            shelfObjects.Add (parent.GetChild (i).gameObject);
        }
        InitShelf ();
    }

    public void InitShelf () {
        if (targetController != null) {
            int itemCount = targetController.allItemBoxes.Count;
            if (shelfObjects.Count > itemCount) {
                while (shelfObjects.Count > itemCount) {
                    TakeInventoryObject ();
                }
            }
        }
    }

    public void TakeInventoryObject () {
        GameObject trg;
        if (shelfObjects.Count > 0) {
            if (pickRandomly) {
                trg = shelfObjects[Random.Range (0, shelfObjects.Count)];
            } else {
                trg = shelfObjects[0];
            }
            trg.SetActive (false);
            shelfObjects.Remove (trg);
            pickedObjects.Add (trg);
        }
    }
    public void AddInventoryObject () { // we add them back in the order they were took, looks neater that way
        if (pickedObjects.Count > 0) {
            pickedObjects[0].SetActive (true);
            shelfObjects.Add (pickedObjects[0]);
            pickedObjects.RemoveAt (0);
        }
    }

    // Update is called once per frame
    void Update () {

    }
}