using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie
{
    public interface IVessel
    {
        void Move();
        void AddGameTick();
    }
}
