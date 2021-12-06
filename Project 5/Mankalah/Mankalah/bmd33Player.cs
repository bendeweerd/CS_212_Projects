using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mankalah
{
    /*****************************************************************/
    /*
    /* A smart Mankalah Player that combines DFS with alpha-beta
    /* pruning to improve performance.
    /*
    /*****************************************************************/
    public class bmd33Player : Player
    {
        private int NodeCount = 0;
        public bmd33Player(Position pos, int timeLimit) : base(pos, "bmd33", timeLimit){}
        public override int chooseMove(Board b)
        {
            // Start a timer to keep track of allowed time
            // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-6.0
            Stopwatch timer = new Stopwatch();
            timer.Start();
            MoveResult bestMove = new MoveResult(0, int.MinValue, false);
            int depth = 1;
            // use minimax search to find the predicted best move in the time allowed
            try
            {
                while(!bestMove.IsEndGame())
                {
                    bestMove = minimax(ref b, depth++, int.MinValue, int.MaxValue, timer);
                    Console.WriteLine("Depth: {0}, Best Move: {1}, Predicted Score {2} Nodes Searched: {3} Time: {4}", depth, bestMove.GetMove(), bestMove.GetScore(), NodeCount, timer.ElapsedMilliseconds);
                    NodeCount = 0;
                }
            }
            // time has ended, return best calculated result
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
            // weights, refined by playing against itself
            int currentScoreWeight = 10;
            int stonesTotalWeight = 4;
            int capturesPossibleWeight = 4;
            int goAgainsPossibleWeight = 10;
            score += ( currentScore*currentScoreWeight
                + stonesTotal*stonesTotalWeight 
                + capturesPossible*capturesPossibleWeight 
                + goAgainsPossible*goAgainsPossibleWeight);
            // return the weighted score for this board layout
            return score;
        }

        // perform a minimax search to find the best possible move,
        // ending when time is up.  Uses alpha-beta pruning;
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

            // playing on top
            if(b.whoseMove() == Position.Top)
            {
                bestScore = int.MinValue;
                for(int move = 7; move <= 12 && alpha < beta; move++)
                {
                    if(b.legalMove(move))
                    {
                        // make a copy of the current board, make the proposed move
                        Board testBoard = new Board(b);
                        testBoard.makeMove(move, false);
                        MoveResult val = minimax(ref testBoard, depth - 1, alpha, beta, timer);
                        // if the score improves, mark the proposed move as the best one
                        if(val.GetScore() > bestScore)
                        {
                            bestScore = val.GetScore();
                            bestMove = move;
                            gameEnded = val.IsEndGame();
                        }
                        // prune off bad branches
                        if(bestScore > alpha) alpha = bestScore;
                    }
                }
            // playing on bottom
            } else {
                bestScore = int.MaxValue;
                for(int move = 0; move <= 5 && alpha < beta; move++)
                {
                    if(b.legalMove(move))
                    {
                        // make a copy of the current board, make the proposed move
                        Board testBoard = new Board(b);
                        testBoard.makeMove(move, false);
                        MoveResult val = minimax(ref testBoard, depth - 1, alpha, beta, timer);
                        // if the score improves, mark the proposed move as the best one
                        if(val.GetScore() < bestScore)
                        {
                            bestScore = val.GetScore();
                            bestMove = move;
                            gameEnded = val.IsEndGame();
                        }
                        // prune off bad branches
                        if(bestScore < beta) beta = bestScore;
                    }
                }
            }
            NodeCount++;
            // return the best move found
            return new MoveResult(bestMove, bestScore, gameEnded);
        }

        public override string gloat(){
            return "HAHA DIE YOU PEASANT!";
        }
        public override String getImage(){return "Ben.jpg";}

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