using Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManagerLite : MonoBehaviour
{
    [SerializeField] private AudioSource currentSong = null;
    [SerializeField] private Dictionary<GameState, AudioSource> songsDict;
    [SerializeField] private GameState currentState;

    public Slider MusicVolumeSlider;
    public Slider GeneralVolumeSlider;

    public AudioSource DefaultSong;
    public AudioSource BattleSong;
    public AudioSource MenuSong;

    public AudioClip stepClip;
    public AudioClip shootClip;

    private void Start()
    {
        this.songsDict = new Dictionary<GameState, AudioSource>();
        this.currentState = GameState.Default;

        this.songsDict[GameState.Battle] = BattleSong;
        this.songsDict[GameState.Default] = DefaultSong;
        this.songsDict[GameState.Menu] = MenuSong;

        this.currentSong = this.songsDict[GameState.Default];
    }

    private void Update()
    {
        Scene currScene = SceneManager.GetActiveScene();
        if (currScene.name == "MainMenuBackground")
            this.currentState = GameState.Menu;

        if (CombatManager.singleton != null)
        {
            if (CombatManager.singleton.CheckCombat())
            {
                this.currentState = GameState.Battle;
            }
            else
            {
                this.currentState = GameState.Default;
            }

            foreach (CombatEntity e in CombatManager.singleton.GetCombatEntitiesOfTeam(Team.Enemy))
            {
                AudioSource eAudioSource = e.GetComponent<AudioSource>();
                if (eAudioSource == null)
                    continue;
                if (eAudioSource.isPlaying)
                    continue;
                eAudioSource.PlayOneShot(stepClip);
            }
            foreach (CombatEntity e in CombatManager.singleton.GetCombatEntitiesOfTeam(Team.Player))
            {
                AudioSource eAudioSource = e.GetComponent<AudioSource>();
                if (eAudioSource == null)
                    continue;
                if (eAudioSource.isPlaying)
                    continue;
                TowerAI towerScript = e.GetComponent<TowerAI>();
                if (towerScript == null)
                    continue;
                if (towerScript.GetAgro() != null)
                {
                    eAudioSource.PlayOneShot(stepClip);
                }
            }
        }


        this.currentSong.mute = true;
        this.currentSong = this.songsDict[this.currentState];
        this.currentSong.mute = false;
        this.currentSong.volume = VolumeSlider.value;
    }

    private enum GameState
    {
        Menu,
        Default,
        Battle
    }
}
