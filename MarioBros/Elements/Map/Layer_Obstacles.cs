using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements.Map
{
    public class Layer_Obstacles
    {
        #region Constructor
        public Layer_Obstacles(Elements.Resources resources)
        {
            this.Tile_Size = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            this.Size = new Size(resources.Map_Data.width, resources.Map_Data.height);

            var layerTiles = resources.Map_Data.layers.FirstOrDefault(x => x.name == "Obstacles");
            Matrix = new bool[layerTiles.height, layerTiles.width];
            for (int row = 0; row < layerTiles.height; row++)
            {
                var columns = layerTiles.data.Skip(row * layerTiles.width).Take(layerTiles.width).ToList();
                for (int col = 0; col < layerTiles.width; col++)
                    Matrix[row, col] = Convert.ToInt32(columns[col]) != 0;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Tamaño del mapa en cantidad de celdas
        /// </summary>
        public Size Size { get; private set; }
        /// <summary>
        /// Tamaño de cada tile del mapa en pixeles
        /// </summary>
        private Size Tile_Size { get; set; }
        /// <summary>
        /// Matriz con la informacion de obstaculos
        /// </summary>
        private bool[,] Matrix { get; set; }
        #endregion

        #region Methods
        public void Valid_Colition(Elements.Objects.Base obj, PointF prevPosition)
        {
            if (obj == null)
                return;

            //rectangulo del elemento dentro del mapa
            var objRectangle = obj.MapPositionRec; //new RectangleF(obj.MapPosition.X, obj.MapPosition.Y, obj.SourceRectangle.Width, obj.SourceRectangle.Height);

            int _tileX_Min = (int)Math.Floor(objRectangle.X / Tile_Size.Width);
            int _tileX_Max = (int)Math.Ceiling((objRectangle.X + obj.SourceRectangle.Width) / Tile_Size.Width) - 1;
            int _tileY_Min = (int)Math.Floor(objRectangle.Y / Tile_Size.Height);
            int _tileY_Max = (int)Math.Ceiling((objRectangle.Y + obj.SourceRectangle.Height) / Tile_Size.Height) - 1;

            for (int row = _tileY_Min; row <= _tileY_Max; row++)
                for (int col = _tileX_Min; col <= _tileX_Max; col++)
                    if (Has_TileColition(row, col))
                    {
                        var recTile = new RectangleF((float)col * (float)Tile_Size.Width, (float)row * (float)Tile_Size.Height, Tile_Size.Width, Tile_Size.Height);
                        var area = RectangleF.Intersect(objRectangle, recTile);
                        if (area.Width == 0 && area.Height == 0)
                            continue;

                        var difPosition = new PointF(obj.MapPosition.X - prevPosition.X, obj.MapPosition.Y - prevPosition.Y); // diferencia entre la posicion actual y anterior
                        var adjPosition = Get_PositionAdjust(area, difPosition);

                        if (difPosition.Y != 0) // si existe solo colicion vertical
                        {
                            obj.Fix_MapPosition(0, adjPosition.Y);
                            obj.Velocity = new PointF(obj.Velocity.X, 0);
                            break;
                        }

                        if (difPosition.X != 0) // si existe colicion horizontal
                        {
                            obj.Fix_MapPosition(adjPosition.X, 0);

                            if (obj is Objects.Mario)
                                obj.Velocity = new PointF(0, obj.Velocity.Y);
                            else
                                obj.Velocity = new PointF(-obj.Velocity.X, obj.Velocity.Y);

                            break;
                        }
                    }
        }
        private PointF Get_PositionAdjust(RectangleF colArea, PointF difPosition)
        {
            float _x =
                difPosition.X < 0 ? colArea.Width :
                difPosition.X > 0 ? -colArea.Width :
                0;

            float _y =
                difPosition.Y < 0 ? colArea.Height :
                difPosition.Y > 0 ? -colArea.Height :
                0;

            return new PointF(_x, _y);
        }
        private bool Has_TileColition(int row, int col)
        {
            if (col < 0 || col >= Size.Width)
                return true;

            if (row < 0 || row >= Size.Height)
                return false;

            return Matrix[row, col];
        }
        #endregion
    }
}
