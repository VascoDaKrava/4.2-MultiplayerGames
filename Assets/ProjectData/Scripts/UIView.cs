using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class UIView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text LabelText { get; private set; }
    [field: SerializeField] public Button LoginButton { get; private set; }
    [field: SerializeField] public Button ConnectButton { get; private set; }
    [field: SerializeField] public Button DisconnectButton { get; private set; }
}
