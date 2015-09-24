using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Turn
{
    firstPlayer,
    secondPlayer
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    const int COLUMNS = 3, ROWS = 3;
    [SerializeField]
    TapElementController[] cells;
    [SerializeField]
    GameObject crossElement, zeroElement;
    List<GameObject> elementsOnBoard = new List<GameObject>();
    Side[,] board = new Side[ROWS, COLUMNS];
    Turn startTurn = Turn.firstPlayer;
    Turn currentTurn;
    bool isAI = false;
    PlayerEntity firstPlayer, secondPlayer;

    private void OnEnable()
    {
        ResetBoard();
        ChangeStartTurn();
        EventManager.Instance.Add(EventManager.events.CellTaped, DoTurn);
        AssignSides(startTurn);
    }

    private void AssignSides(Turn startTurn)
    {
        if (startTurn == Turn.firstPlayer)
        {
            firstPlayer.side = Side.cross;
            secondPlayer.side = Side.zero;
        }
        else
        {
            firstPlayer.side = Side.zero;
            secondPlayer.side = Side.cross;
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.Remove(EventManager.events.CellTaped, DoTurn);
    }

    private void ChangeStartTurn()
    {
        startTurn = ChangeTurn(startTurn);
        currentTurn = startTurn;
    }

    private Turn ChangeTurn(Turn turn)
    {
        if (turn == Turn.firstPlayer)
        {
            return Turn.secondPlayer;
        }
        else
        {
            return Turn.firstPlayer;
        }
    }

    void DoTurn(object[] param)
    {
        int row = (int)param[0];
        int column = (int)param[1];
        if (isCellEmpty(row, column))
        {
            Side side = GetPlayerSide(currentTurn);
            GameObject mark = GetPlayerSideObject(side);
            elementsOnBoard.Add(mark);
            TapElementController cell = GetCellByCoord(row, column);
            mark.transform.position = cell.transform.position;
            cell.FillCell();
            SetTurnInterpretation(side, row, column);

            //check whether the game has ended

            currentTurn = ChangeTurn(currentTurn);
        }
    }
    

    private void SetTurnInterpretation(Side side, int row, int column)
    {
        board[row, column] = side;
    }

    private TapElementController GetCellByCoord(int row, int column)
    {
        foreach (TapElementController cell in cells)
        {
            if ((cell.row == row) && (cell.column == column))
            {
                return cell;
            }
        }
        throw new System.Exception("No cell with such row|column found!!!!");
    }

    private Side GetPlayerSide(Turn turn)
    {
        if (turn == Turn.firstPlayer)
        {
            return firstPlayer.side;
        }
        else
        {
            return secondPlayer.side;
        }
    }

    private GameObject GetPlayerSideObject(Side side)
    {
        if (side == Side.cross)
        {
            return Instantiate(crossElement);
        }
        if(side == Side.zero)
        {
            return Instantiate(zeroElement);
        }
        else
        {
            throw new System.Exception("Some bad side");
        }

    }

    private bool isCellEmpty(int row, int column)
    {
        return board[row, column] == Side.empty;
    }

    private void ClearBoardInterpretation()
    {
        for (int row = 0; row < ROWS; row++)
        {
            for (int column = 0; column < COLUMNS; column++)
            {
                board[row, column] = Side.empty;
            }
        }
    }

    private void ResetBoard()
    {
        foreach (TapElementController cell in cells)
        {
            cell.SetFree();
        }
        foreach (GameObject element in elementsOnBoard)
        {
            Destroy(element);
        }
        ClearBoardInterpretation();
        SetAIMode();
        ChangeStartTurn();
        AssignSides(startTurn);
    }

    private void SetAIMode()
    {
        if (PlayerPrefs.HasKey("mode"))
        {
            if (PlayerPrefs.GetInt("mode") == 0)
            {
                isAI = true;
            }
            else
            {
                isAI = false;
            }
        }
        else
        {
            isAI = true;
        }
    }
}
