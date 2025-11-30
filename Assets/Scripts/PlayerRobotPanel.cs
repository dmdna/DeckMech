using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRobotPanel : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text hpText;
    public TMP_Text resistancesText;

    [Header("Stats")]
    public Transform statsContainer;              // parent object for all stat icons
    public GameObject statItemPrefab;             // prefab with Image + Text

    [Header("Icons")]
    public Sprite hpIcon;
    public Sprite thermalIcon;
    public Sprite freezeIcon;
    public Sprite electricIcon;
    public Sprite voidIcon;
    public Sprite impactIcon;

    // Fill the panel with PlayerRobot info
    public void Setup(int playerId, PlayerRobot robot)
    {
        titleText.text = $"Player {playerId}";
        hpText.text = $"{robot.hp}";

        // Build resistance string only with non-zero values
        /* System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (robot.thermal > 0) sb.Append($"Ther:{robot.thermal} ");
        if (robot.freeze > 0) sb.Append($"Frz:{robot.freeze} ");
        if (robot.electric > 0) sb.Append($"Ele:{robot.electric} ");
        if (robot.voidRes > 0) sb.Append($"Vd:{robot.voidRes} ");
        if (robot.impact > 0) sb.Append($"Imp:{robot.impact} ");

        resistancesText.text = sb.ToString().Trim(); */

        // Clear old stat items
        foreach (Transform child in statsContainer)
            Destroy(child.gameObject);
    
        // Add stats dynamically
        AddStat(robot.thermal, thermalIcon);
        AddStat(robot.freeze, freezeIcon);
        AddStat(robot.electric, electricIcon);
        AddStat(robot.voidRes, voidIcon);
        AddStat(robot.impact, impactIcon);
    }

    private void AddStat(int value, Sprite icon)
    {
        if (value <= 0) return;

        GameObject item = Instantiate(statItemPrefab, statsContainer);
        item.GetComponent<RectTransform>().localScale *= 2f;
        Image iconImage = item.transform.Find("Icon").GetComponent<Image>();
        TMP_Text valueText = item.transform.Find("Value").GetComponent<TMP_Text>();

        iconImage.sprite = icon;
        valueText.text = value.ToString();

        Debug.Log("Added stat: " + value);
    }
}
