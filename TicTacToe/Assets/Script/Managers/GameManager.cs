using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Turn
{
    firstPlayer,
    secondPlayer
}

public struct Scoring
{
    public int firstPlayerPoints, secondPlayerPoint, draws;
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
    private Turn AITurn;
    bool isAI = false;
    PlayerEntity firstPlayer, secondPlayer;
    Scoring scoringList = new Scoring() { firstPlayerPoints = 0, secondPlayerPoint = 0, draws = 0 };

    private void OnEnable()
    {
        firstPlayer = new PlayerEntity();
        secondPlayer = new PlayerEntity();
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
        if (EventManager.Instance != null)
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

    private void DoTurn(object[] param)
    {
        int row = (int)param[0];
        int column = (int)param[1];
        if (!isCellEmpty(row, column)) return;

        Side side = GetPlayerSide(currentTurn);
        GameObject mark = GetPlayerSideObject(side);
        elementsOnBoard.Add(mark);
        TapElementController cell = GetCellByCoord(row, column);
        mark.transform.position = cell.transform.position;
        cell.FillCell();
        SetTurnInterpretation(side, row, column);
        Side winner = GameManager.GetWinner(board);
        if (winner != Side.empty)
        {
            RefreshScore(winner);
            FillAllCells(ref board);
        }
        else
        {
            if (GameManager.IsFreeCellAvailable(board))
            {
                currentTurn = ChangeTurn(currentTurn);
                if (isAI && currentTurn != AITurn)
                {
                    AI.Pair move = AI.GetNextTurn(board, secondPlayer.side); //maybe a mistake with player's side
                    DoTurn(new object[] { move.row, move.column });
                }
            }
            else
            {
                RefreshScore(winner);
                FillAllCells(ref board);
            }
        }
    }

    private void RefreshScore(Side winner)
    {
        if (winner == Side.empty)
        {
            scoringList.draws++;
            return;
        }
        if (firstPlayer.side == winner)
        {
            scoringList.firstPlayerPoints++;
        }
        else
        {
            scoringList.secondPlayerPoint++;
        }
    }

    public static bool IsFreeCellAvailable(Side[,] board)
    {
        int rows = board.GetLength(0);
        int columns = board.GetLength(1);
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (board[row, column] == Side.empty)
                    return true;
            }
        }
        return false;
    }

    public static Side GetWinner(Side[,] board)
    {
        Side winner = Side.empty;
        int rows = board.GetLength(0);
        int columns = board.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            if (board[row, 0] == board[row, 1] && board[row, 1] == board[row, 2]
                && board[row, 0] != Side.empty && board[row, 0] != Side.nothing)
                winner = board[row, 0];
        }

        for (int column = 0; column < columns; column++)
        {
            if (board[0, column] == board[1, column] && board[1, column] == board[2, column]
                && board[0, column] != Side.empty && board[0, column] != Side.nothing)
                winner = board[0, column];
        }

        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2]
            && board[1, 1] != Side.empty && board[1, 1] != Side.nothing)
            winner = board[1, 1];
        if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0]
            && board[1, 1] != Side.empty && board[1, 1] != Side.nothing)
            winner = board[1, 1];

        return winner;
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
        if (side == Side.zero)
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
                AITurn = currentTurn;
            }
            else
            {
                isAI = false;
            }
        }
        else
        {
            isAI = true;
            AITurn = currentTurn;
        }
    }

    private void FillAllCells(ref Side[,] board)
    {
        int rows = ROWS;
        int columns = COLUMNS;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                board[row, column] = Side.nothing;
            }
        }
    }

    public void BackToMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    public void StartNewGame()
    {
        ResetBoard();
        ChangeStartTurn();
        AssignSides(startTurn);
        if (AITurn != startTurn)
        {
            SetAIMode();
        }
    }
}
