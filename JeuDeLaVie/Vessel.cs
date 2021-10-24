using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie
{
    class Vessel : IVessel
    {
        private int sizeX, sizeY;
        public Vessel(int sizeX, int sizeY, int gameTickSteps = 20)
        {

            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        public void AddGameTick()
        {
            throw new NotImplementedException();
        }

        public void Move()
        {
            throw new NotImplementedException();
        }
    }
}
