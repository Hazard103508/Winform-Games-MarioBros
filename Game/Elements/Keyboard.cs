using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Elements
{
    /// <summary>
    /// Teclas habilitadas en el juego
    /// </summary>
    public class Keyboard
    {
        #region Keys
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Space { get; private set; }
        #endregion

        #region Methods
        public void SetKey(Keys key)
        {
            if (key == Keys.Left) this.Left = true;
            if (key == Keys.Right) this.Right = true;
            if (key == Keys.Up) this.Up = true;
            if (key == Keys.Down) this.Down = true;
            if (key == Keys.Space) this.Space = true;
        }
        /// <summary>
        /// Limpia las teclas seleccionada
        /// </summary>
        public void Clear()
        {
            Left = false;
            Right = false;
            Up = false;
            Down = false;
            Space = false;
        }
        #endregion
    }
}
