using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Elements;

namespace MarioBros.Elements.Objects
{
    public class Base : Game.Elements.Sprite
    {
        #region Objects
        private int index = 0; // numero que indica el cuadro actual de la animacion que se dibujara
        private int milisec = 0; // variable auxiliar que suma los milisegundos transcurridos en cada iteracion del metodo update 
        private PointF mapPosition;

        public event EventHandler<PositionEventArgs> MapPositionChanged;
        #endregion

        #region Constructor
        public Base()
        {
            FPS = 1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// <summary>
        /// Velocidad del objeto al desplazase, ajusta la posicicion de este en el escenario
        /// </summary>
        public PointF Velocity { get; set; }
        /// <summary>
        /// Indica la velocidad en "cuadros por segundo" que tendran las animaciones del personaje
        /// </summary>
        public int FPS { get; set; }
        /// <summary>
        /// Lista de rectangulos que componen la animacion actual
        /// </summary>
        protected Rectangle[] SourceRectangles { get; set; }
        /// <summary>
        /// Direccion hacia donde mira el personaje
        /// </summary>
        protected Direction Direction_State { get; set; }
        /// <summary>
        /// Rectangulo a dibujar en el cuadro actual
        /// </summary>
        public Rectangle SourceRectangle { get { return SourceRectangles[index]; } }
        /// <summary>
        /// Posision del objeto en el mapa
        /// </summary>
        public virtual PointF MapPosition
        {
            get { return mapPosition; }
            set
            {
                var arg = new PositionEventArgs(value, mapPosition);
                mapPosition = value;
                if (arg.Previous != arg.Current)
                {
                    if (SourceRectangles != null)
                        MapPositionRec = new RectangleF(value.X, value.Y, SourceRectangle.Width, SourceRectangle.Height);

                    if (MapPositionChanged != null)
                        MapPositionChanged(this, arg);
                }
                // desencadena el evento indicando que el objeto se desplazo en el mapa
            }
        }
        /// <summary>
        /// Rectangulo que ocupa el personaje en el mapa
        /// </summary>
        public RectangleF MapPositionRec { get; private set; }
        /// <summary>
        /// Indica que el objeto sera removido del mapa
        /// </summary>
        public bool Removing { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Realiza la animacion del personaje
        /// </summary>
        /// <param name="gameTime"></param>
        public void Animation(GameTime gameTime)
        {
            milisec += gameTime.FrameMilliseconds; // contador de tiempo en milisegundos (variable auxiliar)
            if (milisec >= 1000 / FPS) // se calcula la demora que tendra el paso de cada cuadro de la animacion
            {
                milisec = 0;
                if (index < SourceRectangles.Length - 1)
                    index++; // si el el estado actual posee mas cuadros, adelanto 1
                else
                    index = 0; // si el el estado actual NO posee mas cuadros, vuelvo al primero para hacer una animacion ciclica
            }
        }
        /// <summary>
        /// Ajusta la posicion del objeto en el mapa sin desencadenar el evento
        /// </summary>
        /// <param name="x">ajuste en X</param>
        /// <param name="y">ajuste en Y</param>
        public void Fix_MapPosition(float x, float y)
        {
            mapPosition = new PointF(mapPosition.X + x, mapPosition.Y + y);
        }
        /// <summary>
        /// Valida la colicion del con otro objeto
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Check_Collision(Base obj, PointF prevPosition)
        {
        }
        /// <summary>
        /// Crea la coleccion de rectangulos que componen una animacion
        /// </summary>
        /// <param name="size">tamaño de los rectangulos</param>
        /// <param name="locations">ubicacion de los rectangulos</param>
        /// <returns></returns>
        protected Rectangle[] Create_Rectangles(Size size, params Point[] locations)
        {
            Rectangle[] _rect = new Rectangle[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                _rect[i] = new Rectangle(locations[i], size);

            return _rect;
        }
        /// <summary>
        /// Reinicia la animacion
        /// </summary>
        protected void ResetAnimation()
        {
            index = 0;
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            //this.MapPosition = new PointF(this.MapPosition.X + Velocity.X, this.MapPosition.Y + Velocity.Y);
            this.Animation(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(DrawHandler drawHandler)
        {
            drawHandler.Draw(base.Image, this.SourceRectangle, (int)base.Position.X, (int)base.Position.Y, Direction_State == Direction.Left);
        }
        #endregion
    }
    /// <summary>
    /// Direccion hacia donde mira el personaje
    /// </summary>
    public enum Direction
    {
        Right,
        Left
    }
    public class PositionEventArgs : EventArgs
    {
        public PositionEventArgs(PointF current, PointF previous)
        {
            this.Current = current;
            this.Previous = previous;
        }

        public PointF Current { get; set; }
        public PointF Previous { get; set; }
    }

}
