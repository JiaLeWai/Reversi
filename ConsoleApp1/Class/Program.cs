
//#define DEBUG
#undef DEBUG
#define DEBUGVALIDMOVE
//#undef DEBUGVALIDMOVE
//#define DEBUGUNDO
//#undef DEBUGUNDO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int size = 8; //chessBoard size

            int mode = chooseMode();

            //if (mode == 1)
            //{
            Console.WriteLine("AI Difficulties - 1:Easy\t2: Medium\t3:Hard");
            playWithAI(2,3);
            //}
            //else if (mode == 2)
            //{
            //    int[,] chessBoard = chessBoardInitialise(size);

            //    playMultiplayer(chessBoard);

            //    checkTotalChess(chessBoard, true);
            //}


            Console.ReadKey(true);

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

        public static void displayChessBoard(int[,] chessBoard, bool turn)
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

        static void playMultiplayer(int[,] chessBoard)
        {
            int size = chessBoard.GetLength(0);
            int totalMove = size*size; //totalMove
            int moveCount = 4; // 4 chess is aldy placed in the board
            bool turn = false; //false = black, true = white
            int noMoveCountNum = -1;
            List<(int, int)> validMove = new List<(int, int)>();
            List<(int[,] chessBoard, bool turn)> chessBoardRound = new List<(int[,], bool)>
            {
                (deepCopyBoard(chessBoard), turn) //add the initial chessboard to list
            };

            while (moveCount <= totalMove)
            {
                checkTotalChess(chessBoard, false);

                validMove = checkAllNextMoves(chessBoard, turn, true);
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

#if DEBUGUNDO
                debugUndo(chessBoardRound);
#endif

                string moveInput = Console.ReadLine();

                if (moveInput.Length == 1 && moveInput == "9" && chessBoardRound.Count>1) // undo one move
                {
                    (chessBoard, turn) = undoMove(chessBoardRound);
                    moveCount--;
                    continue;
                }
                else if (moveInput.Length == 2)
                {
                    int rowMove = moveInput[0] - '0' - 1;
                    int colMove = moveInput[1] - '0' - 1;

                    if (rowMove >= 0 && rowMove < 8 && colMove >= 0 && colMove < 8) //check the range of the input
                    {
                        if (!validMove.Contains((rowMove, colMove))) //check if the move is valid
                        {
                            Console.WriteLine("Invalid Move, Please type a new position!");
                            continue;
                        }

                        chessBoard = chessBoardRunOne(chessBoard, rowMove, colMove, turn);
                        chessBoardRound.Add((deepCopyBoard(chessBoard), !turn));
                        turn = !turn;
                        moveCount++;
                    }
                    else 
                    { 
                        Console.WriteLine("Invalid Move, Please type a new position!");
                        continue;
                    }
                }
                else //input is not valid
                { 
                    Console.WriteLine("Invalid Move, Please make sure the input is valid!");
                    continue;
                } 
            }

            Console.WriteLine("\nGame Ends!");
        }

        public static int[,] chessBoardRunOne(int[,] chessBoard, int rowMove, int colMove, bool turn)
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

        public static List<(int,int)> checkAllNextMoves(int[,] chessBoard, bool turn, bool player)
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
                            validMoves.Add((i, j));
                        }
                    }
                }
            }

#if DEBUGVALIDMOVE
            if (player)
            {
                Console.WriteLine("\nValidMoves: ");
                foreach (var moves in validMoves)
                {
                    Console.WriteLine("(" + (moves.Item1 + 1) + ", " + (moves.Item2 + 1) + ")");
                }
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

        public static int[,] deepCopyBoard (int[,]chessBoard)
        {
            int size = chessBoard.GetLength(0);
            int [,] copy = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    copy[i, j] = chessBoard[i, j];
                }
            }
            return copy;
        }


        static (int[,], bool) undoMove(List<(int[,] chessBoard, bool turn)> chessBoardRound)
        {
            chessBoardRound.RemoveAt(chessBoardRound.Count - 1);
            return (deepCopyBoard(chessBoardRound.LastOrDefault().chessBoard), chessBoardRound.LastOrDefault().turn);
        }

#if DEBUGUNDO
        static void debugUndo (List<(int[,] chessBoard, bool turn)> chessBoardRound)
        {
            for (int i = 0; i<chessBoardRound.Count; i++)
            {
                Console.WriteLine("\nChessBoardRound:" + i);
                displayChessBoard(chessBoardRound[i].chessBoard, chessBoardRound[i].turn);
            }
        }
#endif

        static int chooseMode()
        {
            int mode = -1;

            Console.WriteLine("\nWelcome to Reversi Game. Please choose one of the game modes!\n1: Single Player \t2: MultiPlayer");

            while(!(mode == 1 || mode == 2))
            {
                string input = Console.ReadLine();

                mode = int.Parse(input);
                if(!(mode == 1 || mode == 2))
                    Console.WriteLine("Incorrect mode, Please choose again!");
            }

            return mode;
        }

        static (int,int) playerMove(int [,] chessBoard, bool turn, List<(int, int)> validMove, int chessBoardRound_Count)
        {

            while (true)
            {
                displayChessBoard(chessBoard, turn);

                Console.WriteLine("ValidMove Counts: " + validMove.Count);
                Console.WriteLine("[Row][Column]: ");
                string moveInput = Console.ReadLine();

                int rowMove = moveInput[0] - '0' - 1;
                int colMove = moveInput[1] - '0' - 1;

                if (rowMove >= 0 && rowMove < 9 && colMove >= 0 && colMove < 9) //check the range of the input
                {
                    if (!validMove.Contains((rowMove, colMove))) //check if the move is valid
                    {
                        Console.WriteLine("Invalid Move, Please type a new position!");
                        continue;
                    }
                    else
                    {
                        return (rowMove, colMove);
                    }
                }
                else if (moveInput.Length == 1 && moveInput == "9" && chessBoardRound_Count >1) // undo one move
                {
                    return (9, 9);
                }

                else
                {
                    Console.WriteLine("Invalid Move, Please type a new position!");
                    continue;
                }

            }
        }

        static void playWithAI(int chessType, int AI_level)
        {
            AlphaBeta AI_player = new AlphaBeta(chessType, AI_level);
            int size = 8;
            int[,] chessBoard = chessBoardInitialise(size);
            int totalMove = size * size;
            int moveCount = 4;
            int noMoveCountNum = -1;
            bool turn = false;
            List<(int, int)> validMove = new List<(int, int)>();
            List<(int[,] chessBoard, bool turn)> chessBoardRound = new List<(int[,], bool)>
            {
                (deepCopyBoard(chessBoard), turn) //add the initial chessboard to list
            };

            while(moveCount <= totalMove)
            {
                int rowMove = -1, colMove = -1;

                checkTotalChess(chessBoard, false);

                if (AI_player.Turn == turn)
                    validMove = checkAllNextMoves(chessBoard, turn, false);
                else
                    validMove = checkAllNextMoves(chessBoard, turn, true);


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

               
                if (AI_player.Turn == turn)
                {
                    (rowMove, colMove) = AI_player.runAlphaBeta(Program.deepCopyBoard(chessBoard));
                    Console.WriteLine("AIMove: " + (rowMove + 1) + (colMove + 1 ));
                   
                }
                else
                {
                    (rowMove, colMove) = playerMove(chessBoard, turn, validMove, chessBoardRound.Count);

                    if(rowMove == 9)
                    {
                        (chessBoard, turn) = undoMove(chessBoardRound);
                        moveCount--;
                        continue;
                    }

                }
                chessBoard = chessBoardRunOne(chessBoard, rowMove, colMove, turn);
                //displayChessBoard(chessBoard, turn);
                //Console.ReadKey();
                chessBoardRound.Add((deepCopyBoard(chessBoard), !turn));
                turn = !turn;
                moveCount++;
            }

            Console.WriteLine("\nGame Ends!");
            checkTotalChess(chessBoard, true);

        }
    }


}
