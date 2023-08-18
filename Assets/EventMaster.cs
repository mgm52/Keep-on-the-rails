using UnityEngine;
using UnityLibrary;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

public class EventMaster : MonoBehaviour
{
    // UI Components
    public TMP_Text dayTMP;
    public TMP_Text descriptionTMP;
    public List<TMP_Text> optionTMPs;
    public List<SpriteRenderer> eventSprites;
    public SpriteRenderer defaultSprite;
    public SpriteRenderer backgroundSprite;
    public GameObject winScreen;
    public TMP_Text winText;
    public GameObject failScreen;
    public TMP_Text failText;

    // Sounds
    public AudioClip midSound;
    public AudioClip goodSound;
    public AudioClip badSound;
    public AudioSource audioSource;

    // Other Components
    public CameraShake castleShake;
    public TextAsset allEventsJSON;
    public List<NumberText> statTexts;
    public Color negStatColor;
    public Color posStatColor;

    private List<Event> allEvents;
    private Event currentEvent;
    private List<int> currentStats = new List<int>() { 5, 5, 5, 0 };
    private bool failed = false;
    private int day = -1;
    private string prevKeyPresses = "";
    private int continuousKeyCount = 0;
    private int prevKey = -1;
    private readonly string[] failReasons = new string[] { "No food.", "No fun.", "Your keep crumbled.", "Dragon apocalypse. All hope is lost!" };

    private Dictionary<string, int> keyMap = new Dictionary<string, int>() { { "r", 0 }, { "a", 1 }, { "i", 2 }, { "l", 3 } };
    private Dictionary<int, string> keyReverse = new Dictionary<int, string>() { { 0, "r" }, { 1, "a" }, { 2, "i" }, { 3, "l" } };

    private void Start()
    {
        allEvents = ParseJsonFile(allEventsJSON);
        UpdateStats(new int[] { 0, 0, 0, 0 });
        LoadNewEvent();
    }

    private void Update()
    {
        HandleInputs();
    }

    private void HandleInputs()
    {
        if (failed && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Gameplay");
            return;
        }

        if (failed)
            return;

        string keyPressed = Input.inputString;
        if (keyMap.ContainsKey(keyPressed))
        {
            HandleEventSelection(keyPressed);
        }
    }

    private void HandleEventSelection(string keyPressed)
    {
        int optionNo = keyMap[keyPressed[0].ToString()];

        prevKeyPresses += keyPressed[0];

        if (optionNo >= 0 && optionNo < currentEvent.Options.Count)
        {
            if (prevKey == optionNo) continuousKeyCount++;
            else continuousKeyCount = 1;
            prevKey = optionNo;

            EventOption chosenOption = currentEvent.Options[optionNo];
            UpdateStats(new int[] { chosenOption.FoodChange, chosenOption.FunChange, chosenOption.FortificationChange, chosenOption.DARLChange });
            LoadNewEvent();
        }
    }

    private void LoadNewEvent()
    {
        if (failed) return;

        day++;
        dayTMP.text = "DAY " + day;

        if (day == 20)
        {
            Win("You reached day 20!");
            return;
        }

        currentEvent = GetEventBasedOnDay();

        descriptionTMP.text = $"<size=200%><b>{currentEvent.Title}</b><size=100%>\n{currentEvent.Description}";

        UpdateOptionTexts();
        UpdateEventSprites();

        if (currentEvent.EventFlags.Count == 0)
            defaultSprite.enabled = true;
    }

    private Event GetEventBasedOnDay()
    {
        if (day == 0) return PredefinedEvents.difficultyEvent;
        if (day == 1) return PredefinedEvents.introEvent;
        if (continuousKeyCount > 0 && continuousKeyCount % 10 == 0) return PredefinedEvents.spamEvent;
        if (prevKeyPresses.EndsWith("rail") || prevKeyPresses.Substring(0, prevKeyPresses.Length - 1).EndsWith("rail")) return PredefinedEvents.railEvent;
        return allEvents[UnityEngine.Random.Range(0, allEvents.Count)];
    }

    private void UpdateOptionTexts()
    {
        Func<string, string> negFormat = stat => $" <color={ColorToHex(negStatColor)}>(-{stat})</color>";
        Func<string, string> posFormat = stat => $" <b><color={ColorToHex(posStatColor)}>(+{stat})</b></color>";

        for (int i = 0; i < currentEvent.Options.Count; i++)
        {
            optionTMPs[i].text = $"<b>({keyReverse[i]}) <size=150%>{currentEvent.Options[i].Description}<size=125%></b>\n";
            optionTMPs[i].text += FormatOptionText(currentEvent.Options[i], negFormat, posFormat);
        }
        for (int i = currentEvent.Options.Count; i < optionTMPs.Count; i++) optionTMPs[i].text = "";
    }

    private string FormatOptionText(EventOption option, Func<string, string> negFormat, Func<string, string> posFormat)
    {
        StringBuilder sb = new StringBuilder();

        // Map property names to their display names
        Dictionary<string, string> propertyToDisplayName = new Dictionary<string, string>
        {
            {"FoodChange", "Food"},
            {"FunChange", "Fun"},
            {"FortificationChange", "Fort"},
            {"DARLChange", "DARL"},
        };

        foreach (var kvp in propertyToDisplayName)
        {
            PropertyInfo propertyInfo = typeof(EventOption).GetProperty(kvp.Key);
            if (propertyInfo != null)
            {
                int value = (int)propertyInfo.GetValue(option);
                if (value > 0)
                    sb.Append(posFormat(kvp.Value));
                else if (value < 0)
                    sb.Append(negFormat(kvp.Value));
            }
        }

        return sb.ToString();
    }

    private void UpdateEventSprites()
    {
        bool anySpriteEnabled = false;

        foreach (SpriteRenderer eventSprite in eventSprites)
            eventSprite.enabled = false;

        Dictionary<string, int> flagToIndex = new Dictionary<string, int>
        {
            {"lizard", 0},
            {"rain", 1},
            {"chest", 2},
            {"goblin", 3},
            {"magic", 4},
            {"night", 5},
            {"person", 6},
            {"ufo", 7},
            {"fire", 8},
        };

        foreach (string flag in currentEvent.EventFlags)
        {
            if (flagToIndex.ContainsKey(flag))
            {
                eventSprites[flagToIndex[flag]].enabled = true;
                anySpriteEnabled = true;
            }
        }

        defaultSprite.enabled = !anySpriteEnabled;
    }

    private void UpdateStats(int[] changes)
    {
        int totalChange = changes.Sum();
        if (totalChange < 0)
        {
            audioSource.clip = badSound;
            audioSource.Play();
        }
        else if (totalChange > 0)
        {
            audioSource.clip = goodSound;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = midSound;
            audioSource.Play();
        }

        if (changes[2] < 0)
        {
            castleShake.Shake();
        }

        for (int i = 0; i < statTexts.Count; i++)
        {
            // statTMPs[i].text to int using int.Parse()
            currentStats[i] = Mathf.Max(0, currentStats[i] + changes[i]);

            // e.g. "+20" or "-20"
            string changeString = changes[i] > 0 ? "(+" + changes[i] + ")" : changes[i] == 0 ? "" : "(" + changes[i].ToString() + ")";
            if (i == 3) statTexts[i].SetText(currentStats[i].ToString() + "%");
            else statTexts[i].SetText(currentStats[i].ToString());
            //= currentStats[i].ToString() + "  " + changeString;
            statTexts[i].IndicateChange(changes[i]);

            if ((i < 3 && currentStats[i] < 1) || (i == 3 && currentStats[i] > 99))
            {
                Fail(failReasons[i]);
            }
        }
    }

    private string ColorToHex(Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }

    private void Win(string winCause)
    {
        failed = true;
        winScreen.SetActive(true);
        winText.text = winCause + "\n\nPress space to restart.";
        descriptionTMP.enabled = false;
        foreach (SpriteRenderer eventSprite in eventSprites)
        {
            eventSprite.enabled = false;
        }
        defaultSprite.enabled = false;
        dayTMP.color = Color.white;
        dayTMP.transform.DOLocalMoveY(-1.1f, 0.25f);
        castleShake.StopShake();
        castleShake.transform.DOLocalMoveX(castleShake.transform.localPosition.x + 10, 1.5f).SetEase(Ease.OutBounce);
        for (int i = 0; i < 4; i++)
        {
            optionTMPs[i].enabled = false;
        }
        backgroundSprite.gameObject.SetActive(false);
    }

    private void Fail(string failCause)
    {
        failed = true;
        failScreen.SetActive(true);
        failText.text = failCause + "\n\nPress space to restart.";
        descriptionTMP.enabled = false;

        foreach (SpriteRenderer eventSprite in eventSprites)
        {
            eventSprite.enabled = false;
        }
        defaultSprite.enabled = false;

        dayTMP.color = Color.white;
        dayTMP.transform.DOLocalMoveY(-1.1f, 0.25f);
        castleShake.StopShake();
        castleShake.transform.DOLocalMoveY(castleShake.transform.localPosition.y - 10, 1f).SetEase(Ease.OutBounce);
        for (int i = 0; i < 4; i++)
        {
            optionTMPs[i].enabled = false;
        }
        backgroundSprite.gameObject.SetActive(false);

        if (currentStats[3] >= 100)
        {
            Camera.main.backgroundColor = Color.red;
        }
    }
    public static List<Event> ParseJsonFile(TextAsset jsonAsset)
    {
        // Read the JSON file
        //var json = File.ReadAllText(filePath);
        var json = jsonAsset.text;
        
        // Deserialize the JSON to a list of Event objects
        var events = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Event>>(json);

        return events;
    }
}


