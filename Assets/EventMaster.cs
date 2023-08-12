using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using System.Data;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityLibrary;
using DG.Tweening;
using System;

public class EventMaster : MonoBehaviour
{
    // TextMasterPro text to update
    public TMPro.TMP_Text dayTMP;
    public TMPro.TMP_Text descriptionTMP;
    public List<TMPro.TMP_Text> optionTMPs;
    public List<NumberText> statTexts;

    public List<SpriteRenderer> eventSprites;
    public SpriteRenderer defaultSprite;

    public SpriteRenderer backgroundSprite;

    public GameObject winScreen;
    public TMPro.TMP_Text winText;

    public GameObject failScreen;
    public TMPro.TMP_Text failText;

    private List<Event> allEvents;
    private Event currentEvent;

    public AudioClip midSound;
    public AudioClip goodSound;
    public AudioClip badSound;
    public AudioSource audioSource;

    public CameraShake castleShake;

    public TextAsset allEventsJSON;

    // food, fun, fort, darl
    private List<int> currentStats = new List<int>() { 5, 5, 5,  0 };
    string[] failReasons = new string[] { "No food.", "No fun.", "Your keep crumbled.", "Dragon apocalypse. All hope is lost!" };
    private bool failed = false;

    Dictionary<string, int> keyMap = new Dictionary<string, int>() { { "r", 0 }, { "a", 1 }, { "i", 2 }, { "l", 3 } };
    Dictionary<int, string> keyReverse = new Dictionary<int, string>() { { 0, "r" }, { 1, "a" }, { 2, "i" }, { 3, "l" } };

    private int day = -1;
    private string prevKeyPresses = "";

    // Start is called before the first frame update
    void Start()
    {
        allEvents = ParseJsonFile(allEventsJSON);

        UpdateStats(new int[] { 0, 0, 0, 0 });
        LoadNewEvent();
    }

    int continuousKeyCount = 0;
    int prevKey = -1;

    // Update is called once per frame
    void Update()
    {
        // if user presses "r", "a", "i", or "l", then update the text
        if ((!failed) && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.L)))
        {
            string keyPressed = Input.inputString;
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
        else if (failed && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Gameplay");
        }
    }

    void Fail(string failCause)
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

    void Win(string winCause)
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

    public Color negStatColor;
    public Color posStatColor;

    public string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        string hex = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        return hex;
    }

    private Event introEvent = new Event()
    {
        Title = "Bon voyage",
        Description = "Your <b>keep</b> is ready to hit the rails. How will you set rail?",
        EventFlags = new List<string>(),
        Options = new List<EventOption>()
    {
        new EventOption() {Description = "Commence joyfully", FoodChange = 0, FunChange = 5, FortificationChange = 0, DARLChange = 0 },
        new EventOption() {Description = "Commence cautiously", FoodChange = 5, FunChange = 0, FortificationChange = 0, DARLChange = 0 },
    }
    };

    private Event difficultyEvent = new Event()
    {
        Title = "Welcome!",
        Description = "Choose your <b>difficulty</b> below to begin!",
        EventFlags = new List<string>() { "night" },
        Options = new List<EventOption>()
        {
            new EventOption() {Description = "Regular", FoodChange = 5, FunChange = 5, FortificationChange = 5, DARLChange = 0 },
            new EventOption() {Description = "Challenging", FoodChange = 0, FunChange = 0, FortificationChange = 0, DARLChange = 45 },
        }
    };

    private Event spamEvent = new Event()
    {
        Title = "RSI",
        Description = "Oh no! You feel yourself developing a repetitive strain injury.",
        EventFlags = new List<string>() { "person" },
        Options = new List<EventOption>()
        {
            new EventOption() {Description = "Ignore it", FoodChange = 0, FunChange = -20, FortificationChange = 0, DARLChange = 0 },
            new EventOption() {Description = "Ignore it", FoodChange = 0, FunChange = -20, FortificationChange = 0, DARLChange = 0 },
            new EventOption() {Description = "Ignore it", FoodChange = 0, FunChange = -20, FortificationChange = 0, DARLChange = 0 },
            new EventOption() {Description = "Press a new button", FoodChange = 0, FunChange = 10, FortificationChange = 0, DARLChange = 0 },
        }
    };

    private Event railEvent = new Event()
    {
        Title = "rail",
        Description = "rail",
        EventFlags = new List<string>() { "ufo" },
        Options = new List<EventOption>()
        {
            new EventOption() {Description = "rail", FoodChange = 5, FunChange = 5, FortificationChange = 5, DARLChange = 50 },
        }
    };


    void LoadNewEvent()
    {
        if (!failed)
        {
            day += 1;
            dayTMP.text = "DAY " + day.ToString();
            if (day == 20)
            {
                Win("You reached day 20!");
                return;
            }
            if (day == 0)
                currentEvent = difficultyEvent;
            else if (day == 1)
                currentEvent = introEvent;
            else if (continuousKeyCount > 0 && continuousKeyCount % 10 == 0)
                currentEvent = spamEvent;
            else if (prevKeyPresses.EndsWith("rail") || prevKeyPresses.Substring(0, prevKeyPresses.Length-1).EndsWith("rail"))
            {
                currentEvent = railEvent;
            }
            else
                currentEvent = allEvents[UnityEngine.Random.Range(0, allEvents.Count)];
            //titleTMP.text = currentEvent.Title;
            descriptionTMP.text = "<size=200%><b>" + currentEvent.Title + "</b><size=100%>\n" + currentEvent.Description;
            Func<string, string> negFormat = stat => " <color=" + ColorToHex(negStatColor) + ">(-" + stat + ")</color>";
            Func<string, string> posFormat = stat => " <b><color=" + ColorToHex(posStatColor) + ">(+" + stat + ")</b></color>";
            for (int i = 0; i < currentEvent.Options.Count; i++)
            {
                // TODO: reactivate deactivated options 
                optionTMPs[i].text = "<b>(" + keyReverse[i] + ") <size=150%>" + currentEvent.Options[i].Description + "<size=125%></b>\n";
                if (currentEvent.Options[i].FoodChange > 0) optionTMPs[i].text += posFormat("Food");
                if (currentEvent.Options[i].FoodChange < 0) optionTMPs[i].text += negFormat("Food");
                if (currentEvent.Options[i].FunChange > 0) optionTMPs[i].text += posFormat("Fun");
                if (currentEvent.Options[i].FunChange < 0) optionTMPs[i].text += negFormat("Fun");
                if (currentEvent.Options[i].FortificationChange > 0) optionTMPs[i].text += posFormat("Fort");
                if (currentEvent.Options[i].FortificationChange < 0) optionTMPs[i].text += negFormat("Fort");
                if (currentEvent.Options[i].DARLChange > 0) optionTMPs[i].text += posFormat("DARL");
                if (currentEvent.Options[i].DARLChange < 0) optionTMPs[i].text += negFormat("DARL");
            }
            for (int i = currentEvent.Options.Count; i < optionTMPs.Count; i++)
            {
                // TODO: deactivate other options beyond JSON
                optionTMPs[i].text = "";
            }

            foreach (SpriteRenderer eventSprite in eventSprites)
            {
                eventSprite.enabled = false;
            }
            defaultSprite.enabled = false;

            foreach (string flag in currentEvent.EventFlags)
            {
                if (flag == "lizard") eventSprites[0].enabled = true;
                else if (flag == "rain") eventSprites[1].enabled = true;
                else if (flag == "chest") eventSprites[2].enabled = true;
                else if (flag == "goblin") eventSprites[3].enabled = true;
                else if (flag == "magic") eventSprites[4].enabled = true;
                else if (flag == "night") eventSprites[5].enabled = true;
                else if (flag == "person") eventSprites[6].enabled = true;
                else if (flag == "ufo") eventSprites[7].enabled = true;
                else if (flag == "fire") eventSprites[8].enabled = true;
            }

            if (currentEvent.EventFlags.Count == 0)
                defaultSprite.enabled = true;
        }
    }

    void UpdateStats(int[] changes)
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
    
    public static List<Event> ParseJsonFile(TextAsset jsonAsset)
    {
        // Read the JSON file
        //var json = File.ReadAllText(filePath);
        var json = jsonAsset.text;

        // Deserialize the JSON to a list of Event objects
        var events = JsonConvert.DeserializeObject<List<Event>>(json);

        return events;
    }
}

public class EventOption
{
    public string Description { get; set; }
    public int FoodChange { get; set; }
    public int FunChange { get; set; }
    public int FortificationChange { get; set; }
    public int DARLChange { get; set; }
}

public class Event
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> EventFlags { get; set; } // New EventFlags property
    public List<EventOption> Options { get; set; }
}

















