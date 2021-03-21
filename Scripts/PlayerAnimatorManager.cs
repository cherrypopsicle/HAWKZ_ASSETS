using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Io.WeAreStudios.Hawkz
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private variables
        private Animator animator;
        private PhotonView photonView;
        [SerializeField]
        // This allows the character to dampen the rotation for a much smoother user experience
        private float directionDampTime = 0.25f;
        #endregion
        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is missing the animator component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            if (!animator)
            {
                return; 
            }

            // Deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // Only allow jumping if the state is currently in the Run of the Base Layer
            if (stateInfo.IsName("Base Layer.Run"))
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            animator.SetFloat("Speed", h * h + v * v);

            // Time.deltaTime is frame-rate independent inside the Update() callback. Essentially, we are telling 
            // the animator to set the Direction on the horizontal axis depending on where the user pressed, with dampening
            // time over deltaTime. Neat, eh? 
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion
    }

}