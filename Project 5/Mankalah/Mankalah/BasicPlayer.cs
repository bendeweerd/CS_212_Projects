using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Mankalah
{
    /*****************************************************************/
    /*
    /* A Dumb Mankalah player.  Chooses a random legal move.
    /*
    /*****************************************************************/
    public class BasicPlayer : Player
    {
        public BasicPlayer(Position pos, int timeLimit) : base(pos, "Basic", timeLimit) { }

        public override int chooseMove(Board b)
        {
            Random rnd = new Random();
            List<int> possibleMoves = new List<int>();
            for(int i = 0; i <= 12; i++)
            {
                if(b.legalMove(i))
                {
                    possibleMoves.Add(i);
                }
            }
            int move = possibleMoves[rnd.Next(0, possibleMoves.Count)];
            return move;
        }
    }
}
