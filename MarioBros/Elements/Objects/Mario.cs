using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Elements;

namespace MarioBros.Elements.Objects
{
    public class Mario : Base, IGravity
    {
        #region Objects
        private MarioAction _action;
        private Direction _direction;

        public event EventHandler Died;
        #endregion

        #region Constructor
        public Mario(Elements.Resources resources, Data.Object obj)
        {
            base.Image = resources.SpriteSheet;

            Size _recSize = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            SourceRec_Small_Stand = base.Create_Rectangles(_recSize, new Point(320, 640));
            SourceRec_Small_Stop = base.Create_Rectangles(_recSize, new Point(384, 640));
            SourceRec_Small_Walk = base.Create_Rectangles(_recSize, new Point(416, 640), new Point(320, 672), new Point(352, 672));
            SourceRec_Small_Jump = base.Create_Rectangles(_recSize, new Point(384, 672));
            SourceRec_Small_Dead = base.Create_Rectangles(_recSize, new Point(352, 640));
            SourceRec_Small_Flag = base.Create_Rectangles(_recSize, new Point(320, 704));

            Action_State = MarioAction.Idle;
            Direction_State = Direction.Right;

            this.FPS = 12;
            this.Velocity = PointF.Empty;
            this.MapPosition = new PointF(obj.x, (int)obj.y - resources.Map_Data.tileheight);
        }
        #endregion

        #region Properties
        public MarioAction Action_State
        {
            get { return _action; }
            set
            {
                _action = value;
                SetSourceRectangle();
            }
        }
        /// <summary>
        /// Direccion hacia donde mira el personaje
        /// </summary>
        new public Direction Direction_State
        {
            get { return _direction; }
            set
            {
                _direction = value;
                SetSourceRectangle();
            }
        }

        private Rectangle[] SourceRec_Small_Stop { get; set; }
        private Rectangle[] SourceRec_Small_Run { get; set; }
        private Rectangle[] SourceRec_Small_Stand { get; set; }
        private Rectangle[] SourceRec_Small_Walk { get; set; }
        private Rectangle[] SourceRec_Small_Jump { get; set; }
        private Rectangle[] SourceRec_Small_Dead { get; set; }
        private Rectangle[] SourceRec_Small_Flag { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Setea el valor del array de rectangulos que se va a dibujar
        /// </summary>
        private void SetSourceRectangle()
        {
            // dependiendo el estado nuevo del perosnaje reasigna el array de rectangulo que corresponda a SourceRectangle para que se dibuje correctamente
            if (Action_State == MarioAction.Idle)
                SourceRectangles = SourceRec_Small_Stand;
            else if (Action_State == MarioAction.Walk)
                SourceRectangles = SourceRec_Small_Walk;
            else if (Action_State == MarioAction.Stop)
                SourceRectangles = SourceRec_Small_Stop;
            else if (Action_State == MarioAction.Jump || Action_State == MarioAction.Falling)
                SourceRectangles = SourceRec_Small_Jump;
            else if (Action_State == MarioAction.Die)
                SourceRectangles = SourceRec_Small_Dead;
            else if (Action_State == MarioAction.Flag)
                SourceRectangles = SourceRec_Small_Flag;

            ResetAnimation();
        }
        /// <summary>
        /// Interaccion con el personaje
        /// </summary>
        public void MoveCharacter()
        {
            if (this.Action_State == MarioAction.Die)
                return;

            float _aceleration = 0.2f;
            float _maxAceleration = 6f;

            #region TURBO
            if (Elements.Keyboard.Turbo) // duplico la velocidad cuando el personaje corre
            {
                _aceleration *= 2;
                _maxAceleration *= 2;
            }
            else if (Math.Abs(Velocity.X) > _maxAceleration)
            {
                // si dejo de usar turbo, desacelero
                Velocity = (Velocity.X < 0 ? new PointF(Velocity.X + _aceleration, Velocity.Y) : new PointF(Velocity.X - _aceleration, Velocity.Y));
                Velocity = new PointF((float)Math.Round(Velocity.X, 1), (float)Math.Round(Velocity.Y, 1)); // correccion
            }
            FPS = Math.Abs(Velocity.X) <= _maxAceleration / 2 ? 6 : 12; // acelero la animacion dependiendo la velocidad
            if (Elements.Keyboard.Turbo)
                FPS *= 2;
            #endregion

            #region RIGHT
            if (Elements.Keyboard.Right) // desplzaimiento hacia la derecha
            {
                if (Velocity.X < _maxAceleration)
                    Velocity = new PointF(Velocity.X + _aceleration, Velocity.Y);

                if ((Action_State != MarioAction.Jump && Action_State != MarioAction.Falling))
                {
                    if (Direction_State != Direction.Right)
                        Direction_State = Direction.Right;

                    if (Velocity.X <= 0)
                    {
                        if (Action_State != MarioAction.Stop)
                            Action_State = MarioAction.Stop;
                    }
                    else if (Action_State != MarioAction.Walk)
                        Action_State = MarioAction.Walk;
                }
            }
            #endregion

            #region LEFT
            if (Elements.Keyboard.Left) // desplamiento hacia la izquierda
            {
                if (Velocity.X > -_maxAceleration)
                    Velocity = new PointF(Velocity.X - _aceleration, Velocity.Y);

                if ((Action_State != MarioAction.Jump && Action_State != MarioAction.Falling))
                {
                    if (Direction_State != Direction.Left)
                        Direction_State = Direction.Left;

                    if (Velocity.X > 0)
                    {
                        if (Action_State != MarioAction.Stop)
                            Action_State = MarioAction.Stop;
                    }
                    else if (Action_State != MarioAction.Walk)
                        Action_State = MarioAction.Walk;
                }
            }
            #endregion

            #region JUMP
            if (Elements.Keyboard.Jump && (Action_State != MarioAction.Jump && Action_State != MarioAction.Falling))
            {
                Action_State = MarioAction.Jump;
                float _jAaceleration = Elements.Keyboard.Turbo ? 24 : 20;
                Velocity = new PointF(Velocity.X, -_jAaceleration);
            }

            if (Action_State == MarioAction.Falling && Velocity.Y == 0)
                Action_State = Velocity.X != 0 ? MarioAction.Walk : MarioAction.Stop;

            if (Action_State == MarioAction.Jump && Velocity.Y >= 0)
                Action_State = MarioAction.Falling;
            #endregion

            #region STOP WALK
            if (Action_State != MarioAction.Jump && !Elements.Keyboard.Right && !Elements.Keyboard.Left) // deja de caminar
            {
                float _velX = (Velocity.X > 0 ? -(_aceleration * 2) : Velocity.X < 0 ? (_aceleration * 2) : 0) + Velocity.X;
                if (Math.Abs(Velocity.X) < (_aceleration * 2)) _velX = 0;

                Velocity = new PointF((float)Math.Round(_velX, 2), Velocity.Y);
            }
            #endregion

            #region IDLE
            if (Action_State != MarioAction.Jump && Action_State != MarioAction.Falling && Action_State != MarioAction.Idle && Velocity.X == 0) // retorna a estado de espera
                Action_State = MarioAction.Idle;
            #endregion
        }
        /// <summary>
        /// Mata a mario bros
        /// </summary>
        public void Kill()
        {
            this.Action_State = MarioAction.Die; // cambia el estado para mostrar el sprite correspondiente
            this.Velocity = new PointF(Velocity.X, -20); // cambia velocidad para mostrar el salto de muerte
            this.Died(this, EventArgs.Empty); // notifica al controlador del juego que mario murio para cambiar el estado del juego
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            this.MoveCharacter();

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(DrawHandler drawHandler)
        {
            drawHandler.Draw(base.Image, base.SourceRectangle, (int)base.Position.X, (int)base.Position.Y, Direction_State == Direction.Left);
        }
        #endregion
    }
    public enum MarioAction // Diferentes tipos de acciones que puede realizar el personaje
    {
        Idle,   
        Walk,   
        Die,    
        Flag,   
        Jump,  
        Falling,
        Stop,   
    }
}
