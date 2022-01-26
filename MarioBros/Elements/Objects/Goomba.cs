using Game.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Objects
{
    public class Goomba : Base, IGravity
    {
        #region Objects
        private GoombaState _state;
        private int miliseconsDying;
        #endregion

        #region Constructor
        public Goomba(Elements.Resources resources, Data.Object obj)
        {
            base.Image = resources.SpriteSheet;

            Size _recSize = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            SourceRec_Normal = base.Create_Rectangles(_recSize,
                new Point(0, 480),
                new Point(32, 480)
            );
            SourceRec_Dying = base.Create_Rectangles(_recSize, new Point(64, 480));

            this.MapPosition = new PointF(obj.x, (int)obj.y - resources.Map_Data.tileheight);
            this.Velocity = new PointF(-2, 0);
            this.FPS = 6;
            this.State = GoombaState.Normal;
        }
        #endregion

        #region Properties
        private Rectangle[] SourceRec_Normal { get; set; }
        private Rectangle[] SourceRec_Dying { get; set; }

        public GoombaState State
        {
            get { return _state; }
            set
            {
                _state = value;
                base.ResetAnimation();
                SourceRectangles = value == GoombaState.Normal ? SourceRec_Normal : SourceRec_Dying;
            }
        }
        #endregion

        #region Methods
        public override void Check_Collision(Base obj, PointF prevPosition)
        {
            if (this.State == GoombaState.Dying)
                return;

            var difPosition = new PointF(obj.MapPosition.X - prevPosition.X, obj.MapPosition.Y - prevPosition.Y); // diferencia entre la posicion actual y anterior

            if (obj is Mario)
            {
                var mario = (Mario)obj;
                if (difPosition.Y != 0) // si existe solo colicion vertical
                {
                    this.State = GoombaState.Dying;
                    this.Velocity = PointF.Empty;

                    mario.Action_State = MarioAction.Jump;
                    mario.Velocity = new PointF(obj.Velocity.X, -15); // rebota mario bros
                }
                else
                    mario.Kill(); // asesina a mario
            }
            else if (obj is Box || obj is Brick)
                this.MapPosition = new PointF(this.MapPosition.X, obj.MapPosition.Y - obj.SourceRectangle.Height);
            else
                obj.Velocity = new PointF(-obj.Velocity.X, obj.Velocity.Y); // cambienza a caminar en direccion contraria
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (this.State == GoombaState.Dying)
            {
                // contador antes de desaparecer
                miliseconsDying += gameTime.FrameMilliseconds;
                if (miliseconsDying >= 1000)
                    Removing = true;
            }

            base.Update(gameTime);
        }
        #endregion

        #region Structures
        public enum GoombaState
        {
            Normal,
            Dying
        }
        #endregion
    }
}
