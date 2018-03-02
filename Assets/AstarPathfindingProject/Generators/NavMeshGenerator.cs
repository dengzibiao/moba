//#define ASTARDEBUG    //Some debugging
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;

namespace Pathfinding {
	public interface INavmesh {
		Int3[] vertices {
			get;
			set;
		}
		
		/** Bounding Box Tree */
		BBTree bbTree {
			get;
			set;
		}
		
		//Int3[] originalVertices {
		//	get;
		//	set;
		//}
	}
	
	[System.Serializable]
	[JsonOptIn]
	/** Generates graphs based on navmeshes.
\ingroup graphs
Navmeshes are meshes where each polygon define a walkable area.
These are great because the AI can get so much more information on how it can walk.
Polygons instead of points mean that the funnel smoother can produce really nice looking paths and the graphs are also really fast to search
and have a low memory footprint because of their smaller size to describe the same area (compared to grid graphs).
\see Pathfinding.RecastGraph

\shadowimage{navmeshgraph_graph.png}
\shadowimage{navmeshgraph_inspector.png}

	 */
	public class NavMeshGraph : NavGraph, INavmesh, ISerializableGraph, IUpdatableGraph, IFunnelGraph
	,IRaycastableGraph 
	{
		
		public override Node[] CreateNodes (int number) {
			MeshNode[] tmp = new MeshNode[number];
			for (int i=0;i<number;i++) {
				tmp[i] = new MeshNode ();
				tmp[i].penalty = initialPenalty;
			}
			return tmp as Node[];
		}
		
		[JsonMember]
		public Mesh sourceMesh; /**< Mesh to construct navmesh from */
		
		[JsonMember]
		public Vector3 offset; /**< Offset in world space */
		
		[JsonMember]
		public Vector3 rotation; /**< Rotation in degrees */
		
		[JsonMember]
		public float scale = 1; /**< Scale of the graph */
		
		[JsonMember]
		/** More accurate nearest node queries.
		 * When on, looks for the closest point on every triangle instead of if point is inside the node triangle in XZ space.
		 * This is slower, but a lot better if your mesh contains overlaps (e.g bridges over other areas of the mesh).
		 * Note that for maximum effect the Full Get Nearest Node Search setting should be toggled in A* Inspector Settings.
		 */
		public bool accurateNearestNode = true;
		
		//public Node[] graphNodes;
		
		/** Bounding Box Tree. Enables really fast lookups of nodes. \astarpro */
		BBTree _bbTree;
		public BBTree bbTree {
			get { return _bbTree; }
			set { _bbTree = value;}
		}
		
		[System.NonSerialized]
		Int3[] _vertices;
		
		public Int3[] vertices {
			get {
				return _vertices;
			}
			set {
				_vertices = value;
			}
		}
		
		[System.NonSerialized]
		Vector3[] originalVertices;
		
		//[System.NonSerialized]
		//Int3[] _originalVertices;
		Matrix4x4 _originalMatrix;
		
		/*public Int3[] originalVertices {
			get { 	return _originalVertices; 	}
			set { 	_originalVertices = value;	}
		}*/
		
		[System.NonSerialized]
		public int[] triangles;
		
		public void GenerateMatrix () {
			
#if !PhotonImplementation
			matrix = Matrix4x4.TRS (offset,Quaternion.Euler (rotation),new Vector3 (scale,scale,scale));
#else
			matrix = Matrix4x4.TRS (offset,rotation,new Vector3 (scale,scale,scale));
#endif
			
		}
		
		/** Relocates the nodes to match the newMatrix.
		 * The "oldMatrix" variable can be left out in this function call (only for this graph generator) since it is not used */
		public override void RelocateNodes (Matrix4x4 oldMatrix, Matrix4x4 newMatrix) {
			//base.RelocateNodes (oldMatrix,newMatrix);
			
			if (vertices == null || vertices.Length == 0 || originalVertices == null || originalVertices.Length != vertices.Length) {
				return;
			}
			
			for (int i=0;i<vertices.Length;i++) {
				//Vector3 tmp = inv.MultiplyPoint3x4 (vertices[i]);
				//vertices[i] = (Int3)newMatrix.MultiplyPoint3x4 (tmp);
				vertices[i] = (Int3)newMatrix.MultiplyPoint3x4 ((Vector3)originalVertices[i]);
			}
			
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = (MeshNode)nodes[i];
				node.position = (vertices[node.v1]+vertices[node.v2]+vertices[node.v3])/3F;
				
				if (node.connections != null) {
					for (int q=0;q<node.connections.Length;q++) {
						node.connectionCosts[q] = (node.position-node.connections[q].position).costMagnitude;
					}
				}
			}
		}
	
		public static NNInfo GetNearest (INavmesh graph, Node[] nodes, Vector3 position, NNConstraint constraint, bool accurateNearestNode) {
			if (nodes == null || nodes.Length == 0) {
				Debug.LogError ("NavGraph hasn't been generated yet or does not contain any nodes");
				return new NNInfo ();
			}
			
			if (constraint == null) constraint = NNConstraint.None;
			
			
			Int3[] vertices = graph.vertices;
			
			//Query BBTree
			
			if (graph.bbTree == null) {
				return GetNearestForce (nodes,vertices, position, constraint, accurateNearestNode);
				//Debug.LogError ("No Bounding Box Tree has been assigned");
				//return new NNInfo ();
			}
			
			//Searches in radiuses of 0.05 - 0.2 - 0.45 ... 1.28 times the average of the width and depth of the bbTree
			float w = (graph.bbTree.root.rect.width + graph.bbTree.root.rect.height)*0.5F*0.02F;
			
			NNInfo query = graph.bbTree.QueryCircle (position,w,constraint);//graph.bbTree.Query (position,constraint);
			
			if (query.node == null) {
				
				for (int i=1;i<=8;i++) {
					query = graph.bbTree.QueryCircle (position, i*i*w, constraint);
					
					if (query.node != null || (i-1)*(i-1)*w > AstarPath.active.maxNearestNodeDistance*2) { // *2 for a margin
						break;
					}
				}
			}
			
			if (query.node != null) {
				query.clampedPosition = ClosestPointOnNode (query.node as MeshNode,vertices,position);
			}
			
			if (query.constrainedNode != null) {
				if (constraint.constrainDistance && ((Vector3)query.constrainedNode.position - position).sqrMagnitude > AstarPath.active.maxNearestNodeDistanceSqr) {
					query.constrainedNode = null;
				} else {
					query.constClampedPosition = ClosestPointOnNode (query.constrainedNode as MeshNode, vertices, position);
				}
			}
			
			return query;	
		}
		
		public override NNInfo GetNearest (Vector3 position, NNConstraint constraint, Node hint) {
			return GetNearest (this, nodes,position, constraint, accurateNearestNode);
		}
		
		/** This performs a linear search through all polygons returning the closest one.
		 * This is usually only called in the Free version of the A* Pathfinding Project since the Pro one supports BBTrees and will do another query
		 */
		public override NNInfo GetNearestForce (Vector3 position, NNConstraint constraint) {
			
			return GetNearestForce (nodes,vertices,position,constraint, accurateNearestNode);
			//Debug.LogWarning ("This function shouldn't be called since constrained nodes are sent back in the GetNearest call");
			
			//return new NNInfo ();
		}
		
		/** This performs a linear search through all polygons returning the closest one */
		public static NNInfo GetNearestForce (Node[] nodes, Int3[] vertices, Vector3 position, NNConstraint constraint, bool accurateNearestNode) {
			NNInfo nn = GetNearestForceBoth (nodes,vertices,position,constraint,accurateNearestNode);
			nn.node = nn.constrainedNode;
			nn.clampedPosition = nn.constClampedPosition;
			return nn;
		}
		
		/** This performs a linear search through all polygons returning the closest one.
		  * This will fill the NNInfo with .node for the closest node not necessarily complying with the NNConstraint, and .constrainedNode with the closest node
		  * complying with the NNConstraint.
		  * \see GetNearestForce(Node[],Int3[],Vector3,NNConstraint,bool)
		  */
		public static NNInfo GetNearestForceBoth (Node[] nodes, Int3[] vertices, Vector3 position, NNConstraint constraint, bool accurateNearestNode) {
			Int3 pos = (Int3)position;
			
			float minDist = -1;
			Node minNode = null;
			
			float minConstDist = -1;
			Node minConstNode = null;
			
			float maxDistSqr = constraint.constrainDistance ? AstarPath.active.maxNearestNodeDistanceSqr : float.PositiveInfinity;
			
			if (nodes == null || nodes.Length == 0) {
				return new NNInfo ();
			}
			
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = nodes[i] as MeshNode;
				
				if (accurateNearestNode) {
					
					Vector3 closest = Polygon.ClosestPointOnTriangle((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],(Vector3)vertices[node.v3],position);
					float dist = ((Vector3)pos-closest).sqrMagnitude;
					
					if (minNode == null || dist < minDist) {
						minDist = dist;
						minNode = node;
					}
					
					if (dist < maxDistSqr && constraint.Suitable (node)) {
						if (minConstNode == null || dist < minConstDist) {
							minConstDist = dist;
							minConstNode = node;
						}
					}
					
				} else {
					#if SafeIntMath
					if (!Polygon.IsClockwise ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],position) || !Polygon.IsClockwise ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3],position) || !Polygon.IsClockwise ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1],position))
					#else
					if (!Polygon.IsClockwise (vertices[node.v1],vertices[node.v2],pos) || !Polygon.IsClockwise (vertices[node.v2],vertices[node.v3],pos) || !Polygon.IsClockwise (vertices[node.v3],vertices[node.v1],pos))
					#endif
					{
						
						float dist = (node.position-pos).sqrMagnitude;
						if (minNode == null || dist < minDist) {
							minDist = dist;
							minNode = node;
						}
						
						if (dist < maxDistSqr && constraint.Suitable (node)) {
							if (minConstNode == null || dist < minConstDist) {
								minConstDist = dist;
								minConstNode = node;
							}
						}
						
					} else {
					
#if ASTARDEBUG
						Debug.DrawLine ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],Color.blue);
						Debug.DrawLine ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3],Color.blue);
						Debug.DrawLine ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1],Color.blue);
#endif
						
						int dist = Mathfx.Abs (node.position.y-pos.y);
						
						if (minNode == null || dist < minDist) {
							minDist = dist;
							minNode = node;
						}
						
						if (dist < maxDistSqr && constraint.Suitable (node)) {
							if (minConstNode == null || dist < minConstDist) {
								minConstDist = dist;
								minConstNode = node;
							}
						}
					}
				}
			}
			
			NNInfo nninfo = new NNInfo (minNode);
			
			//Find the point closest to the nearest triangle
				
			if (nninfo.node != null) {
				MeshNode node = nninfo.node as MeshNode;//minNode2 as MeshNode;
				
				Vector3 clP = Polygon.ClosestPointOnTriangle ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],(Vector3)vertices[node.v3],position);
				
				nninfo.clampedPosition = clP;
			}
			
			nninfo.constrainedNode = minConstNode;
			if (nninfo.constrainedNode != null) {
				MeshNode node = nninfo.constrainedNode as MeshNode;//minNode2 as MeshNode;
				
				Vector3 clP = Polygon.ClosestPointOnTriangle ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],(Vector3)vertices[node.v3],position);
				
				nninfo.constClampedPosition = clP;
			}
			
			return nninfo;
		}
		
		public void BuildFunnelCorridor (List<Node> path, int startIndex, int endIndex, List<Vector3> left, List<Vector3> right) {
			BuildFunnelCorridor (this,path,startIndex,endIndex,left,right);
		}
		
		public static void BuildFunnelCorridor (INavmesh graph, List<Node> path, int startIndex, int endIndex, List<Vector3> left, List<Vector3> right) {
			
			if (graph == null) {
				Debug.LogError ("Couldn't cast graph to the appropriate type (graph isn't a Navmesh type graph, it doesn't implement the INavmesh interface)");
				return;
			}
			
			Int3[] vertices = graph.vertices;
			
			int lastLeftIndex = -1;
			int lastRightIndex = -1;
			
			for (int i=startIndex;i<endIndex;i++) {
				//Find the connection between the nodes
				
				MeshNode n1 = path[i] as MeshNode;
				MeshNode n2 = path[i+1] as MeshNode;
				
				bool foundFirst = false;
				
				int first = -1;
				int second = -1;
				
				for (int x=0;x<3;x++) {
					//Vector3 vertice1 = vertices[n1.vertices[x]];
					int vertice1 = n1.GetVertexIndex (x);
					for (int y=0;y<3;y++) {
						//Vector3 vertice2 = vertices[n2.vertices[y]];
						int vertice2 = n2.GetVertexIndex (y);
						
						if (vertice1 == vertice2) {
							if (foundFirst) {
								second = vertice2;
								break;
							} else {
								first = vertice2;
								foundFirst = true;
							}
						}
					}
				}
				
				if (first == -1 || second == -1) {
					left.Add ((Vector3)n1.position);
					right.Add ((Vector3)n1.position);
					left.Add ((Vector3)n2.position);
					right.Add ((Vector3)n2.position);
					lastLeftIndex = first;
					lastRightIndex = second;
					
				} else {
				
					//Debug.DrawLine ((Vector3)vertices[first]+Vector3.up*0.1F,(Vector3)vertices[second]+Vector3.up*0.1F,Color.cyan);
					//Debug.Log (first+" "+second);
					if (first == lastLeftIndex) {
						left.Add ((Vector3)vertices[first]);
						right.Add ((Vector3)vertices[second]);
						lastLeftIndex = first;
						lastRightIndex = second;
						
					} else if (first == lastRightIndex) {
						left.Add ((Vector3)vertices[second]);
						right.Add ((Vector3)vertices[first]);
						lastLeftIndex = second;
						lastRightIndex = first;
						
					} else if (second == lastLeftIndex) {
						left.Add ((Vector3)vertices[second]);
						right.Add ((Vector3)vertices[first]);
						lastLeftIndex = second;
						lastRightIndex = first;
						
					} else {
						left.Add ((Vector3)vertices[first]);
						right.Add ((Vector3)vertices[second]);
						lastLeftIndex = first;
						lastRightIndex = second;
					}
				}
			}
		}
		
		public void AddPortal (Node n1, Node n2, List<Vector3> left, List<Vector3> right) {
		}
		
		/** Returns if \a origin is visible from \a end on the graph.
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end) {
			return Linecast (origin, end, GetNearest (origin, NNConstraint.None).node);
		}
		
		/** Returns if \a origin is visible from \a end on the graph.
		 * \param [in] origin Point to linecast from
		 * \param [in] end Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint You need to pass the node closest to the start point
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end, Node hint, out GraphHitInfo hit) {
			return Linecast (this as INavmesh, origin,end,hint,false,0, out hit);
		}
		
		/** Returns if \a end is visible from \a origin on the graph.
		 * \param [in] origin Point to linecast from
		 * \param [in] end Point to linecast to
		 * \param [in] hint You need to pass the node closest to the start point
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public bool Linecast (Vector3 origin, Vector3 end, Node hint) {
			GraphHitInfo hit;
			return Linecast (this as INavmesh, origin,end,hint,false,0, out hit);
		}
		
		/** Returns if \a _b is visible from \a _a on the graph.
		 * \param [in] graph The graph to perform the search on
		 * \param [in] tmp_origin Point to start from
		 * \param [in] tmp_end Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint You need to pass the node closest to the start point
		 * \param [in] thick An experimental feature can be enabled to use thick linecasts, does not always work as expected
		 * \param [in] thickness Thickness of the thick linecast
		 * This is not the same as Physics.Linecast, this function traverses the \b graph and looks for collisions instead of checking for collider intersection.
		 * \astarpro */
		public static bool Linecast (INavmesh graph, Vector3 tmp_origin, Vector3 tmp_end, Node hint, bool thick, float thickness, out GraphHitInfo hit) {
			
			Int3 end = (Int3)tmp_end;
			Int3 origin = (Int3)tmp_origin;
			//bool thick = true;
			//float thickness = 2F;
			
			if (thickness <= 0) {
				thick = false;
			}
			
			//System.DateTime startTime = System.DateTime.UtcNow;
			
			hit = new GraphHitInfo ();
			
			
			MeshNode node = hint as MeshNode;
			if (node == null) {
				Debug.LogError ("NavMeshGenerator:Linecast: The 'hint' must be a MeshNode");
				return true;
			}
			
			if (origin == end) {
				hit.node = node;
				return false;
			}
			
			Int3[] vertices = graph.vertices;
			
			origin = (Int3)node.ClosestPoint ((Vector3)origin, vertices);
			hit.origin = (Vector3)origin;
			
			Vector3 direction = (Vector3)(end-origin);
			Vector3 normalizedDir = direction.normalized;
			
			Int3 normalizedIntDir = (Int3)(normalizedDir*(1000.0f / Int3.Precision));
			
			/*if (thick) {
				Vector3 normal = Vector3.Cross (direction,Vector3.up).normalized;
				//Debug.DrawLine (tmp_origin+normal*thickness,tmp_end+normal*thickness,Color.black);
				//Debug.DrawLine (tmp_origin-normal*thickness,tmp_end-normal*thickness,Color.black);
			} else {
				//Debug.DrawLine (tmp_origin-Vector3.up,tmp_end-Vector3.up,Color.black);
			}*/
			
#if ASTARDEBUG
			Debug.DrawLine ((Vector3)origin,(Vector3)end,Color.black);
#endif
			
			//Current position for tracing
			Int3 currentPosition = origin;
			
			//Intersected nodes
			int count = 0;
			
			//Temporary variables
			int[] vs = new int[3];
			
			//Reason for termination
			string reason = "";
			
			MeshNode previousNode = null;
			
			if (!node.walkable) {
				reason += " Node is unwalkable";
				hit.point = (Vector3)origin;
				hit.tangentOrigin = (Vector3)origin;
				return true;
			}
				
			while (true) {
				
				count++;
				
				//Because of floating point errors, this can actually return true sometimes... I think
				if (currentPosition == end) {
					reason += " Current == End";
					break;
				}
					
				if (ContainsPoint (node,(Vector3)end,vertices)) {
					reason = "Contains point";
					break;
				}
				
				if (count > 200) {
					Debug.DrawRay ((Vector3)origin,normalizedDir*100,Color.cyan);
					Debug.DrawRay ((Vector3)currentPosition,normalizedDir*100,Color.cyan);
					Debug.LogError ("Possible infinite loop, intersected > 200 nodes");
					Debug.Break ();
					return true;
				}
				
				//Loop through all vertices
				for (int i=0;i<3;i++) {
					
					if (vertices[node[i]] == currentPosition) {
						vs[i] = 0;
						continue;
					}
					
					int tmp = Int3.Dot (normalizedIntDir, (vertices[node[i]]-currentPosition).NormalizeTo (1000));
					vs[i] = tmp;
					
#if ASTARDEBUG
					Debug.DrawRay (Vector3.Lerp ((Vector3)vertices[node[i]],(Vector3)node.position,0.5F),Vector3.up*2*tmp*0.001F,new Color (1,0.5F,0));
#endif
				}
				
				int max = 0;
				
				if (vs[1] > vs[max]) max = 1;
				if (vs[2] > vs[max]) max = 2;
				
				int v1 = node[max];
				int v2 = 0;
				
				if (count == 70 || count == 71) {
					string s2 = "Count "+count+" "+node.position+"\n";
					for (int i=0;i<vs.Length;i++) {
						s2 += vs[i].ToString ("0.00")+"\n";
					}
					Debug.Log (s2);
				}
				
#if ASTARDEBUG
				Debug.DrawLine ((Vector3)node.position+Vector3.up*count,(Vector3)vertices[v1]+Vector3.up*count,Color.yellow);
				
				Debug.DrawRay ((Vector3)vertices[v1]+Vector3.up*count,Vector3.up,Color.yellow);
#endif
				
				int preNodeV2 = 0;
				
				long triangleArea = Polygon.TriangleArea2 (currentPosition,currentPosition+normalizedIntDir,vertices[v1]);
				
				if (triangleArea == 0) {
					//Polygon.IsColinear (currentPosition,currentPosition+normalizedIntDir,vertices[v1])) {
					int max2 = -1;
					for (int i=0;i<3;i++) {
						if (max != i && (max2 == -1 || vs[i] > vs[max2])) {
							max2 = i;
						}
					}
					
					v2 = node[max2];
				} else if (triangleArea < 0) {
					//if (Polygon.Left (currentPosition,currentPosition+normalizedIntDir,vertices[v1])) {
						preNodeV2 = max - 1 < 0 ? 2 : max-1;
						v2 = node[preNodeV2];
				} else {
					preNodeV2 = max + 1 > 2 ? 0 : max+1;
					v2 = node[preNodeV2];
				}
				
				Vector3 intersectionPoint;
				
				bool intersectionSuccess = true;
				
				if (thick) {
					//Vector3 intersectionPoint = Polygon.IntersectionPoint (currentPosition,end,vertices[v1],vertices[v2]);
					float intersectionFactor = Polygon.IntersectionFactor ((Vector3)vertices[v1],(Vector3)vertices[v2],(Vector3)currentPosition,(Vector3)end);
					
					if (intersectionFactor < 0 || intersectionFactor > 1) {
						Debug.LogError ("This should not happen");
						hit.point = intersectionFactor < 0 ? (Vector3)vertices[v1] : (Vector3)vertices[v2];
						return true;
					}
					
					Vector3 dir2 = (Vector3)(vertices[v2]-vertices[v1]);
					
					intersectionPoint = (Vector3)vertices[v1]+dir2*intersectionFactor;
					
					float magn = dir2.magnitude;
					
					intersectionFactor *= magn;
						
					if (intersectionFactor-thickness < 0) {
						hit.point = (Vector3)vertices[v1];
						return true;
					} else if (intersectionFactor+thickness > magn) {
						hit.point = (Vector3)vertices[v2];
						return true;
					}
				} else {
					
					float intersectionFactor = Polygon.IntersectionFactor ((Vector3)vertices[v1],(Vector3)vertices[v2],(Vector3)currentPosition,(Vector3)end);
					
					
					//Lines were colinear
					if (intersectionFactor == -1) {
						intersectionSuccess = false;
					}
					
					intersectionFactor = Mathf.Clamp01 (intersectionFactor);
					intersectionPoint = (Vector3)vertices[v1] + (Vector3)(vertices[v2]-vertices[v1])*intersectionFactor;
					
					if (!intersectionSuccess) {
						
						if ((vertices[v1]-currentPosition).sqrMagnitude >= (end-currentPosition).sqrMagnitude) {
							intersectionPoint = (Vector3)end;
							
							reason = "Colinear - Aborting";
							
							break;
						} else {
							preNodeV2 = max != 0 && preNodeV2 != 0 ? 0 : (max != 1 && preNodeV2 != 1 ? 1 : 2);
							
							v2 = node[preNodeV2];
							intersectionPoint = (Vector3)vertices[v1];
							intersectionSuccess = true;
							reason = "Colinear - Continuing";
						}
					}
					
					float distanceFactor = Mathfx.NearestPointFactor ((Vector3)origin,(Vector3)end,intersectionPoint);
					if (distanceFactor > 1F) {
						intersectionSuccess = false;
					}
				}
#if ASTARDEBUG
				Debug.DrawLine ((Vector3)vertices[v1]+Vector3.up*count,(Vector3)vertices[v2]+Vector3.up*count,Color.magenta);
#endif
				
				MeshNode nextNode = null;
				
				bool breakOutFromLoop = false;
				
				for (int i=0;i<node.connections.Length;i++) {
					MeshNode other = node.connections[i] as MeshNode;
					
					//Make sure the node is a MeshNode and that it doesn't of some reason link to itself.
					if (other == null || other == node) {
						continue;
					}
					
					int matches = 0;
					
					
					for (int v=0;v<3;v++) {
						if (other[v] == v1 || other[v] == v2) {
							matches++;
						}
					}
					
					if (matches == 2) {
						
						//The node is the previous node, the endpoint must be in between the nodes (floating point error), abort
						if (other == previousNode) {
							reason += "Other == previous node\n";
							breakOutFromLoop = true;
							break;
						} else {
							nextNode = other;
						}
						break;
					}
				}
				
				if (breakOutFromLoop) {
					break;
				}
				
				if (nextNode == null || !intersectionSuccess || !nextNode.walkable) {
					if (nextNode == null) {
						reason += "No next node (wall)";
					}
					
#if ASTARDEBUG
					Debug.DrawLine ((Vector3)origin,(Vector3)intersectionPoint,Color.red);
#endif			
					hit.tangentOrigin = (Vector3)vertices[v1];
					hit.tangent = (Vector3)(vertices[v2]-vertices[v1]);
					hit.point = intersectionPoint;
					hit.node = node;
					return true;
				} else {
#if ASTARDEBUG
					Debug.DrawLine ((Vector3)node.position+Vector3.up*count,(Vector3)nextNode.position+Vector3.up*(count+1),Color.green);
#endif
					previousNode = node;
					node = nextNode;
					currentPosition = (Int3)intersectionPoint;
				}
			}
#if ASTARDEBUG		
			Debug.DrawLine ((Vector3)origin,(Vector3)end,Color.green);
#endif	
			hit.node = node;
			return false;
		}
		
		public void UpdateArea (GraphUpdateObject o) {
			
		}
		
		public static void UpdateArea (GraphUpdateObject o, NavGraph graph) {
			
			INavmesh navgraph = graph as INavmesh;
			
			if (navgraph == null) { Debug.LogError ("Update Area on NavMesh must be called with a graph implementing INavmesh"); return; }
			
			if (graph.nodes == null || graph.nodes.Length == 0) {
				Debug.LogError ("NavGraph hasn't been generated yet or does not contain any nodes");
				return;// new NNInfo ();
			}
			
			//System.DateTime startTime = System.DateTime.UtcNow;
				
			Bounds bounds = o.bounds;
			
			Rect r = Rect.MinMaxRect (bounds.min.x,bounds.min.z,bounds.max.x,bounds.max.z);
			
			Vector3 a = new Vector3 (r.xMin,0,r.yMin);//	-1 	-1
			Vector3 b = new Vector3 (r.xMin,0,r.yMax);//	-1	 1 
			Vector3 c = new Vector3 (r.xMax,0,r.yMin);//	 1 	-1
			Vector3 d = new Vector3 (r.xMax,0,r.yMax);//	 1 	 1
			
#if ASTARDEBUG
			Debug.DrawLine (a,b,Color.white);
			Debug.DrawLine (a,c,Color.white);
			Debug.DrawLine (c,d,Color.white);
			Debug.DrawLine (d,b,Color.white);
#endif
			
			for (int i=0;i<graph.nodes.Length;i++) {
				MeshNode node = graph.nodes[i] as MeshNode;
				
				bool inside = false;
				
				int allLeft = 0;
				int allRight = 0;
				int allTop = 0;
				int allBottom = 0;
				
				for (int v=0;v<3;v++) {
					
					Vector3 vert = (Vector3)navgraph.vertices[node[v]];
					Vector2 vert2D = new Vector2 (vert.x,vert.z);
					
					if (r.Contains (vert2D)) {
						//Debug.DrawRay (vert,Vector3.up,Color.yellow);
						inside = true;
						break;
					}
					
					if (vert.x < r.xMin) allLeft++;
					if (vert.x > r.xMax) allRight++;
					if (vert.z < r.yMin) allTop++;
					if (vert.z > r.yMax) allBottom++;
					
					//if (!bounds.Contains (node[v]) {
					//	inside = false;
					//	break;
					//}
				}
				if (!inside) {
					if (allLeft == 3 || allRight == 3 || allTop == 3 || allBottom == 3) {
						continue;
					}
				}
				
				for (int v=0;v<3;v++) {
					int v2 = v > 1 ? 0 : v+1;
					
					Vector3 vert1 = (Vector3)navgraph.vertices[node[v]];
					Vector3 vert2 = (Vector3)navgraph.vertices[node[v2]];
					
					if (Polygon.Intersects (a,b,vert1,vert2)) { inside = true; break; }
					if (Polygon.Intersects (a,c,vert1,vert2)) { inside = true; break; }
					if (Polygon.Intersects (c,d,vert1,vert2)) { inside = true; break; }
					if (Polygon.Intersects (d,b,vert1,vert2)) { inside = true; break; }
				}
				
				
				
				if (!inside && ContainsPoint (node,a,navgraph.vertices)) { inside = true; }//Debug.DrawRay (a+Vector3.right*0.01F*i,Vector3.up,Color.red); }
				if (!inside && ContainsPoint (node,b,navgraph.vertices)) { inside = true; } //Debug.DrawRay (b+Vector3.right*0.01F*i,Vector3.up,Color.red); }
				if (!inside && ContainsPoint (node,c,navgraph.vertices)) { inside = true; }//Debug.DrawRay (c+Vector3.right*0.01F*i,Vector3.up,Color.red); }
				if (!inside && ContainsPoint (node,d,navgraph.vertices)) { inside = true; }//Debug.DrawRay (d+Vector3.right*0.01F*i,Vector3.up,Color.red); }
				
				if (!inside) {
					continue;
				}
				
				o.WillUpdateNode(node);
				o.Apply (node);
				//Debug.DrawLine (vertices[node.v1],vertices[node.v2],Color.blue);
				//Debug.DrawLine (vertices[node.v2],vertices[node.v3],Color.blue);
				//Debug.DrawLine (vertices[node.v3],vertices[node.v1],Color.blue);
				//Debug.Break ();
			}
			
			//System.DateTime endTime = System.DateTime.UtcNow;
			//float theTime = (endTime-startTime).Ticks*0.0001F;
			//Debug.Log ("Intersecting bounds with navmesh took "+theTime.ToString ("0.000")+" ms");
		
		}
		
		/** Returns the closest point of the node */
		public static Vector3 ClosestPointOnNode (MeshNode node, Int3[] vertices, Vector3 pos) {
			return Polygon.ClosestPointOnTriangle ((Vector3)vertices[node[0]],(Vector3)vertices[node[1]],(Vector3)vertices[node[2]],pos);
		}
		
		/** Returns if the point is inside the node in XZ space */
		public bool ContainsPoint (MeshNode node, Vector3 pos) {
			if (	Polygon.IsClockwise ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2], pos)
			    && 	Polygon.IsClockwise ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3], pos)
			    && 	Polygon.IsClockwise ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1], pos)) {
				return true;
			}
			return false;
		}
		
		/** Returns if the point is inside the node in XZ space */
		public static bool ContainsPoint (MeshNode node, Vector3 pos, Int3[] vertices) {
			if (!Polygon.IsClockwiseMargin ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2], (Vector3)vertices[node.v3])) {
				Debug.LogError ("Noes!");
			}
			
			if ( 	Polygon.IsClockwiseMargin ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2], pos)
			    && 	Polygon.IsClockwiseMargin ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3], pos)
			    && 	Polygon.IsClockwiseMargin ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1], pos)) {
				return true;
			}
			return false;
		}
		
		/** Scans the graph using the path to an .obj mesh */
		public void Scan (string objMeshPath) {
			
			Mesh mesh = ObjImporter.ImportFile (objMeshPath);
			
			if (mesh == null) {
				Debug.LogError ("Couldn't read .obj file at '"+objMeshPath+"'");
				return;
			}
			
			sourceMesh = mesh;
			Scan ();
		}
		
		public override void Scan () {
			
			if (sourceMesh == null) {
				return;
			}
			
			GenerateMatrix ();
			
			//float startTime = 0;//Time.realtimeSinceStartup;
			
			Vector3[] vectorVertices = sourceMesh.vertices;
			
			triangles = sourceMesh.triangles;
			
			GenerateNodes (this,vectorVertices,triangles, out originalVertices, out _vertices);
		
		}
		
		/** Generates a navmesh. Based on the supplied vertices and triangles. Memory usage is about O(n) */
		public static void GenerateNodes (NavGraph graph, Vector3[] vectorVertices, int[] triangles, out Vector3[] originalVertices, out Int3[] vertices) {
			
			if (!(graph is INavmesh)) {
				Debug.LogError ("The specified graph does not implement interface 'INavmesh'");
				originalVertices = vectorVertices;
				vertices = new Int3[0];
				graph.nodes = graph.CreateNodes (0);
				return;
			}
			
			if (vectorVertices.Length == 0 || triangles.Length == 0) {
				originalVertices = vectorVertices;
				vertices = new Int3[0];
				graph.nodes = graph.CreateNodes (0);
				return;
			}
			
			vertices = new Int3[vectorVertices.Length];
			
			//Backup the original vertices
			//for (int i=0;i<vectorVertices.Length;i++) {
			//	vectorVertices[i] = graph.matrix.MultiplyPoint (vectorVertices[i]);
			//}
			
			int c = 0;
			/*int maxX = 0;
			int maxZ = 0;
			
			//Almost infinity
			int minX = 0xFFFFFFF;
			int minZ = 0xFFFFFFF;*/
			
			for (int i=0;i<vertices.Length;i++) {
				vertices[i] = (Int3)graph.matrix.MultiplyPoint (vectorVertices[i]);
				/*maxX = Mathfx.Max (vertices[i].x, maxX);
				maxZ = Mathfx.Max (vertices[i].z, maxZ);
				minX = Mathfx.Min (vertices[i].x, minX);
				minZ = Mathfx.Min (vertices[i].z, minZ);*/
			}
			
			//maxX = maxX-minX;
			//maxZ = maxZ-minZ;
			
			Dictionary<Int3,int> hashedVerts = new Dictionary<Int3,int> ();
			
			int[] newVertices = new int[vertices.Length];
				
			for (int i=0;i<vertices.Length-1;i++) {
				
				//int hash = Mathfx.ComputeVertexHash (vertices[i].x,vertices[i].y,vertices[i].z);
				
				//(vertices[i].x-minX)+(vertices[i].z-minX)*maxX+vertices[i].y*maxX*maxZ;
				//if (sortedVertices[i] != sortedVertices[i+1]) {
				if (!hashedVerts.ContainsKey (vertices[i])) {
					newVertices[c] = i;
					hashedVerts.Add (vertices[i], c);
					c++;
				}// else {
					//Debug.Log ("Hash Duplicate "+hash+" "+vertices[i].ToString ());
				//}
			}
			
			newVertices[c] = vertices.Length-1;
			
			//int hash2 = (newVertices[c].x-minX)+(newVertices[c].z-minX)*maxX+newVertices[c].y*maxX*maxZ;
			//int hash2 = Mathfx.ComputeVertexHash (newVertices[c].x,newVertices[c].y,newVertices[c].z);
			if (!hashedVerts.ContainsKey (vertices[newVertices[c]])) {
				
				hashedVerts.Add (vertices[newVertices[c]], c);
				c++;
			}
			
			for (int x=0;x<triangles.Length;x++) {
				Int3 vertex = vertices[triangles[x]];
				
				//int hash3 = (vertex.x-minX)+(vertex.z-minX)*maxX+vertex.y*maxX*maxZ;
				//int hash3 = Mathfx.ComputeVertexHash (vertex.x,vertex.y,vertex.z);
				//for (int y=0;y<newVertices.Length;y++) {
				triangles[x] = hashedVerts[vertex];
			}
			
			/*for (int i=0;i<triangles.Length;i += 3) {
				
				Vector3 offset = Vector3.forward*i*0.01F;
				Debug.DrawLine (newVertices[triangles[i]]+offset,newVertices[triangles[i+1]]+offset,Color.blue);
				Debug.DrawLine (newVertices[triangles[i+1]]+offset,newVertices[triangles[i+2]]+offset,Color.blue);
				Debug.DrawLine (newVertices[triangles[i+2]]+offset,newVertices[triangles[i]]+offset,Color.blue);
			}*/
			
			//Debug.Log ("NavMesh - Old vertice count "+vertices.Length+", new vertice count "+c+" "+maxX+" "+maxZ+" "+maxX*maxZ);
			
			Int3[] totalIntVertices = vertices;
			vertices = new Int3[c];
			originalVertices = new Vector3[c];
			for (int i=0;i<c;i++) {
				
				vertices[i] = totalIntVertices[newVertices[i]];//(Int3)graph.matrix.MultiplyPoint (vectorVertices[i]);
				originalVertices[i] = (Vector3)vertices[i];//vectorVertices[newVertices[i]];
			}
			
			Node[] nodes = graph.CreateNodes (triangles.Length/3);//new Node[triangles.Length/3];
			graph.nodes = nodes;
			for (int i=0;i<nodes.Length;i++) {
				
				MeshNode node = (MeshNode)nodes[i];//new MeshNode ();
				node.walkable = true;
				
				node.position = (vertices[triangles[i*3]] + vertices[triangles[i*3+1]] + vertices[triangles[i*3+2]])/3F;
				
				node.v1 = triangles[i*3];
				node.v2 = triangles[i*3+1];
				node.v3 = triangles[i*3+2];
				
				if (!Polygon.IsClockwise (vertices[node.v1],vertices[node.v2],vertices[node.v3])) {
					//Debug.DrawLine (vertices[node.v1],vertices[node.v2],Color.red);
					//Debug.DrawLine (vertices[node.v2],vertices[node.v3],Color.red);
					//Debug.DrawLine (vertices[node.v3],vertices[node.v1],Color.red);
					
					int tmp = node.v1;
					node.v1 = node.v3;
					node.v3 = tmp;
				}
				
				if (Polygon.IsColinear (vertices[node.v1],vertices[node.v2],vertices[node.v3])) {
					Debug.DrawLine ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],Color.red);
					Debug.DrawLine ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3],Color.red);
					Debug.DrawLine ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1],Color.red);
				}
				
				nodes[i] = node;
			}
			
			List<Node> connections = new List<Node> ();
			List<int> connectionCosts = new List<int> ();
			
			int identicalError = 0;
			
			for (int i=0;i<triangles.Length;i+=3) {
				
				connections.Clear ();
				connectionCosts.Clear ();
				
				//Int3 indices = new Int3(triangles[i],triangles[i+1],triangles[i+2]);
				
				Node node = nodes[i/3];
				
				for (int x=0;x<triangles.Length;x+=3) {
					
					if (x == i) {
						continue;
					}
					
					int count = 0;
					if (triangles[x] 	== 	triangles[i]) { count++; }
					if (triangles[x+1]	== 	triangles[i]) { count++; }
					if (triangles[x+2] 	== 	triangles[i]) { count++; }
					if (triangles[x] 	== 	triangles[i+1]) { count++; }
					if (triangles[x+1] 	== 	triangles[i+1]) { count++; }
					if (triangles[x+2] 	== 	triangles[i+1]) { count++; }
					if (triangles[x] 	== 	triangles[i+2]) { count++; }
					if (triangles[x+1] 	== 	triangles[i+2]) { count++; }
					if (triangles[x+2] 	== 	triangles[i+2]) { count++; }
					
					if (count >= 3) {
						identicalError++;
						Debug.DrawLine ((Vector3)vertices[triangles[x]],(Vector3)vertices[triangles[x+1]],Color.red);
						Debug.DrawLine ((Vector3)vertices[triangles[x]],(Vector3)vertices[triangles[x+2]],Color.red);
						Debug.DrawLine ((Vector3)vertices[triangles[x+2]],(Vector3)vertices[triangles[x+1]],Color.red);
						
					}
					
					if (count == 2) {
						Node other = nodes[x/3];
						connections.Add (other);
						connectionCosts.Add (Mathf.RoundToInt ((node.position-other.position).costMagnitude));
					}
				}
				
				node.connections = connections.ToArray ();
				node.connectionCosts = connectionCosts.ToArray ();
			}
			
			if (identicalError > 0) {
				Debug.LogError ("One or more triangles are identical to other triangles, this is not a good thing to have in a navmesh\nIncreasing the scale of the mesh might help\nNumber of triangles with error: "+identicalError+"\n");
			}
			RebuildBBTree (graph);
			
#if ASTARDEBUG
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = nodes[i] as MeshNode;
				
				float a1 = Polygon.TriangleArea2 ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],(Vector3)vertices[node.v3]);
				
				long a2 = Polygon.TriangleArea2 (vertices[node.v1],vertices[node.v2],vertices[node.v3]);
				if (a1 * a2 < 0) Debug.LogError (a1+ " " + a2);
				
				
				if (Polygon.IsClockwise (vertices[node.v1],vertices[node.v2],vertices[node.v3])) {
					Debug.DrawLine ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],Color.green);
					Debug.DrawLine ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3],Color.green);
					Debug.DrawLine ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1],Color.green);
				} else {
					Debug.DrawLine ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2],Color.red);
					Debug.DrawLine ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3],Color.red);
					Debug.DrawLine ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1],Color.red);
				}
			}
#endif
			//Debug.Log ("Graph Generation - NavMesh - Time to compute graph "+((Time.realtimeSinceStartup-startTime)*1000F).ToString ("0")+"ms");
		}
		
		/** Rebuilds the BBTree on a NavGraph.
		 * \astarpro
		 * \see NavMeshGraph.bbTree */
		public static void RebuildBBTree (NavGraph graph) {
			//Build Axis Aligned Bounding Box Tree
			
			INavmesh meshGraph = graph as INavmesh;
			
			BBTree bbTree = new BBTree (graph as INavmesh);
			
			for (int i=0;i<graph.nodes.Length;i++) {
				bbTree.Insert (graph.nodes[i] as MeshNode);
			}
			
			meshGraph.bbTree = bbTree;
		}
		
		public void PostProcess () {
			int rnd = Random.Range (0,nodes.Length);
			
			Node nodex = nodes[rnd];
			
			NavGraph gr = null;
			
			if (AstarPath.active.astarData.GetGraphIndex(this) == 0) {
				gr = AstarPath.active.graphs[1];
			} else {
				gr = AstarPath.active.graphs[0];
			}
			
			rnd = Random.Range (0,gr.nodes.Length);
			
			List<Node> connections = new List<Node> ();
			List<int> connectionCosts = new List<int> ();
			
			connections.AddRange (nodex.connections);
			connectionCosts.AddRange (nodex.connectionCosts);
			
			Node otherNode = gr.nodes[rnd];
			
			connections.Add (otherNode);
			connectionCosts.Add ((nodex.position-otherNode.position).costMagnitude);
			
			nodex.connections = connections.ToArray ();
			nodex.connectionCosts = connectionCosts.ToArray ();
		}
		
		public void Sort (Vector3[] a) {
			
			bool changed = true;
		
			while (changed) {
				changed = false;
				for (int i=0;i<a.Length-1;i++) {
					if (a[i].x > a[i+1].x || (a[i].x == a[i+1].x && (a[i].y > a[i+1].y || (a[i].y == a[i+1].y && a[i].z > a[i+1].z)))) {
						Vector3 tmp = a[i];
						a[i] = a[i+1];
						a[i+1] = tmp;
						changed = true;
					}
				}
			}
		}
		
		public override void OnDrawGizmos (bool drawNodes) {
			
			if (!drawNodes) {
				return;
			}
			
			Matrix4x4 preMatrix = matrix;
			
			GenerateMatrix ();
			
			if (nodes == null) {
				Scan ();
			}
			
			if (nodes == null) {
				return;
			}
			
			if (preMatrix != matrix) {
				//Debug.Log ("Relocating Nodes");
				RelocateNodes (preMatrix, matrix);
			}
			
			for (int i=0;i<nodes.Length;i++) {
				
				
				MeshNode node = (MeshNode)nodes[i];
				
				Gizmos.color = NodeColor (node,AstarPath.active.debugPathData);
				
				if (node.walkable ) {
					
					if (AstarPath.active.showSearchTree && AstarPath.active.debugPathData != null && node.GetNodeRun(AstarPath.active.debugPathData).parent != null) {
						Gizmos.DrawLine ((Vector3)node.position,(Vector3)node.GetNodeRun(AstarPath.active.debugPathData).parent.node.position);
					} else {
						for (int q=0;q<node.connections.Length;q++) {
							Gizmos.DrawLine ((Vector3)node.position,(Vector3)node.connections[q].position);
						}
					}
				
					Gizmos.color = AstarColor.MeshEdgeColor;
				} else {
					Gizmos.color = Color.red;
				}
				Gizmos.DrawLine ((Vector3)vertices[node.v1],(Vector3)vertices[node.v2]);
				Gizmos.DrawLine ((Vector3)vertices[node.v2],(Vector3)vertices[node.v3]);
				Gizmos.DrawLine ((Vector3)vertices[node.v3],(Vector3)vertices[node.v1]);
				
			}
			
		}
		
		public override byte[] SerializeExtraInfo () {
			return SerializeMeshNodes (this,nodes);
		}
		
		public override void DeserializeExtraInfo (byte[] bytes) {
			DeserializeMeshNodes (this,nodes,bytes);
		}
		
		public static void DeserializeMeshNodes (INavmesh graph, Node[] nodes, byte[] bytes) {
			
			System.IO.MemoryStream mem = new System.IO.MemoryStream(bytes);
			System.IO.BinaryReader stream = new System.IO.BinaryReader(mem);
			
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = nodes[i] as MeshNode;
				
				if (node == null) {
					Debug.LogError ("Serialization Error : Couldn't cast the node to the appropriate type - NavMeshGenerator");
					return;
				}
				
				node.v1 = stream.ReadInt32 ();
				node.v2 = stream.ReadInt32 ();
				node.v3 = stream.ReadInt32 ();
				
			}
			
			int numVertices = stream.ReadInt32 ();
			
			graph.vertices = new Int3[numVertices];
			
			for (int i=0;i<numVertices;i++) {
				int x = stream.ReadInt32 ();
				int y = stream.ReadInt32 ();
				int z = stream.ReadInt32 ();
				
				graph.vertices[i] = new Int3 (x,y,z);
			}
			
			RebuildBBTree (graph as NavGraph);
		}
		
		//These functions are for serialization, the static ones are there so other graphs using mesh nodes can serialize them more easily
		public static byte[] SerializeMeshNodes (INavmesh graph, Node[] nodes) {
			
			System.IO.MemoryStream mem = new System.IO.MemoryStream();
			System.IO.BinaryWriter stream = new System.IO.BinaryWriter(mem);
			
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = nodes[i] as MeshNode;
				
				if (node == null) {
					Debug.LogError ("Serialization Error : Couldn't cast the node to the appropriate type - NavMeshGenerator. Omitting node data.");
					return null;
				}
				
				stream.Write (node.v1);
				stream.Write (node.v2);
				stream.Write (node.v3);
			}
			
			Int3[] vertices = graph.vertices;
			
			if (vertices == null) {
				vertices = new Int3[0];
			}
			
			stream.Write (vertices.Length);
			
			for (int i=0;i<vertices.Length;i++) {
				stream.Write (vertices[i].x);
				stream.Write (vertices[i].y);
				stream.Write (vertices[i].z);
			}
			
			stream.Close ();
			return mem.ToArray();
		}
		
#region OldSerializer
		
		//These functions are for serialization, the static ones are there so other graphs using mesh nodes can serialize them more easily
		public static void SerializeMeshNodes (INavmesh graph, Node[] nodes, AstarSerializer serializer) {
			
			System.IO.BinaryWriter stream = serializer.writerStream;
			
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = nodes[i] as MeshNode;
				
				if (node == null) {
					Debug.LogError ("Serialization Error : Couldn't cast the node to the appropriate type - NavMeshGenerator");
					return;
				}
				
				stream.Write (node.v1);
				stream.Write (node.v2);
				stream.Write (node.v3);
			}
			
			Int3[] vertices = graph.vertices;
			
			if (vertices == null) {
				vertices = new Int3[0];
			}
			
			stream.Write (vertices.Length);
			
			for (int i=0;i<vertices.Length;i++) {
				stream.Write (vertices[i].x);
				stream.Write (vertices[i].y);
				stream.Write (vertices[i].z);
			}
		}
		
		public static void DeSerializeMeshNodes (INavmesh graph, Node[] nodes, AstarSerializer serializer) {
			
			System.IO.BinaryReader stream = serializer.readerStream;
			
			for (int i=0;i<nodes.Length;i++) {
				MeshNode node = nodes[i] as MeshNode;
				
				if (node == null) {
					Debug.LogError ("Serialization Error : Couldn't cast the node to the appropriate type - NavMeshGenerator");
					return;
				}
				
				node.v1 = stream.ReadInt32 ();
				node.v2 = stream.ReadInt32 ();
				node.v3 = stream.ReadInt32 ();
			}
			
			int numVertices = stream.ReadInt32 ();
			
			graph.vertices = new Int3[numVertices];
			
			for (int i=0;i<numVertices;i++) {
				int x = stream.ReadInt32 ();
				int y = stream.ReadInt32 ();
				int z = stream.ReadInt32 ();
				
				graph.vertices[i] = new Int3 (x,y,z);
			}
				
			RebuildBBTree (graph as NavGraph);
		}
		
		public void SerializeNodes (Node[] nodes, AstarSerializer serializer) {
			NavMeshGraph.SerializeMeshNodes (this as INavmesh, nodes, serializer);
		}
		
		public void DeSerializeNodes (Node[] nodes, AstarSerializer serializer) {
			NavMeshGraph.DeSerializeMeshNodes (this as INavmesh, nodes, serializer);
		}
		
		public void SerializeSettings (AstarSerializer serializer) {
			
			System.IO.BinaryWriter stream = serializer.writerStream;
			
			serializer.AddValue ("offset",offset);
			serializer.AddValue ("rotation",rotation);
			serializer.AddValue ("scale",scale);
			
			if (sourceMesh != null) {
				
				Vector3[] verts = sourceMesh.vertices;
				int[] tris = sourceMesh.triangles;
				
				stream.Write (verts.Length);
				stream.Write (tris.Length);
				
				for (int i=0;i<verts.Length;i++) {
					stream.Write (verts[i].x);
					stream.Write (verts[i].y);
					stream.Write (verts[i].z);
				}
				
				for (int i=0;i<tris.Length;i++) {
					stream.Write (tris[i]);
				}
			} else {
				stream.Write (0);
				stream.Write (0);
			}
			
			serializer.AddUnityReferenceValue ("sourceMesh",sourceMesh);
		}
		
		public void DeSerializeSettings (AstarSerializer serializer) {
			
			System.IO.BinaryReader stream = serializer.readerStream;
			
			offset = (Vector3)serializer.GetValue ("offset",typeof(Vector3));
			rotation = (Vector3)serializer.GetValue ("rotation",typeof(Vector3));
			scale = (float)serializer.GetValue ("scale",typeof(float));
			
			GenerateMatrix ();
			
			Vector3[] verts = new Vector3[stream.ReadInt32 ()];
			int[] tris = new int[stream.ReadInt32 ()];
			
			for (int i=0;i<verts.Length;i++) {
				verts[i] = new Vector3(stream.ReadSingle (),stream.ReadSingle (),stream.ReadSingle ());
			}
				
			for (int i=0;i<tris.Length;i++) {
				tris[i] = stream.ReadInt32 ();
			}
			
			sourceMesh = serializer.GetUnityReferenceValue ("sourceMesh",typeof(Mesh)) as Mesh;
			
			if (Application.isPlaying) {
				sourceMesh = new Mesh ();
				sourceMesh.name = "NavGraph Mesh";
				sourceMesh.vertices = verts;
				sourceMesh.triangles = tris;
			}
		}
		
#endregion
		
	}
}