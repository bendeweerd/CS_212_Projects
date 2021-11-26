using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mankalah
{
    /*****************************************************************/
    /*
    /* A (hopefully) smart Mankalah Player
    /*
    /*****************************************************************/
    public class bmd33Player : Player
    {
        public bmd33Player(Position pos, int timeLimit) : base(pos, "bmd33", timeLimit){}
        
        public override int chooseMove(Board b)
        {
            MoveResult bestMove = new MoveResult(0, int.MinValue, false);

            Console.WriteLine("Move:\tScore:");

            if(b.whoseMove() == Position.Top)
            {
                for(int i = 7; i <= 12; i++)
                {
                    if(b.legalMove(i))
                    {
                        Board testBoard = new Board(b);
                        testBoard.makeMove(i, false);
                        int testBoardScore = evaluate(testBoard);
                        Console.WriteLine("{0}\t{1}", i, testBoardScore);
                        if(testBoardScore > bestMove.GetScore())
                        {
                            bestMove = new MoveResult(i, testBoardScore, false);
                        }
                    }
                }
            } else {
                for(int i = 0; i <= 5; i++)
                {
                    if(b.legalMove(i))
                    {
                        Board testBoard = new Board(b);
                        testBoard.makeMove(i, false);
                        int testBoardScore = -1 * evaluate(testBoard);
                        Console.WriteLine("{0}\t{1}", i, testBoardScore);
                        if(testBoardScore > bestMove.GetScore())
                        {
                            bestMove = new MoveResult(i, testBoardScore, false);
                        }
                    }
                }
            }

            Console.WriteLine("Best Move: {0}", bestMove.GetMove());
            Console.WriteLine("Best Move Score: {0}", bestMove.GetScore());

            return bestMove.GetMove();
        }

        // This function looks at the current state of the board and generates
        // a heurisitic for how 'good' that board is.  A positive value means a
        // good move for the TOP player, a negative value a good move for the
        // BOTTOM player.
        public override int evaluate(Board b)
        {
            int score = b.stonesAt(13)-b.stonesAt(6);
            int stonesTotal = 0;
            int goAgainsPossible = 0;
            int capturesPossible = 0;

            // calculate heurisitics for TOP player
            for(int i=7; i<=12; i++)
            {
                // add total number of stones in top row
                stonesTotal += b.stonesAt(i);
                // add possible go-agains for top row
                if(b.stonesAt(i) - (13 - i) == 0)
                    goAgainsPossible += 1;
                // add any stones that can be captured
                int landing = i + b.stonesAt(i);
                if(landing < 13)
                {
                    int landingStones = b.stonesAt(landing);
                    // get the position across from the landing
                    int captureTarget = 13 - landing - 1;
                    if(landingStones == 0 && b.whoseMove() == Position.Top && b.stonesAt(captureTarget) != 0)
                    {
                        capturesPossible += b.stonesAt(captureTarget);                        
                    }
                }
            }

            // calculate heurisitics for BOTTOM player (subtract from those for TOP player)
            for(int i=0; i<=5; i++)
            {
                // subtract total number of stones in bottom row
                stonesTotal -= b.stonesAt(i);
                // add possible go-agains for bottom row
                if(b.stonesAt(i) - (6 - i) == 0)
                    goAgainsPossible -= 1;
                // add any stones that can be captured
                int landing = i + b.stonesAt(i);
                if(landing < 6)
                {
                    int landingStones = b.stonesAt(landing);
                    // get the position across from the landing
                    int captureTarget = 13 - landing - 1;
                    if(landingStones == 0 && b.whoseMove() == Position.Bottom && b.stonesAt(captureTarget) != 0)
                    {
                        capturesPossible -= b.stonesAt(captureTarget);
                    }
                }
            }
        score += (stonesTotal + capturesPossible + goAgainsPossible*2);
        return score;
        }

        public override string gloat(){
            return "I WIN! MY ALGORITHM CONQUERS ALL!";
        }
        public override String getImage(){return "Ben.png";}

        // simple helper class to hold a move and the score associated with it
        private class MoveResult
        {
            private int BestMove;
            private int BestScore;
            private bool EndGame;
            // constructor
            public MoveResult(int move, int score, bool end)
            {
                BestMove = move;
                BestScore = score;
                EndGame = end;
            }
            public int GetMove() {return BestMove;}
            public int GetScore() {return BestScore;}
            public bool IsEndGame() {return EndGame;}
        }
    }
}