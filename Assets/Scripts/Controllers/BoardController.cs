using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent;
    public event Action EndGameEvent;
    public bool IsBusy { get; private set; }

    [SerializeField] Camera m_cam;
    [SerializeField] Item itemPref;

    [SerializeField] Board m_board;
    [SerializeField] Holder m_holder;

    private GameManager.eLevelMode gameMode;
    private GameSettings gameSettings;

    private bool m_gameWin;
    private bool m_gameOver;

    public void StartGame(GameManager m_gameManager, GameSettings gameSettings, GameManager.eLevelMode gameMode)
    {
        m_gameManager.StateChangedAction += OnGameStateChange;

        this.gameSettings = gameSettings;
        this.gameMode = gameMode;

        m_gameOver = false;
        m_gameWin = false;

        m_board.Init(transform, gameSettings, itemPref, m_holder, this);
        m_holder.Init(gameSettings);

        m_holder.OnLose += EndGameCondition;

        FillFirstTime();
    }

    private void FillFirstTime()
    {
        m_board.FirstTimeFill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_WIN:
                m_gameWin = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }

    public void HandleItemSelected(Item item)
    {
        if (m_gameWin) return;
        if (m_gameOver) return;

        if (item.state == Item.State.ON_BOARD)
        {
            m_board.RemoveItem(item);
            m_holder.AddItem(item);
        }
        else if(item.state == Item.State.ON_HOLDER)
        {
            m_holder.RemoveItem(item);
            m_board.AddItem(item);
        }

        if (gameMode != GameManager.eLevelMode.TIMER)
        {
            item.OnItemSelected -= HandleItemSelected;
        }
    }

    internal void Clear()
    {
        m_board.Clear();
        m_holder.Clear();
    }

    public void EndGameCondition()
    {
        m_gameOver = true;
        EndGameEvent?.Invoke();
    }
}
