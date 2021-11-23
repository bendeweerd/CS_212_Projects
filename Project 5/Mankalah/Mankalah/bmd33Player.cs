using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mankalah
{
    /*****************************************************************/
    /*
    /* A smart Mankalah Player
    /*
    /*****************************************************************/
    public class bmd33Player : Player
    {
        public bmd33Player(Position pos, int timeLimit) : base(pos, "bmd33", timeLimit){}
        public override string gloat(){
            return "I WIN! MY ALGORITHM CONQUERS ALL!";
        }
        public override int chooseMove(Board b)
        {
            // very dumb - just chooses first available move
            if(b.whoseMove() == Position.Top)
            {
                for(int i=12; i>=7; i--)
                    if(b.stonesAt(i) > 0) return i;
            } else {
                for(int i=5; i>=0; i--)
                    if(b.stonesAt(i) > 0) return i;
            }
            return -1;
        }
        public override String getImage(){return "Ben.png";}
    }
}