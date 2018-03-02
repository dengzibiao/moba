//#define ASTARDEBUG   //"BBTree Debug" If enables, some queries to the tree will show debug lines. Turn off multithreading when using this since DrawLine calls cannot be called from a different thread

using System;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

namespace Pathfinding
{
	/** Axis Aligned Bounding Box Tree.
	 * Holds a bounding box tree of triangles.\n
	 * \b Performance: Insertion - Practically O(1) - About 0.003 ms
	 * \astarpro
	 */
	public class BBTree
	{
		/** Holds an Axis Aligned Bounding Box Tree used for faster node lookups.
		 * \astarpro */
		public BBTreeBox root;
		public INavmesh graph;
		
		public BBTree (INavmesh graph) {
			this.graph = graph;
		}
		
		public NNInfo Query (Vector3 p, NNConstraint constraint) {
			
			BBTreeBox c = root;
			
			if (c == null) {
				return new NNInfo();
			}
			
			NNInfo nnInfo = new NNInfo ();
			
			SearchBox (c,p, constraint, ref nnInfo);
			
			nnInfo.UpdateInfo ();
			
			return nnInfo;
		}
		
		/** Queries the tree for the best node, searching within a circle around \a p with the specified radius */
		public NNInfo QueryCircle (Vector3 p, float radius, NNConstraint constraint) {
			BBTreeBox c = root;
			
			if (c == null) {
				return new NNInfo();
			}
			
#if ASTARDEBUG
			Vector3 prev = new Vector3 (1,0,0)*radius+p;
			for (double i=0;i< Math.PI*2; i += Math.PI/50.0) {
				Vector3 cpos = new Vector3 ((float)Math.Cos (i),0,(float)Math.Sin (i))*radius+p;
				Debug.DrawLine (prev,cpos,Color.yellow);
				prev = cpos;
			}
#endif
			
			NNInfo nnInfo = new NNInfo (null);
			
			SearchBoxCircle (c,p, radius, constraint, ref nnInfo);
			
			nnInfo.UpdateInfo ();
			
			return nnInfo;
		}
		
		public void SearchBoxCircle (BBTreeBox box, Vector3 p, float radius, NNConstraint constraint, ref NNInfo nnInfo) {//, int intendentLevel = 0) {
			
			if (box.node != null) {
				//Leaf node
				if (NodeIntersectsCircle (box.node,p,radius)) {
					//Update the NNInfo
					
#if ASTARDEBUG
					Debug.DrawLine ((Vector3)graph.vertices[box.node[0]],(Vector3)graph.vertices[box.node[1]],Color.red);
					Debug.DrawLine ((Vector3)graph.vertices[box.node[1]],(Vector3)graph.vertices[box.node[2]],Color.red);
					Debug.DrawLine ((Vector3)graph.vertices[box.node[2]],(Vector3)graph.vertices[box.node[0]],Color.red);
#endif
					
					Vector3 closest = NavMeshGraph.ClosestPointOnNode (box.node,graph.vertices,p);
					float dist = (closest-p).sqrMagnitude;
					
					if (nnInfo.node == null) {
						nnInfo.node = box.node;
						nnInfo.clampedPosition = closest;
					} else if (dist < (nnInfo.clampedPosition - p).sqrMagnitude) {
						nnInfo.node = box.node;
						nnInfo.clampedPosition = closest;
					}
					if (constraint.Suitable (box.node)) {
						if (nnInfo.constrainedNode == null) {
							nnInfo.constrainedNode = box.node;
							nnInfo.constClampedPosition = closest;
						} else if (dist < (nnInfo.constClampedPosition - p).sqrMagnitude) {
							nnInfo.constrainedNode = box.node;
							nnInfo.constClampedPosition = closest;
						}
					}
				} else {
#if ASTARDEBUG
					Debug.DrawLine ((Vector3)graph.vertices[box.node[0]],(Vector3)graph.vertices[box.node[1]],Color.blue);
					Debug.DrawLine ((Vector3)graph.vertices[box.node[1]],(Vector3)graph.vertices[box.node[2]],Color.blue);
					Debug.DrawLine ((Vector3)graph.vertices[box.node[2]],(Vector3)graph.vertices[box.node[0]],Color.blue);
#endif
				}
				return;
			}
			
#if ASTARDEBUG
			Debug.DrawLine (new Vector3 (box.rect.xMin,0,box.rect.yMin),new Vector3 (box.rect.xMax,0,box.rect.yMin),Color.white);
			Debug.DrawLine (new Vector3 (box.rect.xMin,0,box.rect.yMax),new Vector3 (box.rect.xMax,0,box.rect.yMax),Color.white);
			Debug.DrawLine (new Vector3 (box.rect.xMin,0,box.rect.yMin),new Vector3 (box.rect.xMin,0,box.rect.yMax),Color.white);
			Debug.DrawLine (new Vector3 (box.rect.xMax,0,box.rect.yMin),new Vector3 (box.rect.xMax,0,box.rect.yMax),Color.white);
#endif
			
			//Search children
			if (RectIntersectsCircle (box.c1.rect,p,radius)) {
				SearchBoxCircle (box.c1,p, radius, constraint, ref nnInfo);
			}
			
			if (RectIntersectsCircle (box.c2.rect,p,radius)) {
				SearchBoxCircle (box.c2,p, radius, constraint, ref nnInfo);
			}
		}
		
		public void SearchBox (BBTreeBox box, Vector3 p, NNConstraint constraint, ref NNInfo nnInfo) {//, int intendentLevel = 0) {
			
			if (box.node != null) {
				//Leaf node
				if (NavMeshGraph.ContainsPoint (box.node,p,graph.vertices)) {
					//Update the NNInfo
					
					if (nnInfo.node == null) {
						nnInfo.node = box.node;
					} else if (Mathf.Abs(((Vector3)box.node.position).y - p.y) < Mathf.Abs (((Vector3)nnInfo.node.position).y - p.y)) {
						nnInfo.node = box.node;
					}
					if (constraint.Suitable (box.node)) {
						if (nnInfo.constrainedNode == null) {
							nnInfo.constrainedNode = box.node;
						} else if (Mathf.Abs(box.node.position.y - p.y) < Mathf.Abs (nnInfo.constrainedNode.position.y - p.y)) {
							nnInfo.constrainedNode = box.node;
						}
					}
				}
				return;
			}
			
			//Search children
			if (RectContains (box.c1.rect,p)) {
				SearchBox (box.c1,p, constraint, ref nnInfo);
			}
			
			if (RectContains (box.c2.rect,p)) {
				SearchBox (box.c2,p, constraint, ref nnInfo);
			}
		}
		
		public void Insert (MeshNode node) {
			BBTreeBox box = new BBTreeBox (this,node);
			
			if (root == null) {
				root = box;
				return;
			}
			
			BBTreeBox c = root;
			while (true) {
				
				c.rect = ExpandToContain (c.rect,box.rect);
				if (c.node != null) {
					//Is Leaf
					c.c1 = box;
					BBTreeBox box2 = new BBTreeBox (this,c.node);
					//Console.WriteLine ("Inserted "+box.node+", rect "+box.rect.ToString ());
					c.c2 = box2;
					
					
					c.node = null;
					//c.rect = c.rect.
					return;
				} else {
					float e1 = ExpansionRequired (c.c1.rect,box.rect);
					float e2 = ExpansionRequired (c.c2.rect,box.rect);
					
					//Choose the rect requiring the least expansion to contain box.rect
					if (e1 < e2) {
						c = c.c1;
					} else if (e2 < e1) {
						c = c.c2;
					} else {
						//Equal, Choose the one with the smallest area
						c = RectArea (c.c1.rect) < RectArea (c.c2.rect) ? c.c1 : c.c2;
					}
				}
			}
		}
		
		public void OnDrawGizmos () {
			//Gizmos.color = new Color (1,1,1,0.01F);
			//OnDrawGizmos (root);
		}
		
		public void OnDrawGizmos (BBTreeBox box) {
			if (box == null) {
				return;
			}
			
			Vector3 min = new Vector3 (box.rect.xMin,0,box.rect.yMin);
			Vector3 max = new Vector3 (box.rect.xMax,0,box.rect.yMax);
			
			Vector3 center = (min+max)*0.5F;
			Vector3 size = (max-center)*2;
			
			Gizmos.DrawCube (center,size);
			
			OnDrawGizmos (box.c1);
			OnDrawGizmos (box.c2);
		}
		
		public void TestIntersections (Vector3 p, float radius) {
			
			BBTreeBox box = root;
			
			
			TestIntersections (box,p,radius);
		}
		
		public void TestIntersections (BBTreeBox box, Vector3 p, float radius) {
			
			if (box == null) {
				return;
			}
			
			RectIntersectsCircle (box.rect,p,radius);
			
			TestIntersections (box.c1,p,radius);
			TestIntersections (box.c2,p,radius);
		}
		
		public bool NodeIntersectsCircle (MeshNode node, Vector3 p, float radius) {
			
			if (NavMeshGraph.ContainsPoint (node,p,graph.vertices)) {
				return true;
			}
			
			Int3[] vertices = graph.vertices;
			Vector3 p1 = (Vector3)vertices[node[0]], p2 = (Vector3)vertices[node[1]], p3 = (Vector3)vertices[node[2]];
			
			float r2 = radius*radius;
			p1.y = p.y;
			p2.y = p.y;
			p3.y = p.y;
			
			return 	Mathfx.DistancePointSegmentStrict (p1,p2,p) < r2 ||
					Mathfx.DistancePointSegmentStrict (p2,p3,p) < r2 ||
					Mathfx.DistancePointSegmentStrict (p3,p1,p) < r2;
			
		}
		
		public bool RectIntersectsCircle (Rect r, Vector3 p, float radius) {
			if (RectContains (r,p)) {
				return true;
			}
			
			return XIntersectsCircle (r.xMin,r.xMax,r.yMin,p,radius) ||
			XIntersectsCircle (r.xMin,r.xMax,r.yMax,p,radius) ||
			ZIntersectsCircle (r.yMin,r.yMax,r.xMin,p,radius) ||
			ZIntersectsCircle (r.yMin,r.yMax,r.xMax,p,radius);
			
		}
		
		/** Returns if a rect contains the 3D point in XZ space */
		public bool RectContains (Rect r, Vector3 p) {
			return p.x >= r.xMin && p.x <= r.xMax && p.z >= r.yMin && p.z <= r.yMax;
		}
		
		public bool ZIntersectsCircle (float z1, float z2, float xpos, Vector3 circle, float radius) {
			double f = Math.Abs (xpos-circle.x)/radius;
			if (f > 1.0 || f < -1.0) {
				return false;
			}
			
			double s = Math.Acos (f);
			
			float s1 = (float)Math.Sin (s)*radius;
			float s2 = circle.z - s1;
				  s1 += circle.z;
			
			float min = Math.Min (s1,s2);
			float max = Math.Max (s1,s2);
			
			min = Mathf.Max (z1,min);
			max = Mathf.Min (z2,max);
			
			return max > min;
		}
		
		public bool XIntersectsCircle (float x1, float x2, float zpos, Vector3 circle, float radius) {
			double f = Math.Abs (zpos-circle.z)/radius;
			if (f > 1.0 || f < -1.0) {
				return false;
			}
			
			double s = Math.Asin (f);
			
			float s1 = (float)Math.Cos (s)*radius;
			float s2 = circle.x - s1;
			s1 += circle.x;
			
			float min = Math.Min (s1,s2);
			float max = Math.Max (s1,s2);
			
			min = Mathf.Max (x1,min);
			max = Mathf.Min (x2,max);
			
			return max > min;
		}
		
		/** Returns the difference in area between \a r and \a r expanded to contain \a r2 */
		public float ExpansionRequired (Rect r, Rect r2) {
			float xMin = Mathf.Min (r.xMin,r2.xMin);
			float xMax = Mathf.Max (r.xMax,r2.xMax);
			float yMin = Mathf.Min (r.yMin,r2.yMin);
			float yMax = Mathf.Max (r.yMax,r2.yMax);
			
			return (xMax-xMin)*(yMax-yMin)-RectArea (r);
		}
		
		/** Returns a new rect which contains both \a r and \a r2 */
		public Rect ExpandToContain (Rect r, Rect r2) {
			float xMin = Mathf.Min (r.xMin,r2.xMin);
			float xMax = Mathf.Max (r.xMax,r2.xMax);
			float yMin = Mathf.Min (r.yMin,r2.yMin);
			float yMax = Mathf.Max (r.yMax,r2.yMax);
			
			return Rect.MinMaxRect (xMin,yMin,xMax,yMax);
		}
		
		/** Returns the area of a rect */
		public float RectArea (Rect r) {
			return r.width*r.height;
		}
		
		public new void ToString () {
			Console.WriteLine ("Root "+(root.node != null ? root.node.ToString () : ""));
			
			BBTreeBox c = root;
			
			Stack<BBTreeBox> stack = new Stack<BBTreeBox>();
			stack.Push (c);
			
			c.WriteChildren (0);
		}
		
	}
	
	public class BBTreeBox {
		public Rect rect;
		public MeshNode node;
		
		public BBTreeBox c1;
		public BBTreeBox c2;
		
		public BBTreeBox (BBTree tree, MeshNode node) {
			this.node = node;
			Vector3 first = (Vector3)tree.graph.vertices[node[0]];
			Vector2 min = new Vector2(first.x,first.z);
			Vector2 max = min;
			
			for (int i=1;i<3;i++) {
				Vector3 p = (Vector3)tree.graph.vertices[node[i]];
				min.x = Mathf.Min (min.x,p.x);
				min.y = Mathf.Min (min.y,p.z);
				
				max.x = Mathf.Max (max.x,p.x);
				max.y = Mathf.Max (max.y,p.z);
			}
			
			rect = Rect.MinMaxRect (min.x,min.y,max.x,max.y);
		}
			
		public bool Contains (Vector3 p) {
			return rect.Contains (p);
		}
		
		public void WriteChildren (int level) {
			for (int i=0;i<level;i++) {
				Console.Write ("  ");
			}
			if (node != null) {
				Console.WriteLine ("Leaf ");//+triangle.ToString ());
			} else {
				Console.WriteLine ("Box ");//+rect.ToString ());
				c1.WriteChildren (level+1);
				c2.WriteChildren (level+1);
			}
		}
	}
}

