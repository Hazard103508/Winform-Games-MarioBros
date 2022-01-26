using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Elements
{
    public class GameTime
    {
        public GameTime()
        {
            StartDate =
            FrameDate = DateTime.Now;
        }

        /// <summary>
        /// Fecha y hora de inicio del juego
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Fecha y hora de ejecucion del frame actual
        /// </summary>
        public DateTime FrameDate { get; set; }
        /// <summary>
        /// Tiempo total de ejecucion del juego
        /// </summary>
        public TimeSpan TotalTime { get { return FrameDate - StartDate; } }
        /// <summary>
        /// Milisegundos transcurridos entre el cuadro actual y el anterior
        /// </summary>
        public int FrameMilliseconds { get; set; }
    }
}
