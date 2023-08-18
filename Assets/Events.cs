using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

public static class PredefinedEvents
{
    public static Event introEvent = new Event()
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

    public static Event difficultyEvent = new Event()
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

    public static Event spamEvent = new Event()
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

    public static Event railEvent = new Event()
    {
        Title = "rail",
        Description = "rail",
        EventFlags = new List<string>() { "ufo" },
        Options = new List<EventOption>()
        {
            new EventOption() {Description = "rail", FoodChange = 5, FunChange = 5, FortificationChange = 5, DARLChange = 50 },
        }
    };
}

