// Unity Engine imports
using UnityEngine;
using UnityEngine.UI;

// Photon imports
using Photon.Pun;
using Photon.Realtime;

// C# imports
using System.Collections;

namespace Io.WeAreStudios.Hawkz
{ 
    /// <summary>
    ///  Player name input field. Let the player input their name so that other players can see who's with them. 
    ///  It is a requirement for this script to incorporate the InputField component.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants
        // This const string is a `key` in the PlayerPrefs file. 
        const string playerNamePrefKey = "PlayerName";
        #endregion
        #region MonoBehaviourCallbacks
        /// <summary>
        /// Check if the Input Field component is found. If yes, check if the PlayerName Key is in the PlayerPrefs file. 
        /// If yes, declare the defaultName variable into the value of the PlayerName key, while setting the inputField.text
        /// into the defaultName.
        /// </summary>
        void Start()
        {
            string defaultName = string.Empty;
            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null) 
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }
            PhotonNetwork.NickName = defaultName;
        }


        // Update is called once per frame
        void Update()
        {

        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Sets the name of the player and saves it to both the PlayerPrefs and the PhotonNetwork for future sessions.
        /// </summary>
        /// <param name="name">The name of the player. </param>
        public void SetPlayerName(string name)
        {
            // #Important
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player name is null or empty!!");
                return;
            }
            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString(playerNamePrefKey, name);
        }
        #endregion
    }
}