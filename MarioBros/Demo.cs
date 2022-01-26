using Game.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarioBros
{
    public partial class Demo : Game.Game
    {
        #region Constructor
        public Demo()
        {
            InitializeComponent();
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Recursos graficos del juego
        /// </summary>
        public Elements.Resources Resources { get; set; }
        /// <summary>
        /// C
        /// </summary>
        public Elements.Map.MapHandler MapHandler { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Carga los recursos graficos del juego
        /// </summary>
        private void Initialize()
        {
            string directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            this.Resources = new Elements.Resources()
            {
                SpriteSheet = Load_Image($"{directory}/TileSet.png"),
                Map_Data = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.Map>(Load_Text($"{directory}/Level_1_1.json"))
            };

            Canvas.BackColor = System.Drawing.ColorTranslator.FromHtml(this.Resources.Map_Data.backgroundcolor);
            InitializeMap();
        }
        /// <summary>
        /// Carga el mapa
        /// </summary>
        private void InitializeMap()
        {
            MapHandler = new Elements.Map.MapHandler(this.Resources, this.Canvas.Size);
            MapHandler.Restart += (obj, e) => InitializeMap(); // reinicia el mapa
        }
        #endregion

        #region Events
        private void btnInfo_Click(object sender, EventArgs e)
        {
            base.Open_ZeroSoft_URL();
        }
        private void Demo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                Elements.Keyboard.Left = true;

            if (e.KeyCode == Keys.Right)
                Elements.Keyboard.Right = true;

            if (e.KeyCode == Keys.Z)
                Elements.Keyboard.Turbo = true;

            if (e.KeyCode == Keys.X)
                Elements.Keyboard.Jump = true;
        }
        private void Demo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                Elements.Keyboard.Left = false;

            if (e.KeyCode == Keys.Right)
                Elements.Keyboard.Right = false;

            if (e.KeyCode == Keys.Z)
                Elements.Keyboard.Turbo = false;

            if (e.KeyCode == Keys.X)
                Elements.Keyboard.Jump = false;
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            this.MapHandler.Update(gameTime);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Dibuja la grilla
        /// </summary>
        /// <param name="drawHandler"></param>
        public override void Draw(DrawHandler drawHandler)
        {
            this.MapHandler.Draw(drawHandler);
        }
        #endregion

    }
}
