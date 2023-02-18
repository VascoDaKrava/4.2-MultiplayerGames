using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public sealed class GameManager : MonoBehaviourPunCallbacks
{

    #region Fields

    private const string SCENE_LOBBY_NAME = "Lobby";

    [SerializeField] private Button _exitButton;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField] private GameObject _playerPrefab;

    #endregion


    #region UnityMethods

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
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


    #region Methods

    private void OnExitButtonClickHandler()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

}
