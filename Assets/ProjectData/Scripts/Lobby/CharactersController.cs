using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;


public sealed class CharactersController : MonoBehaviour
{

    #region Fields

    private const string CATALOG_VERSION_MASTER = "NewCatalog";// Only there item can converted into character.
    private const string CATALOG_VERSION = "NewCatalog";// Version/name as PlayFab web creation. Same as CatalogLoader.cs, but not work with GrantCharacterToUser
    private const string NEW_CHARACTER_ITEM_ID = "Master";//item ID for purchase
    private const string VIRTUAL_CURRENCY = "CO";// use this currency for purchase
    private const int PRICE = 100;// price of item for purchase

    private const int MAX_CHARACTERS = 2;

    [SerializeField] private CharactersUIView _view;

    private List<CharacterData> _characters = new List<CharacterData>();
    private CharacterData _currentCharacter;
    private string _newCharacterName = "Bob";

    #endregion


    #region UnityMethods

    private void Start()
    {
        GetCharacters();
        _view.MaxCharacters = MAX_CHARACTERS;
        _view.ExistsCharacters = _characters.Count;

        if (_characters.Count < MAX_CHARACTERS)
        {
            _view.SetNewCharacterActive(true);
        }
    }

    private void OnEnable()
    {
        _view.OnCharacterChange += OnCharacterChangeHandler;
        _view.CreateNewCharacterButton.onClick.AddListener(OnCreateNewCharacterButtonClickHandler);
    }

    private void OnDisable()
    {
        _view.OnCharacterChange -= OnCharacterChangeHandler;
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

        _view.FillCharacters(_characters.ToArray());

        if (_characters.Count >= MAX_CHARACTERS)
        {
            _view.SetNewCharacterActive(false);
        }

        _view.ExistsCharacters = _characters.Count;
        _currentCharacter ??= _characters[0];
        _view.SetCurrentCharacterData(_currentCharacter);
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
        string itemInstanceId = default;

        foreach (var itemInstance in result.Inventory)
        {
            if (itemInstance.ItemId == NEW_CHARACTER_ITEM_ID)
            {
                itemInstanceId = itemInstance.ItemInstanceId;
                Debug.Log("Find NEW_CHARACTER_ITEM_ID in inventory");
                break;
            }
        }

        if (string.IsNullOrEmpty(itemInstanceId))
        {
            Debug.LogError("No tokens for new Character");
            return;
        }

        PlayFabClientAPI.GrantCharacterToUser(
            new GrantCharacterToUserRequest
            {
                CharacterName = _newCharacterName,
                ItemId = NEW_CHARACTER_ITEM_ID,
                CatalogVersion = CATALOG_VERSION_MASTER,
            },
            result => UpdateCharacterStatistics(result.CharacterId),
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });

        GetCharacters();
    }

    #endregion


    #region Methods

    private void OnCharacterChangeHandler(CharacterData character)
    {
        Debug.Log("Select " + character.Name);
        _currentCharacter = character;
    }

    private void MakePurchaseNewCharacter()
    {
        Debug.Log("MakePurchaseNewCharacter");

        PlayFabClientAPI.PurchaseItem(
            new PurchaseItemRequest
            {
                CatalogVersion = CATALOG_VERSION_MASTER,
                ItemId = NEW_CHARACTER_ITEM_ID,
                VirtualCurrency = VIRTUAL_CURRENCY,
                Price = PRICE
            },
            OnPurchaseItem,
            Debug.LogError
            );
    }

    private void OnCreateNewCharacterButtonClickHandler()
    {
        if (!string.IsNullOrEmpty(_view.InputedCharacterName))
        {
            _newCharacterName = _view.InputedCharacterName;
        }

        Debug.Log("Create new " + _newCharacterName);

        MakePurchaseNewCharacter();
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        int damage = default;
        int health = default;
        int xp = default;

        if (_currentCharacter != null)
        {
            if (string.Equals(characterId, _currentCharacter.CharacterId))
            {
                damage = _currentCharacter.Damage;
                health = (int)(_currentCharacter.Health * 100.0f);
                xp = _currentCharacter.Experience;
            }
        }

        PlayFabClientAPI.UpdateCharacterStatistics(
            new UpdateCharacterStatisticsRequest
            {
                CharacterId = characterId,
                CharacterStatistics = new Dictionary<string, int>
                {
                    {"Damage", damage},
                    {"Health", health},
                    {"XP", xp}
                }
            },
            success => GetCharacters(),
            Debug.LogError);
    }

    private void GetCharacters()
    {
        Debug.Log("GetCharacters");

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.GetAllUsersCharacters(
                new ListUsersCharactersRequest(),
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
