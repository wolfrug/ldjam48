    using System.Collections;
    using UnityEngine;

    public class DynamicSortingOrder : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
        public bool useUpdate = true;

        void Start () {
            if (spriteRenderer == null) { spriteRenderer = gameObject.GetComponent<SpriteRenderer> (); };
            spriteRenderer.sortingOrder = (int) Camera.main.WorldToScreenPoint (this.spriteRenderer.bounds.min).y * -1;
        }

        void LateUpdate () {
            if (useUpdate) {
                spriteRenderer.sortingOrder = (int) Camera.main.WorldToScreenPoint (this.spriteRenderer.bounds.min).y * -1;
            };
        }
    }