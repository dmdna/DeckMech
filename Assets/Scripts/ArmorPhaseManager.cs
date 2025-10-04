using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArmorPhaseManager : MonoBehaviour
{
    [Header("Managers")]
    public DeckManager deckManager;
    public TurnManager turnManager;
    public UIManager uiManager;
    public FightPhaseManager fightManager;

    [Header("UI References")]
    public Transform drawPanel;
    public Transform[] hangarColumns;     // 0=Helmet, 1=Chest, 2=Gauntlets, 3=Legs
    public GameObject cardUIPrefab;
    public Button drawButton;

    private List<GameObject> currentDraw = new List<GameObject>();
    private bool selectingArmorSet = false;

    // --- DRAW ---
    public void DrawThreeArmor()
    {
        if (selectingArmorSet) return;
        if (turnManager.IsPlayerDone(turnManager.CurrentPlayer)) return;

        ClearDrawPanel();
        turnManager.UpdatePhaseText("draw cards");

        for (int i = 0; i < 3; i++)
        {
            CardData card = deckManager.DrawArmorCard();
            if (card == null) continue;

            GameObject cardUI = Instantiate(cardUIPrefab, drawPanel);
            cardUI.GetComponent<CardUI>().Setup(card);

            CardHolder holder = cardUI.GetComponent<CardHolder>();
            holder.cardData = card;

            Button btn = cardUI.GetComponent<Button>();
            btn.onClick.AddListener(() => PickCard(holder));

            currentDraw.Add(cardUI);
        }

        drawButton.interactable = false;
    }

    // --- PICK ---
    void PickCard(CardHolder chosenHolder)
    {
        CardData chosen = chosenHolder.cardData;

        // Add to hangar
        Transform column = hangarColumns[(int)chosen.armorSlot];
        GameObject cardUI = Instantiate(cardUIPrefab, column);
        cardUI.GetComponent<CardUI>().Setup(chosen);
        cardUI.GetComponent<CardHolder>().cardData = chosen;

        // Discard other two
        foreach (GameObject ui in currentDraw)
        {
            CardHolder holder = ui.GetComponent<CardHolder>();
            if (holder.cardData != chosen)
            {
                deckManager.DiscardArmor(holder.cardData);
            }
            Destroy(ui);
        }
        currentDraw.Clear();

        // Check for full set
        if (HangarComplete())
        {
            selectingArmorSet = true;
            turnManager.UpdatePhaseText("choose your armor set");
            PromptArmorSelection();
        }
        else
        {
            // Next player's turn
            turnManager.NextTurn();
            drawButton.interactable = true;
        }
    }

    // --- ARMOR SELECTION ---
    void PromptArmorSelection()
    {
        selectingArmorSet = true;

        // Go through each column
        foreach (Transform column in hangarColumns)
        {
            if (column.childCount == 1)
            {
                // Auto-select if only one option
                AutoSelect(column.GetChild(0).GetComponent<CardHolder>());
            }
            else
            {
                // Multiple options → wait for clicks
                foreach (Transform card in column)
                {
                    CardHolder holder = card.GetComponent<CardHolder>();
                    Button btn = card.GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        if (turnManager.IsPlayerDone(turnManager.CurrentPlayer)) return;
                        MarkSelected(holder);
                    });
                }
            }
        }

        // if all slots auto-selected, immediately finalize
        TryFinalizeRobot();
    }



    void AutoSelect(CardHolder holder)
    {
        holder.selected = true;
        holder.GetComponent<Image>().color = Color.green;
    }

    void MarkSelected(CardHolder holder)
    {
        foreach (Transform sibling in holder.transform.parent)
        {
            sibling.GetComponent<CardHolder>().selected = false;
            sibling.GetComponent<Image>().color = Color.white;
        }
        holder.selected = true;
        holder.GetComponent<Image>().color = Color.green;
        TryFinalizeRobot();
    }

    void TryFinalizeRobot()
    {
        foreach (Transform column in hangarColumns)
        {
            bool hasSelection = false;
            foreach (Transform card in column)
            {
                CardHolder h = card.GetComponent<CardHolder>();
                if (h.selected) { hasSelection = true; break; }
            }
            if (!hasSelection) return; // wait
        }
        AssembleRobot();
    }

    void AssembleRobot()
    {
        int playerId = turnManager.CurrentPlayer;
        if (turnManager.IsPlayerDone(playerId)) return; // prevent double build

        PlayerRobot robot = new PlayerRobot(playerId);

        // Get selected armor cards from each column
        CardHolder helmet = GetSelectedInColumn(0);
        CardHolder chest = GetSelectedInColumn(1);
        CardHolder gauntlet = GetSelectedInColumn(2);
        CardHolder legs = GetSelectedInColumn(3);

        robot.helmetCard = helmet ? helmet.cardData : null;
        robot.chestCard = chest ? chest.cardData : null;
        robot.gauntletCard = gauntlet ? gauntlet.cardData : null;
        robot.legCard = legs ? legs.cardData : null;

        // compute stats
        robot.RecalculateStats();

        Debug.Log($"✅ {robot.ToDebugString()}");

        // remove chosen armor from hangar
        if (helmet) Destroy(helmet.gameObject);
        if (chest) Destroy(chest.gameObject);
        if (gauntlet) Destroy(gauntlet.gameObject);
        if (legs) Destroy(legs.gameObject);

        // save robot globally
        GameManager.Instance.SaveRobot(robot);
        turnManager.MarkPlayerDone(playerId);
        selectingArmorSet = false;

        // move to next phase
        if (turnManager.BothPlayersDone())
        {
            Debug.Log("Both robots ready, switch to Fight Phase!");
            fightManager.EnterFightPhase();
            uiManager.ShowFightPhase();
        }
        else
        {
            turnManager.NextTurn();
            drawButton.interactable = true;
        }
    }

    // helper to get selected card in a column
    CardHolder GetSelectedInColumn(int columnIndex)
    {
        Transform column = hangarColumns[columnIndex];
        foreach (Transform child in column)
        {
            CardHolder ch = child.GetComponent<CardHolder>();
            if (ch != null && ch.selected)
                return ch;
        }
        return null;
    }


    bool HangarComplete()
    {
        foreach (Transform column in hangarColumns)
        {
            if (column.childCount == 0) return false;
        }
        return true;
    }

    void ClearDrawPanel()
    {
        foreach (GameObject ui in currentDraw) Destroy(ui);
        currentDraw.Clear();
    }
}
