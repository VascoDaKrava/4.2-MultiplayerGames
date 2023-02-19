using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public sealed class GameManager : MonoBehaviourPunCallbacks
{

    #region Fields

    private const string SCENE_LOBBY_NAME = "Lobby";
    private const string PLAYFAB_HEALTH_KEY = "HP";

    private PlayerManager_Demo _player;

    [SerializeField] private Button _exitButton;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField] private GameObject _playerPrefab;

    #endregion


    #region UnityMethods

    private void Start()
    {
        _player = PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0).GetComponent<PlayerManager_Demo>();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfo, error => Debug.LogError(error.GenerateErrorReport()));
    }

    #endregion


    #region PUN callbacks

    public override void OnEnable()
    {
        base.OnEnable();
        _exitButton.onClick.AddListener(OnExitButtonClickHandler);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _exitButton.onClick.RemoveAllListeners();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel(SCENE_LOBBY_NAME);
    }

    #endregion


    #region Playfab callbacks

    private void OnGetAccountInfo(GetAccountInfoResult result)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = result.AccountInfo.PlayFabId,
            Keys = null
        },
        OnGetUserDataPlayfab,
        error => Debug.Log(error.GenerateErrorReport())
        );
    }

    #endregion


    #region Methods

    public void LeaveRoom()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                    {PLAYFAB_HEALTH_KEY, _player.Health.ToString()}
                }
            },
            result => Debug.Log("Successfully updated user data"),
            error => Debug.Log(error.GenerateErrorReport())
            );
        }

        PhotonNetwork.LeaveRoom();
    }

    private void OnExitButtonClickHandler()
    {
        LeaveRoom();
    }

    private void OnGetUserDataPlayfab(GetUserDataResult userData)
    {
        Debug.Log("OnGetUserDataPlayfab");

        if (userData.Data.TryGetValue(PLAYFAB_HEALTH_KEY, out var data))
        {
            Debug.Log($"Get {PLAYFAB_HEALTH_KEY}: {data.Value}");
            if (float.TryParse(data.Value, out var health))
            {
                _player.Health = health;//SetHealth(health);
            }
            else
            {
                Debug.LogError("Cannot parse health");
            }
        }
        else
        {
            Debug.LogWarning("No health. Use default!");
        }
    }

    #endregion

}
