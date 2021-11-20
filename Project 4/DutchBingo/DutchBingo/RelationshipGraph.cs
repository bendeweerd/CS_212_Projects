using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bingo
{
    /// <summary>
    /// Represents a directed labeled graph with a string name at each node
    /// and a string Label for each edge.
    /// </summary>
    class RelationshipGraph
    {
        /*
         *  This data structure contains a list of nodes (each of which has
         *  an adjacency list) and a dictionary (hash table) for efficiently 
         *  finding nodes by name
         */
        public List<GraphNode> nodes { get; private set; }
        private Dictionary<String, GraphNode> nodeDict;

        // constructor builds empty relationship graph
        public RelationshipGraph()
        {
            nodes = new List<GraphNode>();
            nodeDict = new Dictionary<String,GraphNode>();
        }

        // AddNode creates and adds a new node if there isn't already one by that name
        public void AddNode(string name)
        {
            if (!nodeDict.ContainsKey(name))
            {
                GraphNode n = new GraphNode(name);
                nodes.Add(n);
                nodeDict.Add(name, n);
            }
        }

        // AddEdge adds the edge, creating endpoint nodes if necessary.
        // Edge is added to adjacency list of from edges.
        public void AddEdge(string name1, string name2, string relationship) 
        {
            AddNode(name1);                     // create the node if it doesn't already exist
            GraphNode n1 = nodeDict[name1];     // now fetch a reference to the node
            AddNode(name2);
            GraphNode n2 = nodeDict[name2];
            GraphEdge e = new GraphEdge(n1, n2, relationship);
            n1.AddIncidentEdge(e);
        }

        // Get a node by name using dictionary
        public GraphNode GetNode(string name)
        {
            if (nodeDict.ContainsKey(name))
                return nodeDict[name];
            else
                return null;
        }

        // Return a list of each orphan in the graph (nodes with no parent)
        public List<GraphNode> GetOrphans()
        {
            List<GraphNode> orphans = new List<GraphNode>();
            foreach (GraphNode n in nodes)
            { 
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                if (parentEdges.Count == 0) 
                {
                    orphans.Add(n);
                }
            }
            return orphans;
        }

        // Return a list of all the siblings for a given node
        public List<GraphNode> GetSiblings(string name) //TODO: if parents have spouses, add their children as well?
        {
            List<GraphNode> siblings = new List<GraphNode>();
            if (nodeDict.ContainsKey(name))
            {
                List<GraphEdge> parentEdges = nodeDict[name].GetEdges("hasParent");
                foreach (GraphEdge parent in parentEdges)
                {
                    GraphNode parentNode = nodeDict[parent.To()];
                    siblings = GetChildren(parentNode);
                    siblings.Remove(nodeDict[name]);    //don't count yourself as your sibling
                }
            }
            return siblings;
        }

        // Helper function to return all children of a node
        public List<GraphNode> GetChildren(GraphNode parent)
        {
            List<GraphNode> children = new List<GraphNode>();
            List<GraphEdge> childEdges = parent.GetEdges("hasChild");
            foreach (GraphEdge childEdge in childEdges)
            {
                children.Add(nodeDict[childEdge.To()]);
            }
            return children;
        }

        // Return a list of all descendants, with their 'descent level'
        public void GetDescendants(GraphNode ancestor, Dictionary<GraphNode, uint> descendants, uint level)
        {
            List<GraphNode> children = GetChildren(ancestor);
            foreach (GraphNode child in children)
            {
                if (!descendants.ContainsKey(child))    // Only do this once for each person
                {
                    descendants.Add(child, level + 1);
                    GetDescendants(child, descendants, level + 1);      //recursive call, continue down family tree
                }
            }
        }

        /*
         * Run a breadth-first search starting at the root node, return a list of
         * all connected nodes and the shortest path to them from the root
         */
        public List<GraphNode> BreadthFirstSearch(GraphNode root)
        {
            // Create a list to hold all connected nodes
            List<GraphNode> connectedNodes = new List<GraphNode>();
            foreach (GraphNode n in nodes)
            {
                n.Label = "Unvisited";
                n.bfsPathEdges.Clear();
            }
            root.Label = "Visited";
            GraphNode currentNode = root;
            // Create a queue of graph nodes, will hold connected nodes in the order they should be accessed
            Queue<GraphNode> nodeQueue = new Queue<GraphNode>();
            nodeQueue.Enqueue(currentNode);

            while (nodeQueue.Any())
            {
                // Take the first node off the queue
                currentNode = nodeQueue.Dequeue();
                // Get all the edges that connect to the current node
                List<GraphEdge> adjacentEdges = currentNode.GetEdges();
                foreach (GraphEdge edge in adjacentEdges)
                {
                    // Find the node that each edge is pointing to
                    GraphNode toNode = nodeDict[edge.To()];
                    // Only go to adjacent nodes that haven't been visited yet
                    if (toNode.Label == "Unvisited")
                    {
                        // To reach the "to" node, we must have taken all the paths to get to the "from" node
                        foreach (GraphEdge bfsEdge in currentNode.bfsPathEdges)
                        {
                            toNode.AddBFSPathEdge(bfsEdge);
                        }
                        toNode.Label = "Visited";
                        // Add the edge that it took to get to the current point
                        toNode.AddBFSPathEdge(edge);
                        // Add the "to" node to the queue to be traversed later
                        nodeQueue.Enqueue(toNode);
                        connectedNodes.Add(toNode);
                    }
                }
            }
            return connectedNodes;
        }

        // Return a text representation of graph
        public void Dump()
        {
            Console.WriteLine("There are {0} people in this graph:", nodes.Count);
            foreach (GraphNode n in nodes)
            {
                Console.Write(n.ToString());
            }
        }
    }
}
