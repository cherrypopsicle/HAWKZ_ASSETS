using UnityEngine;


namespace Io.WeAreStudios.Hawkz
{
    public class CameraWork : MonoBehaviour
    {
        #region Private Fields
        [Tooltip("The distance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 7.0f;

        [Tooltip("The total height the camera is above the target")]
        [SerializeField]
        private float height = 3.0f;

        [Tooltip("Allow the camera to be offseted vertically from the target, ")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab is being instantiated by Photon and manually call OnStartFollowing() when and if needed")]
        [SerializeField]
        private bool followOnStart = false;

        [Tooltip("The smoothing for the camera to follow the target")]
        [SerializeField]
        private float smoothSpeed = 0.125f;

        // cached transform of the target to be continuously updated
        Transform cameraTransform;

        // check if target is still being followed
        bool isFollowing;

        // Cache for camera offset
        Vector3 cameraOffset = Vector3.zero;
        #endregion
        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            // if true, start following the target upon initialization 
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }

        // called after all update funcitons have bene called.
        void LateUpdate()
        {
            // The transform target may  destroy on level load, so we cover cases where the MainCamera is different everytime we load a scene
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }

            // Only follow is explicitly declared
            if (isFollowing)
            {
                Follow();
            }

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// OnStartFollowing(), cache the cameraTransform so it bypasses first gate of LateUpdate. 
        /// We Cut() on the beginning so that there are no smooth transitions
        /// </summary>
        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            // We don't smooth anything, we go straight to the right camera shot 
            // TODO: play around with this and maybe smooth it out? 
            Cut();
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Smoothly follow the target
        /// </summary>
        void Follow()
        {
            // set the Camera offset z variable to the distance (-) and the y to height variable
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
            cameraTransform.LookAt(this.transform.position + centerOffset);
        }

        /// <summary>
        /// Same as Follow() but without the smooth Vector3.Lerp()
        /// </summary>
        void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);
            cameraTransform.LookAt(this.transform.position + centerOffset);
        }
        #endregion
    }
}