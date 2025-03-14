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

        public int ChessType { get => chessType; set => chessType = value; }
        public bool Turn { get => turn; set => turn = value; }
        //private int depth, alpha, beta;
        public AlphaBeta(int chessTypeAssign, int depth)
        {
            ChessType = chessTypeAssign;
            turn = (ChessType == 1) ? true : false;

            if (depth == 1)
            {
                searchDepth = 1;
            }
            else if (depth == 2)
            {
                searchDepth = 3;
            }
            else if (depth == 3)
            {
                searchDepth = 5;
            }
        }



        public (int, int) runAlphaBeta(int[,] chessBoard)
        {
            bestMove = (-1, -1);
            _ = alphaBeta(chessBoard, searchDepth, int.MinValue, int.MaxValue, true);
            return bestMove;
        }


        public int alphaBeta(int[,] chessBoard, int depth, int alpha, int beta, bool isMaxPlayer)
        {
            if (depth == 0)
                return (evaluateBoard(chessBoard));

            int bestValue = isMaxPlayer ? int.MinValue : int.MaxValue;

            foreach (var move in Program.checkAllNextMoves(chessBoard, turn))
            {
                int row = move.Item1;
                int col = move.Item2;

                int[,] newBoard = Program.deepCopyBoard(Program.chessBoardRunOne(chessBoard, row, col, turn));
                int value = alphaBeta(newBoard, depth - 1, alpha, beta, !isMaxPlayer);

                if (isMaxPlayer)
                {
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = (row, col);
                    }
                    alpha = Math.Max(alpha, value);
                }
                else
                {
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestMove = (row, col);
                    }
                    beta = Math.Min(beta, value); //comparing with value and both value are the same implementation
                }

                if (beta <= alpha) break;
            }

            return bestValue;
        }

        private int evaluateBoard(int[,] chessBoard)
        {
            int score = 0;
            int size = chessBoard.GetLength(0);
            int opponent = (ChessType == 1) ? 2 : 1;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (chessBoard[i, j] == ChessType)
                    {
                        score++;
                    }
                    else if (chessBoard[i, j] == opponent)
                    {
                        score--;
                    }
                }
            }
            return score;
        }


    }
}
