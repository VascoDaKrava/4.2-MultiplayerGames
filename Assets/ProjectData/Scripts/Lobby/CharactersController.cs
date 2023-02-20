using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;


public sealed class CharactersController : MonoBehaviour
{

    #region Fields

    private const string CATALOG_VERSION = "NewCatalog";// Version/name as PlayFab web creation. Same as CatalogLoader.cs
    private const string NEW_CHARACTER_ITEM_ID = "Master";//item ID for purchase
    private const string VIRTUAL_CURRENCY = "CO";// use this currency for purchase

    //private string itemId;

    [SerializeField] private CharactersUIView _view;
    [SerializeField] private int _maxCharacters = 2;

    private List<CharacterData> _characters = new List<CharacterData>();
    private CharacterData _currentCharacter = new CharacterData();

    #endregion


    #region UnityMethods

    private void Start()
    {
        GetCharacters();
        _view.MaxCharacters = _maxCharacters;
        _view.ExistsCharacters = _characters.Count;

        if (_characters.Count < _maxCharacters)
        {
            _view.SetNewCharacterActive(true);
        }
    }

    private void OnEnable()
    {
        _view.OnCharacterChange += _view_OnCharacterChange;
        _view.CreateNewCharacterButton.onClick.AddListener(OnCreateNewCharacterButtonClickHandler);
    }

    private void _view_OnCharacterChange(CharacterData obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnDisable()
    {
        _view.OnCharacterChange -= _view_OnCharacterChange;
        _view.CreateNewCharacterButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region PlayFab callbacks

    private void OnGetCharacters(ListUsersCharactersResult result)
    {
        Debug.Log("OnGetCharacters : " + result.Characters.Count);

        _characters.Clear();

        if (result.Characters.Count == 0)
        {
            _view.SetCurrentCharacterData();
            return;
        }

        foreach (var item in result.Characters)
        {
            Debug.Log("ID : " + item.CharacterId);
            Debug.Log("Name : " + item.CharacterName);
            Debug.Log("Type : " + item.CharacterType);

            _characters.Add(new CharacterData
            {
                CharacterId = item.CharacterId,
                Name = item.CharacterName,
            });
        }
    }

    private void OnPurchaseItem(PurchaseItemResult result)
    {
        Debug.Log("OnPurchaseItem");

        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest(),
            OnGetUserInventory,
            Debug.LogError
            );
    }

    private void OnGetUserInventory(GetUserInventoryResult result)
    {
        Debug.Log("OnGetUserInventory");
        
    }

    #endregion


    #region Methods

    private void MakePurchaseNewCharacter()
    {
        Debug.Log("MakePurchaseNewCharacter");

        PlayFabClientAPI.PurchaseItem(
            new PurchaseItemRequest
            {
                CatalogVersion = CATALOG_VERSION,
                ItemId = NEW_CHARACTER_ITEM_ID,
                VirtualCurrency = VIRTUAL_CURRENCY,
            },
            OnPurchaseItem,
            Debug.LogError
            );
    }

    private void OnCreateNewCharacterButtonClickHandler()
    {
        _currentCharacter.Name = _view.InputedCharacterName;
        Debug.Log("Create new " + _currentCharacter.Name);

        MakePurchaseNewCharacter();

        //PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        //{
        //    CharacterName = _currentCharacter.Name,
        //    ItemId = itemId
        //},
        //result => UpdateCharacterStatistics(result.CharacterId),
        //Debug.LogError);
    }



    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
                {
                    {"Level", 1},
                    {"XP", 0},
                    {"Gold", 0}
                }
        },
        result =>
        {
            Debug.Log($"Initial stats set, telling client to update character list");
        },
        Debug.LogError);
    }

    private void GetCharacters()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
                OnGetCharacters,
                error => Debug.LogError(error.GenerateErrorReport())
                );
        }
        else
        {
            Debug.LogError("GetCharacters - PlayFab not logged in");
        }
    }

    #endregion

}
