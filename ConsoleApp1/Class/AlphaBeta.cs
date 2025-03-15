using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class AlphaBeta
    {
        private int chessType;
        private bool turn;
        private (int, int) bestMove;
        private int searchDepth;
        private int[,] weightedBoard =
        {
            { 100, -20, 10, 5, 5, 10, -20, 100 },
            { -20, -50,  -2, -2, -2,  -2, -50, -20 },
            {  10,  -2,   1,  1,  1,   1,  -2,  10 },
            { 5,  -2,   1,  0,  0,   1,  -2,   5 },
            { 5,  -2,   1,  0,  0,   1,  -2,   5 },
            { 10,  -2,   1,  1,  1,   1,  -2,  10 },
            { -20, -50,  -2, -2, -2,  -2, -50, -20 },
            { 100, -20,  10,  5,  5,  10, -20, 100 }
        };
        private int difficulty;

        public int ChessType { get => chessType; set => chessType = value; }
        public bool Turn { get => turn; set => turn = value; }
        //private int depth, alpha, beta;
        public AlphaBeta(int chessTypeAssign, int AI_level)
        {
            ChessType = chessTypeAssign;
            turn = (ChessType == 1) ? true : false;
            difficulty = AI_level;

            Random randomDepth = new Random();

            if (AI_level == 1)
            {
                searchDepth = randomDepth.Next(1, 3);
            }
            else if (AI_level == 2)
            {
                searchDepth = randomDepth.Next(3, 5);
            }
            else if (AI_level == 3)
            {
                searchDepth = randomDepth.Next(5, 32);
            }
        }



        public (int, int) runAlphaBeta(int[,] chessBoard)
        {
            bestMove = (-1, -1);
            Program.displayChessBoard(chessBoard, turn);
            _ = alphaBeta(chessBoard, searchDepth, int.MinValue, int.MaxValue, true);
            return bestMove;
        }


        public int alphaBeta(int[,] chessBoard, int depth, int alpha, int beta, bool isMaxPlayer)
        {
            Random mistakePercentage = new Random();

            if (depth == 0)
                return (evaluateBoard(chessBoard));

            int bestValue = isMaxPlayer ? int.MinValue : int.MaxValue;
            List<(int, int)> moves = Program.checkAllNextMoves(chessBoard, turn, false);

            foreach (var move in moves)
            {
                int row = move.Item1;
                int col = move.Item2;
               

                if (isMaxPlayer)
                {
                    int[,] newBoard = Program.deepCopyBoard(Program.chessBoardRunOne(chessBoard, row, col, turn));
                    int value = alphaBeta(newBoard, depth - 1, alpha, beta, !isMaxPlayer);

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = (row, col);
                    }
                    alpha = Math.Max(alpha, value);
                }
                else
                {
                    int[,] newBoard = Program.deepCopyBoard(Program.chessBoardRunOne(chessBoard, row, col, !turn));
                    int value = alphaBeta(newBoard, depth - 1, alpha, beta, !isMaxPlayer);

                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestMove = (row, col);
                    }
                    beta = Math.Min(beta, value); //comparing with value and both value are the same implementation
                }

                if (beta <= alpha) break;
            }
            
            if ((difficulty == 1 && mistakePercentage.NextDouble() < 0.45) || (difficulty == 2 && mistakePercentage.NextDouble() < 0.2))
            {
                bestMove = moves[mistakePercentage.Next(moves.Count-1)];
            }
            return bestValue;
        }

        private int evaluateBoard(int[,] chessBoard)
        {
            int score = 0;
            int size = chessBoard.GetLength(0);
            int opponent = (chessType == 1) ? 2 : 1;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (chessBoard[i, j] == chessType)
                    {
                        score += weightedBoard[i,j];
                    }
                    else if (chessBoard[i, j] == opponent)
                    {
                        score -= weightedBoard[i,j];
                    }
                }
            }
            return score;
        }


    }
}
