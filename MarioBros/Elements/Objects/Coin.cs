using Game.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Objects
{
    public class Coin : Base, IGravity
    {
        #region Objects
        private PointF? _originalPosition;
        #endregion

        #region Constructor
        public Coin(Elements.Resources resources)
        {
            base.Image = resources.SpriteSheet;

            Size _recSize = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            SourceRectangles = base.Create_Rectangles(_recSize,
                new Point(256, 192),
                new Point(288, 192),
                new Point(320, 192),
                new Point(352, 192)
            );

            this.Velocity = new PointF(0, -20);
            this.FPS = 6;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (_originalPosition == null)
                _originalPosition = this.MapPosition;

            if (this.MapPosition.Y > _originalPosition.Value.Y)
                this.Removing = true;

            base.Update(gameTime);
        }
        #endregion
    }
}
