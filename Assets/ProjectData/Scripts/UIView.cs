using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class UIView : MonoBehaviour
{

    #region Fields

    private const string LABEL_MAIN = "Please make your choose";
    private const string LABEL_LOGIN = "Login now";
    private const string LABEL_REGISTER = "Create account";
    private const string LABEL_LESSON_3 = "Please log in";

    private const string LOGIN_BUTTON_TEXT = "LOGIN";
    private const string REGISTER_BUTTON_TEXT = "REGISTER";

    private List<RectTransform> _containers = new List<RectTransform>();

    [SerializeField] private RectTransform _buttonsLesson3Container;
    [SerializeField] private RectTransform _mainButtonsContainer;
    [SerializeField] private RectTransform _userDataContainer;

    [SerializeField] private Button _backUserDataButton;
    [SerializeField] private Button _backFromLesson3Button;

    [SerializeField] private Button _mainLoginButton;
    [SerializeField] private Button _mainRegisterButton;
    [SerializeField] private Button _mainLesson3Button;

    [SerializeField] private TMP_Text _userDataButtonLabel;

    [SerializeField] public TMP_Text UserIDText;

    #endregion


    #region Properties

    #region Lesson3 (SDK) data

    [field: SerializeField] public Button LoginButton { get; private set; }
    [field: SerializeField] public Button ConnectButton { get; private set; }
    [field: SerializeField] public Button DisconnectButton { get; private set; }

    #endregion

    [field: SerializeField] public TMP_Text LabelText { get; private set; }

    [field: SerializeField] public TMP_InputField LoginField { get; private set; }
    [field: SerializeField] public TMP_InputField PasswordField { get; private set; }
    [field: SerializeField] public TMP_InputField EmailField { get; private set; }

    [field: SerializeField] public Button UserDataButton { get; private set; }
    [field: SerializeField] public Button LoginIDUserDataButton { get; private set; }
    [field: SerializeField] public Button MainResetButton { get; private set; }

    #endregion


    #region UnityMethods

    private void Start()
    {
        _containers.Add(_mainButtonsContainer);
        _containers.Add(_buttonsLesson3Container);
        _containers.Add(_userDataContainer);

        ShowMainMenu();
    }

    private void OnEnable()
    {
        _mainLoginButton.onClick.AddListener(ShowLoginMenu);
        _mainRegisterButton.onClick.AddListener(ShowRegisterMenu);
        _mainLesson3Button.onClick.AddListener(ShowLesson3Menu);
        _backUserDataButton.onClick.AddListener(ShowMainMenu);
        _backFromLesson3Button.onClick.AddListener(ShowMainMenu);
    }

    private void OnDisable()
    {
        _mainLoginButton.onClick.RemoveAllListeners();
        _mainRegisterButton.onClick.RemoveAllListeners();
        _mainLesson3Button.onClick.RemoveAllListeners();
        _backUserDataButton.onClick.RemoveAllListeners();
        _backFromLesson3Button.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void HideAll()
    {
        foreach (var item in _containers)
        {
            item.gameObject.SetActive(false);
        }
    }

    private void ShowLoginMenu()
    {
        HideAll();
        _userDataContainer.gameObject.SetActive(true);
        EmailField.gameObject.SetActive(false);
        LoginIDUserDataButton.gameObject.SetActive(true);
        _userDataButtonLabel.text = LOGIN_BUTTON_TEXT;
        LabelText.text = LABEL_LOGIN;
    }

    private void ShowRegisterMenu()
    {
        HideAll();
        _userDataContainer.gameObject.SetActive(true);
        LoginIDUserDataButton.gameObject.SetActive(false);
        EmailField.gameObject.SetActive(true);
        _userDataButtonLabel.text = REGISTER_BUTTON_TEXT;
        LabelText.text = LABEL_REGISTER;
    }

    private void ShowLesson3Menu()
    {
        HideAll();
        _buttonsLesson3Container.gameObject.SetActive(true);
        LabelText.text = LABEL_LESSON_3;
    }

    private void ShowMainMenu()
    {
        HideAll();
        _mainButtonsContainer.gameObject.SetActive(true);
        LabelText.text = LABEL_MAIN;
        LabelText.color = Color.white;
    }

    #endregion

}
