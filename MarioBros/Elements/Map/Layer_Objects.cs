using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Elements;
using MarioBros.Elements.Objects;

namespace MarioBros.Elements.Map
{
    public class Layer_Objects : Game.Elements.Sprite
    {
        #region Objects
        private Size _canvasSize;
        #endregion

        #region Constructor
        public Layer_Objects(Elements.Resources resources, Size canvasSize)
        {
            this._canvasSize = canvasSize;
            base.Image = resources.SpriteSheet;
            var _layer = resources.Map_Data.layers.First(x => x.name == "Objects");

            MapObjectsNew = new List<Base>();
            MapObjects = new List<Base>();
            _layer.objects.ForEach(x =>
            {
                switch (Convert.ToInt32(x.type))
                {
                    case 1:
                        Mario = new Mario(resources, x);
                        //MapObjects.Add(Mario);
                        break;
                    case 2:
                        var box = new Box(resources, x);
                        ((Box)box).DropCoin += (sender, e) =>
                        {
                            var coin = new Coin(resources);
                            coin.MapPosition = box.MapPosition;
                            MapObjectsNew.Add(coin); 
                        };
                        MapObjects.Add(box);
                        break;
                    case 3:
                        var brick = new Brick(resources, x);
                        MapObjects.Add(brick);
                        break;
                    //case 4:
                    //    // hongo de vida
                    //    break;
                    case 5:
                        Flag = new Rectangle(x.x, x.y - x.height, x.width, x.height);
                        break;
                    case 6:
                        var goomba = new Goomba(resources, x);
                        MapObjects.Add(goomba);
                        break;
                    //case 7:
                    //    var koopa = new Koopa(resources, x);
                    //    MapObjects.Add(koopa);
                    //    break;
                }
            });

            MapObjects.Add(Mario); // lo agrega como ultimo objeto del mapa
        }
        #endregion

        #region Properties
        /// <summary>
        /// Personaje jugable mario bros
        /// </summary>
        public Mario Mario { get; private set; }
        /// <summary>
        /// Ubicacion de la Bandera
        /// </summary>
        public Rectangle Flag { get; set; }
        /// <summary>
        /// Objetos del mapa
        /// </summary>
        public List<Base> MapObjects { get; set; }
        /// <summary>
        /// Objetos nuevos que se deben agregar al mapa
        /// </summary>
        public List<Base> MapObjectsNew { get; set; }
        #endregion

        #region Update
        public void Update(Game.Elements.GameTime gameTime)
        {
            MapObjects.ForEach(x =>
            {
                if (x.Position.X + x.SourceRectangle.Width > 0 && x.Position.X <= _canvasSize.Width) // solo actualizo el objeto si se ve en pantalla
                    x.Update(gameTime);
            });
        }
        #endregion

        #region Draw
        public override void Draw(DrawHandler drawHandler)
        {
            MapObjects.ForEach(x =>
            {
                if (x.Position.X + x.SourceRectangle.Width > 0 && x.Position.X <= _canvasSize.Width) // solo dibujo el objeto si se ve en pantalla
                    x.Draw(drawHandler);
            });
        }
        #endregion

        #region Methods
        public void Valid_Colition(Elements.Objects.Base obj, PointF prevPosition)
        {
            var objTargets = this.MapObjects.Where(x => !x.Equals(obj) && x.MapPositionRec.IntersectsWith(obj.MapPositionRec)).ToList();
            if (objTargets.Any())
            {
                foreach (var item in objTargets)
                    item.Check_Collision(obj, prevPosition);
            }
        }
        #endregion
    }
}
