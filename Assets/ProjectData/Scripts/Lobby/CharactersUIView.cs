using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class CharactersUIView : MonoBehaviour
{

    #region Fields

    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _damage;
    [SerializeField] private TMP_Text _health;
    [SerializeField] private TMP_Text _experience;

    [SerializeField] private CharacterItemUIView _characterItemTemplate;
    [SerializeField] private RectTransform _charactersContainer;

    [SerializeField] private TMP_InputField _newCharacterNameInputField;

    public int ExistsCharacters;
    public int MaxCharacters;

    public event Action<CharacterData> OnCharacterChange;

    private Dictionary<CharacterItemUIView, CharacterData> _charactersDictionary = new Dictionary<CharacterItemUIView, CharacterData>();

    #endregion


    #region Properties

    [field: SerializeField] public Button CreateNewCharacterButton { get; private set; }
    public string InputedCharacterName { get => _newCharacterNameInputField.text; }

    #endregion


    #region Methods

    public void SetCurrentCharacterData(CharacterData character)
    {
        _name.text = $"Current ({ExistsCharacters} / {MaxCharacters}) : {character.Name}";
        _damage.text = $"Damage : {character.Damage}";
        _health.text = $"Health : {character.Health * 100}";
        _experience.text = $"XP : {character.Experience}";
        SetPropertiesActive(true);
    }

    public void SetCurrentCharacterData()
    {
        _name.text = $"Characters ({ExistsCharacters} / {MaxCharacters})";
        SetPropertiesActive(false);
    }

    /// <summary>
    /// Allow/disallow creating new character
    /// </summary>
    /// <param name="state"></param>
    public void SetNewCharacterActive(bool state)
    {
        _newCharacterNameInputField.interactable = state;
        CreateNewCharacterButton.interactable = state;
    }

    public void FillCharacters(CharacterData[] charactersData)
    {
        foreach (var item in _charactersDictionary.Keys)
        {
            item.OnClickItem -= OnCharacterItemButtonClickHandler;
            Destroy(item.transform.gameObject);
        }

        _charactersDictionary.Clear();

        foreach (var characterData in charactersData)
        {
            var character = Instantiate(_characterItemTemplate, _charactersContainer);
            character.SetData(characterData);
            character.gameObject.SetActive(true);
            character.OnClickItem += OnCharacterItemButtonClickHandler;
            _charactersDictionary.Add(character, characterData);
        }

        _charactersContainer.gameObject.SetActive(true);
    }

    private void OnCharacterItemButtonClickHandler(CharacterItemUIView characterItemUI)
    {
        SetCurrentCharacterData(_charactersDictionary[characterItemUI]);
        OnCharacterChange?.Invoke(_charactersDictionary[characterItemUI]);
    }

    /// <summary>
    /// Show properties for current character
    /// </summary>
    /// <param name="state"></param>
    private void SetPropertiesActive(bool state)
    {
        _damage.gameObject.SetActive(state);
        _health.gameObject.SetActive(state);
        _experience.gameObject.SetActive(state);
    }

    #endregion

}
