using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    class PathNodePriorityQueue
    {
        LinkedList<PathNode> priorityQueue;

        public PathNodePriorityQueue()
        {
            priorityQueue = new LinkedList<PathNode>();
        }

        public int size()
        {
            return priorityQueue.Count;
        }

        public IEnumerable<PathNode> getEnumerableType()
        {
            return priorityQueue;
        }

        public PathNode pop()
        {
            PathNode result = priorityQueue.First.Value;
            priorityQueue.RemoveFirst();
            return result;

        }

        public void insertNode(PathNode toInsert)
        {
            int i = 0;
            PathNode toInsertBefore = null;
            LinkedListNode<PathNode> insertBeforeNode = priorityQueue.First;
            foreach (PathNode test in priorityQueue)
            {
                
                if (test.costToGetHere + test.approximateDistanceToTarget > toInsert.approximateDistanceToTarget + toInsert.costToGetHere)
                {
                    toInsertBefore = test;
                    
                    break;
                }
                insertBeforeNode = insertBeforeNode.Next;
                i++;
            }
            if (toInsertBefore != null)
            {
                priorityQueue.AddBefore(insertBeforeNode, toInsert);
            }
            else
            {
                priorityQueue.AddLast(toInsert);
            }
        }
    }

}
