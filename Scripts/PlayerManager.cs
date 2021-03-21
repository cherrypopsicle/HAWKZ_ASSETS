// UnityEngine imports
using UnityEngine;
using UnityEngine.EventSystems;

// Photon imports
using Photon.Pun;

// C# imports
using System.Collections;

namespace Io.WeAreStudios.Hawkz
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // we can only write when photonView == true
            if (stream.IsWriting)
            {
                // this is our player... send everyone else our data of isFiring .. even if it's false
                stream.SendNext(isFiring);
                // We also send our health data for everyone to read. 
                stream.SendNext(health);
            }
            else
            {
                // this is the network player, receive data.
                this.isFiring = (bool)stream.ReceiveNext();
                this.health = (float)stream.ReceiveNext();
            }
        }
        #endregion
        #region Public Fields
        [Tooltip("The current Health of our Player")]
        public float health = 1.0f;

        [Tooltip("The local playter instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUIPrefab;
        #endregion
        #region Private Fields
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;
        private PhotonView photonView;
        // check if firing
        bool isFiring;
        #endregion
        #region Private Methods
#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif
        #endregion
        #region MonoBehaviour Callbacks

        void Awake()
        {
            photonView = GetComponent<PhotonView>();
            // keep track of the player instance to prevent further instantiation when the levels are syncrhonized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // The instantiated player gets flagged with a DontDestoyOnLoad() so that it seamlessly traverses 
            // through multiple levels as other rooms get loaded
            DontDestroyOnLoad(this.gameObject);
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a> Missing </a> </Color> Beams Reference.");
            }
            else
            {
                beams.SetActive(false);
            }
        }
        void Start()
        {
            if (PlayerUIPrefab != null)
            {
                GameObject uiStart = Instantiate(PlayerUIPrefab);
                // Might be wrong, double check.
                uiStart.GetComponent<PlayerUIScript>().SetTarget(this);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
            // if camera work exists as part of  this gameObject's component and it's the player's view (photonView.isMine), then start camera work
            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }
            #if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
        }
        void Update()
        {
            if (photonView.IsMine)
            {
                // dope function that runs on loop and always listening to player inputs
                this.ProcessInputs();
                if (health <= 0.0f)
                {
                    // Since we assigned GameManager as a singleton, we can now access it from anywhere. We call the method LeaveRoom() when our 
                    // health hits zero. 
                    GameManager.Instance.LeaveRoom();
                }

                // if beams exist and isFiring is true then it is firing.
                if (beams != null && isFiring != beams.activeInHierarchy)
                {
                    beams.SetActive(isFiring);
                }
            }

        }
        /// <summary>
        /// If our photonView is ours and we are triggered by a collider of type beam, then kindly remove -0.01f from our health.
        /// </summary>
        /// <param name="other"> Collider parameter </param>
        void OnTriggerEnter(Collider other)
        {
            // is this view mine? Am I in control? If not, leave and check for the next client.
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }

            else
            {
                health -= 0.1f;
            }
        }
        /// <summary>
        /// If our photon view is ours and we are colliding with a beam for an extended period of time, then keep removing -0.1f from our health 
        /// for every independent frame
        /// </summary>
        /// <param name="other"> Collider parameter </param>
        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }
            else
            {
                health -= 0.1f * Time.deltaTime;
            }
        }
        #if !UNITY_5_4_OR_NEWER
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
        #endif

        void CalledOnLevelWasLoaded(int level)
        {
            // Check if we are outside the arena. If yes, spawn in the middle up
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0.0f, 5.0f, 0.0f);
            }

            GameObject uiGo = Instantiate(this.PlayerUIPrefab);
            uiGo.GetComponent<PlayerUIScript>().SetTarget(this);
        }

        public override void OnDisable()
        {
            // Call the base to remove further callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion
        #region Custom

        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (isFiring)
                {
                    isFiring = false;
                }
            }
        }
        #endregion
    }
}