namespace WordSearchGPTwMAUI
{
    public partial class MainPage : ContentPage
    {
        private char[,] grid;
        private List<string> words;
        private int gridSize;
        private int difficultyLevel;

        public MainPage()
        {
            InitializeComponent();
        }

        private void GeneratePuzzle_Clicked(object sender, EventArgs e)
        {
            // Get word list and grid size from input fields
            words = WordEntry.Text.Split(',').Select(w => w.Trim()).ToList();
            gridSize = Convert.ToInt32(GridSizeEntry.Text);
            difficultyLevel = Convert.ToInt32(DifficultySlider.Value);

            // Generate word search grid
            grid = GenerateGrid(gridSize, gridSize);

            // Place words in the grid
            PlaceWords(words, grid);

            // Display the word search grid
            DisplayGrid();
        }

        private void ShowSolution_Clicked(object sender, EventArgs e)
        {
            // Display the solution
            DisplaySolution(words, grid);

        }

        private char[,] GenerateGrid(int rows, int cols)
        {
            char[,] newGrid = new char[rows, cols];
            Random random = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Generate a random letter (ASCII value between 65 and 90 for uppercase letters)
                    newGrid[i, j] = (char)(random.Next(26) + 'A');
                }
            }

            return newGrid;
        }

        private void PlaceWords(List<string> words, char[,] grid)
        {
            Random random = new Random();

            foreach (string word in words)
            {
                // Choose a random direction for the word (horizontal, vertical, or diagonal)
                int direction = random.Next(3);

                // Choose random starting position
                int row = random.Next(grid.GetLength(0));
                int col = random.Next(grid.GetLength(1));

                // Check if the word fits in the chosen direction
                int wordLength = word.Length;
                int maxRow = grid.GetLength(0) - 1;
                int maxCol = grid.GetLength(1) - 1;

                switch (direction)
                {
                    case 0: // Horizontal
                        while (col + wordLength > maxCol)
                        {
                            col = random.Next(grid.GetLength(1));
                        }
                        for (int i = 0; i < wordLength; i++)
                        {
                            grid[row, col + i] = word[i];
                        }
                        break;
                    case 1: // Vertical
                        while (row + wordLength > maxRow)
                        {
                            row = random.Next(grid.GetLength(0));
                        }
                        for (int i = 0; i < wordLength; i++)
                        {
                            grid[row + i, col] = word[i];
                        }
                        break;
                    case 2: // Diagonal
                        while (row + wordLength > maxRow || col + wordLength > maxCol)
                        {
                            row = random.Next(grid.GetLength(0));
                            col = random.Next(grid.GetLength(1));
                        }
                        for (int i = 0; i < wordLength; i++)
                        {
                            grid[row + i, col + i] = word[i];
                        }
                        break;
                }
            }
        }

        private void DisplayGrid()
        {
            WordSearchGrid.RowDefinitions.Clear();
            WordSearchGrid.ColumnDefinitions.Clear();
            WordSearchGrid.Children.Clear();

            for (int i = 0; i < gridSize; i++)
            {
                WordSearchGrid.RowDefinitions.Add(new RowDefinition());
                WordSearchGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Label label = new Label
                    {
                        Text = grid[i, j].ToString(),
                        FontSize = 20,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);

                    WordSearchGrid.Children.Add(label);
                }
            }
        }

        private bool CheckHorizontal(string word, char[,] grid, int row, int col)
        {
            if (col + word.Length > grid.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (grid[row, col + i] != word[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckVertical(string word, char[,] grid, int row, int col)
        {
            if (row + word.Length > grid.GetLength(0))
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (grid[row + i, col] != word[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDiagonal(string word, char[,] grid, int row, int col)
        {
            if (row + word.Length > grid.GetLength(0) || col + word.Length > grid.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (grid[row + i, col + i] != word[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void DisplaySolution(List<string> words, char[,] grid)
        {
            // Iterate through each word in the list
            foreach (string word in words)
            {
                // Iterate through the grid to find the word
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        // Check for horizontal, vertical, and diagonal occurrences of the word
                        if (CheckHorizontal(word, grid, i, j) ||
                            CheckVertical(word, grid, i, j) ||
                            CheckDiagonal(word, grid, i, j))
                        {
                            // Highlight the word in the grid
                            HighlightWord(word, i, j);
                        }
                    }
                }
            }
        }


        private void HighlightWord(string word, int startRow, int startCol)
        {
            // Calculate the end row and column based on the direction of the word
            int endRow = startRow;
            int endCol = startCol;
            bool horizontal = CheckHorizontal(word, grid, startRow, startCol);
            bool vertical = CheckVertical(word, grid, startRow, startCol);
            bool diagonal = CheckDiagonal(word, grid, startRow, startCol);

            if (horizontal)
            {
                endCol += word.Length - 1;
            }
            else if (vertical)
            {
                endRow += word.Length - 1;
            }
            else if (diagonal)
            {
                endRow += word.Length - 1;
                endCol += word.Length - 1;
            }

            // Highlight the word in the grid
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {
                    var label = WordSearchGrid.Children
                      .OfType<Label>()
                     .FirstOrDefault(c => Grid.GetRow(c) == i && Grid.GetColumn(c) == j);

                    if (label != null)
                    {
                        var yellowColor = Color.FromRgb(255, 255, 0); // Using RGB values for yellow

                        // Create a frame to wrap the label
                        var frame = new Frame
                        {
                            Padding = new Thickness(5),
                            BackgroundColor = yellowColor, // Setting background color to yellow
                            CornerRadius = 5,
                            Content = label
                        };


                        // Replace the label with the frame in the grid
                        // Replace the label with the frame in the grid
                        WordSearchGrid.Children.Remove(label);
                        Grid.SetRow(frame, Grid.GetRow(label)); // Set the row of the frame
                        Grid.SetColumn(frame, Grid.GetColumn(label)); // Set the column of the frame
                        WordSearchGrid.Children.Add(frame); // Add the frame to the grid
                    }
                }
            }
        }
    }

    }


