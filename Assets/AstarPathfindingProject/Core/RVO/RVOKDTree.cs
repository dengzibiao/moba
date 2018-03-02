using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pathfinding.RVO {
	/** KD-Tree implementation for rvo agents.
	  * \see Pathfinding.RVO.Simulator
	  * 
	  * \astarpro 
	  */
	public class KDTree {
		
		const int MAX_LEAF_SIZE = 10;
		ObstacleTreeNode obstacleRoot;
		private AgentTreeNode[] agentTree;
		
		private List<Agent> agents;
		
		private Simulator simulator;
		
		private bool rebuildAgents = false;
		
		public KDTree (Simulator simulator) {
			this.simulator = simulator;
			//agentTree = new List<AgentTreeNode>();
			agentTree = new AgentTreeNode[0];
		}
		
		public void RebuildAgents () {
			rebuildAgents = true;
		}
		
		public void BuildAgentTree () {
			
			List<Agent> ag = simulator.GetAgents ();
			if (agents == null || agents.Count != ag.Count || rebuildAgents) {
				rebuildAgents = false;
				if (agents == null) agents = new List<Agent>(ag.Count);
				else agents.Clear ();
				
				agents.AddRange (simulator.GetAgents());
			}
			
			if (agentTree.Length != agents.Count*2) {
				agentTree = new AgentTreeNode[agents.Count*2];
				for (int i=0;i<agentTree.Length;i++) agentTree[i] = new AgentTreeNode();
			}
			
			if (agents.Count != 0) {
				BuildAgentTreeRecursive (0,agents.Count, 0);
			}
		}
		
		private void BuildAgentTreeRecursive (int start, int end, int node) {
			
			AgentTreeNode nd = agentTree[node];
			
			nd.start = start;
			nd.end = end;
			nd.xmin = nd.xmax = agents[start].position.x;
			nd.ymin = nd.ymax = agents[start].position.z;
			
			for (int i=start+1;i<end;i++) {
				nd.xmin = Math.Min (nd.xmin, agents[i].position.x);
				nd.xmax = Math.Max (nd.xmax, agents[i].position.x);
				nd.ymin = Math.Min (nd.ymin, agents[i].position.z);
				nd.ymax = Math.Max (nd.ymax, agents[i].position.z);
			}
			
			//Debug.DrawLine (new Vector3 (nd.xmin,0,nd.ymin),new Vector3 (nd.xmin,0,nd.ymax),Color.blue);
			//Debug.DrawLine (new Vector3 (nd.xmax,0,nd.ymin),new Vector3 (nd.xmax,0,nd.ymax),Color.blue);
			//Debug.DrawLine (new Vector3 (nd.xmin,0,nd.ymin),new Vector3 (nd.xmax,0,nd.ymin),Color.blue);
			//Debug.DrawLine (new Vector3 (nd.xmin,0,nd.ymax),new Vector3 (nd.xmax,0,nd.ymax),Color.blue);
			
			if (end - start > MAX_LEAF_SIZE) {
				/* This is no leaf node */
				
				bool isVertical = (nd.xmax - nd.xmin > nd.ymax - nd.ymin);
				float splitValue = (isVertical ? 0.5f * (nd.xmax + nd.xmin) : 0.5f * (nd.ymax + nd.ymin));
				
#if ASTARDEBUG
				if (isVertical) {
					Debug.DrawLine (new Vector3 (splitValue,0,-2000),new Vector3(splitValue,0,2000),Color.green);
				} else {
					Debug.DrawLine (new Vector3 (-2000,0,splitValue),new Vector3(2000,0,splitValue),Color.green);
				}
#endif
				
				int left = start;
				int right = end;
				
				while (left < right) {
					while (left < right && (isVertical ? agents[left].position.x : agents[left].position.z) < splitValue) left++;
					while (right > left && (isVertical ? agents[right-1].position.x : agents[right-1].position.z) >= splitValue) right--;
					
					if (left < right) {
						Agent tmp = agents[left];
						agents[left] = agents[right-1];
						agents[right-1] = tmp;
						
						left++;
						right--;
					}
				}
				
				int leftSize = left-start;
				if (leftSize == 0) {
					leftSize++;
					left++;
					right++;
				}
				
				nd.left = node+1;
				nd.right = node+1 + (2*leftSize-1);
				
				agentTree[node] = nd;
				
				BuildAgentTreeRecursive (start, left, agentTree[node].left);
				BuildAgentTreeRecursive (left, end, agentTree[node].right);
			} else {
				agentTree[node] = nd;
			}
		}
		
		
		public void BuildObstacleTree () {
			List<ObstacleVertex> obstacles = Pathfinding.Util.ListPool<ObstacleVertex>.Claim ();
			List<ObstacleVertex> src = simulator.GetObstacles ();
			
			for (int i=0;i<src.Count;i++) {
				ObstacleVertex c = src[i];
				do {
					obstacles.Add (c);
					c = c.next;
				} while (c != src[i]);
			}
			
			RecycleOTN (obstacleRoot);
			
			obstacleRoot = BuildObstacleTreeRecursive (obstacles);
		}
		
		private int countDepth (ObstacleTreeNode node) {
			if (node == null) return 0;
			return 1 + countDepth (node.left) + countDepth (node.right);
		}
		
		private ObstacleTreeNode BuildObstacleTreeRecursive (List<ObstacleVertex> obstacles) {
			if (obstacles.Count	== 0) {
				Pathfinding.Util.ListPool<ObstacleVertex>.Release (obstacles);
				return null;
			}
			
			ObstacleTreeNode node = GetOTN ();//new ObstacleTreeNode ();
			
			int optimalSplit = 0;
			int minLeft = obstacles.Count;
			int minRight = obstacles.Count;
			
			for (int i=0;i<obstacles.Count;i++){
				int leftSize = 0;
				int rightSize = 0;
				
				ObstacleVertex obI1 = obstacles[i];
				ObstacleVertex obI2 = obI1.next;
				
				for (int j=0;j<obstacles.Count;j++) {
					if (i == j) continue;
					
					ObstacleVertex obJ1 = obstacles[j];
					ObstacleVertex obJ2 = obJ1.next;
					
					float j1LeftOfI = Polygon.TriangleArea (obI1.position, obI2.position, obJ1.position);
					float j2LeftOfI = Polygon.TriangleArea (obI1.position, obI2.position, obJ2.position);
					
					if (j1LeftOfI >= -float.Epsilon	&& j2LeftOfI >= -float.Epsilon) {
						leftSize++;
					} else if (j1LeftOfI <= float.Epsilon && j2LeftOfI <= float.Epsilon) {
						rightSize++;
					} else {
						leftSize++;
						rightSize++;
					}
					
					int v1a = Math.Max (leftSize,rightSize);
					int v1b = Math.Min (leftSize, rightSize);
					
					int v2a = Math.Max (minLeft, minRight);
					int v2b = Math.Min (minLeft, minRight);
					
					// (v1a, v1b) >= (v2a, v2b)
					
					//!(lhs._a < rhs._a || !(rhs._a < lhs._a) && lhs._b < rhs._b);
					
					if (!(v1a < v2a || !(v2a < v1a) && v1b < v2b)) {
						break;
					}
				}
				
				int x1a = Math.Max (leftSize,rightSize);
				int x1b = Math.Min (leftSize, rightSize);
				
				int x2a = Math.Max (minLeft, minRight);
				int x2b = Math.Min (minLeft, minRight);
				
				if (x1a < x2a || !(x2a < x1a) && x1b < x2b) {
					minLeft = leftSize;
					minRight = rightSize;
					optimalSplit = i;
				}
			}
			
			//Split node
			{
				List<ObstacleVertex> leftObstacles = Pathfinding.Util.ListPool<ObstacleVertex>.Claim (minLeft);//new List<ObstacleVertex>(minLeft);
				List<ObstacleVertex> rightObstacles = Pathfinding.Util.ListPool<ObstacleVertex>.Claim (minRight);//new List<ObstacleVertex>(minRight);
				
				//int leftCounter = 0;
				//int rightCounter = 0;
				
				int i = optimalSplit;
				
				ObstacleVertex obI1 = obstacles[i];
				ObstacleVertex obI2 = obI1.next;
				
				for (int j=0;j<obstacles.Count;j++) {
					if (optimalSplit == j) continue;
					
					ObstacleVertex obJ1 = obstacles[j];
					ObstacleVertex obJ2 = obJ1.next;
					
					float j1LeftOfI = Polygon.TriangleArea (obI1.position, obI2.position, obJ1.position);
					float j2LeftOfI = Polygon.TriangleArea (obI1.position, obI2.position, obJ2.position);
					
					if (j1LeftOfI >= -float.Epsilon	&& j2LeftOfI >= -float.Epsilon) {
						leftObstacles.Add (obJ1);
					} else if (j1LeftOfI <= float.Epsilon && j2LeftOfI <= float.Epsilon) {
						rightObstacles.Add (obJ1);
					} else {
						
						//Split Obstacle
						
						float t = Polygon.IntersectionFactor (obJ1.position,obJ2.position,obI1.position,obI2.position);
						
						Vector3 splitPoint = obJ1.position + (obJ2.position - obJ1.position) * t;
						
						ObstacleVertex obst = new ObstacleVertex();
						
						obst.position = splitPoint;
						obJ1.next = obst;
						obst.prev = obJ1;
						obst.next = obJ2;
						obJ2.prev = obst;
						
						obst.dir = obJ1.dir;
						
						//Mark as split point so that it can be identified
						//and removed if an update of this obstacle should be done
						obst.split = true;
						
						//New vertex is added at split, angle will be 180 degrees
						obst.convex = true;
#if ASTARDEBUG
						Debug.DrawRay (splitPoint,Vector3.up,Color.cyan);
#endif				
						
						if (j1LeftOfI > 0.0f) {
							leftObstacles.Add (obJ1);
							rightObstacles.Add (obst);
						} else {
							rightObstacles.Add (obJ1);
							leftObstacles.Add (obst);
						}
					}
				}
				
				Pathfinding.Util.ListPool<ObstacleVertex>.Release (obstacles);
				
				node.obstacle = obI1;
				node.left = BuildObstacleTreeRecursive (leftObstacles);
				node.right = BuildObstacleTreeRecursive (rightObstacles);
				return node;
			}
		}
		
		
		public void GetAgentNeighbours (Agent agent, float rangeSq) {
			QueryAgentTreeRecursive (agent, ref rangeSq, 0);
		}
		
		public void GetObstacleNeighbours (Agent agent, float rangeSq) {
			QueryObstacleTreeRecursive (agent, rangeSq, obstacleRoot);
		}
		
		private float Sqr (float v) { return v*v; }
		
		private void QueryAgentTreeRecursive (Agent agent, ref float rangeSq, int node) {
			if (agentTree[node].end - agentTree[node].start <= MAX_LEAF_SIZE) {
				for (int i = agentTree[node].start; i < agentTree[node].end; i++) {
					rangeSq = agent.InsertAgentNeighbour (agents[i], rangeSq);
				}
			} else {
				//Calculate squared distance to the closest point inside the bounding boxes of the child nodes
				float distSqLeft = Sqr (Math.Max (0.0f, agentTree[agentTree[node].left].xmin - agent.position.x))
								 + Sqr (Math.Max (0.0f, agent.position.x - agentTree[agentTree[node].left].xmax))
								 + Sqr (Math.Max (0.0f, agentTree[agentTree[node].left].ymin - agent.position.z))
								 + Sqr (Math.Max (0.0f, agent.position.z - agentTree[agentTree[node].left].ymax));
				
				float distSqRight =Sqr (Math.Max (0.0f, agentTree[agentTree[node].right].xmin - agent.position.x))
								 + Sqr (Math.Max (0.0f, agent.position.x - agentTree[agentTree[node].right].xmax))
								 + Sqr (Math.Max (0.0f, agentTree[agentTree[node].right].ymin - agent.position.z))
								 + Sqr (Math.Max (0.0f, agent.position.z - agentTree[agentTree[node].right].ymax));
				if (distSqLeft < distSqRight) {
					if (distSqLeft < rangeSq) {
						QueryAgentTreeRecursive (agent, ref rangeSq, agentTree[node].left);
						
						if (distSqRight < rangeSq) {
							QueryAgentTreeRecursive (agent, ref rangeSq, agentTree[node].right);
						}
					}
				} else {
					if (distSqRight < rangeSq) {
						QueryAgentTreeRecursive (agent, ref rangeSq, agentTree[node].right);
						
						if (distSqLeft < rangeSq) {
							QueryAgentTreeRecursive (agent, ref rangeSq, agentTree[node].left);
						}
					}
				}
			}
		}
		
		private void QueryObstacleTreeRecursive (Agent agent, float rangeSq, ObstacleTreeNode node) {
			if (node == null) return;
			
			ObstacleVertex ob1 = node.obstacle;
			ObstacleVertex ob2 = ob1.next;
			
			float agentLeftOfLine = Polygon.TriangleArea (ob1.position,ob2.position,agent.position);
			
			QueryObstacleTreeRecursive (agent, rangeSq, agentLeftOfLine >= 0.0f ? node.left : node.right);
			
			Vector3 dir2D = ob1.position-ob2.position;
			dir2D.y = 0;
			
			//Isn't this 4 times too large since TriangleArea is actually 2*triangle area
			float distSqLine = Sqr(agentLeftOfLine) / dir2D.sqrMagnitude;
			
			if (distSqLine < rangeSq) {
				if (agentLeftOfLine < 0.0f) {
					/*
			         * Try obstacle at this node only if agent is on right side of
			         * obstacle (and can see obstacle).
			         */
					agent.InsertObstacleNeighbour (node.obstacle, rangeSq);
				}
				
				QueryObstacleTreeRecursive (agent, rangeSq, (agentLeftOfLine >= 0.0f ? node.right : node.left));
			}
		}
		
		private struct AgentTreeNode {
			public int start, end, left, right;
			public float xmax,ymax, xmin, ymin;
		}
		
		private class ObstacleTreeNode {
			public ObstacleTreeNode left, right;
			public ObstacleVertex obstacle;
		}
		
		private static Stack<ObstacleTreeNode> OTNPool = new Stack<ObstacleTreeNode>();
		private static ObstacleTreeNode GetOTN () {
			if (OTNPool.Count > 0) {
				return OTNPool.Pop ();
			} else {
				return new ObstacleTreeNode ();
			}
		}
		
		private static void RecycleOTN (ObstacleTreeNode node) {
			if (node == null) return;
			OTNPool.Push (node);
			node.obstacle = null;
			RecycleOTN (node.left);
			RecycleOTN (node.right);
		}
	}
}