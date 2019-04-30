using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace BrickBreak
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        //Tamaño Canvas
        private int _gameWidth = 1200;
        private int _gameHeight = 768;

        //Pausar juego
        private bool _pause = true;

        //Velocidad y direccion pelota
        private double _xMove = 2;
        private double _yMove = 2;

        //Pelota posición
        private double _balLocX = 480;
        private double _balLocY = 150;
        private int _ballSpeed = 6;

        //Board Variables
        private int _boardLoc = 15;
        private double _boardSpeed = 1;
        private int _boardSpeedScreen = 10;

        //Block variables 
        //Definimos width y height de los bloques
        private readonly int _blockWH = 30;

        //Block Margin
        private readonly int _blockM = 10;

        //Game life
        private int _life = 3;

        //Ball thread
        private Thread _ballMoveThread;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }
        //inicializar todos los componentes
        public void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //obtener tamaño pantalla
            _gameWidth = (int)CanvasGame.ActualWidth;
            _gameHeight = (int)CanvasGame.ActualHeight;
            //this.
            _boardSpeedScreen = _gameHeight / 80;
            //Definir localisacion de la esfera
            _balLocX = _gameWidth / 2;
            _balLocY = _gameHeight / 3;
            SetBallLocation(_balLocX, _balLocY);
            //Llamando a la funcion que dibuja los bloques
            DrawBlocks();
            _ballMoveThread = new Thread(BallMovement);
            _ballMoveThread.Start();

            _boardLoc = _gameWidth / 2 - 75;
            Canvas.SetLeft(Board, _boardLoc);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); //boton cerrar, termina con los procedimientos actuales
        }

        //Draw Blocks
        private void DrawBlocks()
        {
            //Primera linea de bloques
            for (var i=1; i*(_blockWH+_blockM)+_blockM < _gameWidth - _blockWH - 2*_blockM; i++)
            {
                var rec = new Rectangle { Width = _blockWH, Height = _blockWH, Fill = Brushes.Gold };
                CanvasGame.Children.Add(rec);
                Canvas.SetTop(rec, 120);
                Canvas.SetLeft(rec, i * 40 + 10);
            }
            for (var i=2; i * 40+10 < _gameWidth - 3 * 40 + 10; i++)
            {
                var rec = new Rectangle { Width = 30, Height = 30, Fill = Brushes.Indigo };
                CanvasGame.Children.Add(rec);
                Canvas.SetTop(rec, 180);
                Canvas.SetLeft(rec, i * 40 + 10);
            }
        }
        #region Board move
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _pause = false;
            //Si presionamos tecla derecha se mueve a la derecha
            if (e.Key == Key.Right)
            {
                if (Canvas.GetLeft(Board) <= _gameWidth - _boardSpeedScreen * _boardSpeed - 150)
                    Canvas.SetLeft(Board, Canvas.GetLeft(Board) + _boardSpeedScreen * _boardSpeed);
                else
                    Canvas.SetLeft(Board, _gameWidth - 150);

            }
            //Si presionamos tecla izquierda se mueve a la izquierda
            if (e.Key == Key.Left)
            {
                if (Canvas.GetLeft(Board) >= _boardSpeedScreen)
                    Canvas.SetLeft(Board, Canvas.GetLeft(Board) - _boardSpeedScreen * _boardSpeed);
                else
                    Canvas.SetLeft(Board, 0);

            }
            //Mientras se precione la tecla la velocidad aumenta 0.05;
            _boardSpeed += 0.05;
            _boardLoc = (int)Canvas.GetLeft(Board);
        }
        private void Windows_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _boardSpeed = 1;
        }
        #endregion
        #region Ball Move
        //Check Edges

            //Lado izquierdo
        private void CheckLeftBorder()
        {
            //Revertir la direccion de la esfera cuando toque este lado
            if (_balLocX <= 0)
                _xMove *= -1;
        }

        private void CheckRightBorder()
        {
            if (_balLocX + 30 >= _gameWidth)
                _xMove *= -1;
        }
        //Revisar si la esfera toca la barra
        private void CheckBoard()
        {
            //La esfera toca la barra
            if(_balLocX + 30 - 5 >= _boardLoc && _balLocX + 5 <= _boardLoc + 150)
            {
                //Si la esfera toca el centro de la barra
                if(_balLocX >= _boardLoc && _balLocY <= _boardLoc + 120)
                {
                    if(_balLocY + 95 >= _gameHeight && _yMove > 0)
                    {
                        _yMove = -2;
                        _ballSpeed = 6;
                    }
                }

                //La esfera toca un extremo de la barra
                else
                {
                    if(_balLocY + 95 >= _gameHeight && _yMove > 0)
                    {
                        _yMove = -2.5;
                        _ballSpeed = 8;
                    }
                }
            }
        }
        #endregion

        private void SetBallLocation(double x, double y)
        {
            void Act()
            {
                Canvas.SetLeft(Ballota, x);
                Canvas.SetTop(Ballota, y);
            }
            Dispatcher.BeginInvoke((Action)Act);
        }
        //Esfera Loop
        private void BallMovement()
        {
            while (true)
            {
                if (!_pause)
                {
                    //Check sides
                    CheckLeftBorder();
                    CheckRightBorder();
                    CheckBoard();

                    //Movimiento esfera
                    _balLocX += _xMove;
                    _balLocY += _yMove;

                    //Dibuujar la esfera
                    SetBallLocation(_balLocX, _balLocY);
                }
                Thread.Sleep(_ballSpeed);
            }
        }
    }
}
