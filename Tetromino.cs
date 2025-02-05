using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Tetromino
    {
 
        public int[,] Shape { get; private set; } // Форма фигуры
        public int PositionX { get; set; } // Позиция по X
        public int PositionY { get; set; } // Позиция по Y
        public ConsoleColor Color { get; private set; }
        private static readonly ConsoleColor[] Colors =
    {
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.Blue,
        ConsoleColor.Yellow,
        ConsoleColor.Cyan,
        ConsoleColor.Magenta,
        ConsoleColor.White,
        ConsoleColor.DarkBlue,
        ConsoleColor.DarkCyan,
        ConsoleColor.DarkMagenta,
        ConsoleColor.DarkYellow,
        ConsoleColor.Gray,
        ConsoleColor.DarkGreen,
        ConsoleColor.DarkRed
    };

        public Tetromino(int[,] shape)
        {
            Shape = shape;
            PositionX = Game.Width / 2 - shape.GetLength(0) / 2; // Центрируем по горизонтали
            PositionY = 0; // Начинаем с верхней границы поля
            Random rand = new Random();
            Color = Colors[rand.Next(Colors.Length)];
        }

        public void Move(int offsetX, int offsetY)
        {
            PositionX += offsetX;
            PositionY += offsetY;
        }

        public void Rotate(int direction = +1)
        {
            Shape = direction == +1 ? RotateClockwise(Shape) : RotateCounterClockwise(Shape);
        }

        private static int[,] RotateClockwise(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    rotated[j, rows - i - 1] = matrix[i, j];

            return rotated;
        }
        public static Tetromino GenerateRandomTetromino()
        {
            Random rand = new Random();
            int type = rand.Next(7); // Генерируем случайное число от 0 до 6
            
            return type switch
            {
                0 => new Tetromino(new int[,] { { 1, 1, 1 }, { 0, 1, 0 } }), // T-образная фигура
                1 => new Tetromino(new int[,] { { 1, 1 }, { 1, 1 } }),       // Квадратная фигура
                2 => new Tetromino(new int[,] { { 1, 1, 1 }, { 1, 0, 0 } }), // L-образная фигура
                3 => new Tetromino(new int[,] { { 1, 1, 1 }, { 0, 0, 1 } }), // J-образная фигура
                4 => new Tetromino(new int[,] { { 0, 1, 1 }, { 1, 1, 0 } }), // S-образная фигура
                5 => new Tetromino(new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }), // Z-образная фигура
                _ => new Tetromino(new int[,] { { 1 }, { 1 }, { 1 }, { 1 } }) // Прямая линия (I)
            };
        }
        private static int[,] RotateCounterClockwise(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    rotated[cols - j - 1, i] = matrix[i, j];

            return rotated;
        }
    }
}
