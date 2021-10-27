/*************************************************
 * CS 212a Program 1 - lg(lg(n))
 * Ben DeWeerd
 * 9.08.2021
 * Prof. Harry Plantinga
 * 
 * This simple program takes the floor(lg(lg(n)),
 * prompting the user for an integer n greater than 1.
 *************************************************/

using System;

namespace Project1
{
	class Program
	{

		static void Main(string[] args)
		{
			Console.WriteLine("This program will calculate floor(lg(lg(n)).");
			bool validInput = false;
			ulong n = 2;
			
			while (!validInput)
			{
				try
				{
					Console.Write("Enter an integer n greater than 1: ");
					n = ulong.Parse(Console.ReadLine());
					if (n <= 1)
					{
						throw new Exception("n must be a number greater than 1.");
					}
					validInput = true;
				}
				catch (Exception e)
				{
					Console.WriteLine($"\nInvalid input: {e.Message}");
					Console.WriteLine("Please try again.\n");
				}
			}

			uint lglgn = GetLg(GetLg(n));
			Console.WriteLine($"\nResult: {lglgn}");
		}

		// helper function, calculates floor(lg(n))
		static uint GetLg(ulong n)
		{
			uint result = 0;
			while (n > 1)
			{
				n /= 2;
				result++;
			}
			return result;
		}
	}
}
