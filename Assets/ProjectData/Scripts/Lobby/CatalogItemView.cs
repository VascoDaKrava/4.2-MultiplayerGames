using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class CatalogItemView : MonoBehaviour
{

    #region Properties


    [field: SerializeField] public Image Icon { get; private set; }
    [field: SerializeField] public TMP_Text Name { get; private set; }
    [field: SerializeField] public TMP_Text Price { get; private set; }
    [field: SerializeField] public TMP_Text Description { get; private set; }

    #endregion

}
