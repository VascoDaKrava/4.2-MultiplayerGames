using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class CharacterItemUIView : MonoBehaviour
{

    #region Fields

    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _damage;
    [SerializeField] private TMP_Text _health;
    [SerializeField] private TMP_Text _experience;

    [SerializeField] private Button _characterItemButton;

    #endregion


    #region Properties

    public event Action<CharacterItemUIView> OnClickItem;

    #endregion


    #region UnityMethods

    private void OnEnable()
    {
        _characterItemButton.onClick.AddListener(() => OnClickItem?.Invoke(this));
    }

    private void OnDisable()
    {
        _characterItemButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    public void SetData(CharacterData character)
    {
        _name.text = $"Name : {character.Name}";
        _damage.text = $"Damage : {character.Damage}";
        _health.text = $"Health : {character.Health * 100}";
        _experience.text = $"XP : {character.Experience}";
    }

    #endregion

}
