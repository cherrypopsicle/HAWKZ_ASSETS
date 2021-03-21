using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Io.WeAreStudios.Hawkz
{

    public class PlayerUIScript : MonoBehaviour
    {
        #region Private Fields
        [Tooltip("UI Text to display Player's name")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("UI Slider to display player's health")]
        [SerializeField]
        private Slider playerHealth;

        private PlayerManager target;

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup canvasGroup;
        Vector3 targetPosition;

        #endregion
        #region MonoBehaviour Callbacks
        void Awake()
        {
            // Not recommended as it's a slow operation
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
            canvasGroup = this.GetComponent<CanvasGroup>();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }

            if (playerHealth != null)
            {
                playerHealth.value = target.health;
            }
        }

        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera .. to avoid certain bugs bruv
            if (targetRenderer != null)
            {
                this.canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            // #Critical
            // Follow the Target GameObject on screen
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }
        #endregion
        #region Public Methods
        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            target = _target; 
            if (playerNameText != null)
            {
                // if PlayerName text is a component in PlayerUI, then set the text to the owner's nickname of PlayerManager
                playerNameText.text = target.photonView.Owner.NickName;
            }

            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
        }
        #endregion
    }
}