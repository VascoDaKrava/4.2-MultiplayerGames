using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using System;

public sealed class ServerAuthorization : MonoBehaviour
{

    #region Fields

    private const string AUTHORIZATION_KEY = "authorization-guid";

    [SerializeField] private UIView _ui;

    private string _userID;
    private bool _needCreateNewAccount;

    #endregion


    #region UnityMethods

    private void Start()
    {
        _needCreateNewAccount = PlayerPrefs.HasKey(AUTHORIZATION_KEY);
        Debug.Log($"Need create new - {_needCreateNewAccount}");
        _userID = PlayerPrefs.GetString(AUTHORIZATION_KEY, Guid.NewGuid().ToString());
        Debug.Log($"Current UserID : {_userID}");
        _ui.UserIDText.text = _userID;
    }

    private void OnEnable()
    {
        _ui.UserDataButton.onClick.AddListener(OnUserDataButtonClickHandler);
        _ui.LoginIDUserDataButton.onClick.AddListener(OnLoginIDUserDataButtonClickHandler);
        _ui.MainResetButton.onClick.AddListener(OnMainResetButtonClickHandler);
    }

    private void OnDisable()
    {
        _ui.UserDataButton.onClick.RemoveAllListeners();
        _ui.LoginIDUserDataButton.onClick.RemoveAllListeners();
        _ui.MainResetButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void OnMainResetButtonClickHandler()
    {
        Debug.Log($"Old ID : {_userID}");
        _userID = Guid.NewGuid().ToString();
        Debug.Log($"New ID : {_userID}");
    }

    private void OnLoginIDUserDataButtonClickHandler()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = _userID,
            CreateAccount = _needCreateNewAccount
        },
        success => {
            var message = success.NewlyCreated ? "Create and login succes" : "Login success";
            Debug.Log(message);
            _ui.LabelText.color = Color.green;
            _ui.LabelText.text = message;
            PlayerPrefs.SetString(AUTHORIZATION_KEY, _userID);
        },
        OnErrorHandler
        );
    }

    private void OnUserDataButtonClickHandler()
    {
        if (_ui.EmailField.IsActive())
        {
            CreateAccount();
        }
        else
        {
            SignIn();
        }
    }

    private void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _ui.LoginField.text,
            Password = _ui.PasswordField.text,
            Email = _ui.EmailField.text,
            RequireBothUsernameAndEmail = true
        },
        result =>
        {
            Debug.Log($"Success create : {_ui.LoginField.text}");
        },
        OnErrorHandler);
    }

    private void SignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _ui.LoginField.text,
            Password = _ui.PasswordField.text
        },

        result =>
        {
            Debug.Log($"Success login : {_ui.LoginField.text}");
        },

        OnErrorHandler);
    }



    private void OnErrorHandler(PlayFabError error)
    {
        _ui.LabelText.color = Color.red;
        _ui.LabelText.text = error.GenerateErrorReport();

        Debug.LogError($"Fail : {error.ErrorMessage}");

        if (error.ErrorDetails != null)
        {
            foreach (var details in error.ErrorDetails)
            {
                foreach (var item in details.Value)
                {
                    Debug.LogError(item);
                }
            }
        }
    }

    private void SaveAuthLocal()
    {
        PlayerPrefs.SetString(AUTHORIZATION_KEY, _userID);
    }

    #endregion

}
