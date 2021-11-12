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

        public Dictionary<GraphNode, uint> BreadthFirstSearch(GraphNode currentNode)
        {
            Dictionary<GraphNode, uint> result = new Dictionary<GraphNode, uint>();

            foreach (GraphNode n in nodes)
            {
                n.Label = "Unvisited";
            }
            Queue<(GraphNode, uint)> nodeQueue = new Queue<(GraphNode, uint)>();
            currentNode.Label = "Visited";
            uint currentLevel = 0;
            nodeQueue.Enqueue((currentNode, currentLevel));
            while (nodeQueue.Any())
            {
                (currentNode, currentLevel) = nodeQueue.Dequeue();
                result.Add(currentNode, currentLevel);

                //TODO: filter by specific edge labels
                List<GraphEdge> currentNodeEdges = currentNode.GetEdges();
                foreach (GraphEdge edge in currentNodeEdges)
                {
                    GraphNode toNode = nodeDict[edge.To()];
                    if (toNode.Label == "Unvisited")
                    {
                        toNode.Label = "Visited";
                        nodeQueue.Enqueue((toNode, currentLevel + 1));
                    }
                }
            }
            return result;
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
