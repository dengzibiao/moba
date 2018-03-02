using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

/** Adds a navmesh as RVO obstacles.
 * Add this to a scene in which has a navmesh based graph, when scanning (or loading from cache) the graph
 * it will be added as RVO obstacles to the RVOSimulator (which must exist in the scene).
 * 
 * \todo Support for grid based graphs will be added in future versions
 * 
 * \astarpro 
 */
[AddComponentMenu("Local Avoidance/RVO Navmesh")]
public class RVONavmesh : GraphModifier {
	
	/** Height of the walls added for each obstacle edge.
	 * If a graph contains overlapping you should set this low enough so
	 * that edges on different levels do not interfere, but high enough so that
	 * agents cannot move over them by mistake.
	 */
	public float wallHeight = 5;
	
	/** Obstacles currently added to the simulator */
	private List<ObstacleVertex> obstacles = new List<ObstacleVertex>();
	
	/** Last simulator used */
	private Simulator lastSim = null;
	
	public override void OnPostCacheLoad ()
	{
		OnLatePostScan ();
	}
	
	public override void OnLatePostScan () {
		if (!Application.isPlaying) return;
		
		RemoveObstacles ();
		
		NavGraph[] graphs = AstarPath.active.graphs;
		
		RVOSimulator rvosim = FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
		if (rvosim == null) throw new System.NullReferenceException ("No RVOSimulator could be found in the scene. Please add one to any GameObject");
		
		Pathfinding.RVO.Simulator sim = rvosim.GetSimulator ();
		
		for (int i=0;i<graphs.Length;i++) {
			AddGraphObstacles (sim, graphs[i]);
		}
		
		sim.UpdateObstacles ();
	}
	
	/** Removes obstacles which were added with AddGraphObstacles */
	public void RemoveObstacles () {
		if (lastSim == null) return;
		
		Pathfinding.RVO.Simulator sim = lastSim;
		lastSim = null;
		
		for (int i=0;i<obstacles.Count;i++) sim.RemoveObstacle (obstacles[i]);
		
		obstacles.Clear ();
	}
	
	/** Adds obstacles for a graph */
	public void AddGraphObstacles (Pathfinding.RVO.Simulator sim, NavGraph graph) {
		if (obstacles.Count > 0 && lastSim != null && lastSim != sim) {
			Debug.LogError ("Simulator has changed but some old obstacles are still added for the previous simulator. Deleting previous obstacles.");
			RemoveObstacles ();
		}
		
		//Remember which simulator these obstacles were added to
		lastSim = sim;
		
		INavmesh ng = graph as INavmesh;
		
		if (ng == null) return;
		
		Node[] nodes = graph.nodes;
		
		Int3[] vertices = ng.vertices;
		
		int[] uses = new int[3];
		
		for (int i=0;i<nodes.Length;i++) {
			MeshNode node = nodes[i] as MeshNode;
			
			uses[0] = uses[1] = uses[2] = 0;
			
			if (node != null) {
				
				for (int j=0;j<node.connections.Length;j++) {
					MeshNode other = node.connections[j] as MeshNode;
					if (other == null) continue;
					
					int first = -1;
					int second = -1;
					
					for (int x=0;x<3;x++) {
						for (int y=0;y<3;y++) {
							if (node[x] == other[y] && first < 0) {
								first = x;
								break;
							} else if (node[x] == other[y]) {
								second = x;
								break;
							}
						}
						if (second >= 0) break;
					}
					
					//Only shared one vertex
					if (second == -1) continue;
					
					if ((first+1) % 3 == second) {
						uses[first]++;
					} else { //if ((second+1) % 3 == first) {
						uses[second]++;
					}
				}
			}
			
			for (int j=0;j<3;j++) {
				if (uses[j] == 0) {
					Vector3 v1 = (Vector3)vertices[node[j]];
					Vector3 v2 = (Vector3)vertices[node[(j+1) % 3]];
					
					//I think node vertices always should be clockwise, but it's good to be certain
					if (!Polygon.IsClockwise (v1,v2,(Vector3)vertices[node[(j+2) % 3]])) {
						Vector3 tmp = v2;
						v2 = v1;
						v1 = tmp;
					}
					
#if ASTARDEBUG
					Debug.DrawLine (v1,v2,Color.red);
					Debug.DrawRay (v1,Vector3.up*wallHeight,Color.red);
#endif
					
					float height = System.Math.Abs(v1.y-v2.y);
					height = System.Math.Max (height,5);
					
					obstacles.Add (sim.AddObstacle (v1, v2, wallHeight));
				}
			}
		}
		
	}
}
