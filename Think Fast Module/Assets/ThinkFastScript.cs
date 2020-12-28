using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class ThinkFastScript : MonoBehaviour {

    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    public KMNeedyModule Module;
    public KMAudio Audio;
    public KMSelectable Button;

    private string[] Insults = { "buffoon", "idiot", "dunce", "dumbass", "twat", "halfwit", "nincompoop", "dolt", "dimwit", "nitwit", "wally", "dummy", "dumbo", "goon", "dum-dum", "fool", "imbecile", "dullard", "moron", "simpleton", "clod", "ninny", "jackass", "numbskull", "squarehead", "dingleberry", "pinhead", "airhead", "fathead", "numpty", "dipstick", "turkey", "dunderhead", "thickhead", "muttonhead", "lamebrain", "pea-brain", "blockhead", "spoony", "noodle", "twit", "donkey", "muppet", "simp", "birdbrain", "pillock", "wanker", "tosser", "minger", "nutter", "knob head", "berk", "maggot", "lazy sod", "git" };
    private Settings _Settings;

    class Settings
    {
        public int ActivationTime = 5;
        public bool FakeActivationSounds = true;
    }

    void GetSettings()
    {
        var SettingsConfig = new ModConfig<Settings>("ThinkFast");
        _Settings = SettingsConfig.Settings; // This reads the settings from the file, or creates a new file if it does not exist
        SettingsConfig.Settings = _Settings; // This writes any updates or fixes if there's an issue with the file
    }

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        GetSettings();
        Module.CountdownTime = _Settings.ActivationTime;
        Module.OnActivate += delegate { Audio.PlaySoundAtTransform("activate", Button.transform); };
        Module.OnTimerExpired += delegate { Module.HandleStrike(); Debug.LogFormat("[Think Fast #{0}] You struck on Think Fast, you {1}.", _moduleID, Insults[Rnd.Range(0,Insults.Length)]); };
        Button.OnInteract += delegate { StartCoroutine(ButtonPress()); return false; };
        StartCoroutine(ButtonRotate());
        if (_Settings.FakeActivationSounds)
        {
            StartCoroutine(BeAnnoying());
        }
    }

    private IEnumerator ButtonRotate()
    {
        while (true)
        {
            Button.transform.localEulerAngles += new Vector3(0,0.05f,0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator BeAnnoying()
    {
        while (true)
        {
            yield return new WaitForSeconds(Rnd.Range(10f,20f));
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.NeedyActivated, Button.transform);
        }
    }

    private IEnumerator ButtonPress()
    {
        Button.AddInteractionPunch();
        for (int i = 0; i < 3; i++)
        {
            Button.transform.localPosition -= new Vector3(0,0.005f,0);
            yield return new WaitForSeconds(0.01f);
        }
        Audio.PlaySoundAtTransform("press", Button.transform);
        Module.HandlePass();
        for (int i = 0; i < 3; i++)
        {
            Button.transform.localPosition += new Vector3(0, 0.005f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Send any command to press the button. Note that you cannot send empty commands (so don't send \"!{0}\" with nothing after it). The module's time can be increased in mod settings to account for stream lag.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        Button.OnInteract();
        yield return null;
    }

    // bruh momento
}
