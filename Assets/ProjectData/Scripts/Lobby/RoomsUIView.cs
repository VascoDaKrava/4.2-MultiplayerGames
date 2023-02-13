using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class RoomsUIView : MonoBehaviour
{

    #region Fields

    [SerializeField] private Toggle _isRoomLock;
    [SerializeField] private TMP_InputField _maxPlayers;
    [SerializeField] private RoomTemplateView _roomTemplateView;

    #endregion


    #region Properties

    [field: SerializeField] public Button StartGameButton { get; private set; }
    [field: SerializeField] public TMP_InputField RoomName { get; private set; }
    public byte MaxPlayers { get => byte.TryParse(_maxPlayers.text, out byte result) ? result : default; }

    #endregion


    #region UnityMethods

    private void OnEnable()
    {
        _isRoomLock.isOn = false;
        StartGameButton.interactable = false;
        _isRoomLock.onValueChanged.AddListener(OnValueChangeLockToggleHandler);
    }

    private void OnDisable()
    {
        _isRoomLock.onValueChanged.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void OnValueChangeLockToggleHandler(bool state)
    {
        StartGameButton.interactable = state;
    }

    #endregion

}
