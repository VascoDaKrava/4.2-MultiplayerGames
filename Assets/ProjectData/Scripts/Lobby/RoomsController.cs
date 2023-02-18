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
    private const byte MAX_PLAYERS_MIN = 2;
    private const byte MAX_PLAYERS_MAX = 20;
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
        _ui.CreateOrJoinCustomRoomButton.onClick.AddListener(OnClickCreateCustomRoomButtonHandler);
        _ui.CreateOrJoinRandomRoomButton.onClick.AddListener(OnClickCreateRandomRoomButtonHandler);
        _ui.IsRoomLock.onValueChanged.AddListener(OnUIvalueChangeRoomLockHandler);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _ui.StartGameButton.onClick.RemoveAllListeners();
        _ui.CreateOrJoinCustomRoomButton.onClick.RemoveAllListeners();
        _ui.CreateOrJoinRandomRoomButton.onClick.RemoveAllListeners();
        _ui.IsRoomLock.onValueChanged.RemoveAllListeners();
        Disconnect();
    }

    #endregion


    #region PUN Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("OnJoinedLobby");
        _ui.IsInLobby = PhotonNetwork.InLobby;
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        Debug.Log("OnLeftLobby");
        _ui.IsInLobby = PhotonNetwork.InLobby;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        _ui.FillRooms(roomList);
        Debug.Log($"OnRoomListUpdate {roomList.Count}");
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        base.OnPlayerEnteredRoom(otherPlayer);
        Debug.LogFormat("<color=aqua>Joined NickName : {0}</color>", otherPlayer.NickName);
        Debug.LogFormat("<color=cyan>Full : {0}</color>", otherPlayer.ToStringFull());
        _ui.IsInLobby = PhotonNetwork.InLobby;
        _ui.UpdateCurrentRoomInfo(PhotonNetwork.CurrentRoom, PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"Join to {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Max players {PhotonNetwork.CurrentRoom.MaxPlayers}");
        Debug.Log($"Is room open : {PhotonNetwork.CurrentRoom.IsOpen}");
        _ui.IsInLobby = PhotonNetwork.InLobby;
        _ui.UpdateCurrentRoomInfo(PhotonNetwork.CurrentRoom, PhotonNetwork.CurrentRoom.PlayerCount);
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

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.LogFormat("<color=green>Call : {0}</color>", "OnCreatedRoom");
        _ui.IsInLobby = PhotonNetwork.InLobby;
        _ui.UpdateCurrentRoomInfo(PhotonNetwork.CurrentRoom, PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion


    #region Methods

    private void OnGetAccountInfo(GetAccountInfoResult result)
    {
        _userName = result.AccountInfo.Username;
    }

    private void OnClickStartGameButtonHandler()
    {
        if (!PhotonNetwork.InRoom)
        {
            OnClickCreateCustomRoomButtonHandler();
        }

        PhotonNetwork.CurrentRoom.IsOpen = false;
        _ui.UpdateCurrentRoomInfo(PhotonNetwork.CurrentRoom, PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.LogFormat("<color=yellow>Call : {0}</color>", "START GAME");
    }

    private void OnClickCreateCustomRoomButtonHandler()
    {
        ConfigureRoom(out var roomName, out var roomOptions);
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, PhotonNetwork.CurrentLobby);
    }

    private void OnClickCreateRandomRoomButtonHandler()
    {
        ConfigureRoom(out var roomName, out var roomOptions);
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: roomName, roomOptions: roomOptions);
    }

    private void ConfigureRoom(out string roomName, out RoomOptions roomOptions)
    {
        PhotonNetwork.LocalPlayer.NickName = _userName;
        roomName = _ui.RoomName.text.Equals(string.Empty) ? $"Room {Random.Range(ROOM_NUMBER_MIN, ROOM_NUMBER_MAX)}" : _ui.RoomName.text;
        byte maxPlayers;

        if (_ui.MaxPlayers < MAX_PLAYERS_MIN || _ui.MaxPlayers > MAX_PLAYERS_MAX)
        {
            maxPlayers = (byte)(Random.Range(MAX_PLAYERS_MIN, MAX_PLAYERS_MAX));
        }
        else
        {
            maxPlayers = _ui.MaxPlayers;
        }

        roomOptions = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = PLAYER_TTL, IsOpen = !_ui.IsRoomLock.isOn };
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

    private void OnUIvalueChangeRoomLockHandler(bool state)
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        PhotonNetwork.CurrentRoom.IsOpen = !state;
        _ui.UpdateCurrentRoomInfo(PhotonNetwork.CurrentRoom, PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion

}
