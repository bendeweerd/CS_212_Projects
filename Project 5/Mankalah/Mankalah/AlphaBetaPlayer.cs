using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mankalah
{
    /*****************************************************************/
    /*
    /* A (hopefully) smart Mankalah Player that combines DFS with
    /* alpha-beta pruning to improve performance.  Needs ~500 ms to beat Bonzo.
    /*
    /*****************************************************************/
    public class AlphaBetaPlayer : Player
    {
        private int NodeCount = 0;
        public AlphaBetaPlayer(Position pos, int timeLimit) : base(pos, "AlphaBeta", timeLimit){}
        public override int chooseMove(Board b)
        {
            // Start a timer to keep track of allowed time
            // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-6.0
            Stopwatch timer = new Stopwatch();
            timer.Start();
            MoveResult bestMove = new MoveResult(0, int.MinValue, false);
            int depth = 1;

            try
            {
                while(!bestMove.IsEndGame())
                {
                    bestMove = minimax(ref b, depth++, int.MinValue, int.MaxValue, timer);
                    Console.WriteLine("Depth: {0}, Best Move: {1}, Predicted Score {2} Nodes Searched: {3} Time: {4}", depth, bestMove.GetMove(), bestMove.GetScore(), NodeCount, timer.ElapsedMilliseconds);
                    NodeCount = 0;
                }
            }
            catch(TimeoutException)
            {
                Console.WriteLine("Depth: {0}, Best Move: {1}, Predicted Score: {2}, Nodes Searched: {3}, Time: {4}", depth, bestMove.GetMove(), bestMove.GetScore(), NodeCount, timer.ElapsedMilliseconds);
                NodeCount = 0;
            }
            return bestMove.GetMove();
        }

        // This function looks at the current state of the board and generates
        // a heurisitic for how 'good' that board is.  A positive value means a
        // good move for the TOP player, a negative value a good move for the
        // BOTTOM player.
        public override int evaluate(Board b)
        {
            int currentScore = b.stonesAt(13) - b.stonesAt(6);
            int score = 0;
            int stonesTotal = 0;
            int goAgainsPossible = 0;
            int capturesPossible = 0;

            // calculate heurisitics for TOP player
            for(int i=7; i<=12; i++)
            {
                // add total number of stones in top row
                stonesTotal += b.stonesAt(i);
                // add possible go-agains for top row
                if(b.stonesAt(i) - (13 - i) == 0) goAgainsPossible += 1;
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
                if(b.stonesAt(i) - (6 - i) == 0) goAgainsPossible -= 1;
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
            // TODO: refine weights
            int currentScoreWeight = 10;
            int stonesTotalWeight = 1;
            int capturesPossibleWeight = 1;
            int goAgainsPossibleWeight = 10;
            score += ( currentScore*currentScoreWeight
                + stonesTotal*stonesTotalWeight 
                + capturesPossible*capturesPossibleWeight 
                + goAgainsPossible*goAgainsPossibleWeight);
            return score;
        }

        // perform a minimax search to find the best possible move,
        // ending when time is up.  it uses alpha-beta pruning;
        // alpha is the maximum value and beta is the minimum 
        private MoveResult minimax(ref Board b, int depth, int alpha, int beta, Stopwatch timer)
        {
            if(timer.ElapsedMilliseconds > getTimePerMove())
            {
                // end if all our time has been used
                throw new TimeoutException();
            }
            // base case: game is over or we've reached the bottom
            if(b.gameOver() || depth == 0)
            {
                return new MoveResult(0, evaluate(b), b.gameOver());
            }

            int bestMove = 0;
            int bestScore;
            bool gameEnded = false;

            if(b.whoseMove() == Position.Top)
            {
                bestScore = int.MinValue;
                for(int move = 7; move <= 12 && alpha < beta; move++)
                {
                    if(b.legalMove(move))
                    {
                        Board testBoard = new Board(b);
                        testBoard.makeMove(move, false);
                        MoveResult val = minimax(ref testBoard, depth - 1, alpha, beta, timer);
                        if(val.GetScore() > bestScore)
                        {
                            bestScore = val.GetScore();
                            bestMove = move;
                            gameEnded = val.IsEndGame();
                        }
                        // prune off unncessary branches
                        if(bestScore > alpha) alpha = bestScore;
                    }
                }
            } else {
                bestScore = int.MaxValue;
                for(int move = 0; move <= 5 && alpha < beta; move++)
                {
                    if(b.legalMove(move))
                    {
                        Board testBoard = new Board(b);
                        testBoard.makeMove(move, false);
                        MoveResult val = minimax(ref testBoard, depth - 1, alpha, beta, timer);
                        if(val.GetScore() < bestScore)
                        {
                            bestScore = val.GetScore();
                            bestMove = move;
                            gameEnded = val.IsEndGame();
                        }
                        // prune off unnecessary branches
                        if(bestScore < beta) beta = bestScore;
                    }
                }
            }

            // we've evaluated another node, keep track of it
            NodeCount++;
            return new MoveResult(bestMove, bestScore, gameEnded);
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