    using System.Collections;
    using UnityEngine;

    public class DynamicSortingOrder : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer targetSpriteRenderer;
        public int sortingDifferenceToTarget = -1;
        public bool useUpdate = true;

        void Start () {
            if (spriteRenderer == null) { spriteRenderer = gameObject.GetComponent<SpriteRenderer> (); };
            if (targetSpriteRenderer == null) {
                spriteRenderer.sortingOrder = (int) Camera.main.WorldToScreenPoint (this.spriteRenderer.bounds.min).y * -1;
            } else {
                spriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + sortingDifferenceToTarget;
            }
        }

        void LateUpdate () {
            if (useUpdate) {
                if (targetSpriteRenderer == null) {
                    spriteRenderer.sortingOrder = (int) Camera.main.WorldToScreenPoint (this.spriteRenderer.bounds.min).y * -1;
                } else {
                    spriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder + sortingDifferenceToTarget;
                }
            };
        }

        [NaughtyAttributes.Button]
        void TrySetOrder () {
            spriteRenderer.sortingOrder = (int) Camera.main.WorldToScreenPoint (this.spriteRenderer.bounds.min).y * -1;
        }
    }