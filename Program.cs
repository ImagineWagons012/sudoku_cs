using System.Text.RegularExpressions;

namespace Sudoku_cs
{
    class Pos
    {
        public int x;
        public int y;
        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    class BoardManager
    {
        private int[,] currentBoard;
        private bool[,] changeable;
        public Pos cursorPos;
        public bool running;
        void resetColor()
        {
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private void writeTopRow(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Write("┌");
            for (int j = 1; j < 37; j++)
            {
                if (j == 36)
                {
                    Console.Write("┐");
                }
                else if (j % 4 == 0)
                {
                    Console.Write("┬");
                }
                else
                {
                    Console.Write("─");
                }

            }
            resetColor();
            Console.WriteLine();
        }
        private void writeInbetweenRows(ConsoleColor color, int i)
        {
            for (int j = 1; j < 37; j++)
            {
                if (i % 3 == 2)
                {
                    Console.BackgroundColor = color;
                }
                if (j == 12 || j == 24)
                {
                    Console.BackgroundColor = color;
                    if (i == 8)
                    {
                        Console.Write("┴");
                    }
                    else
                    {
                        Console.Write("┼");
                    }
                    resetColor();
                }
                else if (j == 36)
                {
                    Console.BackgroundColor = color;
                    if (i == 8)
                    {
                        Console.Write("┘");
                    }
                    else
                    {
                        Console.Write("┤");
                    }
                    resetColor();
                }
                else if (j % 4 == 0 && j != 0)
                {
                    if (i == 8)
                    {
                        Console.Write("┴");
                    }
                    else
                    {
                        Console.Write("┼");
                    }
                }
                else
                {
                    Console.Write("─");
                }

            }
            resetColor();
        }

        private void writeRows(ConsoleColor bgColor, ConsoleColor cursorColor, int i)
        {
            Console.BackgroundColor = bgColor;
            Console.Write("│");
            resetColor();
            for (int j = 0; j < 9; j++)
            {
                resetColor();
                if (j == cursorPos.x && i == cursorPos.y)
                {
                    Console.BackgroundColor = cursorColor;
                }
                Console.Write($" {(currentBoard[j, i] == 0 ? " " : currentBoard[j, i])} ");
                resetColor();
                if (j % 3 == 2)
                {
                    Console.BackgroundColor = bgColor;
                }
                Console.Write("│");
                resetColor();
            }
            Console.WriteLine();
            if (i % 3 == 2)
            {
                Console.BackgroundColor = bgColor;
            }
            Console.BackgroundColor = bgColor;
            if (i == 8)
            {
                Console.Write("└");
            }
            else
            {
                Console.Write("├");
            }
            resetColor();
        }

        private void writeBoard()
        {
            Console.SetCursorPosition(0, 0);
            writeTopRow(ConsoleColor.DarkMagenta);
            for (int i = 0; i < 9; i++)
            {
                writeRows(ConsoleColor.DarkMagenta, ConsoleColor.DarkBlue, i);
                writeInbetweenRows(ConsoleColor.DarkMagenta, i);
                Console.WriteLine();
            }
            resetColor();
        }

        private void writeHelp()
        {
            Console.Write(@"
Digits 1-9: Writes that digit where the cursor is if the cell can be changed
Arrows: Move cursor
C: Clears the board
N: Generates new board
S: Solves the board from current state
Delete/Backspace: Remove number
Last action: ");
        }

        private bool checkGroups()
        {
            int xOffset = 0;
            int yOffset = 0;
            List<int> found = new();
            bool result = true;
            bool checkGroup()
            {
                found = new();
                for (int i = xOffset * 3; i < xOffset * 3 + 3; i++)
                {
                    for (int j = yOffset * 3; j < yOffset * 3 + 3; j++)
                    {
                        if (!found.Contains(currentBoard[i, j]))
                        {
                            found.Add(currentBoard[i, j]);
                        }
                    }
                }
                if (!found.Contains(0) && found.Count > 8)
                {
                    return true;
                }
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    yOffset = i;
                    xOffset = j;
                    result = checkGroup();
                }
            }
            return result;
        }

        private bool checkRows()
        {
            bool checkRow(int row)
            {
                List<int> found = new();
                for (int i = 0; i < 9; i++)
                {
                    if (!found.Contains(currentBoard[i, row]))
                    {
                        found.Add(currentBoard[i, row]);
                    }
                }
                return !found.Contains(0) && found.Count > 8;
            }
            bool result = true;
            for (int i = 0; i < 9; i++)
            {
                result = checkRow(i);
            }
            return result;
        }
        private bool checkColumns()
        {
            bool checkColumn(int column)
            {
                List<int> found = new();
                for (int i = 0; i < 9; i++)
                {
                    if (!found.Contains(currentBoard[column, i]))
                    {
                        found.Add(currentBoard[column, i]);
                    }
                }
                return !found.Contains(0) && found.Count > 8;
            }
            bool result = true;
            for (int i = 0; i < 9; i++)
            {
                result = checkColumn(i);
            }
            return result;
        }
        private bool checkBoard()
        {
            checkRows();
            checkColumns();
            if (checkGroups() && checkRows() && checkColumns())
            {
                return true;
            }
            return false;
        }

        private bool canBePlaced(int x, int y, int value)
        {
            // group
            int xOffset = x / 3;
            int yOffset = y / 3;
            for (int i = xOffset * 3; i < (xOffset + 1) * 3; i++)
            {
                for (int j = yOffset * 3; j < (yOffset + 1) * 3; j++)
                {
                    if (currentBoard[i, j] == value)
                    {
                        return false;
                    }
                }
            }

            // row
            for (int i = 0; i < 9; i++)
            {
                if (currentBoard[i, y] == value)
                {
                    return false;
                }
            }

            // column
            for (int i = 0; i < 9; i++)
            {
                if (currentBoard[x, i] == value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool solvable()
        {
            bool checkGroup(int x, int y)
            {
                List<int> found = new();
                for (int i = x * 3; i < x * 3 + 3; i++)
                {
                    for (int j = y * 3; j < y * 3 + 3; j++)
                    {
                        if (!found.Contains(currentBoard[i, j]))
                        {
                            found.Add(currentBoard[i, j]);
                        }
                        else if (currentBoard[i, j] == 0)
                        {
                            continue;
                        }
                        else return false;
                    }
                }
                return true;
            }

            bool checkColumn(int column)
            {
                List<int> found = new();
                for (int i = 0; i < 9; i++)
                {
                    if (!found.Contains(currentBoard[column, i]))
                    {
                        found.Add(currentBoard[column, i]);
                    }
                    else if (currentBoard[column, i] == 0)
                    {
                        continue;
                    }
                    else return false;
                }
                return true;
            }

            bool checkRow(int row)
            {
                List<int> found = new();
                for (int i = 0; i < 9; i++)
                {
                    if (!found.Contains(currentBoard[i, row]))
                    {
                        found.Add(currentBoard[i, row]);
                    }
                    else if (currentBoard[i, row] == 0)
                    {
                        continue;
                    }
                    else return false;
                }
                return true;
            }

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (!checkGroup(x, y))
                        return false;
                }
            }
            for (int i = 0; i < 9; i++)
            {
                if (!checkColumn(i))
                    return false;
                if (!checkRow(i))
                    return false;
            }
            return true;
        }
        private bool solveBoard(int nthSolution)
        {
            if (!solvable())
            {
                Console.Write(" NO POSSIBLE SOLUTIONS!");
                return false;
            }
            int i = 0;
            int solutions = 0;
            bool recursive()
            {

                if (currentBoard[i % 9, i / 9] != 0)
                {
                    if (i == 80)
                    {
                        if (solutions + 1 >= nthSolution)
                        {
                            return true;
                        }
                        else
                        {
                            solutions++;
                            return false;
                        }
                    }
                    i++;
                    if (recursive())
                    {
                        return true;
                    }
                    else
                    {
                        i--;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    if (canBePlaced(i % 9, i / 9, j))
                    {
                        currentBoard[i % 9, i / 9] = j;
                        if (i == 80)
                        {
                            if (solutions + 1 >= nthSolution)
                            {
                                return true;
                            }
                            else
                            {
                                solutions++;
                                return false;
                            }
                        }
                        i++;
                        if (recursive())
                        {
                            return true;
                        }
                        else
                        {
                            i--;
                        }
                    }
                }
                currentBoard[i % 9, i / 9] = 0;
                return false;
            }
            return recursive();
        }

        private void swapColumn(int a, int b)
        {
            int temp;
            int other;
            int group = a / 3;

            List<int> helper = new()
            {
                group * 3,
                group * 3 + 1,
                group * 3 + 2
            };
            helper.Remove(a);

            other = helper[b];

            for (int i = 0; i < 9; i++)
            {
                temp = currentBoard[a, i];
                currentBoard[a, i] = currentBoard[other, i];
                currentBoard[other, i] = temp;
            }
        }
        private void swapRow(int a, int b)
        {
            int temp;
            int other;
            int group = a / 3;

            List<int> helper = new()
            {
                group * 3,
                group * 3 + 1,
                group * 3 + 2
            };
            helper.Remove(a);

            other = helper[b];
            for (int i = 0; i < 9; i++)
            {
                temp = currentBoard[i, a];
                currentBoard[i, a] = currentBoard[i, other];
                currentBoard[i, other] = temp;
            }
        }

        private void swapGroupColumn(int a, int b)
        {
            int temp;
            int other;
            int group = a;

            List<int> helper = new()
            {
                group,
                (group + 1) % 3,
                (group + 2) % 3
            };
            helper.Remove(a);

            other = helper[b];

            for (int j = 0; j < 3; j++)
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        temp = currentBoard[a*3 + x, j * 3 +y];
                        currentBoard[a*3 + x, j*3 + y] = currentBoard[other * 3 + x, j * 3 + y];
                        currentBoard[other * 3 + x, j * 3 + y] = temp;
                    }
                }
            }
        }

        private void swapGroupRow(int a, int b)
        {
            int temp;
            int other;
            int group = a;

            List<int> helper = new()
            {
                group % 3,
                (group + 1) %3,
                (group + 2)%3
            };
            helper.Remove(a);

            other = helper[b];
            a *= 3;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    // magic happens
                    temp = currentBoard[j * 3 + i / 3, a + i % 3];
                    currentBoard[j * 3 + i / 3, a + i % 3] =
                    currentBoard[j * 3 + i / 3, other * 3 + i % 3];
                    currentBoard[j * 3 + i / 3, other * 3 + i % 3] = temp;
                }
            }
        }

        private bool trySolveBoard(int solves)
        {
            int[,] tmp = (int[,])currentBoard.Clone();
            bool result = solveBoard(solves);
            currentBoard = (int[,])tmp.Clone();
            return result;
        }

        private void newBoard()
        {
            Random r = new();
            solveBoard(r.Next(1, 500));
            for (int i = 0; i < r.Next(100, 400); i++)
            {
                switch (r.Next(4))
                {
                    case 0:
                        swapColumn(i % 9, r.Next(2));
                        break;
                    case 1:
                        swapRow(i % 9, r.Next(2));
                        break;
                    case 2:
                        swapGroupColumn(i % 3, r.Next(2));
                        break;
                    case 3:
                        swapGroupRow(i % 3, r.Next(2));
                        break;
                }
            }
            int taken = 0;
            int lastTaken;
            int iterations = 0;
            int x;
            int y;
            int n = r.Next(20, 30);
            while (taken < n)
            {
                if (iterations > 300)
                {
                    break;
                }

                x = r.Next(9);
                y = r.Next(9);

                lastTaken = currentBoard[x, y];
                currentBoard[x, y] = 0;
                if (!trySolveBoard(2)) // check if the board is not a unique solve
                {
                    currentBoard[x, y] = lastTaken;
                }
                else
                {
                    taken++;
                }
                iterations++;
            }
            // if (taken < 20)
            // {
            //     newBoard();
            // }
            Console.Clear();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    changeable[i,j] = currentBoard[i,j] == 0;
                }
            }
        }

        private void clearBoard()
        {
            currentBoard = new int[9, 9];
            changeable = new bool[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    changeable[i, j] = true;
                }
            }
            Console.Clear();
        }

        public void update()
        {
            writeBoard();
            writeHelp();
            checkBoard();
        }

        private void setCell(int x, int y, int value)
        {
            if (changeable[x, y])
            {
                currentBoard[x, y] = value;
            }
        }
        public BoardManager()
        {
            this.currentBoard = new int[9, 9];
            this.changeable = new bool[9, 9];
            this.cursorPos = new(0, 0);
            this.running = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    changeable[i, j] = true;
                }
            }
        }
        public void Input(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Escape:
                    running = false;
                    break;
                case ConsoleKey.LeftArrow:
                    cursorPos.x--;
                    if (cursorPos.x < 0)
                    {
                        cursorPos.x = 8;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    cursorPos.x++;
                    cursorPos.x %= 9;
                    break;
                case ConsoleKey.UpArrow:
                    cursorPos.y--;
                    if (cursorPos.y < 0)
                    {
                        cursorPos.y = 8;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    cursorPos.y++;
                    cursorPos.y %= 9;
                    break;
                case ConsoleKey.D1:
                    setCell(cursorPos.x, cursorPos.y, 1);
                    break;
                case ConsoleKey.D2:
                    setCell(cursorPos.x, cursorPos.y, 2);
                    break;
                case ConsoleKey.D3:
                    setCell(cursorPos.x, cursorPos.y, 3);
                    break;
                case ConsoleKey.D4:
                    setCell(cursorPos.x, cursorPos.y, 4);
                    break;
                case ConsoleKey.D5:
                    setCell(cursorPos.x, cursorPos.y, 5);
                    break;
                case ConsoleKey.D6:
                    setCell(cursorPos.x, cursorPos.y, 6);
                    break;
                case ConsoleKey.D7:
                    setCell(cursorPos.x, cursorPos.y, 7);
                    break;
                case ConsoleKey.D8:
                    setCell(cursorPos.x, cursorPos.y, 8);
                    break;
                case ConsoleKey.D9:
                    setCell(cursorPos.x, cursorPos.y, 9);
                    break;
                case ConsoleKey.Backspace:
                    setCell(cursorPos.x, cursorPos.y, 0);
                    break;
                case ConsoleKey.Delete:
                    setCell(cursorPos.x, cursorPos.y, 0);
                    break;
                case ConsoleKey.N:
                    newBoard();
                    break;
                case ConsoleKey.C:
                    clearBoard();
                    break;
                case ConsoleKey.S:
                    solveBoard(1);
                    break;
                default:
                    break;
            }
            update();
        }
    }
    static class Program
    {
        public static void Main()
        {
            Console.Clear();
            Console.CursorVisible = false;
            BoardManager bm = new();
            bm.update();
            Random r = new();
            while (bm.running)
            {
                bm.Input(Console.ReadKey().Key);
            }
            Console.Clear();
            Console.CursorVisible = true;
        }
    }
}