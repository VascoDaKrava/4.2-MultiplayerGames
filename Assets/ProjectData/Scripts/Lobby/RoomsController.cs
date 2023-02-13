using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;


public class RoomsController : MonoBehaviourPunCallbacks
{

    #region OnlyIface

    //public class RoomsController : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
    //{

    //    [SerializeField] private ServerSettings _serverSettings;
    //    private LoadBalancingClient _client;

    //    private void Start()
    //    {
    //        _client = new LoadBalancingClient();
    //        _client.AddCallbackTarget(this);
    //        _client.ConnectUsingSettings(_serverSettings.AppSettings);
    //    }

    //    private void Update()
    //    {
    //        if (_client == null)
    //        {
    //            return;
    //        }

    //        _client.Service();

    //        Debug.Log(_client.State);
    //    }

    #endregion


    #region Fields

    private const int ROOM_NUMBER_MIN = 1000;
    private const int ROOM_NUMBER_MAX = 10000;
    private const int PLAYER_TTL = 10000;

    [SerializeField] private RoomsUIView _ui;

    [SerializeField] private string _userName;

    #endregion


    #region UnityMethods

    private void Start()
    {
        Connect();
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfo, error => Debug.LogError(error.GenerateErrorReport()));
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _ui.StartGameButton.onClick.AddListener(OnClickStartGameButtonHandler);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _ui.StartGameButton.onClick.RemoveAllListeners();
        Disconnect();
    }

    #endregion


    #region PUN Callbacks

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"Join to {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Max players {PhotonNetwork.CurrentRoom.MaxPlayers}");
        Debug.Log($"Is room open : {PhotonNetwork.CurrentRoom.IsOpen}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.LogError($"Join fail : {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogError($"Join fail : {message}");
    }

    #endregion


    #region Methods

    private void OnGetAccountInfo(GetAccountInfoResult result)
    {
        _userName = result.AccountInfo.Username;
    }

    private void OnClickStartGameButtonHandler()
    {
        PhotonNetwork.LocalPlayer.NickName = _userName;
        var roomName = _ui.RoomName.text.Equals(string.Empty) ? $"Room {Random.Range(ROOM_NUMBER_MIN, ROOM_NUMBER_MAX)}" : _ui.RoomName.text;
        var roomOptions = new RoomOptions { MaxPlayers = _ui.MaxPlayers, PlayerTtl = PLAYER_TTL, IsOpen = false };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: roomName, roomOptions: roomOptions);
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        Debug.Log("Connect to PUN");
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
        Debug.Log("Disconnect from PUN");
    }

    #endregion

}
