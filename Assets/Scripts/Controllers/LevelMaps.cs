using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMaps : LevelCondition
{
    private BoardController m_board;

    public override void Setup(BoardController board)
    {
        base.Setup(board);

        m_board = board;

        m_board.EndGameEvent += GameOver;

    }

    public void GameOver()
    {
        OnConditionComplete();
    }

    protected override void OnDestroy()
    {
        if (m_board != null) m_board.EndGameEvent -= GameOver;

        base.OnDestroy();
    }
}
