// C# imports
using System.Collections;
using System.Collections.Generic;

// Unity imports
using UnityEngine;
using UnityEngine.SceneManagement;

// Photon imports
using Photon.Pun;
using Photon.Realtime;


namespace Io.WeAreStudios.Hawkz
{

    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Photon Callbacks
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Not seen if you are the player connecting.
            Debug.LogFormat("{0} entered the room!", newPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("You are the master client! Loading the appropriate arena ...");
                LoadArena(); 
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("{0} left the room!", otherPlayer.NickName);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("You are the master client! Loading the appropriate arena");
                LoadArena();
            }
        }
        /// <summary>
        /// When player leaves Photon room, we override the PunCallback to also tell the Unity Scenemanager to LoadScene(0),
        /// which is the launcher
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom() loading Launcher scene");
            SceneManager.LoadScene(0);
            // do other stuff
        }
        #endregion
        #region Public Fields
        // Can now be accessible anywhere as it's own singleton.
        public static GameManager Instance;
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        #endregion
        #region MonoBehaviourCallbacks
        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                // Instantiate a new player only when the localPlayerInstance is null
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().name);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0.5f, 5.0f, 0.0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            // When GameManager loads, we assign Instance to itself. 
            Instance = this;
        }
        #endregion
        #region Public Methods
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            // do other stuff
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Load the appropriate arena based on the player count. 
        /// </summary>
        public void LoadArena()
        {
            // Guard to check if we are the Master Client or not. 
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Photon Network: Trying to load a level but can't for reason that we are not the Master Client.");
            }
            Debug.LogFormat("PhotonNetwork: Loading Level: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            // We only call LoadLevel if we are the Master Client. We also only rely on Photon to make this call in order to sync all the players
            // together. That's why we enabled PhotonNetwork.AutomaticallySyncScene = true in the Launcher.cs
            PhotonNetwork.LoadLevel("LevelOneRoomFor" + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        #endregion
    }
}