using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class Base : Form
    {
        public Base()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Abre la Url de Zero Soft
        /// </summary>
        public void Open_ZeroSoft_URL()
        {
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/rossoagustin/");
        }
    }
}
