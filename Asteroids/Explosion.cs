using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    
    class Explosion
    {
        
        int iX = 0;
        int iY = -100;
        bool bActive = true;
        int iBackgroundOffset = 0;

        public int X
        {
            get { return iX; }
            set { iX = value; }
        }

        public int Y
        {
            get { return iY; }
            set { iY = value; }
        }

        public bool IsActive
        {
            get { return bActive; }
        }

        public int Offset
        {
            get { return iBackgroundOffset; }
            set { iBackgroundOffset = value; }
        }
    }

}
