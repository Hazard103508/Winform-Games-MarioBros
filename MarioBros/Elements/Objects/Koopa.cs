using Game.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Objects
{
    public class Koopa : Base
    {
        #region Constructor
        public Koopa(Elements.Resources resources, Data.Object obj)
        {
            base.Image = resources.SpriteSheet;

            //Size _recSize = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            SourceRec_Normal = base.Create_Rectangles(new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight * 2), 
                new Point(224, 384), 
                new Point(256, 384)
            );

            SourceRectangles = SourceRec_Normal;
            this.Velocity = new PointF(-2, 0);
            this.FPS = 6;

            this.MapPosition = new PointF(obj.x, (int)obj.y - resources.Map_Data.tileheight);
        }
        #endregion

        #region Properties
        private Rectangle[] SourceRec_Normal { get; set; }
        #endregion

    }
}
