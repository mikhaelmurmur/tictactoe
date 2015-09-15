using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Turn startTurn = Turn.cross;
    [SerializeField]
    TapElementController[] cells;
    [SerializeField]
    GameObject cross, zero;
    Turn turn;
    List<GameObject> elementsOnTheBoard = new List<GameObject>();
    int[,] boardCheck = new int[3, 3];
    const int ROWS = 3;
    const int COLUMNS = 3;
    void OnEnable()
    {
        DontDestroyOnLoad(this);
        EventManager.Instance.Add(EventManager.events.CellTaped, TurnDone);
        EventManager.Instance.Add(EventManager.events.GameEnded, ResetBoard);
        turn = startTurn;
        ResetBoard(null);
    }

    void OnDisable()
    {
        EventManager.Instance.Remove(EventManager.events.CellTaped, TurnDone);
        EventManager.Instance.Remove(EventManager.events.GameEnded, ResetBoard);
    }

    public enum Turn
    {
        cross,
        zero
    }


    void TurnDone(object[] args)
    {
        int row = (int)args[0];
        int column = (int)args[1];
        foreach (TapElementController cell in cells)
        {
            if ((cell.GetPosition()._row == row) && (cell.GetPosition()._column == column))
            {
                if (cell.isAvailable)
                {
                    if (turn == Turn.cross)
                    {
                        elementsOnTheBoard.Add((GameObject)Instantiate(cross, cell.transform.position, cell.transform.rotation));
                        turn = Turn.zero;
                        boardCheck[row, column] = 1;
                    }
                    else
                    {
                        elementsOnTheBoard.Add((GameObject)Instantiate(zero, cell.transform.position, cell.transform.rotation));
                        turn = Turn.cross;
                        boardCheck[row, column] = 2;
                    }
                    cell.FillCell();
                    if(CheckGame())
                    {
                        EventManager.Instance.Call(EventManager.events.GameEnded, null);
                    }
                }
            }
        }

    }

    void ResetBoard(object[] obj)
    {
        foreach (GameObject element in elementsOnTheBoard)
        {
            Destroy(element);
        }
        for (int row = 0; row < ROWS;row++)
        {
            for(int column=0;column<COLUMNS;column++)
            {
                boardCheck[row, column] = -1;
            }
        }
        foreach(TapElementController cell in cells)
        {
            cell.SetFree();
        }
    }

    bool CheckGame()
    {
        for (int row = 0; row < ROWS; row++)
        {
            if (TicTacToeLineCheck(boardCheck[row, 0], boardCheck[row, 1], boardCheck[row, 2]))
                return true;
        }
        for (int column = 0; column < COLUMNS; column++)
        {
            if (TicTacToeLineCheck(boardCheck[0, column], boardCheck[1, column], boardCheck[2, column]))
                return true;
        }

        if (TicTacToeLineCheck(boardCheck[0, 0], boardCheck[1, 1], boardCheck[2, 2]))
            return true;

        if (TicTacToeLineCheck(boardCheck[0, 2], boardCheck[1, 1], boardCheck[2, 0]))
            return true;

        return false;
    }

    bool TicTacToeLineCheck(int one, int two, int three)
    {
        if ((one == -1) || (two == -1) || (three == -1))
            return false;
        return ((one == two) && (two == three));
    }
}
