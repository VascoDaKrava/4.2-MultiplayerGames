using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public sealed class LobbyUIView : MonoBehaviour
{

    #region Fields

    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] public RectTransform _catalogContainer;
    [SerializeField] private CatalogItemView _catalogItemTemplate;

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

    public void FillCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            var newItem = Instantiate(_catalogItemTemplate, _catalogContainer);

            newItem.Name.text = item.DisplayName;
            newItem.Description.text = item.Description;

            var price = item.VirtualCurrencyPrices.GetEnumerator();
            price.MoveNext();

            newItem.Price.text = $"{price.Current.Value} {price.Current.Key}";

            if (item.ItemImageUrl != null)
            {
                StartCoroutine(DownloadImage(item.ItemImageUrl, newItem.Icon));
            }

            newItem.gameObject.SetActive(true);
        }
    }

    private IEnumerator DownloadImage(string MediaUrl, Image image)
    {
        var sprite = image.sprite;

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogWarning($"Load texture fail : {request.error}");
            Debug.LogWarning($"Using native icon");
        }
        else
        {
            var texture = DownloadHandlerTexture.GetContent(request);
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), sprite.pivot, sprite.pixelsPerUnit, 1, SpriteMeshType.FullRect, sprite.border);
        }
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        _titleLabel.text =
            $"Welcome back!\n\n" +
            $"PlayFabId : {result.AccountInfo.PlayFabId}\n\n" +
            $"Username : {result.AccountInfo.Username}\n\n" +
            $"CustomId : {result.AccountInfo.CustomIdInfo?.CustomId}\n\n" +
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