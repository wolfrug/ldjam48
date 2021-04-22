using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class AnimationSpot : MonoBehaviour {
    public GameObject editorWidget; // this is disabled in-game
    public string animation = "sitting";

    public Rigidbody animTargetObject;
    public float animationTime = 0.1f;
    public bool isBool = true;
    public bool cancelIfAgentMoves = true;
    public bool spotOccupied = true;
    public BasicAgent targetAgent;
    private Vector3 rotation;
    // Start is called before the first frame update
    void Start () {
        rotation = editorWidget.transform.rotation.eulerAngles;
        editorWidget.SetActive (false);
    }

    public void InteractableObjectActivate (Interactable_Object obj, BasicAgent agent) {
        DoAnimation (agent);
    }

    public void DoAnimation (BasicAgent agent) {
        // Move, rotate and animate the agent
        NavMeshAgent navMeshAgent = agent.navMeshAgent;
        Sequence animationSequence = DOTween.Sequence ();

        targetAgent = agent;
        if (animTargetObject != null) {
            animTargetObject.detectCollisions = false;
        };

        animationSequence.AppendCallback (() => agent.navMeshAgent.isStopped = true);
        animationSequence.Append (agent.transform.DOMove (transform.position, 0.5f));
        animationSequence.Append (agent.transform.DORotate (rotation, 0.5f));
        if (isBool) {
            animationSequence.AppendCallback (() => agent.animator.SetBool (animation, true));
        } else {
            animationSequence.AppendCallback (() => agent.animator.SetTrigger (animation));
        }
        animationSequence.AppendInterval (animationTime);
        animationSequence.AppendCallback (() => spotOccupied = true);
        animationSequence.AppendCallback (() => agent.navMeshAgent.isStopped = false);
    }
    public void StopAnimation (BasicAgent agent) {
        if (isBool) {
            agent.animator.SetBool (animation, false);
        }
        if (animTargetObject != null) {
            animTargetObject.detectCollisions = true;
        }
        targetAgent = null;
        spotOccupied = false;
    }

    // Update is called once per frame
    void Update () {
        if (isBool && cancelIfAgentMoves && targetAgent != null && spotOccupied) {
            if (targetAgent.navMeshAgent.remainingDistance > 2f) {
                StopAnimation (targetAgent);
            }
        }
    }
}