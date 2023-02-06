using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


public sealed class Authorization : MonoBehaviourPunCallbacks
{

    #region Fields

    [SerializeField] private string _playFabTitle;
    [SerializeField] private UIView _ui;

    #endregion


    #region UnityMethods

    public override void OnEnable()
    {
        base.OnEnable();
        SubscribeUI();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UnSubscribeUI();
    }

    #endregion


    #region Methods

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;

        if (PhotonNetwork.IsConnected)
        {
            _ui.ConnectButton.interactable = false;
            _ui.DisconnectButton.interactable = true;
        }
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
        _ui.ConnectButton.interactable = true;
        _ui.DisconnectButton.interactable = false;
    }

    private void Login()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = "TestUser",
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID
        (
            request,
            result =>
            {
                Debug.Log(result.PlayFabId);
                PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
                PhotonNetwork.NickName = result.PlayFabId;
                SetPlayfabLoginStateForUI(true);
            },
            error =>
            {
                Debug.LogError(error);
                SetPlayfabLoginStateForUI(false);
            }
        );
    }

    private void SetPlayfabLoginStateForUI(bool state)
    {
        if (state)
        {
            _ui.LabelText.text = "Login success";
            _ui.LabelText.color = Color.green;
        }
        else
        {
            _ui.LabelText.text = "Login fail";
            _ui.LabelText.color = Color.red;
        }
    }

    private void SubscribeUI()
    {
        _ui.LoginButton.onClick.AddListener(Login);
        _ui.ConnectButton.onClick.AddListener(Connect);
        _ui.DisconnectButton.onClick.AddListener(Disconnect);
    }

    private void UnSubscribeUI()
    {
        _ui.LoginButton.onClick.RemoveAllListeners();
        _ui.ConnectButton.onClick.RemoveAllListeners();
        _ui.DisconnectButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region PUN

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("OnDisconnected");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");

        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }

    #endregion

}
