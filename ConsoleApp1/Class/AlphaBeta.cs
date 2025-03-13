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
        //private int depth, alpha, beta;
        public AlphaBeta(int chessTypeAssign, int depth)
        {
            chessType = chessTypeAssign;
            turn = (chessType == 1) ? true : false;
            searchDepth = depth;
        }

        public (int,int) runAlphaBeta(int[,] chessBoard)
        {
            bestMove = (-1, -1);
            _ = alphaBeta(chessBoard, searchDepth, int.MinValue, int.MaxValue, true);
            return bestMove;
        }


        public int alphaBeta(int [,] chessBoard, int depth, int alpha, int beta, bool isMaxPlayer)
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
                    if(value > bestValue)
                    {
                        bestValue = value;
                        bestMove = (row, col);
                    }
                    alpha = Math.Max(alpha, value);
                }
                else
                {
                    if(value<bestValue)
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

        private int evaluateBoard (int [,] chessBoard)
        {
            int score = 0;
            int size = chessBoard.GetLength(0);
            int opponent = (chessType == 1) ? 2 : 1;

            for (int i = 0; i<size; i++)
            {
                for(int j = 0; j<size; j++)
                {
                    if (chessBoard[i,j] == chessType)
                    {
                        score++;
                    }
                    else if (chessBoard[i,j] == opponent)
                    {
                        score--;
                    }
                }
            }
            return score;
        }


    }
}
