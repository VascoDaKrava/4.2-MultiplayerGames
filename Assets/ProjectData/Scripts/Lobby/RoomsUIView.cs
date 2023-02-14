using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class RoomsUIView : MonoBehaviour
{

    #region Fields

    [SerializeField] private TMP_InputField _maxPlayers;
    [SerializeField] private RoomTemplateView _roomTemplateView;
    [SerializeField] private RectTransform _roomsContainer;

    [SerializeField] private RectTransform _currentRoomContainer;
    [SerializeField] private TMP_Text _currentRoomName;
    [SerializeField] private TMP_Text _currentRoomLock;
    [SerializeField] private TMP_Text _currentRoomMembers;

    private List<RoomTemplateView> _rooms = new List<RoomTemplateView>();

    #endregion


    #region Properties

    [field: SerializeField] public Toggle IsRoomLock { get; private set; }
    [field: SerializeField] public Button CreateOrJoinCustomRoomButton { get; private set; }
    [field: SerializeField] public Button CreateOrJoinRandomRoomButton { get; private set; }
    [field: SerializeField] public Button StartGameButton { get; private set; }
    [field: SerializeField] public TMP_InputField RoomName { get; private set; }
    public byte MaxPlayers { get => byte.TryParse(_maxPlayers.text, out byte result) ? result : default; }
    public bool IsInLobby
    {
        set
        {
            _currentRoomContainer.gameObject.SetActive(!value);
        }
    }

    #endregion


    #region UnityMethods

    private void OnEnable()
    {
        IsRoomLock.isOn = false;
        StartGameButton.interactable = false;
        IsRoomLock.onValueChanged.AddListener(OnValueChangeLockToggleHandler);
    }

    private void OnDisable()
    {
        IsRoomLock.onValueChanged.RemoveAllListeners();
    }

    #endregion


    #region Methods

    public void UpdateCurrentRoomInfo(RoomInfo room)
    {
        _currentRoomName.text = $"Room name : {room.Name}";
        _currentRoomLock.text = $"Room {(room.IsOpen ? "opened" : "closed")}.";
        _currentRoomMembers.text = $"Players : {room.PlayerCount} / {room.MaxPlayers}";
        Debug.Log($"Players : {room.PlayerCount} / {room.MaxPlayers}");
    }

    public void FillRooms(List<RoomInfo> roomListNew)
    {
        Debug.Log("Call FillRooms");

        foreach (var room in _rooms)
        {
            Destroy(room.gameObject);
        }

        _rooms.Clear();

        foreach (var room in roomListNew)
        {
            var newRoom = Instantiate(_roomTemplateView, _roomsContainer);
            newRoom.RoomName.text = room.Name;
            newRoom.gameObject.SetActive(true);
            _rooms.Add(newRoom);
        }
    }

    private void OnValueChangeLockToggleHandler(bool state)
    {
        StartGameButton.interactable = state;
    }

    #endregion

}
