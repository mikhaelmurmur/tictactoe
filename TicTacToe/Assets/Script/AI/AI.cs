using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : Singleton<AI>
{
    int columns = 3;
    int rows = 3;
    Turn startTurn;
    Pair choice;
    public GameManager gameManager;
    public Pair GetNextMove(int[,] board, int rows, int columns, Turn turn)
    {
        this.columns = columns;
        this.rows = rows;
        startTurn = turn;
        Minimax(board, 0, turn);
        return choice;
    }

    bool IsGameOver(int[,] board)
    {
        return gameManager.CheckGame(board);
    }

    int GetScore(int[,] board, int depth)
    {
        Turn winner = gameManager.GetWinner(board);
        if (winner != startTurn)
        {
            return depth-10;
        }
        else
        {
            return 10 - depth;
        }
    }

    int Minimax(int[,] board, int depth, Turn turn)
    {
        depth++;
        if (IsGameOver(board))
        {
            return GetScore(board, depth);
        }
        List<Pair> freeCells = new List<Pair>();
        List<int> scores = new List<int>();

        #region checkingAllAvailableMoves
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (board[row, column] == -1)
                {
                    Pair availableMove = new Pair();
                    availableMove._column = column;
                    availableMove._row = row;
                    freeCells.Add(availableMove);
                    int score = 0;
                    if (turn == Turn.cross)
                    {
                        board[row, column] = 1;
                        score = Minimax(board, depth, Turn.zero);
                    }
                    else
                    {
                        board[row, column] = 2;
                        score = Minimax(board, depth, Turn.cross);
                    }
                    board[row, column] = -1;
                    scores.Add(score);
                }
            }
        }

        #endregion

        #region estimating score
        if (scores.Count > 0)
        {
            if (turn != startTurn)
            {
                int minScoreIndex = scores.IndexOf(scores.Min(x => x));
                choice = freeCells[minScoreIndex];
                return scores[minScoreIndex];
            }
            else
            {
                int maxScoreIndex = scores.IndexOf(scores.Max(x => x));
                choice = freeCells[maxScoreIndex];
                return scores[maxScoreIndex];
            }
        }
        else
        {
            return -10;
        }
        #endregion
    }
}
