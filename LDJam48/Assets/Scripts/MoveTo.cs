// MoveTo.cs
using System.Collections;
using UnityEngine;

public class MoveTo : MonoBehaviour {

   public Transform goal;
   private UnityEngine.AI.NavMeshAgent agent;
   void Start () {
      agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
   }

   public void SetTarget (Transform target) {
      goal = target;
   }
   void Update () {

      agent.destination = goal.position;
   }
}