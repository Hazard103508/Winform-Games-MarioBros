using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Elements;

namespace MarioBros.Elements.Map
{
    public class Layer_Tiles : Game.Elements.Sprite
    {
        #region Objects
        private Size _canvasSize;
        #endregion

        #region Constructor
        public Layer_Tiles(Elements.Resources resources, Size canvasSize)
        {
            _canvasSize = canvasSize;
            base.Position = Point.Empty;
            base.Image = resources.SpriteSheet;

            this.Tile_Size = new Size(resources.Map_Data.tilewidth, resources.Map_Data.tileheight);
            this.Size = new Size(resources.Map_Data.width, resources.Map_Data.height);

            #region Carga los tiles del mapa
            Tiles = new Dictionary<int, Rectangle>();
            Size tileSetSize = new Size(resources.SpriteSheet.Width / Tile_Size.Width, resources.SpriteSheet.Height / Tile_Size.Height);
            for (int y = 0; y < tileSetSize.Height; y++)
                for (int x = 0; x < tileSetSize.Width; x++)
                {
                    int _id = ((tileSetSize.Width * y) + x) + 1;
                    Rectangle _rec = new Rectangle(x * Tile_Size.Width, y * Tile_Size.Height, Tile_Size.Width, Tile_Size.Height);
                    Tiles.Add(_id, _rec);
                }
            #endregion

            #region Carga matriz de tiles
            var layerTiles = resources.Map_Data.layers.FirstOrDefault(x => x.name == "Tiles");
            Matrix = new int[layerTiles.height, layerTiles.width];
            for (int row = 0; row < layerTiles.height; row++)
            {
                var columns = layerTiles.data.Skip(row * layerTiles.width).Take(layerTiles.width).ToList();
                for (int col = 0; col < layerTiles.width; col++)
                    Matrix[row, col] = Convert.ToInt32(columns[col]);
            }
            #endregion

            // Tamaño en cantidad de celdas que son visibles en la pantalla
            var _viewPortSize = new Size((int)Math.Ceiling((float)canvasSize.Width / (float)Tile_Size.Width), (int)Math.Ceiling((float)canvasSize.Height / (float)Tile_Size.Height));
            this.ViewPort = new Size(Math.Min(_viewPortSize.Width + 1, Size.Width), Math.Min(_viewPortSize.Height, Size.Height));
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
        public Size Tile_Size { get; private set; }
        /// <summary>
        /// Diccionario con los tiles asociados a su respectivo ID
        /// </summary>
        private Dictionary<int, Rectangle> Tiles { get; set; }
        /// <summary>
        /// Matriz con la informacion grafica del mapa
        /// </summary>
        private int[,] Matrix{ get; set; }
        /// <summary>
        /// Tamaño de la pantalla en celdas (celdas del mapa visibles)
        /// </summary>
        private Size ViewPort { get; set; }
        #endregion

        #region Methods
        public override void Draw(DrawHandler drawHandler)
        {
            int _startX = (int)Math.Floor((float)Position.X / (float)Tile_Size.Width); // coordenada en x de la primera celda a dibujar
            int _startY = (int)Math.Floor((float)Position.Y / (float)Tile_Size.Height); // coordenada en y de la primera celda a dibujar

            for (int x = _startX; x < _startX + ViewPort.Width; x++)
                for (int y = _startY; y < _startY + ViewPort.Height; y++)
                {
                    if (x >= 0 && x < this.Size.Width)
                    {
                        int _id = Matrix[y, x];
                        if (_id != 0)
                        {
                            Point _position = new Point((int)(x * Tile_Size.Width - Position.X), (int)(y * Tile_Size.Height - Position.Y));
                            var _rec = Tiles[_id];
                            drawHandler.Draw(base.Image, _rec, _position);
                        }
                    }
                }
        }
        #endregion
    }
}
