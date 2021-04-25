using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class StartMove : UnityEvent<NavMeshAgent, Vector3> { }

[System.Serializable]
public class FinishMove : UnityEvent<NavMeshAgent, Vector3> { }

public class GenericClickToMove : MonoBehaviour {
    public Camera cam;
    public LayerMask layerMask;
    public LayerMask ignoreMask;
    public bool hitEveryOtherLayer = false;

    public float distance = 100;
    public float arrivedDistance = 0.1f;

    public GameObject clickToMoveObject;
    public GameObject clickToMoveFailedObject;

    [SerializeField]
    private bool active = true;
    [SerializeField]
    private float remainingDistance;
    public bool isMoving = false;
    public NavMeshAgent targetAgent;
    public StartMove startMoveEvent;
    public FinishMove finishMoveEvent;
    // Start is called before the first frame update
    void Start () {
        if (cam == null) {
            cam = Camera.main;
        }
    }

    void CastRay () {
        /* int targetLayer = 1 << layerMask;
         if (hitEveryOtherLayer) {
             targetLayer = ~layerMask;
         }*/
        var pointerEventData = new PointerEventData (EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult> ();
        EventSystem.current.RaycastAll (pointerEventData, raycastResults);

        if (raycastResults.Count > 0) {
           // Debug.LogWarning ("Hit UI, quitting.");
            return;
        }

        int ignoreThis = ignoreMask;
        RaycastHit hit2;

        if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit2, distance, ignoreThis)) {
            Debug.LogWarning ("Hit an ignore layer, quitting");
            return;
        }

        int targetLayer = layerMask;
        RaycastHit hit;

        if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit, distance, targetLayer)) {
            //Debug.Log ("Hit something!");
            targetAgent.SetDestination (hit.point);
            startMoveEvent.Invoke (targetAgent, hit.point);
            if (clickToMoveObject != null) {
                clickToMoveObject.GetComponent<Animator> ().SetBool ("Active", true);
                clickToMoveObject.SetActive (true);
                clickToMoveObject.transform.position = hit.point;
            }
            //Invoke ("LateStartMove", 0.1f);
            isMoving = true;
        } else {

            if (clickToMoveFailedObject != null) {
                clickToMoveFailedObject.GetComponent<Animator> ().SetBool ("Active", true);
                clickToMoveFailedObject.SetActive (true);
                clickToMoveFailedObject.transform.position = hit.point;
                //clickToMoveFailedObject.transform.position = new Vector3 (clickToMoveFailedObject.transform.position.x, clickToMoveFailedObject.transform.transform.position.y, 0f);
                Invoke ("DisableClickLocation", 0.5f);
            }
            //Debug.Log ("hit nothing!");
        }
    }

    void DisableClickLocation () {
        clickToMoveFailedObject.GetComponent<Animator> ().SetBool ("Active", false);
        clickToMoveFailedObject.SetActive (false);
    }

    void LateStartMove () {
        isMoving = true;
    }
    public void Activate (bool activate) {
        active = activate;
        targetAgent.isStopped = !activate;
        targetAgent.SetDestination (targetAgent.transform.position);
        isMoving = false;
        finishMoveEvent.Invoke (targetAgent, targetAgent.transform.position);
        clickToMoveObject.GetComponent<Animator> ().SetBool ("Active", false);
        //clickToMoveObject.SetActive (false);
    }

    // Update is called once per frame
    void Update () {
        if (active && GameManager.instance.GameState == GameStates.GAME) {
            if (Input.GetAxis ("Fire1") > 0f) {
                // Moving by clicking left
                CastRay ();
            }
            if (Input.GetButtonDown ("Fire2")) {
                // Interacting by clicking right
                //CastRay ();
            }
            if (isMoving) {
                if (targetAgent.remainingDistance < arrivedDistance) {
                    finishMoveEvent.Invoke (targetAgent, targetAgent.transform.position);
                    isMoving = false;
                    if (clickToMoveObject != null) {
                        clickToMoveObject.GetComponent<Animator> ().SetBool ("Active", false);
                        //clickToMoveObject.SetActive (false);
                    }
                }
            }
            remainingDistance = targetAgent.remainingDistance;
        };

    }
}