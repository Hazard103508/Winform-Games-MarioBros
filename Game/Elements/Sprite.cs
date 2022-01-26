using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Elements
{
    /// <summary>
    /// Sprite que se dibujara en pantalla
    /// </summary>
    public class Sprite
    {
        #region Constructor
        public Sprite()
        {
            Visible = true;
        }
        /// <summary>
        /// Instancia al Sprite a dibujar
        /// </summary>
        /// <param name="image">Imagen a dibujar</param>
        /// <param name="position">Posicion en pantalla donde se dibujara</param>
        public Sprite(Image image, Point position)
        {
            this.Image = image;
            this.Position = position;

            Visible = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Imagen a dibujar
        /// </summary>
        public Image Image { get; set; }
        /// <summary>
        /// Posicion en pantalla donde se dibujara la imagen
        /// </summary>
        public PointF Position { get; set; }
        /// <summary>
        /// Determina si se debe dibujar o no la imagen
        /// </summary>
        public bool Visible { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Dibuja todos los sprites en pantalla
        /// </summary>
        /// <param name="baseImage">Imagen base a donde se dibujara</param>
        /// <param name="g">Clase con metodos de dibujado</param>
        public virtual void Draw(DrawHandler drawHandler)
        {
            if (this.Visible)
                drawHandler.Draw(this.Image, new Point((int)this.Position.X, (int)this.Position.Y));
        }
        #endregion
    }
}
