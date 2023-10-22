using Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerLite : MonoBehaviour
{
    [SerializeField] private AudioSource currentSong = null;
    [SerializeField] private Dictionary<GameState, AudioSource> songsDict;
    [SerializeField] private GameState currentState;

    public AudioSource DefaultSong;
    public AudioSource BattleSong;
    public AudioSource MenuSong;

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
        if (CombatManager.singleton.CheckCombat())
        {
            this.currentState = GameState.Battle;
        }
        else
        {
            this.currentState = GameState.Default;
        }

        this.currentSong.mute = true;
        this.currentSong = this.songsDict[this.currentState];
        this.currentSong.mute = false;
    }

    private enum GameState
    {
        Menu,
        Default,
        Battle
    }
}
