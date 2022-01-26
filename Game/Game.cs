using Game.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class Game : Base
    {
        #region Objects
        private GameTime _gameTime;
        /// <summary>
        /// Timer que refresca la imagen del juego
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// Evento que se desencadena al liberar el boton del mouse sobre el lienzo
        /// </summary>
        public event EventHandler<MouseEventArgs> Canvas_MouseUp;
        #endregion

        #region Constructor
        public Game()
        {
            InitializeComponent();
            _gameTime = new GameTime();
            Keyboard = new Keyboard();

            _timer = new Timer();
            _timer.Interval = 1000 / 30; // 60 PFS (el intervalo no siempre se respeta en winforms)
            _timer.Tick += (sender, e) =>
            {
                var _now = DateTime.Now;
                _gameTime.FrameMilliseconds = (int)(_now - _gameTime.FrameDate).TotalMilliseconds;
                _gameTime.FrameDate = _now;

                Application.DoEvents();
                this.Update(_gameTime);  // ejecuta logica propia del juego
                this.Keyboard.Clear();

                using (DrawHandler drawHandler = new DrawHandler(this.Canvas.Width, this.Canvas.Height))
                {
                    this.Draw(drawHandler);    // Actualiza la imagen en cada cuadro
                    Canvas.Image = drawHandler.BaseImage; // asigna la imagen del nuevo cuadro al picture box
                }
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Informacion de teclas precionadas
        /// </summary>
        protected Keyboard Keyboard { get; set; }
        #endregion

        #region Events
        private void Game_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
                _timer.Start(); // inicia el juego
        }
        private void pcCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (Canvas_MouseUp != null)
                Canvas_MouseUp(sender, e);
        }
        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            this.Keyboard.SetKey(e.KeyData);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Carga una imagen 
        /// </summary>
        /// <param name="path">ruta de la imagen a cargar</param>
        /// <returns></returns>
        protected Image Load_Image(string path)
        {
            try
            {
                return Image.FromFile(path);
            }
            catch
            {
                MessageBox.Show("Load File Error\n" + path);
                return null;
            }
        }
        /// <summary>
        /// Carga un texto
        /// </summary>
        /// <param name="path">ruta del archivo a cargar</param>
        /// <returns></returns>
        protected string Load_Text(string path)
        {
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
            catch
            {
                MessageBox.Show("Load File Error\n" + path);
                return null;
            }
        }
        /// <summary>
        /// Metodo que donde se escribe la logica del juego
        /// </summary>
        /// <param name="gameTime">Informacion de tiempo de juego transcurrido</param>
        protected virtual void Update(GameTime gameTime)
        {
        }
        /// <summary>
        /// Dibuja todos los sprites en pantalla
        /// </summary>
        /// <param name="drawHandler">controlador de dibujado</param>
        public virtual void Draw(DrawHandler drawHandler)
        {
        }

        #endregion
    }
}
