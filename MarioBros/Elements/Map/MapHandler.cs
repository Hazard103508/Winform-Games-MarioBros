using Game.Elements;
using MarioBros.Elements.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Map
{
    public class MapHandler
    {
        #region Objects
        private Size _canvasSize;
        private RectangleF _canvasRec;
        private float _gravity = 2f;

        public event EventHandler Restart;
        #endregion

        #region Constructor
        public MapHandler(Elements.Resources resources, Size canvasSize)
        {
            _canvasSize = canvasSize;

            Layer_Tiles = new Layer_Tiles(resources, canvasSize);
            Layer_Obstacles = new Layer_Obstacles(resources);
            Layer_Objects = new Layer_Objects(resources, canvasSize);

            Layer_Objects.MapObjects.ForEach(x => x.MapPositionChanged += Objects_MapPositionChanged); // atacha el evento de cambio de posiscion de los objetos en el mapa
            Layer_Objects.Mario.Died += Mario_Died;

            State = GameState.Playing;
            Update_ObjectPosition(); // actualiza la posicion inicial de los objetos
        }
        #endregion

        #region Properties
        /// <summary>
        /// Estado del juego en ejecucion
        /// </summary>
        public GameState State { get; set; }
        /// <summary>
        /// Informacion grafica del mapa
        /// </summary>
        public Layer_Tiles Layer_Tiles { get; set; }
        /// <summary>
        /// Informacion de obstaculos del mapa
        /// </summary>
        public Layer_Obstacles Layer_Obstacles { get; set; }
        /// <summary>
        /// Objetos del mapa
        /// </summary>
        public Layer_Objects Layer_Objects { get; set; }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            if (State == GameState.Playing)
                Update_Playing(gameTime);
            else if (State == GameState.Dying)
                Update_Dying(gameTime);
            else if (State == GameState.Wining)
                Update_Winning(gameTime);

            if (Layer_Objects.Mario.Position.Y > (this._canvasSize.Height * 2))
                this.Restart(null, EventArgs.Empty);
            // si mario se cae a un poso o completa el mapa, se reinicia 
        }
        private void Update_Playing(GameTime gameTime)
        {
            _canvasRec = new RectangleF(this.Layer_Tiles.Position.X, this.Layer_Tiles.Position.Y, _canvasSize.Width, _canvasSize.Height);

            var lstRemove = new List<Base>();
            this.Layer_Objects.MapObjects.ForEach(obj =>
            {
                if (obj is IGravity)
                {
                    obj.Velocity = new PointF(obj.Velocity.X, obj.Velocity.Y + _gravity);
                    obj.MapPosition = new PointF(obj.MapPosition.X, obj.MapPosition.Y + obj.Velocity.Y);
                    // al actualizar la posicion del mapa en Y, se validan las coliciones y se ajusta la posicion en caso de ser necesario
                }

                obj.Update(gameTime);
                if (obj.Velocity.X != 0)
                {
                    if (obj.MapPositionRec.IntersectsWith(_canvasRec)) // los objetos se mueven solo si estan a la vista
                        obj.MapPosition = new PointF(obj.MapPosition.X + obj.Velocity.X, obj.MapPosition.Y);
                }

                if (obj.Removing)
                    lstRemove.Add(obj);
            });

            lstRemove.ForEach(x => this.Layer_Objects.MapObjects.Remove(x)); // remueve de la lista los objetos descartados
            this.Layer_Objects.MapObjectsNew.ForEach(x => this.Layer_Objects.MapObjects.Add(x)); // se agregan los objetos nuevos creados en el transcurso del juego, en este ejemplo las monedas
            this.Layer_Objects.MapObjectsNew.Clear();

            Move_Character();
            Update_ObjectPosition();

            if (this.Layer_Objects.Mario.MapPosition.X + (this.Layer_Objects.Mario.SourceRectangle.Width / 2) >= this.Layer_Objects.Flag.X)
            {
                var mario = this.Layer_Objects.Mario;
                mario.Action_State = MarioAction.Flag;
                mario.MapPosition = new PointF(this.Layer_Objects.Flag.X - 10, mario.MapPosition.Y);
                mario.Velocity = new PointF(0, 0);
                this.State = GameState.Wining;
                // si mario llega a la bandera cambia el estado del juego
            }
        }
        private void Update_Dying(GameTime gameTime)
        {
            // muestra la animacion de mario muriendo
            var mario = Layer_Objects.Mario;
            mario.Velocity = new PointF(mario.Velocity.X, mario.Velocity.Y + _gravity);
            mario.MapPosition = new PointF(mario.MapPosition.X, mario.MapPosition.Y + mario.Velocity.Y);
            // al actualizar la posicion del mapa en Y, se validan las coliciones y se ajusta la posicion en caso de ser necesario

            Update_ObjectPosition(mario); // actualiza la posicion en el mapa solo de mario 
        }
        private void Update_Winning(GameTime gameTime)
        {
            var mario = Layer_Objects.Mario;
            mario.Velocity = new PointF(mario.Velocity.X, mario.Velocity.Y + _gravity);
            mario.MapPosition = new PointF(mario.MapPosition.X + mario.Velocity.X, mario.MapPosition.Y);
            mario.MapPosition = new PointF(mario.MapPosition.X, mario.MapPosition.Y + mario.Velocity.Y);
            mario.Animation(gameTime);

            if (mario.Action_State == MarioAction.Flag)
            {
                var flag = Layer_Objects.Flag;
                if (mario.MapPosition.Y + mario.SourceRectangle.Height == flag.Y + flag.Height)
                {
                    mario.Action_State = MarioAction.Walk;
                    mario.Velocity = new PointF(6, 0);
                }
            }

            Update_ObjectPosition(mario); // actualiza la posicion en el mapa solo de mario 

            if (mario.MapPosition.X >= ((Layer_Tiles.Size.Width - 1) * Layer_Tiles.Tile_Size.Width))
                this.Restart(null, EventArgs.Empty); // reinicia el mapa
        }
        #endregion

        #region Draw
        /// <summary>
        /// Dibuja la grilla
        /// </summary>
        /// <param name="drawHandler"></param>
        public void Draw(DrawHandler drawHandler)
        {
            this.Layer_Tiles.Draw(drawHandler);
            this.Layer_Objects.Draw(drawHandler);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Actualiza la posicion en pantalla de los objetos del mapa
        /// </summary>
        private void Update_ObjectPosition(Base mapObject = null)
        {
            var lstObjects = mapObject != null ? new List<Base>() { mapObject } : this.Layer_Objects.MapObjects;
            lstObjects.ForEach(obj =>
            {
                obj.Position = new PointF(obj.MapPosition.X - this.Layer_Tiles.Position.X, obj.MapPosition.Y - this.Layer_Tiles.Position.Y);
            });
        }
        /// <summary>
        /// Desplazamiento del personaje en el escenario
        /// </summary>
        private void Move_Character()
        {
            if (this.Layer_Objects.Mario.Position.X > _canvasSize.Width / 2)
            {
                var difX = this.Layer_Objects.Mario.Position.X - (float)_canvasSize.Width / 2f;
                this.Layer_Tiles.Position = new PointF(this.Layer_Tiles.Position.X + (float)difX, this.Layer_Tiles.Position.Y);

                int _maxPositionWidth = this.Layer_Tiles.Size.Width * this.Layer_Tiles.Tile_Size.Width - _canvasSize.Width;
                if (this.Layer_Tiles.Position.X > _maxPositionWidth)
                    this.Layer_Tiles.Position = new PointF(_maxPositionWidth, this.Layer_Tiles.Position.Y); // limita el desplazamiento del hasta el borde del mapa

                Update_ObjectPosition();
            }
            else if (this.Layer_Objects.Mario.Position.X < 0)
                this.Layer_Objects.Mario.MapPosition = new PointF(this.Layer_Tiles.Position.X, this.Layer_Objects.Mario.MapPosition.Y); // evita que el personaje se salga del margen izquierdo de la pantalla
        }
        private void Objects_MapPositionChanged(object sender, PositionEventArgs e)
        {
            if (State == GameState.Playing || State == GameState.Wining)
            {
                // valida la colicion del objeto con las celdas bloqueadas del mapa
                this.Layer_Obstacles.Valid_Colition((Elements.Objects.Base)sender, e.Previous);

                // valida la colicion del objeto con otros objetos 
                this.Layer_Objects.Valid_Colition((Elements.Objects.Base)sender, e.Previous);
            }
        }
        private void Mario_Died(object sender, EventArgs e)
        {
            this.State = GameState.Dying; // al detectar que mario murio, cambia el estado del juego
        }
        #endregion

        #region Structures
        public enum GameState
        {
            Playing, // juego en ejecucion
            Dying, // muestra la animacion de mario muriendo
            Wining, // muestra la animacion de mario ganando
        }
        #endregion
    }
}
