using System;

class Program
{
    const char HiddenCell = '*';
    const char MineCell = 'X';

    static void Main()

    {
        // Asking user about field size (size > 3)
        Console.WriteLine("Enter the field size (more than 3, less then 30):"); 
        int size;
        while (!int.TryParse(Console.ReadLine(), out size) || size < 3 || size > 30)
        {
            Console.WriteLine("Invalid input. Enter a positive integer greater than or equal to 3 for the field size:");
        }

        //Asking user about mine counts
        Console.WriteLine("Enter the number of mines:");
        int minesCount;
        while (!int.TryParse(Console.ReadLine(), out minesCount) || minesCount < 0 || minesCount >= size * size)
        {
            Console.WriteLine("Invalid input. Enter a positive integer less than the number of cells:");
        }

        // Create and initialize the game field
        char[,] field = new char[size, size];
        bool[,] mineField = new bool[size, size];
        field = FilledCells(field);
        mineField = FilledMineCells(mineField);
        // Place mines on the field
        mineField = PlaceMines(mineField, minesCount);

        // Main game loop
        bool gameOver = false;
        int uncoveredCells = 0;

        while (!gameOver)
        {
            DrawField(field);

            //Asking user for coordinats for making turn
            Console.WriteLine("Enter the coordinates of the cell (row, column):");
            string[] input = Console.ReadLine().Split(',');
            int row, col;
            int.TryParse(input[0], out row);
            int.TryParse(input[1], out col);
            row = row-1;
            col = col-1;

            //Checking that coordinates are correct
            while (input.Length != 2 || row < 0 || row >= size || col < 0 || col >= size)
            {
                Console.WriteLine("Invalid input. Enter the coordinates of the cell (row, column):"); 
                input = Console.ReadLine().Split(',');
                int.TryParse(input[0], out row);
                int.TryParse(input[1], out col);
                row = row - 1;
                col = col - 1;

            }

            //checking for a mine on users coordinates
            if (IsMine(mineField, row, col))
            {
                field = UncoverMineCells(field, mineField);
                DrawField(field);
                Console.WriteLine("Game over! You hit a mine.");
                gameOver = true;
            }
            else
            {
                int neighboringMines = CountNeighboringMines(mineField, row, col);
                field[row, col] = neighboringMines.ToString()[0];
                uncoveredCells++;
                if (neighboringMines == 0)
                { 

                    field = UncoverNeighboringCells(field, mineField, row, col, ref uncoveredCells);
                }

                //checking for a winning
                if (uncoveredCells == size * size - minesCount)
                {
                    DrawField(field);
                    Console.WriteLine("Congratulations! You won the game.");
                    gameOver = true;
                }
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    //Fill field by hiden cells
    static char[,] FilledCells(char[,] filed)
    {
        int size = filed.GetLength(0);
        for(int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++)
            {
                filed[i, j] = HiddenCell;
            }
        } 
        return filed;
    }

    static bool[,] FilledMineCells(bool[,] minefield)
    {
        int size = minefield.GetLength(0);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                minefield[i, j] = false;
            }
        }
        return minefield;
    }
    //Set mines by random coordinates with random value
    static bool[,] PlaceMines(bool[,] field, int minesCount)
    {
        int size = field.GetLength(0);
        Random random = new Random();

        for (int i = 0; i < minesCount; i++)
        {
            int row = random.Next(0, size);
            int col = random.Next(0, size);

            // Check if the cell already contains a mine, if so, generate new coordinates
            if (field[row, col] == true)
            {
                i--;
                continue;
            }

            field[row, col] = true;
        }

        return field;
    }
    static char[,] UncoverNeighboringCells(char[,] field, bool[,] mineField, int row, int col, ref int uncoverCells)
    {
        int size = field.GetLength(0);

        // Check all neighboring cells in a 3x3 grid centered around the current cell
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newRow = row + i;
                int newCol = col + j;

                // Skip the current cell
                if (i == 0 && j == 0)
                    continue;

                // Check if the neighboring cell is within the bounds of the field
                if (newRow >= 0 && newRow < size && newCol >= 0 && newCol < size)
                {
                    // If the neighboring cell is hidden, uncover it and recursively uncover its neighbors if necessary
                    if (field[newRow, newCol] == HiddenCell)
                    {
                        int countNeighboringMines = CountNeighboringMines(mineField, newRow, newCol);
                        field[newRow, newCol] = countNeighboringMines.ToString()[0];
                        uncoverCells++;
                        //checking if neighboring cells have mine, if they are, we exit loop
                        if (countNeighboringMines > 0)
                        {
                            continue;
                        }
                        //call recursion function
                        field = UncoverNeighboringCells(field, mineField, newRow, newCol, ref uncoverCells);
                    }
                }
            }
        }
        return field;
    }

    // Draw the game field
    static void DrawField(char[,] field)
    {
        int size = field.GetLength(0);

        Console.WriteLine("Field:");
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Console.Write(field[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    // Check if the cell contains a mine
    static bool IsMine(bool[,] field, int row, int col)
    {
        return field[row, col] == true;
    }

    // Count the number of neighboring mines
    static int CountNeighboringMines(bool[,] field, int row, int col)
    {
        int count = 0;
        int size = field.GetLength(0);

        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = col - 1; j <= col + 1; j++)
            {
                if (i >= 0 && i < size && j >= 0 && j < size && field[i, j] == true)
                {
                    count++;
                }
            }
        }
        return count;
    }

    static char[,] UncoverMineCells(char[,] field, bool[,] mineField)
    {
        int size = mineField.GetLength(0);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (mineField[i, j] == true)
                {
                    field[i, j] = MineCell;
                }

            }
        }
        return field;
    }
}
