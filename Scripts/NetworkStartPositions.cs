using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class NetworkStartPosition
{
    // should be the OVRCameraRig!
    public Transform startPosition;
    public string occupiedPlayerId;

    public NetworkStartPosition(Transform _startPosition)
    {
        startPosition = _startPosition;
        occupiedPlayerId = "";
    }
}
public class NetworkStartPositions : MonoBehaviourPunCallbacks
{
    // All the start positions for the player (there are only 2)
    [SerializeField] private List<Transform> startPositions;
    // The player controller
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject networkPlayer;
    private GameObject spawnedPlayerPrefab;
    private bool placeAssOnSeat = false;
    [SerializeField] private List<NetworkStartPosition> networkStartPositions;

    private void Awake()
    {
        Initialize();
    }

    // Initialize the network start positions based on start positions list
    public void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("OVRPlayerController");
        playerController = GameObject.FindGameObjectWithTag("PlayerController");
        startPositions[0] = GameObject.FindGameObjectWithTag("DriverSeat").transform;
        startPositions[1] = GameObject.FindGameObjectWithTag("ShooterSeat").transform;
        networkStartPositions = new List<NetworkStartPosition>();
        foreach (var item in startPositions)
        {
            networkStartPositions.Add(new NetworkStartPosition(item));
        }
    }

    public override void OnJoinedRoom()
    {
        SetPlayerToStartPosition();

    }

    public void SetPlayerToStartPosition()
    {
        int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        int availablePositionIndex = (id == -1) ? 0 : id % networkStartPositions.Count;
        TakePosition(availablePositionIndex);
        player.transform.position = networkStartPositions[availablePositionIndex].startPosition.position;
        player.transform.rotation = networkStartPositions[availablePositionIndex].startPosition.rotation;
        AssignRole(availablePositionIndex);
    }

    //void ParentPlayer(int startPositionId)
    //{
    //    photonObject.TransferOwnership(PhotonNetwork.LocalPlayer);
    //    photonView.RPC("ParentPlayerRPC", RpcTarget.AllBuffered, startPositionId);
    //}

    [PunRPC]
    void ParentPlayerRPC(int startPositionId)
    {
        Debug.Log("About to parent");
        var photonObject = PhotonView.Find(4).gameObject.transform;
        spawnedPlayerPrefab.transform.SetParent(photonObject, false);
        spawnedPlayerPrefab.transform.localPosition = Vector3.zero;
        Debug.Log("Parented successfully");
    }

    void TakePosition(int index)
    {
        // Transfer ownership to current player ..
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        // make a remote procedure call to tell all current players who took ownership of availablePositionIndex
        photonView.RPC("TakePositionRPC", RpcTarget.AllBuffered, index, PhotonNetwork.LocalPlayer.UserId);
    }

    [PunRPC]
    void TakePositionRPC(int index, string userId)
    {
        networkStartPositions[index].occupiedPlayerId = userId;
    }

    // If one player leaves the room ... then remove the index
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int removeIndex = networkStartPositions.FindIndex(x => x.occupiedPlayerId == otherPlayer.UserId);
        if (removeIndex >= 0)
        {
            networkStartPositions[removeIndex].occupiedPlayerId = "";
        }
    }

    // First player that joins automatically is both the driver and the shooter. Second player that joins is the shooter.
    void AssignRole(int index)
    {
        var playerScript = playerController.GetComponent<PlayerControllerOculus>();
        var both = PlayerControllerOculus.PlayerState.Both;
        var shooter = PlayerControllerOculus.PlayerState.Shooter;
        switch (index)
        {
            case 0:
                playerScript.ChangeState(both);
                return;
            case 1:
                playerScript.ChangeState(shooter);
                return;
            default:
                return;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}