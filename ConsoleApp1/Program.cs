
//#define DEBUG
#undef DEBUG
#define DEBUGVALIDMOVE
//#undef DEBUGVALIDMOVE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    //delegate int chessBoard();
    class Program
    {
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

            

            int size = 8; //chessBoard size

            int[,] chessBoard = chessBoardInitialise(size);

            playChess(chessBoard);

            checkTotalChess(chessBoard, true);

            Console.ReadKey(true);




            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }

        static int[,] chessBoardInitialise(int size)
        {
            /* 0 = no chess, 1 = white chess, 2 = black chess*/

            int[,] chessBoard = new int[size, size]; // Initialise chessBoard with size

            int initialPointA = (size / 2) - 1, initialPointB = initialPointA + 1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((i == initialPointA) && (j == initialPointA)) || ((i == initialPointB) && (j == initialPointB))) //set initialPoint for white chess
                    {
                        chessBoard[i, j] = 1;
                    }
                    else if (((i == initialPointA) && (j == initialPointB)) || ((i == initialPointB) && (j == initialPointA)))
                    {
                        chessBoard[i, j] = 2;
                    }
                    else { chessBoard[i, j] = 0; }
                }
            }
            return chessBoard;
        }

        static void displayChessBoard(int[,] chessBoard, bool turn)
        {
            int row = chessBoard.GetLength(0);
            int column = chessBoard.GetLength(1);

            Console.WriteLine("\nTurn: " + ((turn == true) ? "White" : "Black"));
            for (int i = 0; i < row; i++)
            {
                if (i == 0)
                {
                    for (int k = 0; k < column; k++)
                    {
                        Console.Write("\t" + (k+1));
                    }
                    Console.WriteLine("\n**********************************************************************************");
                }
                for (int j = 0; j < column; j++)
                {
                    if (j == 0)
                    {
                        Console.Write(i+1 + "*");
                    }
                    Console.Write("\t" + chessBoard[i, j]);
                }
                Console.WriteLine();
            }
        }

        static void playChess(int[,] chessBoard)
        {
            int totalMove = chessBoard.GetLength(0) * chessBoard.GetLength(1); //totalMove
            int moveCount = 4; // 4 chess is aldy placed in the board
            bool turn = false; //false = black, true = white
            int noMoveCountNum = -1;
            List<(int, int)> validMove = new List<(int, int)>();

            while (moveCount <= totalMove)
            {
                checkTotalChess(chessBoard, false);

                validMove = checkAllNextMoves(chessBoard, turn);
                if (validMove.Count == 0)
                {
                    if (moveCount == noMoveCountNum)
                    {
                        Console.WriteLine("\nBoth players has no valid moves. Game Ends!");
                        break;
                    }

                    Console.WriteLine("\n" + ((turn == true) ? "White Chess" : "Black Chess") + " has not valid moves. Switching turn!");
                    turn = !turn;
                    noMoveCountNum = moveCount;
                    continue;
                }

                displayChessBoard(chessBoard, turn);

                Console.WriteLine("ValidMove Counts: " + validMove.Count);
                Console.WriteLine("[Row][Column]: ");
                string moveInput = Console.ReadLine();
                int rowMove = moveInput[0] - '0' - 1;
                int colMove = moveInput[1] - '0' - 1;


                if (!validMove.Contains((rowMove, colMove)))
                {
                    Console.WriteLine("Invalid Move, Please type a new position!");
                    continue;
                }
                chessBoard = chessBoardRunOne(chessBoard, rowMove, colMove, turn);
                turn = !turn;
                moveCount++;
            }

            Console.WriteLine("\nGame Ends!");
        }

        static int[,] chessBoardRunOne(int[,] chessBoard, int rowMove, int colMove, bool turn)
        {
            int chessType = 0;
            if (turn) { chessType = 1; } else { chessType = 2; }

            // X axis
            chessBoard = checkAxis(chessBoard, rowMove, colMove, chessType, 1);

            // Y axis 
            chessBoard = checkAxis(chessBoard, rowMove, colMove, chessType, 2);

            // \ axis 
            chessBoard = checkAxis(chessBoard, rowMove, colMove, chessType, 3);

            // / axis
            chessBoard = checkAxis(chessBoard, rowMove, colMove, chessType, 4);

            return chessBoard;
        }

        static int[,] checkAxis(int[,] chessBoard, int rowMove, int colMove, int chessType, int condition)
        {
            int size = chessBoard.GetLength(0);
            int nearestPointA = -1, nearestPointB = -1;
            int nearestPointAX = -1, nearestPointAY = -1, nearestPointBX = -1, nearestPointBY = -1;
            if (condition == 1) // X Axis
            {
                int j = colMove-1; //check surroundings, exclude itself
                for(; j >=0; j--)
                {
                    if ((chessBoard[rowMove, j] == 0)) break;
                    if ((chessBoard[rowMove, j] == chessType))
                    {
                        nearestPointA = j;
                        break;
                    }
                }

                j = colMove+1;
                for (; j<size; j++)
                {
                    if ((chessBoard[rowMove, j] == 0)) break;
                    if ((chessBoard[rowMove, j] == chessType))
                    {
                        nearestPointB = j;
                        break;
                    }
                }

                if ((nearestPointA == -1) && (nearestPointB != -1))
                {
                    j = colMove;
                    for (; j < nearestPointB; j++)
                    {
                        chessBoard[rowMove, j] = chessType;
                    }
                }
                else if ((nearestPointA != -1) && (nearestPointB == -1))
                {
                    j = colMove;
                    for (; j > nearestPointA; j--)
                    {
                        chessBoard[rowMove, j] = chessType;
                    }
                }
                else if ((nearestPointA != -1) && (nearestPointB != -1))
                {
                    j = nearestPointA + 1;
                    for (; j < nearestPointB; j++)
                    {
                        chessBoard[rowMove, j] = chessType;
                    }
                }
            }

            else if (condition == 2) // Y Axis
            {
                int j = rowMove-1;
                for (; j >= 0; j--)
                {
                    if ((chessBoard[j, colMove] == 0) && (j != rowMove)) break;
                    if ((chessBoard[j, colMove] == chessType))
                    {
                        nearestPointA = j;
                        break;
                    }
                }

                j = rowMove+1;
                for (; j < size; j++)
                {
                    if ((chessBoard[j, colMove] == 0) && (j != rowMove)) break;
                    if ((chessBoard[j, colMove] == chessType))
                    {
                        nearestPointB = j;
                        break;
                    }
                }


                //for (; j < size; j++)
                //{

                //    if ((chessBoard[j, colMove] == chessType) && (j < rowMove))
                //    {
                //        nearestPointA = j;
                //    }
                //    else if ((chessBoard[j, colMove] == chessType) && (j > rowMove))
                //    {
                //        nearestPointB = j;
                //        break; //find the nearest PointB

                //    }
                //}

                if ((nearestPointA == -1) && (nearestPointB != -1))
                {
                    j = rowMove;
                    for (; j < nearestPointB; j++)
                    {
                        chessBoard[j, colMove] = chessType;
                    }
                }
                else if ((nearestPointA != -1) && (nearestPointB == -1))
                {
                    j = rowMove;
                    for (; j > nearestPointA; j--)
                    {
                        chessBoard[j, colMove] = chessType;
                    }
                }
                else if ((nearestPointA != -1) && (nearestPointB != -1))
                {
                    j = nearestPointA + 1;
                    for (; j < nearestPointB; j++)
                    {
                        chessBoard[j, colMove] = chessType;
                    }
                }
            }

            else if (condition == 3) //Axis \
            {
                int i = rowMove-1, j = colMove-1;
                //  bool pinNearestA = false;


                for (; i >= 0 && j >= 0; i--, j--)
                {
                    if ((chessBoard[i, j] == 0)) break;
                    if ((chessBoard[i, j] == chessType) && (i<rowMove) && (j<colMove))
                    {
                        nearestPointAX = i;
                        nearestPointAY = j;
                        break;
                    }
                }

                i = rowMove+1; j = colMove+1; //Pin at the selected point again
                for (; i < size && j < size; i++, j++)
                {
                    if ((chessBoard[i, j] == 0)) break;
                    if ((chessBoard[i, j] == chessType) && (i > rowMove) && (j > colMove))
                    {
                        nearestPointBX = i;
                        nearestPointBY = j;
                        break;
                    }
                }


                i = rowMove; j = colMove;
                if ((nearestPointAX == -1) && (nearestPointBX != -1))
                {    
                    for (; i < nearestPointBX && j < nearestPointBY; i++, j++)
                    {
                        chessBoard[i, j] = chessType;
                    }
                }
                else if ((nearestPointAX != -1) && (nearestPointBX == -1))
                {
                    for (; i > nearestPointAX && j > nearestPointAY; i--, j--)
                    {
                        chessBoard[i, j] = chessType;
                    }
                }
                else if ((nearestPointAX != -1) && (nearestPointBX != -1))
                {
                    i = nearestPointAX + 1; j = nearestPointAY + 1;
                    for (; i < nearestPointBX && j<nearestPointBY; i++, j++)
                    {
                        chessBoard[i, j] = chessType;
                    }
                }
            }

            else if (condition == 4) //Axis \
            {
                int i = rowMove+1, j = colMove-1;
                for (; i < size && j >= 0; i++, j--)
                {
                    if ((chessBoard[i, j] == 0) && (i != rowMove) && j != colMove) break;
                    if ((chessBoard[i, j] == chessType) && (i > rowMove) && (j < colMove))
                    {
                        nearestPointAX = i;
                        nearestPointAY = j;
                        break;
                    }
                }

                i = rowMove-1; j = colMove+1; //Pin at the selected point again
                for (; i >=0 && j < size; i--, j++)
                {
                    if ((chessBoard[i, j] == 0) && (i != rowMove) && j != colMove) break;
                    if ((chessBoard[i, j] == chessType) && (i < rowMove) && (j > colMove))
                    {
                        nearestPointBX = i;
                        nearestPointBY = j;
                        break;
                    }
                }


                i = rowMove; j = colMove;
                if ((nearestPointAX == -1) && (nearestPointBX != -1))
                {
                    for (; i > nearestPointBX && j < nearestPointBY; i--, j++)
                    {
                        chessBoard[i, j] = chessType;
                    }
                }
                else if ((nearestPointAX != -1) && (nearestPointBX == -1))
                {
                    for (; i < nearestPointAX && j > nearestPointAY; i++, j--)
                    {
                        chessBoard[i, j] = chessType;
                    }
                }
                else if ((nearestPointAX != -1) && (nearestPointBX != -1))
                {
                    i = nearestPointAX - 1; j = nearestPointAY + 1;
                    for (; i > nearestPointBX && j < nearestPointBY; i--, j++)
                    {
                        chessBoard[i, j] = chessType;
                    }
                }
            }

#if DEBUG
            Console.WriteLine("\nCondition:" + condition);
            Console.WriteLine("ChessType:" + chessType);
            Console.WriteLine("A:" + nearestPointA);
            Console.WriteLine("B:" + nearestPointB);
            Console.WriteLine("AX:" + nearestPointAX);
            Console.WriteLine("AY:" + nearestPointAY);
            Console.WriteLine("BX:" + nearestPointBX);
            Console.WriteLine("BY:" + nearestPointBY);

            displayChessBoard(chessBoard);    
            
#else
#endif
            return chessBoard;
        }

        static List<(int,int)> checkAllNextMoves(int[,] chessBoard, bool turn)
        {
            List<(int, int)> validMoves = new List<(int, int)>();
            int size = chessBoard.GetLength(0);
            int i = 0, j = 0;

            for(; i<size; i++)
            {
                j = 0;
                for (; j<size; j++)
                {
                    if (chessBoard[i, j] == 0)
                    {
                        if (isValidMove(chessBoard, turn, i, j))
                        {
                            validMoves.Add((i+1, j+1));
                        }
                    }
                }
            }

#if DEBUGVALIDMOVE
            Console.WriteLine("\nValidMoves: ");
            foreach (var moves in validMoves)
            {
                Console.WriteLine("" + moves);
            }
#else
#endif
            return validMoves;
        }

        static bool isValidMove(int[,] chessBoard, bool turn, int row, int col)
        {
            int[,] directions = { { 0, -1 }, { 0, 1}, { -1, 0 }, { 1, 0 }, { -1, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }}; //← → ↑ ↓ ↖ ↘ ↙ ↗
            int enemy = (turn == true) ? 2 : 1;
            int player = (turn == true) ? 1 : 2;
            int size = chessBoard.GetLength(0);

            for (int i = 0; i<directions.GetLength(0); i++)
            {
                int r = row + directions[i, 0], c = col + directions[i, 1];
                bool foundEnemy = false;

                while (r >= 0 && r < size && c >= 0 && c < size && chessBoard[r, c] == enemy)
                {
                    foundEnemy = true;
                    r += directions[i, 0];
                    c += directions[i, 1];
                }

                if (r >= 0 && r < size && c >= 0 && c < size && chessBoard[r, c] == player && foundEnemy == true) return true;

            }
            return false;
        }

        static void checkTotalChess(int[,] chessBoard, bool gameEnd)
        {
            int blackChess = 0, whiteChess = 0;
            int size = chessBoard.GetLength(0);
            for (int i = 0; i<size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (chessBoard[i, j] == 0) continue;
                    else
                    {
                        _ = ((chessBoard[i, j] == 1) ? whiteChess++ : blackChess++);
                    }
                }
            }
            Console.WriteLine("\nBlack: " + blackChess + "\nWhite: " + whiteChess);

            if (gameEnd)
            {
                if (blackChess > whiteChess) Console.WriteLine("Black wins");
                else if (blackChess < whiteChess) Console.WriteLine("White wins");
                else Console.WriteLine("Fair Game");
            }
        }
    }


}
