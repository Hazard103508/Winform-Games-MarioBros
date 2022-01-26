using Game.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Objects
{
    public class Box : Base, IGravity
    {
        #region Objects
        private BoxState _state;
        private PointF? _originalPosition;
        public event EventHandler DropCoin;
        #endregion

        #region Constructor
        public Box(Elements.Resources resources, Data.Object obj)
        {
            base.Image = resources.SpriteSheet;

            Size _recSize = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            SourceRec_Normal = base.Create_Rectangles(_recSize,
                new Point(320, 0),
                new Point(320, 64),
                new Point(320, 128),
                new Point(320, 64),
                new Point(320, 0)
            );
            SourceRec_Empty = base.Create_Rectangles(_recSize, new Point(224, 64));

            State = BoxState.Normal;
            this.FPS = 8;

            this._originalPosition = new PointF(obj.x, (int)obj.y - resources.Map_Data.tileheight);
            this.MapPosition = _originalPosition.Value;
        }
        #endregion

        #region Properties
        public BoxState State
        {
            get { return _state; }
            set
            {
                _state = value;
                SourceRectangles = value == BoxState.Normal ? SourceRec_Normal : SourceRec_Empty;
                ResetAnimation();
            }
        }
        private Rectangle[] SourceRec_Normal { get; set; }
        private Rectangle[] SourceRec_Empty { get; set; }
        private Rectangle[] SourceRec_Coin { get; set; }
        public override PointF MapPosition
        {
            get => base.MapPosition;
            set => base.MapPosition = new PointF(value.X, Math.Min(value.Y, _originalPosition.Value.Y));
        }
        #endregion

        #region Methods
        public override void Check_Collision(Base obj, PointF prevPosition)
        {
            var difPosition = new PointF(obj.MapPosition.X - prevPosition.X, obj.MapPosition.Y - prevPosition.Y); // diferencia entre la posicion actual y anterior
            if (difPosition.Y > 0)
            {
                obj.Velocity = new PointF(obj.Velocity.X, 0);
                obj.MapPosition = new PointF(obj.MapPosition.X, this.MapPosition.Y - obj.SourceRectangle.Height);
            }
            else if (difPosition.Y < 0)
            {
                obj.Velocity = new PointF(obj.Velocity.X, 0);
                obj.MapPosition = new PointF(obj.MapPosition.X, this.MapPosition.Y + obj.SourceRectangle.Height);

                if (State == BoxState.Normal)
                {
                    State = BoxState.Empty;
                    DropCoin(this, EventArgs.Empty);
                    Velocity = new PointF(Velocity.X, -10);
                }
            }
            else if (difPosition.X > 0)
            {
                obj.Velocity = new PointF(0, obj.Velocity.Y);
                obj.MapPosition = new PointF(this.MapPosition.X - obj.SourceRectangle.Width, obj.MapPosition.Y);
            }
            else if (difPosition.X < 0)
            {
                obj.Velocity = new PointF(0, obj.Velocity.Y);
                obj.MapPosition = new PointF(this.MapPosition.X + obj.SourceRectangle.Width, obj.MapPosition.Y);
            }
        }
        #endregion

        #region Structures
        public enum BoxState
        {
            Normal,
            Empty
        }
        #endregion
    }
}
