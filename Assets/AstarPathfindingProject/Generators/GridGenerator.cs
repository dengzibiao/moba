//#define ASTAR_NoTagPenalty		//Enables or disables tag penalties. Can give small performance boost
//#define ASTARDEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Nodes;
using Pathfinding.Serialization.JsonFx;

namespace Pathfinding {
	[System.Serializable]
	[JsonOptIn]
	/** Generates a grid of nodes.
The GridGraph does exactly what the name implies, generates nodes in a grid pattern.\n
Grid graphs suit well to when you already have a grid based world.
Features:
 - You can update the graph during runtime (good for e.g Tower Defence or RTS games)
 - Throw any scene at it, with minimal configurations you can get a good graph from it.
 - Supports raycast and the funnel algorithm
 - Predictable pattern
 - Can apply penalty and walkability values from a supplied image
 - Perfect for terrain worlds since it can make areas unwalkable depending on the slope

\shadowimage{gridgraph_graph.png}
\shadowimage{gridgraph_inspector.png}

The <b>The Snap Size</b> button snaps the internal size of the graph to exactly contain the current number of nodes, i.e not contain 100.3 nodes but exactly 100 nodes.\n
This will make the "center" coordinate more accurate.\n

<b>The Graph During Runtime</b>
Any graph which implements the IUpdatableGraph interface can be updated during runtime.\n
For grid graphs this is a great feature since you can update only a small part of the grid without causing any lag like a complete rescan would.\n
If you for example just have instantiated a sphere obstacle in the scene and you want to update the grid where that sphere was instantiated, you can do this:\n
\code AstarPath.active.UpdateGraphs (ob.collider.bounds); \endcode
Where \a ob is the obstacle you just instantiated (a GameObject).\n
As you can see, the UpdateGraphs function takes a Bounds parameter and it will send an update call to all updateable graphs.\n
A grid graph will update that area and a small margin around it equal to \link Pathfinding.GraphCollision.diameter collision testing diameter/2 \endlink
\see graph-updates for more info about updating graphs during runtime
\ingroup graphs
\nosubgrouping
	 */
	public class GridGraph : NavGraph, ISerializableGraph, IUpdatableGraph, IFunnelGraph
	,IRaycastableGraph
	{
		
		public override Node[] CreateNodes (int number) {
			
			/*if (nodes != null && graphNodes != null && nodes.Length == number && graphNodes.Length == number) {
				Debug.Log ("Caching");
				return nodes;
			}*/
			
			GridNode[] tmp = new GridNode[number];
			for (int i=0;i<number;i++) {
				tmp[i] = new GridNode ();
				tmp[i].penalty = initialPenalty;
			}
			nodes = tmp;
			return tmp as Node[];
		}
		
		/** This function will be called when this graph is destroyed */
		public override void OnDestroy () {
			base.OnDestroy ();
			
			//Clean up a reference in a static variable which otherwise should point to this graph forever and stop the GC from collecting it
			RemoveGridGraphFromStatic ();
		}
		
		public void RemoveGridGraphFromStatic () {
			GridNode.RemoveGridGraph (this);
		}
		
		/** This is placed here so generators inheriting from this one can override it and set it to false.
		If it is true, it means that the nodes array's length will always be equal to width*depth
		It is used mainly in the editor to do auto-scanning calls, setting it to false for a non-uniform grid will reduce the number of scans */
		public virtual bool uniformWidhtDepthGrid {
			get {
				return true;
			}
		}
		
		/** \name Inspector - Settings
		 * \{ */
		
		public int width; /**< Width of the grid in nodes */
		public int depth; /**< Depth (height) of the grid in nodes */
		
		[JsonMember]
		public float aspectRatio = 1F; /**< Scaling of the graph along the X axis. This should be used if you want different scales on the X and Y axis of the grid */
		//public Vector3 offset;
		[JsonMember]
		public Vector3 rotation; /**< Rotation of the grid in degrees */
		
		public Bounds bounds;
		
		[JsonMember]
		public Vector3 center; /**< Center point of the grid */
		
		[JsonMember]
		public Vector2 unclampedSize; 	/**< Size of the grid. Might be negative or smaller than #nodeSize */
		
		[JsonMember]
		public float nodeSize = 1; /**< Size of one node in world units */
		
		/* Collision and stuff */
		
		/** Settings on how to check for walkability and height */
		[JsonMember]
		public GraphCollision collision;
		
		/** The max position difference between two nodes to enable a connection. Set to 0 to ignore the value.*/
		[JsonMember]
		public float maxClimb = 0.4F;
		
		/** The axis to use for #maxClimb. X = 0, Y = 1, Z = 2. */
		[JsonMember]
		public int maxClimbAxis = 1;
		
		/** The max slope in degrees for a node to be walkable. */
		[JsonMember]
		public float maxSlope = 90;
		
		/** Use heigh raycasting normal for max slope calculation.
		 * True if #maxSlope is less than 90 degrees.
		 */
		public bool useRaycastNormal { get { return System.Math.Abs (90-maxSlope) > float.Epsilon; }}
		
		/** Erosion of the graph.
		 * The graph can be eroded after calculation.
		 * This means a margin is put around unwalkable nodes or other unwalkable connections.
		 * It is really good if your graph contains ledges where the nodes without erosion are walkable too close to the edge.
		 * 
		 * Below is an image showing a graph with erode iterations 0, 1 and 2
		 * \shadowimage{erosion.png}
		 * 
		 * \note A high number of erode iterations can seriously slow down graph updates during runtime (GraphUpdateObject)
		 * and should be kept as low as possible.
		 * \see erosionUseTags
		 */
		[JsonMember]
		public int erodeIterations = 0;
		
		/** Use tags instead of walkability for erosion.
		 * Tags will be used for erosion instead of marking nodes as unwalkable. The nodes will be marked with tags in an increasing order starting with the tag #erosionFirstTag.
		 * Debug with the Tags mode to see the effect. With this enabled you can in effect set how close different AIs are allowed to get to walls using the Valid Tags field on the Seeker component. 
		 * \shadowimage{erosionTags.png}
		 * \shadowimage{erosionTags2.png}
		 * \see erosionFirstTag
		 */
		[JsonMember]
		public bool erosionUseTags = false;
		
		/** Tag to start from when using tags for erosion.
		 * \see #erosionUseTags
		 * \see #erodeIterations
		 */
		[JsonMember]
		public int erosionFirstTag = 1;
		
		/** Auto link the graph's edge nodes together with other GridGraphs in the scene on Scan. \see #autoLinkDistLimit */
		[JsonMember]
		public bool autoLinkGrids = false;
		
		/** Distance limit for grid graphs to be auto linked. \see #autoLinkGrids */
		[JsonMember]
		public float autoLinkDistLimit = 10F;
		
		/** Number of neighbours for each node. Either four directional or eight directional */
		[JsonMember]
		public NumNeighbours neighbours = NumNeighbours.Eight;
		
		/** If disabled, will not cut corners on obstacles. If \link #neighbours connections \endlink is Eight, obstacle corners might be cut by a connection, setting this to false disables that. \image html images/cutCorners.png */
		[JsonMember]
		public bool cutCorners = true;
		
		[JsonMember]
		/** Offset for the position when calculating penalty.
		  * \see penaltyPosition */
		public float penaltyPositionOffset = 0;
		[JsonMember]
		/** Use position (y-coordinate) to calculate penalty */
		public bool penaltyPosition = false;
		[JsonMember]
		/** Scale factor for penalty when calculating from position.
		 * \see penaltyPosition
		 */
		public float penaltyPositionFactor = 1F;
		
		[JsonMember]
		public bool penaltyAngle = false;
		[JsonMember]
		public float penaltyAngleFactor = 100F;
		
		/** Holds settings for using a texture as source for a grid graph.
		 * Texure data can be used for fine grained control over how the graph will look.
		 * It can be used for positioning, penalty and walkability control.\n
		 * Below is a screenshot of a grid graph with a penalty map applied.
		 * It has the effect of the AI taking the longer path along the green (low penalty) areas.\n
		 * \shadowimage{penaltymap.png}
		 * Color data is got as 0...255 values.
		 * \astarpro
		 * \warning Can only be used with Unity 3.4 and up */
		[JsonMember]
		public TextureData textureData = new TextureData ();
		
		/** \} */
		
		public Vector2 size;			/**< Size of the grid. Will always be positive and larger than #nodeSize. \see #GenerateMatrix */
		
		/* End collision and stuff */
		
		/** Index offset to get neighbour nodes. Added to a node's index to get a neighbour node index */
		[System.NonSerialized]
		public int[] neighbourOffsets;
		
		/** Costs to neighbour nodes */
		[System.NonSerialized]
		public int[] neighbourCosts;
		
		/** Offsets in the X direction for neighbour nodes. Only 1, 0 or -1 */
		[System.NonSerialized]
		public int[] neighbourXOffsets;
		
		/** Offsets in the Z direction for neighbour nodes. Only 1, 0 or -1 */
		[System.NonSerialized]
		public int[] neighbourZOffsets;
		
		/* Same as #nodes, but already in the correct type (GridNode instead of Node) */
		//public GridNode[] graphNodes;
		
		/* If a walkable node wasn't found, then it will search (max) in a square with the side of 2*getNearestForceLimit+1 for a close walkable node */
		//public int getNearestForceLimit = 40;
		/** In GetNearestForce, determines how far to search after a valid node has been found */
		public int getNearestForceOverlap = 2;
		
		
		/** Used for using a texture as a source for a grid graph.
		 * \astarpro
		 * \warning Can only be used with Unity 3.4 and up */
		public class TextureData : ISerializableObject {
			public bool enabled = false;
			
			public Texture2D source = null;
			public float[] factors = new float[3];
			public ChannelUse[] channels = new ChannelUse[3];
			
#if !UNITY_3_3
			Color32[] data = null;
#endif
			
			/** Reads texture data */
			public void Initialize () {
				if (enabled && source != null) {
#if !UNITY_3_3
					for (int i=0;i<channels.Length;i++) {
						if (channels[i] != ChannelUse.None) {
							try {
								data = source.GetPixels32 ();
							} catch (UnityException e) {
								Debug.LogWarning (e.ToString ());
								data = null;
							}
							break;
						}
					}
#else
					Debug.LogError ("Cannot use textures in Unity 3.3, please upgrade to a newer version of Unity");
#endif
				}
			}
			
			/** Applies the texture to the node */
			public void Apply (Node node, int x, int z) {
#if !UNITY_3_3
				if (enabled && data != null && x < source.width && z < source.height) {
					Color32 col = data[z*source.width+x];
					
					if (channels[0] != ChannelUse.None) {
						ApplyChannel (node,x,z,col.r,channels[0],factors[0]);
					}
					
					if (channels[1] != ChannelUse.None) {
						ApplyChannel (node,x,z,col.g,channels[1],factors[1]);
					}
					
					if (channels[2] != ChannelUse.None) {
						ApplyChannel (node,x,z,col.b,channels[2],factors[2]);
					}
				}
#endif
			}
			
			/** Applies a value to the node using the specified ChannelUse */
			void ApplyChannel (Node node, int x, int z, int value, ChannelUse channelUse, float factor) {
				switch (channelUse) {
				case ChannelUse.Penalty:
					node.penalty += (uint)Mathf.RoundToInt (value*factor);
					break;
				case ChannelUse.Position:
					node.position.y = Mathf.RoundToInt (value*factor*Int3.Precision);
					break;
				case ChannelUse.WalkablePenalty:
					if (value == 0) {
						node.walkable = false;
					} else {
						node.penalty += (uint)Mathf.RoundToInt ((value-1)*factor);
					}
					break;
				}
			}
			
			public enum ChannelUse {
				None,
				Penalty,
				Position,
				WalkablePenalty,
			}
			
			public void SerializeSettings (AstarSerializer serializer) {
				serializer.AddUnityReferenceValue ("source",source);
				serializer.AddValue ("enabled",enabled);
				for (int i=0;i<factors.Length;i++) {
					serializer.AddValue ("factor"+i,factors[i]);
				}
				for (int i=0;i<channels.Length;i++) {
					serializer.AddValue ("channel"+i,(int)channels[i]);
				}
			}
			
			public void DeSerializeSettings (AstarSerializer serializer) {
				enabled = (bool)serializer.GetValue ("enabled",typeof(bool));
				source = (Texture2D)serializer.GetUnityReferenceValue ("source",typeof(Texture2D));
				factors = new float[3];
				for (int i=0;i<factors.Length;i++) {
					factors[i] = (float)serializer.GetValue ("factor"+i,typeof(float),1F);
				}
				
				channels = new ChannelUse[3];
				for (int i=0;i<channels.Length;i++) {
					channels[i] = (ChannelUse)serializer.GetValue ("channel"+i,typeof(int));
				}
			}
		}
		
		public Matrix4x4 boundsMatrix;
		public Matrix4x4 boundsMatrix2;
		
		public int scans = 0;
		
		
		public GridGraph () {
			unclampedSize = new Vector2 (10,10);
			nodeSize = 1F;
			collision = new GraphCollision ();
		}
		
		/** Updates #size from #width, #depth and #nodeSize values. Also \link GenerateMatrix generates a new matrix \endlink.
		 * \note This does not rescan the graph, that must be done with Scan */
		public void UpdateSizeFromWidthDepth () {
			unclampedSize = new Vector2 (width,depth)*nodeSize;
			GenerateMatrix ();
		}
		
		/** Generates the matrix used for translating nodes from grid coordinates to world coordintes. */
		public void GenerateMatrix () {
			
			size = unclampedSize;
			
			size.x *= Mathf.Sign (size.x);
			//size.y *= Mathf.Sign (size.y);
			size.y *= Mathf.Sign (size.y);
			
			//Clamp the nodeSize at 0.1
			nodeSize = Mathf.Clamp (nodeSize,size.x/1024F,Mathf.Infinity);//nodeSize < 0.1F ? 0.1F : nodeSize;
			nodeSize = Mathf.Clamp (nodeSize,size.y/1024F,Mathf.Infinity);
			
			size.x = size.x < nodeSize ? nodeSize : size.x;
			//size.y = size.y < 0.1F ? 0.1F : size.y;
			size.y = size.y < nodeSize ? nodeSize : size.y;
			
			
			boundsMatrix.SetTRS (center,Quaternion.Euler (rotation),new Vector3 (aspectRatio,1,1));
			
			//bounds.center = boundsMatrix.MultiplyPoint (Vector3.up*height*0.5F);
			//bounds.size = new Vector3 (width*nodeSize,height,depth*nodeSize);
			
			width = Mathf.FloorToInt (size.x / nodeSize);
			depth = Mathf.FloorToInt (size.y / nodeSize);
			
			if (Mathf.Approximately (size.x / nodeSize,Mathf.CeilToInt (size.x / nodeSize))) {
				width = Mathf.CeilToInt (size.x / nodeSize);
			}
			
			if (Mathf.Approximately (size.y / nodeSize,Mathf.CeilToInt (size.y / nodeSize))) {
				depth = Mathf.CeilToInt (size.y / nodeSize);
			}
			
			//height = size.y;
			
			matrix.SetTRS (boundsMatrix.MultiplyPoint3x4 (-new Vector3 (size.x,0,size.y)*0.5F),Quaternion.Euler(rotation), new Vector3 (nodeSize*aspectRatio,1,nodeSize));
			
		}
		
		//public void GenerateBounds () {
			//bounds.center = offset+new Vector3 (0,height*0.5F,0);
			//bounds.size = new Vector3 (width*scale,height,depth*scale);
		//}
		
		/** \todo Set clamped position for Grid Graph */
		public override NNInfo GetNearest (Vector3 position, NNConstraint constraint, Node hint) {
			
			if (nodes == null || depth*width != nodes.Length) {
				//Debug.LogError ("NavGraph hasn't been generated yet");
				return new NNInfo ();
			}
			
			position = inverseMatrix.MultiplyPoint3x4 (position);
			
			float xf = position.x-0.5F;
			float zf = position.z-0.5f;
			int x = Mathf.Clamp (Mathf.RoundToInt (xf)  , 0, width-1);
			int z = Mathf.Clamp (Mathf.RoundToInt (zf)  , 0, depth-1);
			
			NNInfo nn = new NNInfo(nodes[z*width+x]);
			//Set clamped position
			//nn.clampedPosition = new Vector3(Mathf.Clamp (xf,x-0.5f,x+0.5f),position.y,Mathf.Clamp (zf,z-0.5f,z+0.5f));
			//nn.clampedPosition = matrix.MultiplyPoint3x4 (nn.clampedPosition);
			
			return nn;
		}
		
		/** \todo Set clamped position for Grid Graph */
		public override NNInfo GetNearestForce (Vector3 position, NNConstraint constraint) {
			
			if (nodes == null || depth*width != nodes.Length) {
				return new NNInfo ();
			}
			
			Vector3 globalPosition = position;
			
			position = inverseMatrix.MultiplyPoint3x4 (position);
			
			int x = Mathf.Clamp (Mathf.RoundToInt (position.x-0.5F)  , 0, width-1);
			int z = Mathf.Clamp (Mathf.RoundToInt (position.z-0.5F)  , 0, depth-1);
			
			Node node = nodes[x+z*width];
			
			Node minNode = null;
			float minDist = float.PositiveInfinity;
			int overlap = getNearestForceOverlap;
			
			if (constraint.Suitable (node)) {
				minNode = node;
				minDist = ((Vector3)minNode.position-globalPosition).sqrMagnitude;
			}
			
			if (minNode != null) {
				if (overlap == 0) return new NNInfo(minNode);
				else overlap--;
			}
			
			
			//int counter = 0;
			
			
			float maxDist = constraint.constrainDistance ? AstarPath.active.maxNearestNodeDistance : float.PositiveInfinity;
			float maxDistSqr = maxDist*maxDist;
			
			//for (int w = 1; w < getNearestForceLimit;w++) {
			for (int w = 1;;w++) {
				int nx = x;
				int nz = z+w;
				
				int nz2 = nz*width;
				
				//Check if the nodes are within distance limit
				if (nodeSize*w > maxDist) {
					return new NNInfo(minNode);
				}
				
				bool anyInside = false;
				
				for (nx = x-w;nx <= x+w;nx++) {
					if (nx < 0 || nz < 0 || nx >= width || nz >= depth) continue;
					anyInside = true;
					if (constraint.Suitable (nodes[nx+nz2])) {
						float dist = ((Vector3)nodes[nx+nz2].position-globalPosition).sqrMagnitude;
						//Debug.DrawRay (nodes[nx+nz2].position,Vector3.up*dist,Color.cyan);counter++;
						if (dist < minDist && dist < maxDistSqr) { minDist = dist; minNode = nodes[nx+nz2]; }
					}
				}
				
				nz = z-w;
				nz2 = nz*width;
				
				for (nx = x-w;nx <= x+w;nx++) {
					if (nx < 0 || nz < 0 || nx >= width || nz >= depth) continue;
					anyInside = true;
					if (constraint.Suitable (nodes[nx+nz2])) {
						float dist = ((Vector3)nodes[nx+nz2].position-globalPosition).sqrMagnitude;
						//Debug.DrawRay (nodes[nx+nz2].position,Vector3.up*dist,Color.cyan);counter++;
						if (dist < minDist && dist < maxDistSqr) { minDist = dist; minNode = nodes[nx+nz2]; }
					}
				}
				
				nx = x-w;
				nz = z-w+1;
				
				for (nz = z-w+1;nz <= z+w-1; nz++) {
					if (nx < 0 || nz < 0 || nx >= width || nz >= depth) continue;
					anyInside = true;
					if (constraint.Suitable (nodes[nx+nz*width])) {
						float dist = ((Vector3)nodes[nx+nz*width].position-globalPosition).sqrMagnitude;
						//Debug.DrawRay (nodes[nx+nz*width].position,Vector3.up*dist,Color.cyan);counter++;
						if (dist < minDist && dist < maxDistSqr) { minDist = dist; minNode = nodes[nx+nz*width]; }
					}
				}
				
				nx = x+w;
				
				for (nz = z-w+1;nz <= z+w-1; nz++) {
					if (nx < 0 || nz < 0 || nx >= width || nz >= depth) continue;
					anyInside = true;
					if (constraint.Suitable (nodes[nx+nz*width])) {
						float dist = ((Vector3)nodes[nx+nz*width].position-globalPosition).sqrMagnitude;
						//Debug.DrawRay (nodes[nx+nz*width].position,Vector3.up*dist,Color.cyan);counter++;
						if (dist < minDist && dist < maxDistSqr) { minDist = dist; minNode = nodes[nx+nz*width]; }
					}
				}
				
				if (minNode != null) {
					if (overlap == 0) return new NNInfo(minNode);
					else overlap--;
				}
				
				//No nodes were inside grid bounds
				if (!anyInside) {
					return new NNInfo(minNode);
				}
			}
			//return new NNInfo ();
		}
		
		/** Sets up #neighbourOffsets with the current settings. #neighbourOffsets, #neighbourCosts, #neighbourXOffsets and #neighbourZOffsets are set up.\n
		 * The cost for a non-diagonal movement between two adjacent nodes is RoundToInt (#nodeSize * Int3.Precision)\n
		 * The cost for a diagonal movement between two adjacent nodes is RoundToInt (#nodeSize * Sqrt (2) * Int3.Precision) */
		public virtual void SetUpOffsetsAndCosts () {
#if ASTARDEBUG
			Debug.Log ("+++ --- GridGraph Setting Up Offsets and Costs");
#endif
			neighbourOffsets = new int[8] {
				-width, 1 , width , -1,
				-width+1, width+1 , width-1, -width-1
			};
			
			int straightCost = Mathf.RoundToInt (nodeSize*Int3.Precision);
			int diagonalCost = Mathf.RoundToInt (nodeSize*Mathf.Sqrt (2F)*Int3.Precision);
			
			neighbourCosts = new int[8] {
				straightCost,straightCost,straightCost,straightCost,
				diagonalCost,diagonalCost,diagonalCost,diagonalCost
			};
			
			//Not for diagonal nodes, first 4 is for the four cross neighbours the last 4 is for the diagonals
			neighbourXOffsets = new int[8] {
				0, 1, 0, -1,
				1, 1, -1, -1
			};
			
			neighbourZOffsets = new int[8] {
				-1, 0, 1, 0,
				-1, 1, 1, -1
			};
		}
		
		public override void Scan () {
			
			AstarPath.OnPostScan += new OnScanDelegate (OnPostScan);
			
			scans++;
			
			if (nodeSize <= 0) {
				return;
			}
			
			GenerateMatrix ();
			
			if (width > 1024 || depth > 1024) {
				Debug.LogError ("One of the grid's sides is longer than 1024 nodes");
				return;
			}
			
			//GenerateBounds ();
			
			/*neighbourOffsets = new int[8] {
				-width-1,-width,-width+1,
				-1,1,
				width-1,width,width+1
			}*/
			
			SetUpOffsetsAndCosts ();
			
			//GridNode.RemoveGridGraph (this);
			
			int gridIndex = GridNode.SetGridGraph (this);
			
			//graphNodes = new GridNode[width*depth];
			
			nodes = CreateNodes (width*depth);
			GridNode[] graphNodes = nodes as GridNode[];
			
			if (collision == null) {
				collision = new GraphCollision ();
			}
			collision.Initialize (matrix,nodeSize);
			
			textureData.Initialize ();
			
			//Max slope in cosinus
			//float cosAngle = Mathf.Cos (maxSlope*Mathf.Deg2Rad);
			
			for (int z = 0; z < depth; z ++) {
				for (int x = 0; x < width; x++) {
					
					GridNode node = graphNodes[z*width+x];//new GridNode ();
					
					node.SetIndex (z*width+x);
					
					UpdateNodePositionCollision (node,x,z);
					
					textureData.Apply (node,x,z);
					
					/*node.position = matrix.MultiplyPoint3x4 (new Vector3 (x+0.5F,0,z+0.5F));
					
					RaycastHit hit;
					
					bool walkable = true;
					
					node.position = collision.CheckHeight (node.position, out hit, out walkable);
					
					node.penalty = 0;//Mathf.RoundToInt (Random.value*100);
					
					//Check if the node is on a slope steeper than permitted
					if (walkable && useRaycastNormal && collision.heightCheck) {
						
						if (hit.normal != Vector3.zero) {
							//Take the dot product to find out the cosinus of the angle it has (faster than Vector3.Angle)
							float angle = Vector3.Dot (hit.normal.normalized,Vector3.up);
							
							if (angle < cosAngle) {
								walkable = false;
							}
						}
					}
					
					//If the walkable flag has already been set to false, there is no point in checking for it again
					if (walkable) {
						node.walkable = collision.Check (node.position);
					} else {
						node.walkable = walkable;
					}*/
					
					node.SetGridIndex (gridIndex);
				}
			}
			
			
			for (int z = 0; z < depth; z ++) {
				for (int x = 0; x < width; x++) {
				
					GridNode node = graphNodes[z*width+x];
						
					CalculateConnections (nodes,x,z,node);
					
#if ASTARDEBUG
					if (z == 5 && x == 5) {
						int index = z*width+x;
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.red);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.green);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.blue);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.yellow);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.cyan);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.magenta);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.black);
						Debug.DrawRay ((Vector3)node.position,(Vector3)(nodes[index+neighbourOffsets[0]].position-node.position)*0.5F,Color.white);
					}
#endif
				}
			}
			
			
			ErodeWalkableArea ();
			//Assign the nodes to the main storage
			
			//startIndex = AstarPath.active.AssignNodes (graphNodes);
			//endIndex = startIndex+graphNodes.Length;
		}
		
		/** Updates position, walkability and penalty for the node.
		 * Assumes that collision.Initialize (...) has been called before this function */
		public void UpdateNodePositionCollision (Node node, int x, int z, bool resetPenalty = true) {
			
			node.position = (Int3)matrix.MultiplyPoint3x4 (new Vector3 (x+0.5F,0,z+0.5F));
			
			RaycastHit hit;
			
			bool walkable = true;
			
			node.position = (Int3)collision.CheckHeight ((Vector3)node.position, out hit, out walkable);
			
			if (resetPenalty)
				node.penalty = initialPenalty;//Mathf.RoundToInt (Random.value*100);
			
			if (penaltyPosition && resetPenalty) {
				node.penalty += (uint)Mathf.RoundToInt ((node.position.y-penaltyPositionOffset)*penaltyPositionFactor);
			}
			
			/*if (textureData && textureSourceData != null && x < textureSource.width && z < textureSource.height) {
				for (int i=0;i<3;i++) {
					//node.penalty = textureSourceData
				}
			}*/
			//Check if the node is on a slope steeper than permitted
			if (walkable && useRaycastNormal && collision.heightCheck) {
				
				if (hit.normal != Vector3.zero) {
					//Take the dot product to find out the cosinus of the angle it has (faster than Vector3.Angle)
					float angle = Vector3.Dot (hit.normal.normalized,collision.up);
					
					//Add penalty based on normal
					if (penaltyAngle && resetPenalty) {
						node.penalty += (uint)Mathf.RoundToInt ((1F-angle)*penaltyAngleFactor);
					}
					
					//Max slope in cosinus
					float cosAngle = Mathf.Cos (maxSlope*Mathf.Deg2Rad);
					
					//Check if the slope is flat enough to stand on
					if (angle < cosAngle) {
						walkable = false;
					}
				}
			}
			
			//If the walkable flag has already been set to false, there is no point in checking for it again
			if (walkable)
				node.walkable = collision.Check ((Vector3)node.position);
			else
				node.walkable = walkable;
			
			node.Bit15 = node.walkable;
			//Equal to (node as GridNode).WalkableErosion = node.walkable, but this is faster
			
		}
		
		/** Erodes the walkable area. \see #erodeIterations */
		public virtual void ErodeWalkableArea () {
			ErodeWalkableArea (0,0,width,depth);
		}
		
		/** Erodes the walkable area.
		 * 
		 * xmin, zmin (inclusive)\n
		 * xmax, zmax (exclusive)
		 * 
		 * \see #erodeIterations */
		public virtual void ErodeWalkableArea (int xmin, int zmin, int xmax, int zmax) {
			//Clamp values to grid
			xmin = xmin < 0 ? 0 : (xmin > width ? width : xmin);
			xmax = xmax < 0 ? 0 : (xmax > width ? width : xmax);
			zmin = zmin < 0 ? 0 : (zmin > depth ? depth : zmin);
			zmax = zmax < 0 ? 0 : (zmax > depth ? depth : zmax);
			
			if (!erosionUseTags) {
				for (int it=0;it < erodeIterations;it++) {
					for (int z = zmin; z < zmax; z ++) {
						for (int x = xmin; x < xmax; x++) {
							GridNode node = nodes[z*width+x] as GridNode;
							
							if (!node.walkable) {
								
								/*int index = node.GetIndex ();
								
								for (int i=0;i<4;i++) {
									if (node.GetConnection (i)) {
										nodes[index+neighbourOffsets[i]].walkable = false;
									}
								}*/
							} else {
								bool anyFalseConnections = false;
							
								for (int i=0;i<4;i++) {
									if (!node.GetConnection (i)) {
										anyFalseConnections = true;
										break;
									}
								}
								
								if (anyFalseConnections) {
									node.walkable = false;
								}
							}
						}
					}
					
					//Recalculate connections
					for (int z = zmin; z < zmax; z ++) {
						for (int x = xmin; x < xmax; x++) {
							GridNode node = nodes[z*width+x] as GridNode;
							CalculateConnections (nodes,x,z,node);
						}
					}
				}
			} else {
				if (erodeIterations+erosionFirstTag > 31) {
					Debug.LogError ("Too few tags available for "+erodeIterations+" erode iterations and starting with tag " + erosionFirstTag + " (erodeIterations+erosionFirstTag > 31)");
					return;
				}
				if (erosionFirstTag <= 0) {
					Debug.LogError ("First erosion tag must be greater or equal to 1");
					return;
				}
				
				for (int it=0;it < erodeIterations;it++) {
					for (int z = zmin; z < zmax; z ++) {
						for (int x = xmin; x < xmax; x++) {
							GridNode node = nodes[z*width+x] as GridNode;
							
							if (node.walkable && node.tags >= erosionFirstTag && node.tags < erosionFirstTag + it) {
								
								int index = node.GetIndex ();
								
								for (int i=0;i<4;i++) {
									if (node.GetConnection (i) ) {
										int tag = nodes[index+neighbourOffsets[i]].tags;
										if (tag > erosionFirstTag + it || tag < erosionFirstTag) {
											nodes[index+neighbourOffsets[i]].tags = erosionFirstTag+it;
										}
									}
								}
							} else if (node.walkable && it == 0) {
								bool anyFalseConnections = false;
								
								for (int i=0;i<4;i++) {
									if (!node.GetConnection (i)) {
										anyFalseConnections = true;
										break;
									}
								}
								
								if (anyFalseConnections) {
									node.tags = erosionFirstTag + it;
								}
							}
						}
					}
				}
			}
		}
		
		/** Returns true if a connection between the adjacent nodes \a n1 and \a n2 is valid. Also takes into account if the nodes are walkable */
		public virtual bool IsValidConnection (GridNode n1, GridNode n2) {
			if (!n1.walkable || !n2.walkable) {
				return false;
			}
			
			if (maxClimb != 0 && Mathf.Abs (n1.position[maxClimbAxis] - n2.position[maxClimbAxis]) > maxClimb*Int3.Precision) {
				return false;
			}
			
			return true;
		}
		
		/** To reduce memory allocations this array is reused.
		 * Used in the CalculateConnections function
		 * \see CalculateConnections */
		[System.NonSerialized]
		protected int[] corners;
		
		/** Calculates the grid connections for a single node. Convenience function, it's faster to use CalculateConnections(GridNode[],int,int,node) but that will only show when calculating for a large number of nodes
		  * \todo Test this function, should work ok, but you never know */
		public static void CalculateConnections (GridNode node) {
			GridGraph gg = AstarData.GetGraph (node) as GridGraph;
			
			if (gg != null) {
				int index = node.GetIndex ();
				int x = index % gg.width;
				int z = index / gg.width;
				gg.CalculateConnections (gg.nodes,x,z,node);
			}
		}
		
		/** Calculates the grid connections for a single node */
		public virtual void CalculateConnections (Node[] nodes, int x, int z, GridNode node) {
			
			//Reset all connections
			node.flags = node.flags & -256;
			
			//All connections are disabled if the node is not walkable
			if (!node.walkable) {
				return;
			}
			
			int index = node.GetIndex ();
			
			if (corners == null) {
				corners = new int[4];
			} else {
				for (int i = 0;i<4;i++) {
					corners[i] = 0;
				}
			}
			
			for (int i=0, j = 3; i<4; j = i, i++) {
				
				int nx = x + neighbourXOffsets[i];
				int nz = z + neighbourZOffsets[i];
				
				if (nx < 0 || nz < 0 || nx >= width || nz >= depth) {
					continue;
				}
				
				GridNode other = nodes[index+neighbourOffsets[i]] as GridNode;
				
				if (IsValidConnection (node, other)) {
					node.SetConnectionRaw (i,1);
					
					corners[i]++;
					corners[j]++;
				}
			}
			
			if (neighbours == NumNeighbours.Eight) {
				if (cutCorners) {
					for (int i=0; i<4; i++) {
						
						if (corners[i] >= 1) {
							int nx = x + neighbourXOffsets[i+4];
							int nz = z + neighbourZOffsets[i+4];
						
							if (nx < 0 || nz < 0 || nx >= width || nz >= depth) {
								continue;
							}
					
							GridNode other = nodes[index+neighbourOffsets[i+4]] as GridNode;
							
							if (IsValidConnection (node,other)) {
								node.SetConnectionRaw (i+4,1);
							}
						}
					}
				} else {
					for (int i=0; i<4; i++) {
						
						//We don't need to check if it is out of bounds because if both of the other neighbours are inside the bounds this one must be too
						if (corners[i] == 2) {
							GridNode other = nodes[index+neighbourOffsets[i+4]] as GridNode;
							
							if (IsValidConnection (node,other)) {
								node.SetConnectionRaw (i+4,1);
							}
						}
					}
				}
			}
			
		}
		
		/** Auto links grid graphs together. Called after all graphs have been scanned.
		 * \see autoLinkGrids
		 */
		public void OnPostScan (AstarPath script) {
			
			AstarPath.OnPostScan -= new OnScanDelegate (OnPostScan);
			
			if (!autoLinkGrids || autoLinkDistLimit <= 0) {
				return;
			}
			
			//Link to other grids
			
			int maxCost = Mathf.RoundToInt (autoLinkDistLimit * Int3.Precision);
			
			//Loop through all GridGraphs
			foreach (GridGraph gg in script.astarData.FindGraphsOfType (typeof (GridGraph))) {
				
				if (gg == this || gg.nodes == null || nodes == null) {
					continue;
				}
				
				//Int3 prevPos = gg.GetNearest (nodes[0]).position;
				
				//Z = 0
				for (int x = 0; x < width;x++) {
					
					Node node1 = nodes[x];
					Node node2 = gg.GetNearest ((Vector3)node1.position).node;
					
					Vector3 pos = inverseMatrix.MultiplyPoint3x4 ((Vector3)node2.position);
					
					if (pos.z > 0) {
						continue;
					}
					
					int cost = (node1.position-node2.position).costMagnitude;
					
					if (cost > maxCost) {
						continue;
					}
					
					node1.AddConnection (node2,cost);
					node2.AddConnection (node1,cost);
				}
				
				//X = 0
				for (int z = 0; z < depth;z++) {
					
					Node node1 = nodes[z*width];
					Node node2 = gg.GetNearest ((Vector3)node1.position).node;
					
					Vector3 pos = inverseMatrix.MultiplyPoint3x4 ((Vector3)node2.position);
					
					if (pos.x > 0) {
						continue;
					}
					
					int cost = (node1.position-node2.position).costMagnitude;
					
					if (cost > maxCost) {
						continue;
					}
					
					node1.AddConnection (node2,cost);
					node2.AddConnection (node1,cost);
				}
				
				//Z = max
				for (int x = 0; x < width;x++) {
					
					Node node1 = nodes[(depth-1)*width+x];
					Node node2 = gg.GetNearest ((Vector3)node1.position).node;
					
					Vector3 pos = inverseMatrix.MultiplyPoint3x4 ((Vector3)node2.position);
					
					if (pos.z < depth-1) {
						continue;
					}
					
					//Debug.DrawLine (node1.position,node2.position,Color.red);
					int cost = (node1.position-node2.position).costMagnitude;
					
					if (cost > maxCost) {
						continue;
					}
					
					node1.AddConnection (node2,cost);
					node2.AddConnection (node1,cost);
				}
				
				//X = max
				for (int z = 0; z < depth;z++) {
					
					Node node1 = nodes[z*width+width-1];
					Node node2 = gg.GetNearest ((Vector3)node1.position).node;
					
					Vector3 pos = inverseMatrix.MultiplyPoint3x4 ((Vector3)node2.position);
					
					if (pos.x < width-1) {
						continue;
					}
					
					int cost = (node1.position-node2.position).costMagnitude;
					
					if (cost > maxCost) {
						continue;
					}
					
					
					
					node1.AddConnection (node2,cost);
					node2.AddConnection (node1,cost);
				}
			}
	
		}
		
		public override void OnDrawGizmos (bool drawNodes) {
			
			//GenerateMatrix ();
			
			Gizmos.matrix = boundsMatrix;
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube (Vector3.zero, new Vector3 (size.x,0,size.y));
			
			Gizmos.matrix = Matrix4x4.identity;
			
			if (!drawNodes) {
				return;
			}
			
			if (nodes == null || depth*width != nodes.Length) {
				//Scan (AstarPath.active.GetGraphIndex (this));
				return;
			}
			
			base.OnDrawGizmos (drawNodes);
			for (int z = 0; z < depth; z ++) {
				for (int x = 0; x < width; x++) {
					GridNode node = nodes[z*width+x] as GridNode;
					
					if (!node.walkable) {// || node.activePath != AstarPath.active.debugPath)  
						continue;
					}
					//Gizmos.color = node.walkable ? Color.green : Color.red;
					//Gizmos.DrawSphere (node.position,0.2F);
					
					Gizmos.color = NodeColor (node,AstarPath.active.debugPathData);
					
					//if (true) {
					//	Gizmos.DrawCube (node.position,Vector3.one*nodeSize);
					//}
					//else 
					if (AstarPath.active.showSearchTree && AstarPath.active.debugPathData != null) {
						if (InSearchTree(node,AstarPath.active.debugPath)) {
							NodeRun nodeR = node.GetNodeRun (AstarPath.active.debugPathData);
							NodeRun parentR = nodeR.parent;
							if (parentR != null)
								Gizmos.DrawLine ((Vector3)node.position, (Vector3)parentR.node.position);
						}
					} else {
						
						for (int i=0;i<8;i++) {
							
							if (node.GetConnection (i)) {
								GridNode other = nodes[node.GetIndex ()+neighbourOffsets[i]] as GridNode;
								Gizmos.DrawLine ((Vector3)node.position, (Vector3)other.position);
							}
						}
					}
					
				}
			}
		}
		
		/** Calculates minimum and maximum points for bounds \a b when multiplied with the matrix */
		public void GetBoundsMinMax (Bounds b, Matrix4x4 matrix, out Vector3 min, out Vector3 max) {
			Vector3[] p = new Vector3[8];
			
			p[0] = matrix.MultiplyPoint3x4 (b.center + new Vector3 ( b.extents.x, b.extents.y, b.extents.z));
			p[1] = matrix.MultiplyPoint3x4 (b.center + new Vector3 ( b.extents.x, b.extents.y,-b.extents.z));
			p[2] = matrix.MultiplyPoint3x4 (b.center + new Vector3 ( b.extents.x,-b.extents.y, b.extents.z));
			p[3] = matrix.MultiplyPoint3x4 (b.center + new Vector3 ( b.extents.x,-b.extents.y,-b.extents.z));
			p[4] = matrix.MultiplyPoint3x4 (b.center + new Vector3 (-b.extents.x, b.extents.y, b.extents.z));
			p[5] = matrix.MultiplyPoint3x4 (b.center + new Vector3 (-b.extents.x, b.extents.y,-b.extents.z));
			p[6] = matrix.MultiplyPoint3x4 (b.center + new Vector3 (-b.extents.x,-b.extents.y, b.extents.z));
			p[7] = matrix.MultiplyPoint3x4 (b.center + new Vector3 (-b.extents.x,-b.extents.y,-b.extents.z));
			
			min = p[0];
			max = p[0];
			for (int i=1;i<8;i++) {
				min = Vector3.Min (min,p[i]);
				max = Vector3.Max (max,p[i]);
			}
		}
		
		/** All nodes inside the bounding box.
		  * \note Be nice to the garbage collector and release the list when you have used it (optional)
		  * \see Pathfinding.Util.ListPool
		  * 
		  * \see GetNodesInArea(GraphUpdateShape)
		  */
		public List<Node> GetNodesInArea (Bounds b) {
			return GetNodesInArea (b, null);
		}
		
		/** All nodes inside the shape.
		  * \note Be nice to the garbage collector and release the list when you have used it (optional)
		  * \see Pathfinding.Util.ListPool
		  * 
		  * \see GetNodesInArea(Bounds)
		  */
		public List<Node> GetNodesInArea (GraphUpdateShape shape) {
			return GetNodesInArea (shape.GetBounds (), shape);
		}
		
		/** All nodes inside the shape or if null, the bounding box.
		 * If a shape is supplied, it is assumed to be contained inside the bounding box.
		 * \see GraphUpdateShape.GetBounds
		 */
		private List<Node> GetNodesInArea (Bounds b, GraphUpdateShape shape) {
			
			if (nodes == null || width*depth != nodes.Length) {
				return null;
			}
			
			List<Node> inArea = Pathfinding.Util.ListPool<Node>.Claim ();
			
			Vector3 min, max;
			GetBoundsMinMax (b,inverseMatrix,out min, out max);
			
			int minX = Mathf.RoundToInt (min.x-0.5F);
			int maxX = Mathf.RoundToInt (max.x-0.5F);
			
			int minZ = Mathf.RoundToInt (min.z-0.5F);
			int maxZ = Mathf.RoundToInt (max.z-0.5F);
			
			IntRect originalRect = new IntRect(minX,minZ,maxX,maxZ);
			IntRect gridRect = new IntRect(0,0,width-1,depth-1);
			
			IntRect rect = IntRect.Intersection (originalRect, gridRect);
			
			for (int x = rect.xmin; x <= rect.xmax;x++) {
				for (int z = rect.ymin;z <= rect.ymax;z++) {
					
					int index = z*width+x;
					
					Node node = nodes[index];
					
					if (b.Contains ((Vector3)node.position) && (shape == null || shape.Contains ((Vector3)node.position))) {
						inArea.Add (node);
					}
				}
			}
			
			return inArea;
		}
		
		/** Internal function to update an area of the graph.
		  */
		public void UpdateArea (GraphUpdateObject o) {
			
			if (nodes == null || nodes.Length != width*depth) {
				Debug.LogWarning ("The Grid Graph is not scanned, cannot update area ");
				//Not scanned
				return;
			}
			
			//Copy the bounds
			Bounds b = o.bounds;
			
			Vector3 min, max;
			GetBoundsMinMax (b,inverseMatrix,out min, out max);
			
			int minX = Mathf.RoundToInt (min.x-0.5F);
			int maxX = Mathf.RoundToInt (max.x-0.5F);
			
			int minZ = Mathf.RoundToInt (min.z-0.5F);
			int maxZ = Mathf.RoundToInt (max.z-0.5F);
			//We now have coordinates in local space (i.e 1 unit = 1 node)
			
			IntRect originalRect = new IntRect(minX,minZ,maxX,maxZ);
			IntRect affectRect = originalRect;
			
			IntRect gridRect = new IntRect(0,0,width-1,depth-1);
			
			IntRect physicsRect = originalRect;
			
			int erosion = o.updateErosion ? erodeIterations : 0;
			
#if ASTARDEBUG
			Matrix4x4 debugMatrix = matrix;
			debugMatrix *= Matrix4x4.TRS (new Vector3(0.5f,0,0.5f),Quaternion.identity,Vector3.one);
			
			originalRect.DebugDraw (debugMatrix,Color.red);
#endif
			
			bool willChangeWalkability = o.updatePhysics || o.modifyWalkability;
			
			//Calculate the largest bounding box which might be affected
			
			if (o.updatePhysics && !o.modifyWalkability) {
				//Add the collision.diameter margin for physics calls
				if (collision.collisionCheck) {
					Vector3 margin = new Vector3 (collision.diameter,0,collision.diameter)*0.5F;
					
					min -= margin*1.02F;//0.02 safety margin, physics is rarely very accurate
					max += margin*1.02F;
					
					physicsRect = new IntRect(
					                            Mathf.RoundToInt (min.x-0.5F),
					                            Mathf.RoundToInt (min.z-0.5F),
					                            Mathf.RoundToInt (max.x-0.5F),
					                            Mathf.RoundToInt (max.z-0.5F)
					                            );
					
					affectRect = IntRect.Union (physicsRect, affectRect);
				}
			}
			
			if (willChangeWalkability || erosion > 0) {
				//Add affect radius for erosion. +1 for updating connectivity info at the border
				affectRect = affectRect.Expand (erosion +1);
			}
			
			IntRect clampedRect = IntRect.Intersection (affectRect,gridRect);
			
			//Mark nodes that might be changed
			for (int x = clampedRect.xmin; x <= clampedRect.xmax;x++) {
				for (int z = clampedRect.ymin;z <= clampedRect.ymax;z++) {
					o.WillUpdateNode (nodes[z*width+x]);
				}
			}
			
			//Update Physics
			if (o.updatePhysics && !o.modifyWalkability) {
				
				collision.Initialize (matrix,nodeSize);
				
				clampedRect = IntRect.Intersection (physicsRect,gridRect);
				
				for (int x = clampedRect.xmin; x <= clampedRect.xmax;x++) {
					for (int z = clampedRect.ymin;z <= clampedRect.ymax;z++) {
						
						int index = z*width+x;
						
						GridNode node = nodes[index] as GridNode;
						
						UpdateNodePositionCollision (node,x,z, o.resetPenaltyOnPhysics);
					}
				}
			}
			
			//Apply GUO
			
			clampedRect = IntRect.Intersection (originalRect, gridRect);
			for (int x = clampedRect.xmin; x <= clampedRect.xmax;x++) {
				for (int z = clampedRect.ymin;z <= clampedRect.ymax;z++) {
					int index = z*width+x;
					
					GridNode node = nodes[index] as GridNode;
					
					if (willChangeWalkability) {
						node.walkable = node.WalkableErosion;
						o.Apply (node);
						node.WalkableErosion = node.walkable;
					} else {
						o.Apply (node);
					}
				}
			}
		
#if ASTARDEBUG
			physicsRect.DebugDraw (debugMatrix,Color.blue);
			affectRect.DebugDraw (debugMatrix,Color.black);
#endif
			
			//Recalculate connections
			if (willChangeWalkability && erosion == 0) {
				
				clampedRect = IntRect.Intersection (affectRect, gridRect);
				for (int x = clampedRect.xmin; x <= clampedRect.xmax;x++) {
					for (int z = clampedRect.ymin;z <= clampedRect.ymax;z++) {
						int index = z*width+x;
						
						GridNode node = nodes[index] as GridNode;
						
						CalculateConnections (nodes,x,z,node);
					}
				}
			} else if (willChangeWalkability && erosion > 0) {
				
				
				clampedRect = IntRect.Union (originalRect, physicsRect);
				
				IntRect erosionRect1 = clampedRect.Expand (erosion);
				IntRect erosionRect2 = erosionRect1.Expand (erosion);
				
				erosionRect1 = IntRect.Intersection (erosionRect1,gridRect);
				erosionRect2 = IntRect.Intersection (erosionRect2,gridRect);
				
#if ASTARDEBUG
				erosionRect1.DebugDraw (debugMatrix,Color.magenta);
				erosionRect2.DebugDraw (debugMatrix,Color.cyan);
#endif
				
				/*
				all nodes inside clampedRect might have had their walkability changed
				all nodes inside erosionRect1 might get affected by erosion from clampedRect and erosionRect2
				all nodes inside erosionRect2 (but outside erosionRect1) will be reset to previous walkability
				after calculation since their erosion might not be correctly calculated (nodes outside erosionRect2 would maybe have effect)
				*/
				
				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax;x++) {
					for (int z = erosionRect2.ymin;z <= erosionRect2.ymax;z++) {
						
						int index = z*width+x;
						
						GridNode node = nodes[index] as GridNode;
						
						bool tmp = node.walkable;
						node.walkable = node.WalkableErosion;
						
						if (!erosionRect1.Contains (x,z)) {
							//Save the border's walkabilty data in bit 16 (will be reset later)
							node.Bit16 = tmp;
						}
					}
				}
				
				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax;x++) {
					for (int z = erosionRect2.ymin;z <= erosionRect2.ymax;z++) {
						int index = z*width+x;
						
						GridNode node = nodes[index] as GridNode;
						
#if ASTARDEBUG
						if (!node.walkable)
							Debug.DrawRay ((Vector3)node.position, Vector3.up*2,Color.red);
#endif
						
						CalculateConnections (nodes,x,z,node);
					}
				}
				
				//Erode the walkable area
				ErodeWalkableArea (erosionRect2.xmin,erosionRect2.ymin,erosionRect2.xmax+1,erosionRect2.ymax+1);
				
				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax;x++) {
					for (int z = erosionRect2.ymin;z <= erosionRect2.ymax;z++) {
						if (erosionRect1.Contains (x,z)) continue;
						
						int index = z*width+x;
						
						GridNode node = nodes[index] as GridNode;
						
						//Restore temporarily stored data
						node.walkable = node.Bit16;
					}
				}
				
				//Recalculate connections of all affected nodes
				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax;x++) {
					for (int z = erosionRect2.ymin;z <= erosionRect2.ymax;z++) {
						int index = z*width+x;
						
						GridNode node = nodes[index] as GridNode;
						CalculateConnections (nodes,x,z,node);
					}
				}
			}
		}
		
		//IFunnelGraph Implementation
		
		public void BuildFunnelCorridor (List<Node> path, int sIndex, int eIndex, List<Vector3> left, List<Vector3> right) {
			
			for (int n=sIndex;n<eIndex;n++) {
				
				GridNode n1 = path[n] as GridNode;
				GridNode n2 = path[n+1] as GridNode;
				
				AddPortal (n1,n2,left,right);
			}
		}
		
		public void AddPortal (Node n1, Node n2, List<Vector3> left, List<Vector3> right) {
			//Not implemented
		}
		
		public void AddPortal (GridNode n1, GridNode n2, List<Vector3> left, List<Vector3> right) {
			
			if (n1 == n2) {
				return;
			}
			
			int i1 = n1.GetIndex ();
			int i2 = n2.GetIndex ();
			int x1 = i1 % width;
			int x2 = i2 % width;
			int z1 = i1 / width;
			int z2 = i2 / width;
			
			Vector3 n1p = (Vector3)n1.position;
			Vector3 n2p = (Vector3)n2.position;
			
			int diffx = Mathf.Abs (x1-x2);
			int diffz = Mathf.Abs (z1-z2);
			
			if (diffx > 1 || diffz > 1) {
				//If the nodes are not adjacent to each other
				
				left.Add (n1p);
				right.Add (n1p);
				left.Add (n2p);
				right.Add (n2p);
			} else if ((diffx+diffz) <= 1){
				//If it is not a diagonal move
				
				Vector3 dir = n2p - n1p;
				dir = dir.normalized * nodeSize * 0.5F;
				Vector3 tangent = Vector3.Cross (dir, Vector3.up);
				tangent = tangent.normalized * nodeSize * 0.5F;
				
				left.Add (n1p + dir - tangent);
				right.Add (n1p + dir + tangent);
			} else {
				//Diagonal move
				
				Node t1 = nodes[z1 * width + x2];
				Node t2 = nodes[z2 * width + x1];
				Node target = null;
				
				if (t1.walkable) {
					target = t1;
				} else if (t2.walkable) {
					target = t2;
				}
				
				if (target == null) {
					Vector3 avg = (n1p + n2p) * 0.5F;
					
					left.Add (avg);
					right.Add (avg);
				} else {
					AddPortal (n1,(GridNode)target,left,right);
					AddPortal ((GridNode)target,n2,left,right);
				}
			}
		}
		
		//END IFunnelGraph Implementation
		
		/** Returns if \a _b is visible from \a _a on the graph.
		 * This is not the same as Physics.Linecast, this function traverses the graph and looks for collisions.
		 * \astarpro */
		public bool Linecast (Vector3 _a, Vector3 _b) {
			GraphHitInfo hit;
			return Linecast (_a,_b,null, out hit);
		}
		
		/** Returns if \a _b is visible from \a _a on the graph.
		 * \param [in] _a Point to linecast from
		 * \param [in] _b Point to linecast to
		 * \param [in] hint If you have some idea of what the start node might be (the one close to \a _a), pass it to hint since it can enable faster lookups
		 * This is not the same as Physics.Linecast, this function traverses the graph and looks for collisions.
		 * \astarpro */
		public bool Linecast (Vector3 _a, Vector3 _b, Node hint) {
			GraphHitInfo hit;
			return Linecast (_a,_b,hint, out hit);
		}
		
		/** Returns if \a _b is visible from \a _a on the graph.
		 * \param [in] _a Point to linecast from
		 * \param [in] _b Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint If you have some idea of what the start node might be (the one close to \a _a), pass it to hint since it can enable faster lookups
		 * This is not the same as Physics.Linecast, this function traverses the graph and looks for collisions.
		 * \astarpro */
		public bool Linecast (Vector3 _a, Vector3 _b, Node hint, out GraphHitInfo hit) {
			hit = new GraphHitInfo ();
			
			//
			//Node n2 = GetNearest (_b,NNConstraint.None);
			
			_a = inverseMatrix.MultiplyPoint3x4 (_a);
			_a.x -= 0.5F;
			_a.z -= 0.5F;
			
			_b = inverseMatrix.MultiplyPoint3x4 (_b);
			_b.x -= 0.5F;
			_b.z -= 0.5F;
			
			//Grid coordinates
			//Int3 a = new Int3 (Mathf.RoundToInt (_a.x),Mathf.RoundToInt (_a.y),Mathf.RoundToInt (_a.z));
			//Int3 b = new Int3 (Mathf.RoundToInt (_b.x),Mathf.RoundToInt (_b.y),Mathf.RoundToInt (_b.z));
			
			//Clamping is needed
			if (_a.x < -0.5F || _a.z < -0.5F || _a.x >= width-0.5F || _a.z >= depth-0.5F || 
			    _b.x < -0.5F || _b.z < -0.5F || _b.x >= width-0.5F || _b.z >= depth-0.5F) {
				
				//Bounding points of the grid
				Vector3 p1 = new Vector3 (-0.5F     ,0,      -0.5F);
				Vector3 p2 = new Vector3 (-0.5F     ,0,	depth-0.5F);
				Vector3 p3 = new Vector3 (width-0.5F,0,	depth-0.5F);
				Vector3 p4 = new Vector3 (width-0.5F,0,		 -0.5F);
				
				int intersectCount = 0;
				
				bool intersect = false;
				Vector3 intersection = Polygon.SegmentIntersectionPoint (p1,p2,_a,_b, out intersect);
				
				if (intersect) {
					//Debug.Log ("Intersection with p1 and p2 "+_a+" "+_b+" - Intersection: "+intersection);
					intersectCount++;
					if (!Polygon.Left (p1,p2,_a)) {
						_a = intersection;
					} else {
						_b = intersection;
					}
				}
				intersection = Polygon.SegmentIntersectionPoint (p2,p3,_a,_b, out intersect);
				
				if (intersect) {
					//Debug.Log ("Intersection with p2 and p3 "+_a+" "+_b+" - Intersection: "+intersection);
					intersectCount++;
					if (!Polygon.Left (p2,p3,_a)) {
						_a = intersection;
					} else {
						_b = intersection;
					}
				}
				intersection = Polygon.SegmentIntersectionPoint (p3,p4,_a,_b, out intersect);
				
				if (intersect) {
					//Debug.Log ("Intersection with p3 and p4 "+_a+" "+_b+" - Intersection: "+intersection);
					intersectCount++;
					if (!Polygon.Left (p3,p4,_a)) {
						_a = intersection;
					} else {
						_b = intersection;
					}
				}
				intersection = Polygon.SegmentIntersectionPoint (p4,p1,_a,_b, out intersect);
				
				if (intersect) {
					//Debug.Log ("Intersection with p4 and p1 "+_a+" "+_b+" - Intersection: "+intersection);
					intersectCount++;
					if (!Polygon.Left (p4,p1,_a)) {
						_a = intersection;
					} else {
						_b = intersection;
					}
				}
				
				if (intersectCount == 0) {
					//The line does not intersect with the grid
					return false;
				}
			}
			
			Vector3 dir = _b-_a;
			float magn = dir.magnitude;
			
			if (magn == 0) {
				//Zero length line
				return false;
			}
			
			float sampleLength = 0.2F;
			
			float newMagn = nodeSize * sampleLength;
			newMagn -= nodeSize * 0.02F;
			
			dir = (dir / magn) * newMagn;
			
			//Floor to int, number of samples on the line
			int its = (int)(magn / newMagn);
			
			//Debug.Log ("Num Its: "+its+" "+dir);
			
			Vector3 originOffset = _a + dir * nodeSize * 0.01F;
			
			for (int i=0;i <= its;i++) {
				
				Vector3 p = originOffset + dir * i;
				
				int x = Mathf.RoundToInt (p.x);
				int z = Mathf.RoundToInt (p.z);
				
				x = x < 0 ? 0 : (x >= width ? width-1 : x);
				z = z < 0 ? 0 : (z >= depth ? depth-1 : z);
				
				/*if (x < 0 || z < 0 || x >= width || z >= depth) {
					Debug.LogError ("Point Out Of Bounds "+"("+x+", "+z+") "+p+" in iteration "+i+" of "+its+" With a direction magn "+dir.magnitude+"\nA: "+_a+"\nB: "+_b);
					throw new System.IndexOutOfRangeException ("The point "+x+","+z+ " is outside the bounds of the grid");
					break;
				}*/
				
				Node node = nodes[z*width+x];
				
				if (!node.walkable) {
					if (i > 0) {
						hit.point = matrix.MultiplyPoint3x4 (originOffset + dir * (i-1)+new Vector3 (0.5F,0,0.5F));
					} else {
						hit.point = matrix.MultiplyPoint3x4 (_a+new Vector3 (0.5F,0,0.5F));
					}
					hit.success = false;
					hit.origin = matrix.MultiplyPoint3x4 (_a+new Vector3 (0.5F,0,0.5F));
					hit.node = node;
					return true;
				}
				
				if (i > its-1) {
					if (Mathf.Abs (p.x-_b.x) <= 0.50001F || Mathf.Abs (p.z - _b.z) <= 0.50001F) {
						return false;
					}
				}
					
			}
			
			return false;
		}
		
		/** Returns if \a _b is visible from \a _a on the graph.
		 * This function is different from the other Linecast functions since it 1) snaps the start and end positions directly to the graph
		 * and it uses Bresenham's line drawing algorithm as opposed to the others which use sampling at fixed intervals.
		 * If you only care about if one \b node can see another \b node, then this function is great, but if you need more precision than one node,
		 * use the normal linecast functions
		 * 
		 * \param [in] _a Point to linecast from
		 * \param [in] _b Point to linecast to
		 * \param [out] hit Contains info on what was hit, see GraphHitInfo
		 * \param [in] hint If you have some idea of what the start node might be (the one close to \a _a), pass it to hint since it can enable faster lookups
		 * 
		 * This is not the same as Physics.Linecast, this function traverses the graph and looks for collisions.
		 * \astarpro */
		public bool SnappedLinecast (Vector3 _a, Vector3 _b, Node hint, out GraphHitInfo hit) {
			hit = new GraphHitInfo ();
			
			//System.DateTime startTime = System.DateTime.UtcNow;
			
			Node n1 = GetNearest (_a,NNConstraint.None).node;
			Node n2 = GetNearest (_b,NNConstraint.None).node;
			
			_a = inverseMatrix.MultiplyPoint3x4 ((Vector3)n1.position);
			_a.x -= 0.5F;
			_a.z -= 0.5F;
			
			_b = inverseMatrix.MultiplyPoint3x4 ((Vector3)n2.position);
			_b.x -= 0.5F;
			_b.z -= 0.5F;
			
			Int3 a = new Int3 (Mathf.RoundToInt (_a.x),Mathf.RoundToInt (_a.y),Mathf.RoundToInt (_a.z));
			Int3 b = new Int3 (Mathf.RoundToInt (_b.x),Mathf.RoundToInt (_b.y),Mathf.RoundToInt (_b.z));
			
			hit.origin = (Vector3)a;
			
			//Debug.DrawLine (matrix.MultiplyPoint3x4 (a*100),matrix.MultiplyPoint3x4 (b*100),Color.yellow);
			
			if (!nodes[a.z*width+a.x].walkable) {
				hit.node = nodes[a.z*width+a.x];
				hit.point = matrix.MultiplyPoint3x4 (new Vector3 (a.x+0.5F,0,a.z+0.5F));
				hit.point.y = ((Vector3)hit.node.position).y;
				return true;
			}
			
			int dx = Mathf.Abs (a.x-b.x);
			int dz = Mathf.Abs (a.z-b.z);
			
			int sx = 0;
			int sz = 0;
			
			if (a.x < b.x) {
				sx = 1;
			} else {
				sx = -1;
			}
			
			if (a.z < b.z) {
				sz = 1;
			} else {
				sz = -1;
			}
			
			int err = dx-dz;
			
			while (true) {
				
				if (a.x == b.x && a.z == b.z) {
					
					//System.DateTime endTime2 = System.DateTime.UtcNow;
					//float theTime2 = (endTime2-startTime).Ticks*0.0001F;
			
					//Debug.Log ("Grid Linecast : Time "+theTime2.ToString ("0.00"));
			
					return false;
				}
				
				int e2 = err*2;
				
				int dir = 0;
				
				Int3 newPos = a;
				
				if (e2 > -dz) {
					err = err-dz;
					dir = sx;
					newPos.x += sx;
				}
				
				if (e2 < dx) {
					err = err+dx;
					dir += width*sz;
					newPos.z += sz;
				}
				
				if (dir == 0) {
					Debug.LogError ("Offset is zero, this should not happen");
					return false;
				}
				
				for (int i=0;i<neighbourOffsets.Length;i++) {
					if (neighbourOffsets[i] == dir) {
						if (CheckConnection (nodes[a.z*width+a.x] as GridNode,i)) {
							if (!nodes[newPos.z*width+newPos.x].walkable) {
								hit.node = nodes[a.z*width+a.x];
								hit.point = matrix.MultiplyPoint3x4 (new Vector3 (a.x+0.5F,0,a.z+0.5F));
								hit.point.y = ((Vector3)hit.node.position).y;
								return true;
							}
							
							//Debug.DrawLine (matrix.MultiplyPoint3x4 (a*100),matrix.MultiplyPoint3x4 (newPos*100));
							a = newPos;
							break;
						} else {
						
							hit.node = nodes[a.z*width+a.x];
							hit.point = matrix.MultiplyPoint3x4 (new Vector3 (a.x+0.5F,0,a.z+0.5F));
							hit.point.y = ((Vector3)hit.node.position).y;
							return true;
						}
					}
				}
			}
			
			//Debug.DrawLine (_a,_b,Color.green);
			//hit.success = true;
			
			//return false;
			
		}
		
		/** Returns if \a node is connected to it's neighbour in the specified direction.
		  * This will also return true if #neighbours = NumNeighbours.Four, the direction is diagonal and one can move through one of the adjacent nodes
		  * to the targeted node.
		  */
		public bool CheckConnection (GridNode node, int dir) {
			if (neighbours == NumNeighbours.Eight) {
				return node.GetConnection (dir);
			} else {
				int dir1 = (dir-4-1) & 0x3;
				int dir2 = (dir-4+1) & 0x3;
				
				if (!node.GetConnection (dir1) || !node.GetConnection (dir2)) {
					return false;
				} else {
					GridNode n1 = nodes[node.GetIndex ()+neighbourOffsets[dir1]] as GridNode;
					GridNode n2 = nodes[node.GetIndex ()+neighbourOffsets[dir2]] as GridNode;
					
					if (!n1.walkable || !n2.walkable) {
						return false;
					}
					
					if (!n2.GetConnection (dir1) || !n1.GetConnection (dir2)) {
						return false;
					}
				}
				return true;
			}
		}
		
		/*
		function line(x0, y0, x1, y1)
   dx := abs(x1-x0)
   dy := abs(y1-y0) 
   if x0 < x1 then sx := 1 else sx := -1
   if y0 < y1 then sy := 1 else sy := -1
   err := dx-dy
 
   loop
     setPixel(x0,y0)
     if x0 = x1 and y0 = y1 exit loop
     e2 := 2*err
     if e2 > -dy then 
       err := err - dy
       x0 := x0 + sx
     end if
     if e2 <  dx then 
       err := err + dx
       y0 := y0 + sy 
     end if
   end loop
   */
		
		public override void PostDeserialization () {
			
#if ASTARDEBUG
			Debug.Log ("Grid Graph - Post Deserialize");
#endif
			
			GenerateMatrix ();
			
			SetUpOffsetsAndCosts ();
			
			if (nodes == null || nodes.Length == 0) return;
			
			if (width*depth != nodes.Length) {
				Debug.LogWarning ("Node data did not match with bounds data. Probably a change to the bounds/width/depth data was made after scanning the graph just prior to saving it. Nodes will be discarded");
				nodes = new GridNode[0];
				return;
			}
			
			//graphNodes = new GridNode[nodes.Length];
			
			int gridIndex = GridNode.SetGridGraph (this);
			
			for (int z = 0; z < depth; z ++) {
				for (int x = 0; x < width; x++) {
					
					GridNode node = nodes[z*width+x] as GridNode;
					
					if (node == null) {
						Debug.LogError ("Deserialization Error : Couldn't cast the node to the appropriate type - GridGenerator. Check the CreateNodes function");
						return;
					}
					
					node.SetIndex  (z*width+x);
					node.SetGridIndex (gridIndex);
				}
			}
		}
		
		/** Serializes grid graph specific node stuff to the serializer.
		 * \deprecated This function is obsolete. New serialization functions exist
		 * \astarpro */
		[System.Obsolete]
		public void SerializeNodes (Node[] nodes, AstarSerializer serializer) {

			/** \todo Add check for mask == Save Node Positions. Can't use normal mask since it is changed by SerializeSettings */
			
			GenerateMatrix ();
			
			int maxValue = 0;
			int minValue = 0;
			
			for (int i=0;i<nodes.Length;i++) {
				int val = nodes[i].position.y;
				maxValue = val > maxValue ? val : maxValue;
				minValue = val < minValue ? val : minValue;
			}
			
			int maxRange = maxValue > -minValue ? maxValue : -minValue;
			
			if (maxRange <= System.Int16.MaxValue) {
				//Int16
				serializer.writerStream.Write ((byte)0);
				for (int i=0;i<nodes.Length;i++) {
					serializer.writerStream.Write ((System.Int16)(inverseMatrix.MultiplyPoint3x4 ((Vector3)nodes[i].position).y));
				}
			} else {
				//Int32
				serializer.writerStream.Write ((byte)1);
				for (int i=0;i<nodes.Length;i++) {
					serializer.writerStream.Write (inverseMatrix.MultiplyPoint3x4 ((Vector3)nodes[i].position).y);
				}
			}
			/*for (int i=0;i<nodes.Length;i++) {
				GridNode node = nodes[i] as GridNode;
				
				if (node == null) {
					Debug.LogError ("Serialization Error : Couldn't cast the node to the appropriate type - GridGenerator");
					return;
				}
				
				serializer.writerStream.Write (node.index);
			}*/

		}
		
		/** Deserializes grid graph specific node stuff from the serializer.
		 * \deprecated This function is obsolete. New serialization functions exist
		 * \astarpro */
		[System.Obsolete]
		public void DeSerializeNodes (Node[] nodes, AstarSerializer serializer) {
			

			/*for (int i=0;i<nodes.Length;i++) {
				GridNode node = nodes[i] as GridNode;
				
				if (node == null) {
					Debug.LogError ("DeSerialization Error : Couldn't cast the node to the appropriate type - GridGenerator");
					return;
				}*/
			
			GenerateMatrix ();
			
			SetUpOffsetsAndCosts ();
			
			if (nodes == null || nodes.Length == 0) {
				return;
			}
			
			nodes = new GridNode[nodes.Length];
			
			int gridIndex = GridNode.SetGridGraph (this);
			
			int numberSize = (int)serializer.readerStream.ReadByte ();
			
			for (int z = 0; z < depth; z ++) {
				for (int x = 0; x < width; x++) {
					
					GridNode node = nodes[z*width+x] as GridNode;
				
					nodes[z*width+x] = node;
					
					if (node == null) {
						Debug.LogError ("DeSerialization Error : Couldn't cast the node to the appropriate type - GridGenerator");
						return;
					}
					
					node.SetIndex  (z*width+x);
					node.SetGridIndex (gridIndex);
					
					
					float yPos = 0;
					
					//if (serializer.mask == AstarSerializer.SMask.SaveNodePositions) {
						//Needs to multiply with precision factor because the position will be scaled by Int3.Precision later (Vector3 --> Int3 conversion)
					if (numberSize == 0) {
						yPos = serializer.readerStream.ReadInt16 ();
					} else {
						yPos = serializer.readerStream.ReadInt32 ();
					}
					//}
					
					node.position = (Int3)matrix.MultiplyPoint3x4 (new Vector3 (x+0.5F,yPos,z+0.5F));
				}
			}

			
		}
		
		/** Serialize Settings.
		 * \deprecated This function is obsolete. New serialization functions exist */
		public void SerializeSettings (AstarSerializer serializer) {
			serializer.mask -= AstarSerializer.SMask.SaveNodePositions;
			
			//serializer.AddValue ("Width",width);
			//serializer.AddValue ("Depth",depth);
			//serializer.AddValue ("Height",height);
			
			serializer.AddValue ("unclampedSize",unclampedSize);
			
			serializer.AddValue ("cutCorners",cutCorners);
			serializer.AddValue ("neighbours",(int)neighbours);
			
			serializer.AddValue ("center",center);
			serializer.AddValue ("rotation",rotation);
			serializer.AddValue ("nodeSize",nodeSize);
			serializer.AddValue ("collision",collision == null ? new GraphCollision () : collision);
			
			serializer.AddValue ("maxClimb",maxClimb);
			serializer.AddValue ("maxClimbAxis",maxClimbAxis);
			serializer.AddValue ("maxSlope",maxSlope);
			
			serializer.AddValue ("erodeIterations",erodeIterations);
			
			serializer.AddValue ("penaltyAngle",penaltyAngle);
			serializer.AddValue ("penaltyAngleFactor",penaltyAngleFactor);
			serializer.AddValue ("penaltyPosition",penaltyPosition);
			serializer.AddValue ("penaltyPositionOffset",penaltyPositionOffset);
			serializer.AddValue ("penaltyPositionFactor",penaltyPositionFactor);
			
			serializer.AddValue ("aspectRatio",aspectRatio);
			
			serializer.AddValue ("textureData",textureData);
		}
		
		public void DeSerializeSettings (AstarSerializer serializer) {
			
			//width = (int)serializer.GetValue ("Width",typeof(int));
			//depth = (int)serializer.GetValue ("Depth",typeof(int));
			//height = (float)serializer.GetValue ("Height",typeof(float));
			
			unclampedSize = (Vector2)serializer.GetValue ("unclampedSize",typeof(Vector2));
			
			cutCorners = (bool)serializer.GetValue ("cutCorners",typeof(bool));
			neighbours = (NumNeighbours)serializer.GetValue ("neighbours",typeof(int));
			
			rotation = (Vector3)serializer.GetValue ("rotation",typeof(Vector3));
			
			nodeSize = (float)serializer.GetValue ("nodeSize",typeof(float));
			
			collision = (GraphCollision)serializer.GetValue ("collision",typeof(GraphCollision));
			
			center = (Vector3)serializer.GetValue ("center",typeof(Vector3));
			
			maxClimb = (float)serializer.GetValue ("maxClimb",typeof(float));
			maxClimbAxis = (int)serializer.GetValue ("maxClimbAxis",typeof(int),1);
			maxSlope = (float)serializer.GetValue ("maxSlope",typeof(float),90.0F);
			
			erodeIterations = (int)serializer.GetValue ("erodeIterations",typeof(int));
			
			penaltyAngle = 			(bool)serializer.GetValue ("penaltyAngle",typeof(bool));
			penaltyAngleFactor = 	(float)serializer.GetValue ("penaltyAngleFactor",typeof(float));
			penaltyPosition = 		(bool)serializer.GetValue ("penaltyPosition",typeof(bool));
			penaltyPositionOffset = (float)serializer.GetValue ("penaltyPositionOffset",typeof(float));
			penaltyPositionFactor = (float)serializer.GetValue ("penaltyPositionFactor",typeof(float));
			
			aspectRatio = (float)serializer.GetValue ("aspectRatio",typeof(float),1F);
			
			textureData			=	serializer.GetValue ("textureData",typeof(TextureData)) as TextureData;
			if (textureData == null) textureData = new TextureData ();
			
#if UNITY_EDITOR
			Matrix4x4 oldMatrix = matrix;
#endif
			
			GenerateMatrix ();
			SetUpOffsetsAndCosts ();
			
#if UNITY_EDITOR
			if (serializer.onlySaveSettings) {
				if (oldMatrix != matrix && nodes != null) {
					AstarPath.active.AutoScan ();
				}
			}
#endif
			
			//Debug.Log ((string)serializer.GetValue ("SomeString",typeof(string)));
			//Debug.Log ((Bounds)serializer.GetValue ("SomeBounds",typeof(Bounds)));
		}
	}
	
	public enum NumNeighbours {
		Four,
		Eight
	}
}