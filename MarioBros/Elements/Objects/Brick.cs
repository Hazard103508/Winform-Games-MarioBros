using Game.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Objects
{
    public class Brick : Base, IGravity
    {
        #region Objects
        private PointF? _originalPosition;
        #endregion


        #region Constructor
        public Brick(Elements.Resources resources, Data.Object obj)
        {
            base.Image = resources.SpriteSheet;

            Size _recSize = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            SourceRec_Normal = base.Create_Rectangles(_recSize, new Point(224, 0));
            SourceRectangles = SourceRec_Normal;

            this._originalPosition = new PointF(obj.x, (int)obj.y - resources.Map_Data.tileheight);
            this.MapPosition = _originalPosition.Value;
        }
        #endregion

        #region Properties
        private Rectangle[] SourceRec_Normal { get; set; }
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

                Velocity = new PointF(Velocity.X, -10);
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
    }
    #endregion
}
