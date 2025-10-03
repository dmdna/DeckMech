using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text slotOrTypeText;
    public TMP_Text statsText;

    [Header("Art Layers")]
    public Image baseImage;     // assign in prefab
    public Image detailsImage;  // assign in prefab

    public void Setup(CardData data)
    {
        // Set art
        if (baseImage != null) baseImage.sprite = data.baseArt;
        if (detailsImage != null) detailsImage.sprite = data.detailsArt;

        // Name & type
        nameText.text = data.cardName;

        if (data.cardType == CardType.Armor)
        {
            slotOrTypeText.text = data.armorSlot.ToString();

            StringBuilder sb = new StringBuilder();
            if (data.hp > 0) sb.AppendLine($"HP: {data.hp}");
            if (data.thermal > 0) sb.Append($"Ther:{data.thermal} ");
            if (data.freeze > 0) sb.Append($"Frz:{data.freeze} ");
            if (data.electric > 0) sb.Append($"Ele:{data.electric} ");
            if (data.voidRes > 0) sb.Append($"Vd:{data.voidRes} ");
            if (data.impact > 0) sb.Append($"Imp:{data.impact} ");

            statsText.text = sb.ToString().Trim();
        }
        else
        {
            slotOrTypeText.text = data.damageType.ToString();
            statsText.text = $"DMG: {data.damage}";
        }
    }
}
