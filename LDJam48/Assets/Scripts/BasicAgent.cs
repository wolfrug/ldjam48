using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class BasicAgent : MonoBehaviour {
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public float speed = 0f;
    public bool carrying = false;
    public bool sitting = false;
    public bool interacting = false;
    public Transform carryParent;

    public Interactable_Object carriedObject;

    private Coroutine autoAction;
    // Start is called before the first frame update
    void Start () {
        if (animator == null) {
            animator = GetComponent<Animator> ();
        }
    }

    public void StartWalk () {
        animator.SetFloat ("speed", 1f);
        animator.SetBool ("flipped", navMeshAgent.velocity.x < 0f); // if velocity x is < 0, flip the animation (for 2D stuff)
    }
    public void StopWalk () {
        animator.SetFloat ("speed", 0f);
    }

    public void ActivateAction (string animation, float duration, bool dropCarry = true) {
        navMeshAgent.isStopped = true;
        navMeshAgent.SetDestination (navMeshAgent.transform.position);
        Sequence pickUpSequence = DOTween.Sequence ();
        pickUpSequence.AppendInterval (duration);
        pickUpSequence.AppendCallback (() => navMeshAgent.isStopped = false);
        if (animation != "") {
            animator.SetTrigger (animation);
        }
    }

    public void CancelAutoTask () {
        if (autoAction != null) {
            StopCoroutine (autoAction);
        };
    }
    public void StartAutoTask (Interactable_Object instigator, ContextMenuEntryType action, bool dropCarried = true) {
        //     Debug.Log ("Starting auto-task walk");
        if (carrying && dropCarried) {
            carriedObject.Interact (this, Interactions.DROP);
        }
        navMeshAgent.SetDestination (instigator.transform.position);
        CancelAutoTask ();
        autoAction = StartCoroutine (WalkWatching (instigator.transform, instigator.activateDistance, new System.Action (() => instigator.ContextMenuAction (action, instigator.gameObject))));
    }
    public IEnumerator WalkWatching (Transform target, float activateDistance, System.Action callback) {

        Debug.Log ("Distance to target: " + Vector3.Distance (navMeshAgent.destination, target.position));
        while (Vector3.Distance (transform.position, target.position) > activateDistance) {
            //            Debug.Log ("Waiting to reach target, remaining distance: " + Vector3.Distance (transform.position, target.position));
            yield return new WaitForSeconds (0.1f);
            if (!navMeshAgent.hasPath) {
                Debug.Log ("Navmesh agent has no path, cancelling");
                break;
            }
        }
        //       Debug.Log ("Reached destination or cancelled");
        if (Vector3.Distance (target.position, transform.position) <= activateDistance) {
            //           Debug.Log ("Attempting pick-up!");
            callback.Invoke ();
        } else {
            Debug.Log ("Failed to invoke carry walk due to distance (" + Vector3.Distance (target.position, transform.position) + ")");
        }
        autoAction = null;
    }

    public bool StartCarry (float animationTime) {
        if (!carrying) {
            navMeshAgent.isStopped = true;
            navMeshAgent.SetDestination (navMeshAgent.transform.position);
            Sequence pickUpSequence = DOTween.Sequence ();
            pickUpSequence.AppendInterval (animationTime);
            pickUpSequence.AppendCallback (() => navMeshAgent.isStopped = false);
            pickUpSequence.AppendCallback (() => carrying = true);
            animator.SetBool ("carrying", true);

            return true;
        } else {
            return false;
        }
    }
    public bool StopCarry (float animationTime) {
        if (carrying) {
            navMeshAgent.isStopped = true;
            navMeshAgent.SetDestination (navMeshAgent.transform.position);
            Sequence pickUpSequence = DOTween.Sequence ();
            pickUpSequence.AppendInterval (animationTime);
            pickUpSequence.AppendCallback (() => navMeshAgent.isStopped = false);
            pickUpSequence.AppendCallback (() => carrying = false);
            animator.SetBool ("carrying", false);
            return true;
        } else {
            return false;
        }
    }

    public void SetCarriedObject (Interactable_Object target) {
        if (target != null) {
            TurnTowardsObject (target.self.transform);
        };
        carriedObject = target;
    }
    public void InteractWithObject (Interactable_Object target, ContextMenuEntryType interaction) {
        if (target != null) {
            TurnTowardsObject (target.self.transform);
        };
        target.ContextMenuAction (interaction, target.gameObject);
    }

    public void Interact (bool interact) {
        if (interact) {
            StopCarry (0.1f);
            CancelAutoTask ();
            navMeshAgent.SetDestination (navMeshAgent.transform.position);
            if (this == GameManager.instance.Player) {
                //GameManager.instance.StopPlayerMovement (true);
            };
        } else {
            if (this == GameManager.instance.Player) {
                //GameManager.instance.StopPlayerMovement (false);
            };
        }
        animator.SetBool ("interacting", interact);
        interacting = interact;
    }

    void TurnTowardsObject (Transform target) {
        Debug.Log ("Trying to turn");
        Vector3 endPoint = Quaternion.LookRotation (target.position - transform.position).eulerAngles;
        transform.DORotate (endPoint, .4f);
    }

    // Update is called once per frame
    void Update () {
        if (navMeshAgent.velocity.magnitude > 0.1f) {
            StartWalk ();
        } else {
            StopWalk ();
        }
    }
}