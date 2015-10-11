using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


public static class AI
{
    private static Side mySide;
    private static Pair resultMove;
    public struct Pair
    {
        public int row, column;
    }

    static public Pair GetNextTurn(Side[,] board, Side mySide)
    {
        AI.mySide = mySide;
        MiniMaxAlgoFind(board, 0, mySide);
        return resultMove;
    }

    private static int GetScore(Side winner, int depth)
    {
        if (winner == AI.mySide)
        {
            return 10 - depth;
        }
        if (AI.mySide == Side.cross)
        {
            if (winner == Side.zero)
                return -10 + depth;
            else
            {
                return 0;
            }
        }
        else
        {
            if (winner == Side.cross)
                return -10 + depth;
            else
                return 0;
        }

    }

    private static Side[,] GetCopyBoard(Side[,] board)
    {
        Side[,] myBoard = new Side[board.GetLength(0), board.GetLength(1)];
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
                myBoard[row, column] = board[row, column];
        }
        return myBoard;
    }
    static private int MiniMaxAlgoFind(Side[,] _board, int depth, Side currentSide)
    {
        Side[,] board = GetCopyBoard(_board);
        Side winner = GameManager.GetWinner(board);
        if (board[0, 1] != Side.empty)
        {
            int k = 0;
        }
        if ((winner == Side.empty && !GameManager.IsFreeCellAvailable(board)) || winner != Side.empty)
        {
            return GetScore(winner, depth);
        }
        depth++;
        List<int> scores = new List<int>();
        List<Pair> moves = new List<Pair>();

        foreach (Pair move in GetAvailableMoves(board))
        {
            Side[,] newCombination = DoMove(board, currentSide, move.row, move.column);
            scores.Add(MiniMaxAlgoFind(newCombination, depth, ChangeSide(currentSide)));
            moves.Add(move);
        }

        if (currentSide == mySide)
        {
            int maxIndex = (from x in scores orderby x select scores.IndexOf(x)).Last();
            if (depth == 1)
                resultMove = moves[maxIndex];
            return scores[maxIndex];
        }
        else
        {
            int maxIndex = (from x in scores orderby x select scores.IndexOf(x)).First();
            if (depth == 1)
                resultMove = moves[maxIndex];
            return scores[maxIndex];
        }
    }

    private static Side ChangeSide(Side currentSide)
    {
        if (currentSide == Side.cross)
            return Side.zero;
        return Side.cross;
    }

    private static Side[,] DoMove(Side[,] board, Side currentSide, int row, int column)
    {
        Side[,] resultBoard = GetCopyBoard(board);
        resultBoard[row, column] = currentSide;
        return resultBoard;
    }

    private static List<Pair> GetAvailableMoves(Side[,] board)
    {
        List<Pair> resultPairs = new List<Pair>();
        int rows = board.GetLength(0);
        int columns = board.GetLength(1);
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (board[row, column] == Side.empty)
                {
                    Pair move = new Pair();
                    move.column = column;
                    move.row = row;
                    resultPairs.Add(move);
                }
            }
        }
        return resultPairs;
    }
}







//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;

//public class AI : Singleton<AI>
//{
//    int columns = 3;
//    int rows = 3;
//    Turn startTurn;
//    Pair choice;
//    public GameManager gameManager;
//    public Pair GetNextMove(int[,] board, int rows, int columns, Turn turn)
//    {
//        this.columns = columns;
//        this.rows = rows;
//        startTurn = turn;
//        Minimax(board, 0, turn);
//        return choice;
//    }

//    bool IsGameOver(int[,] board)
//    {
//        return gameManager.CheckGame(board);
//    }

//    int GetScore(int[,] board, int depth)
//    {
//        Turn winner = gameManager.GetWinner(board);
//        if (winner != startTurn)
//        {
//            return depth-10;
//        }
//        else
//        {
//            return 10 - depth;
//        }
//    }

//    int Minimax(int[,] board, int depth, Turn turn)
//    {
//        depth++;
//        if (IsGameOver(board))
//        {
//            return GetScore(board, depth);
//        }
//        List<Pair> freeCells = new List<Pair>();
//        List<int> scores = new List<int>();

//        #region checkingAllAvailableMoves
//        for (int row = 0; row < rows; row++)
//        {
//            for (int column = 0; column < columns; column++)
//            {
//                if (board[row, column] == -1)
//                {
//                    Pair availableMove = new Pair();
//                    availableMove._column = column;
//                    availableMove._row = row;
//                    freeCells.Add(availableMove);
//                    int score = 0;
//                    if (turn == Turn.cross)
//                    {
//                        board[row, column] = 1;
//                        score = Minimax(board, depth, Turn.zero);
//                    }
//                    else
//                    {
//                        board[row, column] = 2;
//                        score = Minimax(board, depth, Turn.cross);
//                    }
//                    board[row, column] = -1;
//                    scores.Add(score);
//                }
//            }
//        }

//        #endregion

//        #region estimating score
//        if (scores.Count > 0)
//        {
//            if (turn != startTurn)
//            {
//                int minScoreIndex = scores.IndexOf(scores.Min(x => x));
//                choice = freeCells[minScoreIndex];
//                return scores[minScoreIndex];
//            }
//            else
//            {
//                int maxScoreIndex = scores.IndexOf(scores.Max(x => x));
//                choice = freeCells[maxScoreIndex];
//                return scores[maxScoreIndex];
//            }
//        }
//        else
//        {
//            return -10;
//        }
//        #endregion
//    }
//}
