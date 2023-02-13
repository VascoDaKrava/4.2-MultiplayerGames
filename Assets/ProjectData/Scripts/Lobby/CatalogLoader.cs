using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;


public sealed class CatalogLoader : MonoBehaviour
{

    #region Fields

    [SerializeField] private LobbyUIView _ui;
    private const string CATALOG_VERSION = "NewCatalog";// Version/name as PlayFab web creation
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    #endregion


    #region UnityMethods

    private void Start()
    {
        var request = new GetCatalogItemsRequest();
        request.CatalogVersion = CATALOG_VERSION;

        PlayFabClientAPI.GetCatalogItems(
            request,
            OnGetCatalogSuccess,
            OnFailure);
    }

    #endregion


    #region Methods

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        _ui.FillCatalog(result.Catalog);
        HandleCatalog(result.Catalog);
        Debug.Log($"Catalog was loaded successfully!");
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Catalog item {item.ItemId} was added successfully!");
        }
    }

    #endregion

}
