using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems) 
		{
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {               
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ",name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges) {
                    Console.Write("{0} ",e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);     
        }

        // Show all orphans, or people with no parents
        private static void ShowOrphans()
        {
            List<GraphNode> orphans = rg.GetOrphans();
            if (orphans.Count() != 0)
            {
                Console.WriteLine("There are {0} Orphans:", orphans.Count());
                foreach (GraphNode orphan in orphans)
                {
                    Console.WriteLine("    {0}", orphan.Name);
                }
            }
            else
                Console.WriteLine("Hooray, there are no orphans!");
        }

        // Show all siblings of a selected node
        private static void ShowSiblings(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                List<GraphNode> siblings = rg.GetSiblings(name);
                if (siblings.Count() != 0)
                {
                    Console.WriteLine("{0} has {1} sibling(s):", name, siblings.Count());
                    foreach (GraphNode sibling in siblings)
                    {
                        Console.WriteLine("    {0}", sibling.Name);
                    }
                }
                else
                {
                    Console.WriteLine("{0} has no siblings.", name);
                }
            }
            else 
            {
                Console.WriteLine("{0} not found", name);
            }
        }

        // Show all descendants of a selected node and their level of descent
        private static void ShowDescendants(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Dictionary<GraphNode, uint> descendants = new Dictionary<GraphNode, uint>();
                rg.GetDescendants(n, descendants, 0);
                // see https://stackoverflow.com/a/1332 for how to sort dictionary by value
                var sortedDescendants = from entry in descendants orderby entry.Value ascending select entry;
                // Print out each ancestor and their relation
                if (sortedDescendants.Count() > 0)
                {
                    foreach (KeyValuePair<GraphNode, uint> entry in sortedDescendants)
                    {
                        switch (entry.Value)
                        {
                            case 0:
                                Console.WriteLine("  Ancestor: {0}", entry.Key.Name);
                                break;
                            case 1:
                                Console.WriteLine("  Child: {0}", entry.Key.Name);
                                break;
                            case 2:
                                Console.WriteLine("  Grandchild: {0}", entry.Key.Name);
                                break;
                            default:
                                string greats = new StringBuilder().Insert(0, "Great-", (int)entry.Value - 2).ToString();
                                Console.WriteLine("  {0}Grandchild: {1}", greats, entry.Key.Name);
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("  {0} has no descendants.", name);
                }
            }
            else
                Console.WriteLine("  {0} not found", name);
        }

        private static void Bingo(string from, string to)
        {
            GraphNode fromNode = rg.GetNode(from);
            GraphNode toNode = rg.GetNode(to);
            if (fromNode == toNode)
            {
                Console.WriteLine("  Of course {0} is connected to themselves, silly!", from);
                return;
            }
            if ((fromNode != null) && (toNode != null))
            {
                List<GraphNode> connections = rg.BreadthFirstSearch(fromNode);
                if (connections.Contains(toNode))
                {
                    Console.WriteLine("  {0} is connected to {1} by {2} connections:", fromNode.Name, toNode.Name, toNode.bfsPathEdges.Count());
                    foreach (GraphEdge edge in toNode.bfsPathEdges)
                    {
                        Console.WriteLine("    {0}", edge.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("  {0} is not connected to {1}.", fromNode.Name, toNode.Name);
                }
            }
            else 
            {
                if (fromNode == null)
                {
                    Console.WriteLine("  {0} not found.", from);
                }
                else
                {
                    Console.WriteLine("  {0} not found.", to);
                }
            }
        }

        private static void BFS(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                List<GraphNode> bfsResult = rg.BreadthFirstSearch(n);
                foreach (GraphNode connectedNode in bfsResult)
                {
                    Console.WriteLine("{0} is connected to {1} by {2} connections:", n.Name, connectedNode.Name, connectedNode.bfsPathEdges.Count());
                    foreach (GraphEdge edge in connectedNode.bfsPathEdges)
                    {
                        Console.WriteLine("  {0}", edge.ToString());
                    }
                    
                }
            }
            else
            {
                Console.WriteLine("  {0} not found", name);
            }
        }

        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Ben's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                else if (command == "orphans")
                    ShowOrphans();

                else if (command == "siblings" && commandWords.Length > 1)
                    ShowSiblings(commandWords[1]);

                else if (command == "descendants" && commandWords.Length > 1)
                    ShowDescendants(commandWords[1]);

                else if (command == "bingo" && commandWords.Length > 2)
                    Bingo(commandWords[1], commandWords[2]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // illegal command
                // TODO: update with full list of available commands
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname]," +
                        "\n  friends [personname], orphans, siblings[personname]," +
                        "\n  descendants[personname], bingo[fromperson, toperson], exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
