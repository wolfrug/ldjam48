using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericWorldSpaceToCanvasIcon : MonoBehaviour {
    public GameObject Target
    {
        get
        {
            return m_target;
        }
        set
        {
            m_target = value;
            useRawPosition = false;
        }
    }
    public RectTransform canvasObject;
    public bool useRawPosition = false;
    public Vector3 TargetPosition
    {
        get
        {
            if (useRawPosition)
            {
                return m_targetPosition;
            }

            try
            {
                return Target.transform.position;
            }
            catch (NullReferenceException e)
            {
                return Vector3.zero;
            }
        }
        set
        {
            m_targetPosition = value;
            useRawPosition = true;
        }
    }

    public bool scaleByDistanceFromCamera = true;
    [SerializeField]
    private float distanceForScaleOne = 200f;

    [SerializeField]
    private Camera targetCamera;
    private Vector3 pos;
    private Vector3 m_targetPosition;
    private GameObject m_target;

    void Awake () {
        if (!useRawPosition && Target == null) {
            Target = gameObject;
        }
        if (targetCamera == null) {
            targetCamera = Camera.main;
        };
    }

    // Update is called once per frame
    void Update () {
        if (canvasObject == null) {
            return;
        }

        if (!useRawPosition && Target == null) {
            canvasObject.gameObject.SetActive(false);
        }

        pos = RectTransformUtility.WorldToScreenPoint(targetCamera, TargetPosition);
        canvasObject.position = pos;
        if (scaleByDistanceFromCamera) {
            float distance = Vector3.Distance(targetCamera.transform.position, TargetPosition);
            float newscale = Mathf.Clamp(distanceForScaleOne / distance, 0.2f, 1f);
            // Debug.Log ("Distance between camera and " + gameObject.name + " :" + distance.ToString ());
            canvasObject.localScale = new Vector3(newscale, newscale, newscale);
        }

    }
}