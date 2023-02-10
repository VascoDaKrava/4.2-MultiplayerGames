using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;


public sealed class LobbyUIView : MonoBehaviour
{

    #region Fields

    [SerializeField] private TMP_Text _titleLabel;

    #endregion


    #region UnityMethods

    private void Start()
    {
        try
        {
            PlayFabClientAPI.GetAccountInfo(
            new GetAccountInfoRequest(),
            OnGetAccountSuccess,
            OnFailure);
        }
        catch (System.Exception error)
        {
            ShowError($"{error.Message}");
        }
    }

    #endregion


    #region Methods

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        _titleLabel.text =
            $"Welcome back!\n\n" +
            $"PlayFabId : {result.AccountInfo.PlayFabId}\n\n" +
            $"Username : {result.AccountInfo.Username}\n\n" +
            $"CustomIdInfo : {result.AccountInfo.CustomIdInfo}\n\n" +
            $"";
    }

    private void OnFailure(PlayFabError error)
    {
        ShowError($"Something went wrong: {error.GenerateErrorReport()}");
    }

    private void ShowError(string errorMessage)
    {
        _titleLabel.color = Color.red;
        _titleLabel.text = errorMessage;
        Debug.LogError($"{errorMessage}");
    }

    #endregion

}