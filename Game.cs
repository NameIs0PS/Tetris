using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Game
    {
        public const int Width = 10; // Ширина игрового поля
        private const int Height = 20; // Высота игрового поля
        private int[,] field = new int[Width, Height]; // Игровое поле
        private Tetromino currentTetromino; // Текущая фигура
        private bool isGameOver; // Флаг окончания игры
        private int dropDelay = 500; // Задержка между падениями фигур (в миллисекундах)
        private bool isDroppingFast = false; // Флаг для ускоренного падения
        private int score; // Переменная для хранения очков
        private System.Diagnostics.Stopwatch stopwatch; // Переменная для отслеживания времени
        private Tetromino nextTetromino; // Следующая фигура
        private ConsoleColor[,] fieldColors = new ConsoleColor[Width, Height];
        public void Start()
        {
            stopwatch = new System.Diagnostics.Stopwatch(); // Инициализация таймера
            stopwatch.Start(); // Запуск таймера
            score = 0; // Инициализация очков
            currentTetromino = Tetromino.GenerateRandomTetromino(); // Текущая фигура
            nextTetromino = Tetromino.GenerateRandomTetromino();   // Следующая фигура
            do
            {
                InitializeField();
                currentTetromino = Tetromino.GenerateRandomTetromino();
                isGameOver = false;

                Thread inputThread = new Thread(HandleInput);
                inputThread.Start();

                while (!isGameOver)
                {
                    DrawField();
                    HandleInput(); // Обработка ввода
                    UpdateGame();
                    Thread.Sleep(isDroppingFast ? 100 : dropDelay); // Ускоряем падение при удерживании стрелки вниз
                }

                Console.Clear();
                Console.WriteLine("Game Over! Нажмите 'R', чтобы перезапустить игру или любую другую кнопку для выхода.");

                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.R)
                        {
                            break;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            } while (true);
        }
        private void InitializeField()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    field[x, y] = 0;
        }
        
        private void DrawField()
        {
            Console.Clear();

            // Отрисовка верхней границы
            Console.WriteLine(new string('═', Width + 2)); // +2 для боковых границ

            for (int y = 0; y < Height; y++)
            {
                Console.Write("║"); // Левая граница
                for (int x = 0; x < Width; x++)
                {
                    if (field[x, y] == 1) // Если ячейка занята
                    {
                        Console.ForegroundColor = fieldColors[x, y]; // Используем сохраненный цвет
                        Console.Write("█");
                        Console.ResetColor();
                    }
                    else if (IsBlockOccupied(x, y)) // Если ячейка занята текущей фигурой
                    {
                        Console.ForegroundColor = currentTetromino.Color;
                        Console.Write("█");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }

                Console.WriteLine("║"); // Правая граница
            }
            // Отрисовка нижней границы
            Console.WriteLine(new string('═', Width + 2)); // +2 для боковых границ
                                                           // Отображение прошедшего времени
            TimeSpan ts = stopwatch.Elapsed; // Получаем прошедшее время
            Console.Write($"Время: {ts.Minutes:D2}:{ts.Seconds:D2} | Очки: {score}");
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine(" ");
            Console.WriteLine("══════════════════════════════════");
            Console.WriteLine("║Управление: R Перезапуск игры   ║");
            Console.WriteLine("║  ← Влево    → Вправо           ║");
            Console.WriteLine("║  ↓  Ускорить падение           ║");
            Console.WriteLine("║  ↑  Повернуть фигуру           ║");
            Console.WriteLine("══════════════════════════════════");
        }
        public bool IsBlockOccupied(int x, int y)
            {
            // Проверка, занята ли ячейка фигурой или полем
            if (currentTetromino != null &&
                x >= currentTetromino.PositionX &&
                x < currentTetromino.PositionX + currentTetromino.Shape.GetLength(0) &&
                y >= currentTetromino.PositionY &&
                y < currentTetromino.PositionY + currentTetromino.Shape.GetLength(1))
            {
                return currentTetromino.Shape[x - currentTetromino.PositionX, y - currentTetromino.PositionY] == 1;
            }
            return field[x, y] == 1;
            }

        public void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow: MoveTetromino(-1, 0); break;
                    case ConsoleKey.RightArrow: MoveTetromino(1, 0); break;
                    case ConsoleKey.DownArrow: isDroppingFast = true; break; // Ускоряем падение
                    case ConsoleKey.UpArrow: RotateTetromino(); break;
                    case ConsoleKey.R: // Добавлено для перезапуска игры
                        isGameOver = true; // Устанавливаем флаг окончания игры
                        Console.WriteLine("Game Over! Press 'R' to restart or any other key to exit.");
                        return; // Выход из метода обработки ввода
                    default: break;
                }
            }
            else
            {
                isDroppingFast = false; // Сбрасываем флаг если клавиша не нажата
            }
        }

        private void UpdateGame()
        {
            if (!MoveTetromino(0, 1)) // Если фигура не может двигаться вниз
            {
                PlaceTetromino(); // Закрепляем фигуру на поле
                CheckForCompleteLines(); // Проверяем и удаляем заполненные линии

                // Генерируем новую фигуру
                currentTetromino = nextTetromino; // Перемещаем следующую фигуру в текущую
                nextTetromino = Tetromino.GenerateRandomTetromino(); // Генерируем новую следующую фигуру
                Console.WriteLine($"Проверка размещения новой фигуры: X={currentTetromino.PositionX}, Y={currentTetromino.PositionY}");
                // Проверяем, можно ли разместить новую фигуру
                if (!CanPlaceTetromino(currentTetromino))
                {
                    isGameOver = true; // Устанавливаем флаг окончания игры
                    Console.WriteLine("Game Over!"); // Выводим сообщение о конце игры
                }
            }
        }

        private bool MoveTetromino(int offsetX, int offsetY)
        {
            currentTetromino.Move(offsetX, offsetY);
            if (!CanPlaceTetromino(currentTetromino))
            {
                currentTetromino.Move(-offsetX, -offsetY);
                return false;
            }
            return true;
        }

        private void RotateTetromino()
        {
            currentTetromino.Rotate();
            if (!CanPlaceTetromino(currentTetromino))
                currentTetromino.Rotate(-1); // Возврат к предыдущему состоянию
        }

        private void PlaceTetromino()
        {
            if (currentTetromino == null) return; // Проверка на null

            for (int i = 0; i < currentTetromino.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < currentTetromino.Shape.GetLength(1); j++)
                {
                    if (currentTetromino.Shape[i, j] == 1) // Если блок фигуры существует
                    {
                        int x = currentTetromino.PositionX + i;
                        int y = currentTetromino.PositionY + j;

                        // Проверка на выход за границы поля
                        if (x >= 0 && x < Width && y >= 0 && y < Height)
                        {
                            field[x, y] = 1; // Устанавливаем блок на поле
                            fieldColors[x, y] = currentTetromino.Color; // Сохраняем цвет
                        }
                        else
                        {
                            // Логирование для отладки
                            //Console.WriteLine($"Ошибка: попытка записи за границы поля. x={x}, y={y}");
                        }
                    }
                }
            }
        }

        private void CheckForCompleteLines()
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                if (IsLineComplete(y))
                {
                    RemoveLine(y);
                    score += 10; // Увеличиваем очки на 100 за каждую удаленную линию
                    y++; // Проверяем эту строку снова
                }
            }
        }

        private bool IsLineComplete(int y)
        {
            for (int x = 0; x < Width; x++)
                if (field[x, y] == 0) return false;
            return true;
        }

            private void RemoveLine(int y)
        {
            for (int moveY = y; moveY > 0; moveY--)
                for (int x = 0; x < Width; x++)
                    field[x, moveY] = field[x, moveY - 1];

            // Убираем верхнюю линию
            for (int x = 0; x < Width; x++)
                field[x, 0] = 0;
        }

            private bool CanPlaceTetromino(Tetromino tetromino)
        {
            for (int i = 0; i < tetromino.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetromino.Shape.GetLength(1); j++)
                {
                    if (tetromino.Shape[i, j] == 1) // Если блок фигуры существует
                    {
                        int newX = tetromino.PositionX + i; // Новая позиция по X
                        int newY = tetromino.PositionY + j; // Новая позиция по Y

                        // Проверяем границы поля и занятость ячейки
                        if (newX < 0 || newX >= Width || newY >= Height ||
                            (newY >= 0 && field[newX, newY] == 1))
                        {
                            return false; // Невозможно разместить фигуру
                        }
                    }
                }
            }
            return true; // Фигура может быть размещена
        }
    }
}

