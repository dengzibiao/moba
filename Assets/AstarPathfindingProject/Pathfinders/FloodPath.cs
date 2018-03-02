using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding
{
	/** Floods the area completely for easy computation of any path to a single point.
This path is a bit special, because it does not do anything useful by itself. What it does is that it calculates paths to all nodes it can reach, floods it.
This data will remain stored in the path. Then you can call a FloodPathTracer path, that path will trace the path from it's starting point all the way to where this path started flooding and thus generating a path extremely quickly.\n
It is very useful in for example TD (Tower Defence) games where all your AIs will walk to the same point, but from different places, and you do not update the graph or change the target point very often,
what changes is their positions and new AIs spawn all the time (which makes it hard to use the MultiTargetPath).\n

With this path type, it can all be handled easily.
- At start, you simply start ONE FloodPath and save the reference (it will be needed later).
- Then when a unit is spawned or needs it's path recalculated, start a FloodPathTracer path from it's position.
   It will then find the shortest path to the point specified when you called the FloodPath extremely quickly.
- If you update the graph (for example place a tower in a TD game) or need to change the target point, you simply call a new FloodPath (and store it's reference).
 
\version From 3.2 and up, path traversal data is now stored in the path class.
So you can now use other path types in parallel with this one.

Here follows some example code of the above list of steps:
\code
public static FloodPath fpath;

public void Start () {
	fpath = new FloodPath (someTargetPosition, null);
	AstarPath.StartPath (fpath);
}
\endcode

When searching for a new path to \a someTargetPosition from let's say \a transform.position, you do
\code
FloodPathTracer fpathTrace = new FloodPathTracer (transform.position,fpath,null);
seeker.StartPath (fpathTrace,OnPathComplete);
\endcode
Where OnPathComplete is your callback function.

\note This path type relies on pathIDs being stored in the graph, but pathIDs are only 16 bits, meaning they will overflow after 65536 paths.
When that happens all pathIDs in the graphs will be cleared, so at that point you will also need to recalculate the FloodPath.\n
To do so, register to the AstarPath.On65KOverflow callback:
\code
public void Start () {
	AstarPath.On65KOverflow += MyCallbackFunction;
}

public void MyCallbackFunction () {
	//The callback is nulled every time it is called, so we need to register again
	AstarPath.On65KOverflow += MyCallbackFunction;
	
	//Recalculate the path
} \endcode 
This will happen after a very long time into the game, but it will happen eventually (look at the 'path number' value on the log messages when paths are completed for a hint about when)
\n
\n
Anothing thing to note is that if you are using NNConstraints on the FloodPathTracer, they must always inherit from Pathfinding.PathIDConstraint.\n
The easiest is to just modify the instance of PathIDConstraint which is created as the default one.

\astarpro

\shadowimage{floodPathExample.png}

\ingroup paths

*/
	public class FloodPath : Path
	{
		
		public Vector3 originalStartPoint;
		public Vector3 startPoint;
		public Node startNode;
		
		protected Dictionary<Node,Node> parents;
		
		public bool HasPathTo (Node node) {
			return parents != null && parents.ContainsKey (node);
		}
		
		public Node GetParent (Node node) {
			return parents[node];
		}
		
		/** Creates a new FloodPath instance */
		[System.Obsolete ("Please use the Construct method instead")]
		public FloodPath (Vector3 start, OnPathDelegate callbackDelegate) {
			Setup (start, callbackDelegate);
			heuristic = Heuristic.None;
		}
		
		public static FloodPath Construct (Vector3 start, OnPathDelegate callback = null) {
			FloodPath p = PathPool<FloodPath>.GetPath ();
			p.Setup (start, callback);
			return p;
		}
		
		protected void Setup (Vector3 start, OnPathDelegate callback) {
			this.callback = callback;
			originalStartPoint = start;
			startPoint = start;
			heuristic = Heuristic.None;
		}
		
		public override void Reset () {
			base.Reset ();
			originalStartPoint = Vector3.zero;
			startPoint = Vector3.zero;
			startNode = null;
			parents = new Dictionary<Node,Node> ();
		}
		
		public FloodPath () {}
		
		protected override void Recycle () {
			PathPool<FloodPath>.Recycle (this);
		}
		
		public override void Prepare (){
			AstarProfiler.StartProfile ("Get Nearest");
			
			//Initialize the NNConstraint
			nnConstraint.tags = enabledTags;
			NNInfo startNNInfo 	= AstarPath.active.GetNearest (originalStartPoint,nnConstraint);
			
			startPoint = startNNInfo.clampedPosition;
			startNode = startNNInfo.node;
			
			AstarProfiler.EndProfile ();
			
#if ASTARDEBUG
			Debug.DrawLine ((Vector3)startNode.position,startPoint,Color.blue);
#endif
			
			if (startNode == null) {
				Error ();
				LogError ("Couldn't find a close node to the start point");
				return;
			}
			
			if (!startNode.walkable) {
#if ASTARDEBUG
				Debug.DrawRay (startPoint,Vector3.up,Color.red);
				Debug.DrawLine (startPoint,(Vector3)startNode.position,Color.red);
#endif
				Error ();
				LogError ("The node closest to the start point is not walkable");
				return;
			}
		}
		public override void Initialize () {
			
			NodeRun startRNode = startNode.GetNodeRun (runData);
			startRNode.pathID = pathID;
			startRNode.parent = null;
			startRNode.cost = 0;
			startRNode.g = startNode.penalty;
			startNode.UpdateH (Int3.zero,Heuristic.None,0, startRNode);
			
			startNode.Open (runData,startRNode,Int3.zero,this);
			parents[startNode] = null;
			
			searchedNodes++;
			
			//any nodes left to search?
			if (runData.open.numberOfItems <= 1) {
				CompleteState = PathCompleteState.Complete;
				return;
			}
			
			currentR = runData.open.Remove ();
		}
		
		/** Opens nodes until there are none left to search (or until the max time limit has been exceeded) */
		public override void CalculateStep (long targetTick) {
			
			int counter = 0;
			
			//Continue to search while there hasn't ocurred an error and the end hasn't been found
			while (!IsDone()) {
				
				//@Performance Just for debug info
				searchedNodes++;
				
				//Loop through all walkable neighbours of the node and add them to the open list.
				currentR.node.Open (runData,currentR, Int3.zero,this);
				
				parents[currentR.node] = currentR.parent.node;
				
				//any nodes left to search?
				if (runData.open.numberOfItems <= 1) {
					CompleteState = PathCompleteState.Complete;
					return;
				}
				
				//Select the node with the lowest F score and remove it from the open list
				currentR = runData.open.Remove ();
				
				//Check for time every 500 nodes, roughly every 0.5 ms usually
				if (counter > 500) {
					
					//Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
					if (System.DateTime.UtcNow.Ticks >= targetTick) {
						
						//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
						return;
					}
					
					counter = 0;
				}
				
				counter++;
			
			}
		}
	}
}

