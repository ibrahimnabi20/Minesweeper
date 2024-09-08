using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        private int rows = 10;
        private int columns = 10;
        private int numberOfMines = 10;
        private MineFieldElement[,] mineField;
        private DispatcherTimer timer;
        private int elapsedTime;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            Grid.Children.Clear(); // Clears the game grid for resetting the game.
            mineField = new MineFieldElement[rows, columns];
            CreateMineField();
            PlaceMines();
            CalculateNeighborMines();
            StartTimer();
        }

        private void CreateMineField()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Button btn = new Button();
                    btn.Tag = new MineFieldElement(row, col);  // Assign a minefield element to each button.
                    btn.Click += Button_Click;
                    Grid.Children.Add(btn);
                    mineField[row, col] = (MineFieldElement)btn.Tag;
                }
            }
        }

        private void PlaceMines()
        {
            Random rand = new Random();
            int minesPlaced = 0;

            while (minesPlaced < numberOfMines)
            {
                int row = rand.Next(rows);
                int col = rand.Next(columns);

                if (!mineField[row, col].HasMine)
                {
                    mineField[row, col].HasMine = true;
                    minesPlaced++;
                }
            }
        }

        private void CalculateNeighborMines()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (!mineField[row, col].HasMine)
                    {
                        mineField[row, col].NeighboringMines = CountNeighboringMines(row, col);
                    }
                }
            }
        }

        private int CountNeighboringMines(int row, int col)
        {
            int count = 0;

            // Check 8 surrounding cells
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i;
                    int c = col + j;
                    if (r >= 0 && r < rows && c >= 0 && c < columns && mineField[r, c].HasMine)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MineFieldElement element = btn.Tag as MineFieldElement;

            if (element.HasMine)
            {
                MessageBox.Show("Game Over!");
                timer.Stop();
                RevealMines();
                return;
            }

            btn.Content = element.NeighboringMines.ToString();
            btn.IsEnabled = false;

            CheckForWin();
        }

        private void RevealMines()
        {
            foreach (Button btn in Grid.Children)
            {
                MineFieldElement element = btn.Tag as MineFieldElement;
                if (element.HasMine)
                {
                    btn.Content = ":(";
                }
            }
        }

        private void CheckForWin()
        {
            foreach (Button btn in Grid.Children)
            {
                MineFieldElement element = btn.Tag as MineFieldElement;
                if (!element.HasMine && btn.IsEnabled)
                {
                    return; 
                 // Not all non-mine cells have been revealed yet.
                }
            }

            timer.Stop();
            MessageBox.Show($"Congratulations! You won in {elapsedTime} seconds.");
        }

        private void StartTimer()
        {
            elapsedTime = 0;
            // Start the timer at 0.
            TimerBlock.Text = "0"; 
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            TimerBlock.Text = elapsedTime.ToString(); // Update timer every second.
        }

        private void ResetGame(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            InitializeGame(); 
            // Reset the game when the reset button is clicked.

        }
    }

    // Represents each square in the Minesweeper game.
    public class MineFieldElement
    {
        public int Row { get; }
        public int Column { get; }
        public bool HasMine { get; set; }
        public int NeighboringMines { get; set; }

        public MineFieldElement(int row, int column)
        {
            Row = row;
            Column = column;
            HasMine = false;
            NeighboringMines = 0;
        }
    }
}
