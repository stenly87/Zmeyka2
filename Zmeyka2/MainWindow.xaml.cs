using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Linq;

using Zmeyka2.Элементы;


namespace Zmeyka2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _elementSize = 55;
        double _gameWidth;
        double _gameHeight;
        int _numberOfRows;
        int _numberOfColumns;

        Apple _apple;
        Random _randoTron;
        DispatcherTimer _gameTimer;
        List<Snake> _snake;
        Snake _tailBackup;

        Direction _currentDirection;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            InitializeGame();
            base.OnContentRendered(e);
        }

        void InitializeGame()
        {
            _randoTron = new Random();
            InistalizeTimer();
            DrawGame();
            InitializeSnake();
            DrawSnake();
        }

        void ResetGame()
        {
            if (_gameTimer != null)
            {
                _gameTimer.Stop();
                _gameTimer.Tick -= MainGameLoop;
                _gameTimer = null;
            }
            if (Game != null)
            {
                Game.Children.Clear();
            }
            _apple = null;
            if (_snake != null)
            {
                _snake.Clear();
                _snake = null;
            }
            _tailBackup = null;
        }


        private void InitializeSnake()
        {
            _snake = new List<Snake>();
            _snake.Add(new Snake(_elementSize)
            {
                X = _numberOfColumns / 2 * _elementSize,
                Y = _numberOfRows / 2 * _elementSize,
                IsHead = true
            });

            _currentDirection = Direction.Right;
        }

        private void DrawGame()
        {
            _gameWidth = Game.ActualWidth;
            _gameHeight = Game.ActualHeight;
            _numberOfColumns = (int)_gameWidth / _elementSize;
            _numberOfRows = (int)_gameHeight / _elementSize;

            for (int i = 0; i < _numberOfRows; i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = 0;
                line.Y1 = i * _elementSize;
                line.X2 = _gameWidth;
                line.Y2 = i * _elementSize;
                Game.Children.Add(line); 
            }
            for (int i = 0; i < _numberOfColumns; i++)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = i * _elementSize;
                line.Y1 = 0;
                line.X2 = i * _elementSize;
                line.Y2 = _gameHeight;
                Game.Children.Add(line);
            }
        }

        private void InistalizeTimer()
        {
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(0.5);
            _gameTimer.Tick += MainGameLoop;
            _gameTimer.Start();
        }

        private void MakeGameFaster()
        {
            _gameTimer.Interval = _gameTimer.Interval - TimeSpan.FromSeconds(0.1);
        }


        private void MainGameLoop(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            DrawSnake();
            CreateApple();
            DrawApples();
        }

        private void DrawSnake()
        {
            foreach (var snake in _snake)
            {
                if (!Game.Children.Contains(snake.UIElement))
                    Game.Children.Add(snake.UIElement);

                Canvas.SetLeft(snake.UIElement, snake.X);
                Canvas.SetTop(snake.UIElement, snake.Y);
            }
        }
        private void DrawApples()
        {
            if (_apple == null)
                return;
            if (!Game.Children.Contains(_apple.UIElement))
                Game.Children.Add(_apple.UIElement);
            Canvas.SetLeft(_apple.UIElement, _apple.X);
            Canvas.SetTop(_apple.UIElement, _apple.Y);
        }

        private void CreateApple()
        {
            if (_apple != null)
                return;
                _apple = new Apple(_elementSize) {
                    X = _randoTron.Next(0, _numberOfColumns) * _elementSize,
                    Y = _randoTron.Next(0, _numberOfRows) * _elementSize
                };
        }

        private void CheckCollision()
        {
            CheckCollisionWithWorldBounds();
            CheckCollisionWithSelf();
            CheckCollisionWithWorldItems();
        }

        private void CheckCollisionWithWorldBounds()
        {
            Snake snakeHead = GetSnakeHead();

            if (snakeHead.X > _gameWidth - _elementSize || snakeHead.X < 0 || snakeHead.Y < 0 || snakeHead.Y > _gameHeight - _elementSize)
            {
                MessageBox.Show("Игра окончена! Заново?");
                ResetGame();
                InitializeGame();
            }
        }

        private void CheckCollisionWithWorldItems()
        {
            if (_apple == null)
                return;
            Snake head = _snake[0];
            if (head.X == _apple.X && head.Y == _apple.Y)
            {
                Game.Children.Remove(_apple.UIElement);
                GrowSnake();
                MakeGameFaster();
                _apple = null;
            }
        }

        private void GrowSnake()
        {
            _snake.Add(new Snake(_elementSize) {X = _tailBackup.X, Y = _tailBackup.Y });
        }

        private void CheckCollisionWithSelf()
        {
            Snake snakeHead = GetSnakeHead();
            bool hadCollision = false;
            if (snakeHead != null)
            {
                foreach (var snake in _snake)
                {
                    if (!snake.IsHead)
                    {
                        if (snake.X == snakeHead.X && snake.Y == snakeHead.Y)
                        {
                            hadCollision = true;
                            break;
                        }
                    }
                }
            }
            if (hadCollision)
            {
                MessageBox.Show("Игра окончена!");
                ResetGame();
                InitializeGame();
            }
        }

        private Snake GetSnakeHead()
        {
            Snake snakeHead = null;
            foreach (var snake in _snake)
            {
                if (snake.IsHead)
                {
                    snakeHead = snake;
                    break;
                }
            }
            return snakeHead;
        }


        private void MoveSnake()
        {
            Snake head = _snake[0];
            Snake tail = _snake[_snake.Count - 1];
            _tailBackup = tail;
            _tailBackup = new Snake(_elementSize)
            {
                X = tail.X,
                Y = tail.Y
            };

            head.IsHead = false;
            tail.IsHead = true;
            tail.X = head.X;
            tail.Y = head.Y;
            switch (_currentDirection)
            {
                case Direction.Right:
                    tail.X += _elementSize;
                    break;
                case Direction.Left:
                    tail.X -= _elementSize;
                    break;
                case Direction.Up:
                    tail.Y -= _elementSize;
                    break;
                case Direction.Down:
                    tail.Y += _elementSize;
                    break;
                default:
                    break;

            }

            _snake.RemoveAt(_snake.Count - 1);
            _snake.Insert(0, tail);

        }

        private void KeyWasReleased(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    if(_currentDirection != Direction.Down)
                    _currentDirection = Direction.Up;
                    break;
                case Key.A:
                    if (_currentDirection != Direction.Right)
                        _currentDirection = Direction.Left;
                    break;
                case Key.S:
                    if (_currentDirection != Direction.Up)
                        _currentDirection = Direction.Down;
                    break;
                case Key.D:
                    if (_currentDirection != Direction.Left)
                        _currentDirection = Direction.Right;
                    break;
            }
        }
    }


    enum Direction{ 
        Right,
        Left,
        Up,
        Down
    }
}
