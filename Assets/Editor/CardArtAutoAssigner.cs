// CardArtAutoAssigner.cs
// Place this file in an "Editor" folder: Assets/Editor/CardArtAutoAssigner.cs

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Ensure the file compiles only in the Editor
#if UNITY_EDITOR

public class CardArtAutoAssigner : EditorWindow
{
    // Options
    private bool randomizeOnMultiElement = true;
    private bool verboseLogging = true;

    // Log buffer for UI
    private StringBuilder logBuilder = new StringBuilder();

    [MenuItem("Window/Card Art Auto Assigner")]
    public static void ShowWindow()
    {
        CardArtAutoAssigner w = GetWindow<CardArtAutoAssigner>("Card Art Auto Assigner");
        w.minSize = new Vector2(540, 420);
    }

    void OnGUI()
    {
        GUILayout.Label("Card Art Auto Assigner", EditorStyles.boldLabel);
        GUILayout.Space(6);

        EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This tool assigns baseArt and detailsArt on CardData assets using your sprite naming conventions.\n\n" +
            "Naming convention examples:\n" +
            "  - Legs_Electric_Base\n" +
            "  - Chest_Impact_Detail\n" +
            "  - Attack_Freeze_Base (or Attack_Freeze)\n\nSelect CardData assets in the Project window and click Assign Selected, or run on all project CardData assets.", MessageType.Info);

        GUILayout.Space(6);

        randomizeOnMultiElement = EditorGUILayout.ToggleLeft("Randomize base/detail when >2 defenses", randomizeOnMultiElement);
        verboseLogging = EditorGUILayout.ToggleLeft("Verbose logging", verboseLogging);

        GUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Assign Selected CardData"))
        {
            RunOnSelected();
        }
        if (GUILayout.Button("Assign All CardData in Project"))
        {
            RunOnAll();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(8);

        if (GUILayout.Button("Clear Log")) { logBuilder.Clear(); }

        GUILayout.Space(6);
        EditorGUILayout.LabelField("Log", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(logBuilder.ToString(), GUILayout.ExpandHeight(true));
    }

    // ----------------------
    // Entry points
    // ----------------------
    void RunOnSelected()
    {
        logBuilder.Clear();
        Object[] selected = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        List<CardData> cards = new List<CardData>();
        foreach (var obj in selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            var cd = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (cd != null) cards.Add(cd);
        }

        if (cards.Count == 0)
        {
            Log("No CardData assets selected. Please select CardData assets in the Project window.");
            return;
        }

        AssignToCards(cards);
    }

    void RunOnAll()
    {
        logBuilder.Clear();

        // Find all ScriptableObjects and try to load CardData from each
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" });
        List<CardData> cards = new List<CardData>();
        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var cd = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (cd != null) cards.Add(cd);
        }

        if (cards.Count == 0)
        {
            Log("No CardData assets found in project.");
            return;
        }

        AssignToCards(cards);
    }

    // ----------------------
    // Core assignment
    // ----------------------
    void AssignToCards(List<CardData> cards)
    {
        int assigned = 0;
        int errors = 0;

        foreach (var card in cards)
        {
            bool ok = AssignArtForCard(card);
            if (ok) assigned++;
            else errors++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Log($"Done. Assigned art for {assigned} cards. {errors} errors/warnings.");
    }

    bool AssignArtForCard(CardData card)
    {
        if (card == null)
        {
            Log("Null CardData encountered - skipping.");
            return false;
        }

        // Armor cards: choose elements from resistances; Attack cards: use damageType
        if (card.cardType == CardType.Armor)
        {
            return AssignArtForArmorCard(card);
        }
        else // Attack
        {
            return AssignArtForAttackCard(card);
        }
    }

    bool AssignArtForAttackCard(CardData card)
    {
        string element = card.damageType.ToString(); // e.g., "Freeze" or "Thermal"
        // Try exact search for Attack_<Element>_Base / _Detail
        Sprite baseSprite = FindSpriteByExactName($"Attack_{element}_Base") ?? FindSpriteByExactName($"Attack_{element}");
        Sprite detailSprite = FindSpriteByExactName($"Attack_{element}_Detail") ?? baseSprite;

        if (baseSprite == null)
        {
            // fallback: search any sprite that contains Attack and element
            baseSprite = FindSpriteContains($"Attack_{element}") ?? FindSpriteContains($"_{element}_");
            if (baseSprite != null && verboseLogging) Log($"(fallback) using {baseSprite.name} for Attack {card.cardName}");
        }
        if (detailSprite == null) detailSprite = baseSprite;

        if (baseSprite == null)
        {
            LogWarning($"[Attack] Could not find base sprite for Attack card '{card.cardName}' (element: {element}).");
            return false;
        }

        // assign and save
        Undo.RecordObject(card, "Assign Card Art");
        card.baseArt = baseSprite;
        card.detailsArt = detailSprite;
        EditorUtility.SetDirty(card);
        Log($"[Attack] {card.cardName} -> Base: {baseSprite.name} | Detail: {(detailSprite != null ? detailSprite.name : "(none)")}");
        return true;
    }

    bool AssignArtForArmorCard(CardData card)
    {
        // Build list of (element,value)
        var elems = new List<System.Tuple<string, int>>();
        elems.Add(System.Tuple.Create("Thermal", card.thermal));
        elems.Add(System.Tuple.Create("Freeze", card.freeze));
        elems.Add(System.Tuple.Create("Electric", card.electric));
        elems.Add(System.Tuple.Create("Void", card.voidRes));
        elems.Add(System.Tuple.Create("Impact", card.impact));

        // Filter non-zero
        var nonZero = elems.Where(e => e.Item2 > 0).OrderByDescending(e => e.Item2).ToList();

        string slotName = card.armorSlot.ToString(); // e.g., Helmet, Chest, Gauntlets, Legs

        string chosenBaseElement = null;
        string chosenDetailElement = null;

        if (nonZero.Count == 0)
        {
            // fallback: use slot generic art (slot + "_Base")
            Log($"[Armor:{card.cardName}] No resistances set. Attempting generic slot art for {slotName}.");
            Sprite slotBase = FindSpriteByExactName($"{slotName}_Base") ?? FindSpriteContains($"{slotName}_Base") ?? FindSpriteContains($"{slotName}_");
            if (slotBase == null)
            {
                LogWarning($"[Armor] No generic base art found for slot {slotName} while assigning '{card.cardName}'");
                return false;
            }
            Undo.RecordObject(card, "Assign Card Art");
            card.baseArt = slotBase;
            card.detailsArt = slotBase;
            EditorUtility.SetDirty(card);
            Log($"[Armor] {card.cardName} -> Base & Detail: {slotBase.name} (generic)");
            return true;
        }
        else if (nonZero.Count == 1)
        {
            chosenBaseElement = nonZero[0].Item1;
            chosenDetailElement = chosenBaseElement;
        }
        else if (nonZero.Count == 2)
        {
            chosenBaseElement = nonZero[0].Item1;
            chosenDetailElement = nonZero[1].Item1;
        }
        else // > 2
        {
            if (randomizeOnMultiElement)
            {
                // Random pick two distinct elements from the non-zero list
                var rng = new System.Random();
                var picks = nonZero.OrderBy(x => rng.Next()).Take(2).ToList();
                chosenBaseElement = picks[0].Item1;
                chosenDetailElement = picks[1].Item1;
                Log($"[Armor:{card.cardName}] >2 elements found. Randomized pick: {chosenBaseElement}, {chosenDetailElement}");
            }
            else
            {
                // deterministic: pick two highest
                chosenBaseElement = nonZero[0].Item1;
                chosenDetailElement = nonZero[1].Item1;
            }
        }

        // Now try to resolve sprites by exact naming convention:
        Sprite baseSprite = FindSpriteByExactName($"{slotName}_{chosenBaseElement}_Base")
                         ?? FindSpriteByExactName($"{slotName}_{chosenBaseElement}")
                         ?? FindSpriteContains($"{slotName}_{chosenBaseElement}_Base")
                         ?? FindSpriteContains($"{slotName}_{chosenBaseElement}");
        Sprite detailSprite = FindSpriteByExactName($"{slotName}_{chosenDetailElement}_Detail")
                         ?? FindSpriteByExactName($"{slotName}_{chosenDetailElement}")
                         ?? FindSpriteContains($"{slotName}_{chosenDetailElement}_Detail")
                         ?? FindSpriteContains($"{slotName}_{chosenDetailElement}");

        // Fallbacks if any of them are null: try swapped names (Element_Slot)
        if (baseSprite == null)
            baseSprite = FindSpriteByExactName($"{chosenBaseElement}_{slotName}_Base") ?? FindSpriteContains($"{chosenBaseElement}_{slotName}_Base");
        if (detailSprite == null)
            detailSprite = FindSpriteByExactName($"{chosenDetailElement}_{slotName}_Detail") ?? FindSpriteContains($"{chosenDetailElement}_{slotName}_Detail");

        // Final fallback: find any sprite that contains slotName and "Base"
        if (baseSprite == null) baseSprite = FindSpriteContains($"{slotName}_Base") ?? FindSpriteContains($"{slotName}_");
        if (detailSprite == null) detailSprite = baseSprite;

        if (baseSprite == null)
        {
            LogWarning($"[Armor] Could not find base sprite for '{card.cardName}' with slot '{slotName}' and element '{chosenBaseElement}'.");
            return false;
        }

        // Assign to asset
        Undo.RecordObject(card, "Assign Card Art");
        card.baseArt = baseSprite;
        card.detailsArt = detailSprite ?? baseSprite;
        EditorUtility.SetDirty(card);

        Log($"[Armor] {card.cardName} -> Slot:{slotName} | Base:{baseSprite.name} | Detail:{(detailSprite != null ? detailSprite.name : "(fallback to base)")}");
        return true;
    }

    // ----------------------
    // Sprite search helpers
    // ----------------------
    Sprite FindSpriteByExactName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;
        // Search for exact match ignoring extension
        string[] guids = AssetDatabase.FindAssets($"{spriteName} t:Sprite");
        foreach (var g in guids)
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(p);
            if (s != null && s.name == spriteName) return s;
        }
        return null;
    }

    Sprite FindSpriteContains(string partialName)
    {
        if (string.IsNullOrEmpty(partialName)) return null;
        string[] guids = AssetDatabase.FindAssets($"t:Sprite");
        foreach (var g in guids)
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(p);
            if (s != null && s.name.IndexOf(partialName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return s;
        }
        return null;
    }

    // ----------------------
    // Logging helpers
    // ----------------------
    void Log(string text)
    {
        if (verboseLogging) Debug.Log(text);
        logBuilder.AppendLine(text);
    }

    void LogWarning(string text)
    {
        Debug.LogWarning(text);
        logBuilder.AppendLine("[WARN] " + text);
    }
}

#endif
