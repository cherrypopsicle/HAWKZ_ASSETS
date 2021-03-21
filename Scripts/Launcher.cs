using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
namespace Io.WeAreStudios.Hawkz
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [Tooltip("The maximum number of players allowed in a room. We can have a maximum of  3 players for Hawkz: one driver, one shooter, and one camera drone.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 3;
        [Tooltip("The UI panel to let the user enter their name, connect and play!")]
        [SerializeField] private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress.")]
        [SerializeField] private GameObject connectionPanel;
        #endregion
        #region Private Fields
        // The game version that the players will log into (along with the same region and appId)
        private string gameVersion = "1";
        // This keeps track of our Async function call back by Photon. Will only be used for the OnConnectedToMaster() Callback
        bool isConnecting;
        #endregion
        /// <summary>
        /// 
        /// All the private callbacks
        /// </summary>
        #region Mono Behaviour Callbacks
        // Awake() is called right before Start()
        void Awake()
        {
            // #Critical: this makes sure that we can use the PhotonNetwork.LoadLevel() on both the Master clients and all the clients in the same room to sync their levels autoamtically 
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {
            controlPanel.SetActive(true);
            connectionPanel.SetActive(false);
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// First, we check if client is connected on the Photon Network. 
        /// - If true: Join a random room()
        /// - If false: try reconnecting using server settings and set the Global game version to the local game version
        /// </summary>
        public void Connect()
        {
            controlPanel.SetActive(false);
            connectionPanel.SetActive(true);
            if (PhotonNetwork.IsConnected)
            {
                // Join random room if connected
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // Try connecting to Photon Online Server if not connected.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        #endregion
        #region MonoBehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                Debug.Log("Hawkz: OnConnectedToMaster() was called by PUN");
                // #Critical: Try joining a room after connecting to Master. If it works, awesome! Else, we call back the method OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            controlPanel.SetActive(true);
            connectionPanel.SetActive(false);
            isConnecting = false;
            // Let the developer know that the player got disconnected.
            Debug.LogWarningFormat("Hawkz: OnDisconnected() was called by PUN for reason: {0}", cause);
        }

        public override void OnJoinedRoom()
        {
            // Let the developer know that we have joined the room!
            Debug.Log("Hawkz: OnJoinedRoom() called by PUN. The player is now in the room. Hooray!");
            PhotonNetwork.LoadLevel("LevelOneRoomForOne");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Hawkz: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            // #Critical: since we failed to join a random room, we try creating a new one.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        #endregion
    }

}
