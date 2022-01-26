using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioBros.Elements
{
    /// <summary>
    /// Clase que carga los recursos del juego
    /// </summary>
    public class Resources
    {
        /// <summary>
        /// Bloque de la grilla Vacio
        /// </summary>
        public Image SpriteSheet { get; set; }

        public Data.Map Map_Data { get; set; }
    }
}
