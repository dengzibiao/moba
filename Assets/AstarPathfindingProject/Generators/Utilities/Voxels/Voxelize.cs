#define ASTAR_MEMCPY //Much faster array copying (used for recast graph generation). Leaving it here since it might not work on iOS (comp. page says so though).

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Threading;

/** \astarpro */
public class Voxelize {

	public static uint NotConnected = 0x3f;
	
	public float walkableHeight = 0.8F;
	public float walkableClimb = 0.8F;
	
	public int voxelWalkableClimb;
	public uint voxelWalkableHeight;
	
	public float cellSize = 0.2F;
	public float cellHeight = 0.1F;
	
	public float maxEdgeLength = 20;
	
	public float maxSlope = 30;
	public Vector3 voxelOffset;
	
	public bool includeOutOfBounds = false;
	
	public Bounds forcedBounds;
	
	public int runTimes = 10;

	public VoxelArea voxelArea;
	public VoxelContourSet countourSet;
	
	//Unmotivated variable, but let's clamp the layers at 256
	public static int MaxLayers = 65535;
	
	//TODO : Check up on this variable
	public static int MaxRegions = 500;
	
	public static int UnwalkableArea = 0;
	
	// If heightfield region ID has the following bit set, the region is on border area
	// and excluded from many calculations.
	static ushort BorderReg = 0x8000;

	// If contour region ID has the following bit set, the vertex will be later
	// removed in order to match the segments and vertices at tile boundaries.
	static int RC_BORDER_VERTEX = 0x10000;

	static int RC_AREA_BORDER = 0x20000;

	public static int VERTEX_BUCKET_COUNT = 1<<12;
	
	public static int RC_CONTOUR_TESS_WALL_EDGES = 0x01;	// Tessellate wall edges
	public static int RC_CONTOUR_TESS_AREA_EDGES = 0x02;	// Tessellate edges between areas.
	
	// Mask used with contours to extract region id.
	static int ContourRegMask = 0xffff;

	public string debugString = "";
	
#if !PhotonImplementation
	public void OnGUI () {
		GUI.Label (new Rect (5,5,200,Screen.height),debugString);
	}
#endif
	
	public static Vector3 CellScale;
	public static Vector3 CellScaleDivision;
	
	public Voxelize (float ch, float cs, float wc, float wh, float ms) {
		cellSize = cs;
		cellHeight = ch;
		walkableHeight = wh;
		walkableClimb = wc;
		maxSlope = ms;
		
		CellScale = new Vector3 (cellSize,cellHeight,cellSize);
		CellScaleDivision = new Vector3 (1F/cellSize,1F/cellHeight,1F/cellSize);
	}
	
	public static Bounds CollectMeshes (MeshFilter[] filters, RecastGraph.ExtraMesh[] extraMeshes, Bounds bounds, out Vector3[] verts, out int[] tris) {
		List<Vector3> verticeList = new List<Vector3>();
		List<int> triangleList = new List<int>();
		
		for (int i=0;i<filters.Length;i++) {
			
			MeshFilter filter = filters[i];
			
			if (filter.GetComponent<Renderer>() == null || filter.sharedMesh == null) {
				continue;
			}
			
			if (!filter.GetComponent<Renderer>().bounds.Intersects (bounds)) {
				continue;
			}
			
			Vector3[] vs = filter.sharedMesh.vertices;
			int[] ts = filter.sharedMesh.triangles;
			
			//Vector3 trOffset = filters.transform.position;
			
			Matrix4x4 offsetMatrix = filter.transform.localToWorldMatrix;
			
			for (int q=0;q<vs.Length;q++) {
				vs[q] = offsetMatrix.MultiplyPoint3x4 (vs[q]);
			}
			
			for (int q=0;q<ts.Length;q++) {
				ts[q] += verticeList.Count;
			}
			verticeList.AddRange (vs);
			triangleList.AddRange (ts);
			
			/*if (!forceBounds) {
				bounds.Encapsulate (filter.renderer.bounds);
			}*/
		}
		
		if (extraMeshes != null) {
			for (int i=0;i< extraMeshes.Length;i++) {
				RecastGraph.ExtraMesh extraMesh = extraMeshes[i];
				
				
				if (!extraMesh.bounds.Intersects (bounds)) {
					continue;
				}
				
				Vector3[] vs = extraMesh.vertices;
				int[] ts = extraMesh.triangles;
				
				if (triangleList.Capacity < triangleList.Count + ts.Length) {
					triangleList.Capacity = Mathf.Max (2*triangleList.Capacity, triangleList.Count + ts.Length);
				}
				
				int tOffset = verticeList.Count;
				
				for (int q=0;q<ts.Length;q++) {
					triangleList.Add (ts[q] + tOffset);
				}
				
				if (extraMesh.matrix.isIdentity) {
					//An identity matrix will change nothing, so skip applying it
					verticeList.AddRange (vs);
				} else {
					Matrix4x4 m = extraMesh.matrix;
					for (int q=0;q<vs.Length;q++) {
						verticeList.Add (m.MultiplyPoint3x4(vs[q]));
					}
				}
			}
		}
		
		verts = verticeList.ToArray ();
		tris = triangleList.ToArray ();
		return bounds;
	}
	
	public VoxelArea VoxelizeMesh (MeshFilter[] filters, RecastGraph.ExtraMesh[] extraMeshes = null) {
		Vector3[] verts;
		int[] tris;
		Bounds bounds = CollectMeshes (filters,extraMeshes, forcedBounds,out verts, out tris);
		
		
		AstarProfiler.StartProfile ("Build Navigation Mesh");
		
		AstarProfiler.StartProfile ("Voxelizing - Step 1");
		
		CellScale = new Vector3 (cellSize,cellHeight,cellSize);
		CellScaleDivision = new Vector3 (1F/cellSize,1F/cellHeight,1F/cellSize);
		
		float ics = 1F/cellSize;
		float ich = 1F/cellHeight;
		
		voxelWalkableHeight = (uint)(walkableHeight/cellHeight);
		voxelWalkableClimb = Mathf.RoundToInt (walkableClimb/cellHeight);
		
		/*Mesh mesh1 = g1.GetComponent<MeshFilter>().sharedMesh;
		Matrix4x4 matrix1 = g1.transform.localToWorldMatrix;
		
		Mesh mesh2 = g2.GetComponent<MeshFilter>().sharedMesh;
		Matrix4x4 matrix2 = g2.transform.localToWorldMatrix;
		
		Vector3 pos = g1.transform.position;
		Vector3 pos2 = g2.transform.position;
		
		int[] tris1 = mesh1.triangles;
		int[] tris2 = mesh2.triangles;
		
		Vector3[] verts1 = mesh1.vertices;
		int v1L = verts1.Length;
		Vector3[] verts2 = mesh2.vertices;
		
		int[] tris = new int[tris1.Length+tris2.Length];
		
		for (int i=0;i<tris1.Length;i++) {
			tris[i] = tris1[i];
		}
		
		
		int sV = tris1.Length;
		for (int i=0;i<tris2.Length;i++) {
			tris[i+sV] = tris2[i]+v1L;
		}
		
		Vector3[] verts = new Vector3[tris1.Length+tris2.Length];
		
		for (int i=0;i<verts1.Length;i++) {
			verts[i] = matrix1.MultiplyPoint3x4 (verts1[i]);//+pos;
		}
		
		for (int i=0;i<verts2.Length;i++) {
			verts[i+v1L] = matrix2.MultiplyPoint3x4 (verts2[i]);//verts2[i]+pos2;
		}*/
		
		//MeshFilter[] filters = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
		
		//Start Voxel
		
		//Bounds
		//Bounds bounds = g1.renderer.bounds;
		//bounds.Encapsulate (g2.renderer.bounds.min);
		//bounds.Encapsulate (g2.renderer.bounds.max);
		
		//Debug.DrawLine (bounds.min,bounds.max,Color.white);
		
		//Vector3 center = bounds.center;
		Vector3 min = bounds.min;
		voxelOffset = min;
		//End Bounds
		
		for (int i=0;i<verts.Length;i++) {
			verts[i] -= bounds.min;
			verts[i].x *= ics;
			verts[i].y *= ich;
			verts[i].z *= ics;
		}
		
		bounds.size = Vector3.Scale (bounds.size,CellScaleDivision);
		
		//Initialize the voxel area
		voxelArea = new VoxelArea (bounds);
		
		AstarProfiler.EndProfile ("Voxelizing - Step 1");
		
		AstarProfiler.StartProfile ("Voxelizing - Step 2");
		
		/*Vector3 p1;
		Vector3 p2;
		Vector3 p3;
		
		int minX;
		int minZ;
		int maxX;
		int maxZ;*/
		
		//Vector3 normal;
		
		float slopeLimit = Mathf.Cos (maxSlope*Mathf.Deg2Rad);
		
		//Utility.StartTimerAdditive (true);
		
		float[] vTris = new float[3*3];
		float[] vOut = new float[7*3];
		float[] vRow = new float[7*3];
		float[] vCellOut = new float[7*3];
		float[] vCell = new float[7*3];
	
		for (int i=0;i<tris.Length;i += 3) {
			//Debug.Log (i);
			//if (Random.value < 0.97F) {
			//	continue;
			//}
			
			Vector3 p1;
			Vector3 p2;
			Vector3 p3;
			
			int minX;
			int minZ;
			int maxX;
			int maxZ;
			
			Vector3 normal;
			
			int area = 8;
			
			p1 = verts[tris[i]];
			p2 = verts[tris[i+1]];
			p3 = verts[tris[i+2]];
			
			minX = Mathf.FloorToInt (Utility.Min (p1.x,p2.x,p3.x));
			// (Mathf.Min (Mathf.Min (p1.x,p2.x),p3.x));
			minZ = Mathf.FloorToInt (Utility.Min (p1.z,p2.z,p3.z));
			
			maxX = Mathf.CeilToInt (Utility.Max (p1.x,p2.x,p3.x));
			maxZ = Mathf.CeilToInt (Utility.Max (p1.z,p2.z,p3.z));
			
			minX = Mathf.Clamp (minX , 0 , voxelArea.width-1);
			maxX = Mathf.Clamp (maxX , 0 , voxelArea.width-1);
			minZ = Mathf.Clamp (minZ , 0 , voxelArea.depth-1);
			maxZ = Mathf.Clamp (maxZ , 0 , voxelArea.depth-1);
			
			normal = Vector3.Cross (p2-p1,p3-p1);
			
			float dot = Vector3.Dot (normal.normalized,Vector3.up);
			
			//if (dot <= 0) {
				//(area = 0;
				//continue;
			if (dot < slopeLimit) {
				area = UnwalkableArea;
				//continue;
			} else {
				area = 1;
			}
			
			//Debug.DrawLine (p1*cellSize+min+Vector3.up*0.2F,p2*cellSize+min+Vector3.up*0.1F,Color.red);
			//Debug.DrawLine (p2*cellSize+min+Vector3.up*0.1F,p3*cellSize+min,Color.red);
			//Debug.DrawRay (((p1+p2+p3)/3.0F)*cellSize-bounds.center,normal,Color.cyan);
			
			Utility.CopyVector (vTris,0,p1);
			Utility.CopyVector (vTris,3,p2);
			Utility.CopyVector (vTris,6,p3);
			
			//Utility.StartTimerAdditive (false);
			for (int x=minX;x<=maxX;x++) {
				
				int nrow = Utility.ClipPolygon (vTris , 3 , vOut , 1F , -x+0.5F,0);
				
				if (nrow < 3) {
					continue;
				}
				
				
				nrow = Utility.ClipPolygon (vOut,nrow,vRow,-1F,x+0.5F,0);
				
				if (nrow < 3) {
					continue;
				}
				
				float clampZ1 = vRow[2];
				float clampZ2 = vRow[2];
				for (int q=1; q < nrow;q++) {
					float val = vRow[q*3+2];
					clampZ1 = Mathf.Min (clampZ1,val);
					clampZ2 = Mathf.Max (clampZ2,val);
				}
				
				int clampZ1I = Mathfx.Clamp (Mathf.RoundToInt (clampZ1),0, voxelArea.depth-1);
				int clampZ2I = Mathfx.Clamp (Mathf.RoundToInt (clampZ2),0, voxelArea.depth-1);
				
				for (int z=clampZ1I;z<=clampZ2I;z++) {
					
					int ncell = Utility.ClipPolygon (vRow , nrow , vCellOut , 1F , -z+0.5F,2);
					
					if (ncell < 3) {
						continue;
					}
					
					ncell = Utility.ClipPolygonY (vCellOut , ncell , vCell , -1F , z+0.5F,2);
					
					if (ncell < 3) {
						continue;
					}
					
					/*if((Mathf.Round (z/2) == (z/2.0F))) {
						continue;
					}
					for (int q=0, j = ncell-1; q < ncell; j=q, q++) {
						Debug.DrawLine (vCell[q]*cellSize+min,vCell[j]*cellSize+min,Color.cyan);
					}*/
					
					float sMin = vCell[1];
					float sMax = vCell[1];
					for (int q=1; q < ncell;q++) {
						float val = vCell[q*3+1];
						sMin = Mathf.Min (sMin,val);
						sMax = Mathf.Max (sMax,val);
					}
					
					//Debug.DrawLine (new Vector3(x,sMin,z)*cellSize+min,new Vector3(x,sMax,z)*cellSize+min,Color.cyan);
					//if (z < 0 || x < 0 || z >= voxelArea.depth || x >= voxelArea.width) {
						//Debug.DrawRay (new Vector3(x,sMin,z)*cellSize+min, Vector3.up, Color.red);
						//continue;
					//}
					int maxi = Mathf.CeilToInt(sMax);
					if (includeOutOfBounds || maxi >= 0) {
						maxi = maxi < 0 ? 0 : maxi;
						int mini = Mathf.FloorToInt(sMin+1);
						voxelArea.cells[z*voxelArea.width+x].AddSpan ((mini >= 0 ? (uint)mini : 0),(uint)maxi,area, voxelWalkableClimb);
					}
				}
			}
			
			//Utility.EndTimerAdditive ("",false);
		}
		AstarProfiler.EndProfile ("Voxelizing - Step 2");
		
		/*int wd = voxelArea.width*voxelArea.depth;
		for (int x=0;x<wd;x++) {
			VoxelCell c = voxelArea.cells[x];
			float cPos = (float)x/(float)voxelArea.width;
			float  cPos2 = Mathf.Floor (cPos);
			
			Vector3 p = new Vector3((cPos-cPos2)*voxelArea.width,0,cPos2);
			p *= cellSize;
			p += min;
			VoxelSpan span = c.firstSpan;
			
			int count =0;
			while (span != null) {
				Color col = count < Utility.colors.Length ? Utility.colors[count] : Color.white;
				//p.y = span.bottom*cellSize+min.y;
				Debug.DrawLine (p+new Vector3(0,span.bottom*cellSize,0),p+new Vector3(0,span.top*cellSize,0),col);
				span = span.next;
				count++;
			}
			
		}*/
		
		//return;
		//Step 2 - Navigable Space
		
		AstarProfiler.StartProfile ("Filter Ledges");
		
		FilterLedges (voxelWalkableHeight, voxelWalkableClimb, cellSize, cellHeight, min);
		
		AstarProfiler.EndProfile ("Filter Ledges");
		
		AstarProfiler.StartProfile ("Filter Low Height Spans");
		FilterLowHeightSpans (voxelWalkableHeight, cellSize, cellHeight, min);
		AstarProfiler.EndProfile ("Filter Low Height Spans");
		/*if (firstStep && false) {
			int sCount = voxelArea.GetSpanCountAll ();
			Vector3[] debugPointsTop = new Vector3[sCount];
			Vector3[] debugPointsBottom = new Vector3[sCount];
			Color[] debugColors = new Color[sCount];
			
			int debugPointsCount = 0;
			
			for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
				for (int x=0;x < voxelArea.width;x++) {
					
					Vector3 p = new Vector3(x,0,pz)*cellSize+min;
					
					//CompactVoxelCell c = voxelArea.compactCells[x+z];
					VoxelCell c = voxelArea.cells[x+z];
					//if (c.count == 0) {
					//	Debug.DrawRay (p,Vector3.up,Color.red);
					//}
					
					//for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) 
					
					for (VoxelSpan s = c.firstSpan; s != null; s = s.next) {
						//CompactVoxelSpan s = voxelArea.compactSpans[i];
						
						p.y = ((float)(s.top))*cellHeight+min.y;
						
						debugPointsTop[debugPointsCount] = p;
						
						p.y = ((float)s.bottom)*cellHeight+min.y;
						debugPointsBottom[debugPointsCount] = p;
						
						debugColors[debugPointsCount] = s.area == 1 ? Color.green : (s.area == 2 ? Color.yellow : Color.red);
						debugPointsCount++;
						
						//Debug.DrawRay (p,Vector3.up*0.5F,Color.green);
					}
				}
			}

			DebugUtility.DrawCubes (debugPointsTop,debugPointsBottom,debugColors, cellSize);
		}*/
		
		BuildCompactField ();
		
		/*
		int sCount = spanCount;//voxelArea.GetSpanCount ();
		Vector3[] debugPointsTop = new Vector3[sCount];
		Vector3[] debugPointsBottom = new Vector3[sCount];
		Color[] debugColors = new Color[sCount];
		
		int debugPointsCount = 0;
		
		for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				Vector3 p = new Vector3(x,0,pz)*cellSize+min;
				
				//CompactVoxelCell c = voxelArea.compactCells[x+z];
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				//if (c.count == 0) {
				//	Debug.DrawRay (p,Vector3.up,Color.red);
				//}
				
				//for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) 
				
				for (int i = (int)c.index; i < c.index+c.count; i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					//CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					p.y = ((float)(s.y+0.1F))*cellHeight+min.y;
					
					debugPointsTop[debugPointsCount] = p;
					
					p.y = ((float)s.y)*cellHeight+min.y;
					debugPointsBottom[debugPointsCount] = p;
					
					debugColors[debugPointsCount] = s.area == 1 ? Color.green : (s.area == 2 ? Color.yellow : Color.red);
					debugPointsCount++;
					
					//Debug.DrawRay (p,Vector3.up*0.5F,Color.green);
				}
			}
		}
		
		if (firstStep) {
			DebugUtility.DrawCubes (debugPointsTop,debugPointsBottom,debugColors, cellSize);
		}*/
		/*for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				Vector3 p = new Vector3(x,0,pz)*cellSize+min;
				Vector3 ph = new Vector3(x,0,pz)*cellSize+min;
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				if (c.count == 0) {
					Debug.DrawRay (p,Vector3.up,Color.red);
				}
				
				for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) {
					
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					p.y = ((float)s.y)*cellSize+min.y;
					ph.y = ((float)s.y+Mathf.Clamp ((float)s.h,0,2))*cellSize+min.y;
					
					Debug.DrawLine (p,ph,Color.green);
					/*for (int d = 0; d<4;d++) {
						int conn = s.GetConnection (d);
						
						
						if (conn == NotConnected) {
							Debug.DrawRay (p,Vector3.up*0.2F,Color.red);
							Debug.DrawRay (p,voxelArea.VectorDirection[d]*cellSize*0.5F,Color.red);
						}
					}*
				}
			}
		}*/
		
		BuildVoxelConnections ();
		
		//ErodeWalkableArea (2);
		AstarProfiler.PrintResults ();
		return voxelArea;
		/*
		
		voxelOffset = min;
		
		AstarProfiler.StartProfile ("Build Distance Field");
		
		BuildDistanceField (min);
		
		AstarProfiler.EndProfile ("Build Distance Field");
		AstarProfiler.StartProfile ("Build Regions");
		
		BuildRegions (min);
		
		AstarProfiler.EndProfile ("Build Regions");
		
		AstarProfiler.StartProfile ("Build Contours");
		
		BuildContours ();
		
		AstarProfiler.EndProfile ("Build Contours");
		
		AstarProfiler.EndProfile ("Build Navigation Mesh");
		AstarProfiler.StartProfile ("Build Debug Mesh");
		
		int sCount = voxelArea.compactSpans.Length;
		Vector3[] debugPointsTop = new Vector3[sCount];
		Vector3[] debugPointsBottom = new Vector3[sCount];
		Color[] debugColors = new Color[sCount];
		
		int debugPointsCount = 0;
		
		//int wd = voxelArea.width*voxelArea.depth;
		
		for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				Vector3 p = new Vector3(x,0,pz)*cellSize+min;
				
				//CompactVoxelCell c = voxelArea.compactCells[x+z];
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				//if (c.count == 0) {
				//	Debug.DrawRay (p,Vector3.up,Color.red);
				//}
				
				for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					p.y = ((float)(s.y+0.1F))*cellHeight+min.y;
					
					debugPointsTop[debugPointsCount] = p;
					
					p.y = ((float)s.y)*cellHeight+min.y;
					debugPointsBottom[debugPointsCount] = p;
					
					debugColors[debugPointsCount] = //Color.Lerp (Color.black, Color.white , (float)voxelArea.dist[i] / (float)voxelArea.maxDistance);
					//Utility.GetColor ((int)s.area);
					Utility.IntToColor ((int)s.reg,0.8F);
					//Color.Lerp (Color.black, Color.white , (float)s.area / 10F);
					//(float)(Mathf.Abs(dst[i]-src[i])) / (float)5);//s.area == 1 ? Color.green : (s.area == 2 ? Color.yellow : Color.red);
					debugPointsCount++;
					
					//Debug.DrawRay (p,Vector3.up*0.5F,Color.green);
				}
			}
		}

		DebugUtility.DrawCubes (debugPointsTop,debugPointsBottom,debugColors, cellSize);
		
		AstarProfiler.EndProfile ("Build Debug Mesh");
		
		/*for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				Vector3 p = new Vector3(x,0,pz)*cellSize+min;
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				if (c.count == 0) {
					//Debug.DrawRay (p,Vector3.up,Color.red);
				}
				
				for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) {
					
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					p.y = ((float)s.y)*cellHeight+min.y;
					
					
					for (int d = 0; d<4;d++) {
						int conn = s.GetConnection (d);
						
						
						if (conn == NotConnected) {
							//Debug.DrawRay (p,Vector3.up*0.2F,Color.red);
							Debug.DrawRay (p,voxelArea.VectorDirection[d]*cellSize*0.5F,Color.red);
						} else {
							Debug.DrawRay (p,voxelArea.VectorDirection[d]*cellSize*0.5F,Color.green);
						}
					}
				}
			}
		}*/
		
		
		/*int sCount = voxelArea.compactSpans.Length;
		Vector3[] debugPointsTop = new Vector3[sCount];
		Vector3[] debugPointsBottom = new Vector3[sCount];
		Color[] debugColors = new Color[sCount];
		
		int debugPointsCount = 0;
		
		//int wd = voxelArea.width*voxelArea.depth;
		
		for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				Vector3 p = new Vector3(x,0,pz)*cellSize+min;
				
				//CompactVoxelCell c = voxelArea.compactCells[x+z];
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				//if (c.count == 0) {
				//	Debug.DrawRay (p,Vector3.up,Color.red);
				//}
				
				for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					p.y = ((float)(s.y+0.1F))*cellHeight+min.y;
					
					debugPointsTop[debugPointsCount] = p;
					
					p.y = ((float)s.y)*cellHeight+min.y;
					debugPointsBottom[debugPointsCount] = p;
					
					Color col = Color.black;
					
					switch (s.area) {
						case 0:
							col = Color.red;
							break;
						case 1:
							col = Color.green;
							break;
						case 2:
							col = Color.yellow;
							break;
						case 3:
							col = Color.magenta;
							break;
					}
					
					debugColors[debugPointsCount] = col;//Color.Lerp (Color.black, Color.white , (float)dst[i] / (float)voxelArea.maxDistance);//(float)(Mathf.Abs(dst[i]-src[i])) / (float)5);//s.area == 1 ? Color.green : (s.area == 2 ? Color.yellow : Color.red);
					debugPointsCount++;
					
					//Debug.DrawRay (p,Vector3.up*0.5F,Color.green);
				}
			}
		}

		DebugUtility.DrawCubes (debugPointsTop,debugPointsBottom,debugColors, cellSize);*/
		
		//AstarProfiler.PrintResults ();
		
		//firstStep = false;
		
	}
	
	public void BuildCompactField () {
		AstarProfiler.StartProfile ("Build Compact Voxel Field");
		
		//Build compact representation
		int spanCount = voxelArea.GetSpanCount ();
		
		voxelArea.compactSpans = new CompactVoxelSpan[spanCount];
		voxelArea.areaTypes = new int[spanCount];
		
		uint idx = 0;
		
		int w = voxelArea.width;
		int d = voxelArea.depth;
		int wd = w*d;
		
		//Parallel.For (0, voxelArea.depth, delegate (int pz) {
		for (int z=0, pz = 0;z < wd;z += w, pz++) {
			
			for (int x=0;x < w;x++) {
				
				VoxelSpan s = voxelArea.cells[x+z].firstSpan;
				
				if (s == null) {
					voxelArea.compactCells[x+z] = new CompactVoxelCell (0,0);
					continue;
				}
				
				uint index = idx;
				uint count = 0;
				
				//Vector3 p = new Vector3(x,0,pz)*cellSize+voxelOffset;
				
				while (s != null) {
					
					if (s.area != 0) {
						int bottom = (int)s.top;
						int top = s.next != null ? (int)s.next.bottom : VoxelArea.MaxHeightInt;
						
						voxelArea.compactSpans[idx] = new CompactVoxelSpan ((ushort)Mathfx.Clamp (bottom, 0, 0xffff) , (uint)Mathfx.Clamp (top-bottom, 0, 0xff));
						voxelArea.areaTypes[idx] = s.area;
						idx++;
						count++;
					}
					s = s.next;
				}
				
				voxelArea.compactCells[x+z] = new CompactVoxelCell (index, count);
			}
		}
		
		AstarProfiler.EndProfile ("Build Compact Voxel Field");
	}
	
	public void BuildVoxelConnections () {
		AstarProfiler.StartProfile ("Build Voxel Connections");
		
		int wd = voxelArea.width*voxelArea.depth;
		
		//Build voxel connections
		//for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
		
		//This will run the loop in multiple threads (speedup by ? 40%)
		Parallel.For (0, voxelArea.depth, delegate (int pz) {
			int z = pz*voxelArea.width;
			
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				Vector3 p = new Vector3(x,0,pz)*cellSize+voxelOffset;
				
				for (int i=(int)c.index, ni = (int)(c.index+c.count);i<ni;i++) {
					
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					voxelArea.compactSpans[i].con = 0xFFFFFFFF;
					
					p.y = ((float)s.y)*cellHeight+voxelOffset.y;
					
					for (int d=0;d<4;d++) {
						//s.SetConnection (d,NotConnected);
						//voxelArea.compactSpans[i].SetConnection (d,NotConnected);
						
						int nx = x+voxelArea.DirectionX[d];
						int nz = z+voxelArea.DirectionZ[d];
						
						if (nx < 0 || nz < 0 || nz >= wd || nx >= voxelArea.width) {
							continue;
						}
						
						CompactVoxelCell nc = voxelArea.compactCells[nx+nz];
						
						for (int k=(int)nc.index, nk = (int)(nc.index+nc.count); k<nk; k++) {
							
							CompactVoxelSpan ns = voxelArea.compactSpans[k];
							
							int bottom = Mathfx.Max (s.y,ns.y);
							
							int top = Mathfx.Min ((int)s.y+(int)s.h,(int)ns.y+(int)ns.h);
							
							if ((top-bottom) >= voxelWalkableHeight && Mathfx.Abs ((int)ns.y - (int)s.y) <= voxelWalkableClimb) {
								uint connIdx = (uint)k - nc.index;
								
								if (connIdx > MaxLayers) {
									Debug.LogError ("Too many layers");
									continue;
								}
								
								voxelArea.compactSpans[i].SetConnection (d,connIdx);
								
							}
							
						}
					}
				}
			}
		});
		
		AstarProfiler.EndProfile ("Build Voxel Connections");
	}
	
	//Nvp = Maximum allowed vertices per polygon
	public void BuildPolyMesh (VoxelContourSet cset, int nvp, out VoxelMesh mesh) {
		
		nvp = 3;
		
		int maxVertices = 0;
		int maxTris = 0;
		int maxVertsPerCont = 0;
		
		for (int i = 0; i < cset.conts.Length; i++) {
			
			// Skip null contours.
			if (cset.conts[i].nverts < 3) continue;
			
			maxVertices += cset.conts[i].nverts;
			maxTris += cset.conts[i].nverts - 2;
			maxVertsPerCont = Mathfx.Max (maxVertsPerCont, cset.conts[i].nverts);
		}
		
		if (maxVertices >= 65534)
		{
			Debug.LogWarning ("To many vertices for unity to render - Unity might screw up rendering, but hopefully the navmesh will work ok");
			//mesh = new VoxelMesh ();
			//yield break;
			//return;
		}
		
		//int[] vflags = new int[maxVertices];
		
		Int3[] verts = new Int3[maxVertices];
		
		int[] polys = new int[maxTris*nvp];//@Why *2*2
		
		//int[] regs = new int[maxTris];
		
		//int[] areas = new int[maxTris];
		
#if ASTAR_MEMCPY
		Pathfinding.Util.Memory.MemSet<int> (polys, 0xff, sizeof(int));
#else
		for (int i=0;i<polys.Length;i++) {
			polys[i] = 0xff;
		}
#endif
		
		//int[] nexVert = new int[maxVertices];
		
		//int[] firstVert = new int[VERTEX_BUCKET_COUNT];
		
		int[] indices = new int[maxVertsPerCont];
		
		int[] tris = new int[maxVertsPerCont*3];
		
		//ushort[] polys 
		
		int vertexIndex = 0;
		int polyIndex = 0;
		
		for (int i=0;i<cset.conts.Length;i++) {
			
			VoxelContour cont = cset.conts[i];
			
			//Skip null contours
			if (cont.nverts < 3) {
				continue;
			}
			
			for (int j=0; j < cont.nverts;j++) {
				indices[j] = j;
				cont.verts[j*4+2] /= voxelArea.width;
			}
			
			//yield return (GameObject.FindObjectOfType (typeof(MonoBehaviour)) as MonoBehaviour).StartCoroutine (
			//Triangulate (cont.nverts, cont.verts, indices, tris);
			int ntris = Triangulate (cont.nverts, cont.verts, ref indices, ref tris);
			
			/*if (ntris > cont.nverts-2) {
				Debug.LogError (ntris + " "+cont.nverts+" "+cont.verts.Length+" "+(cont.nverts-2));
			}
			
			if (ntris > maxVertsPerCont) {
				Debug.LogError (ntris*3 + " "+maxVertsPerCont);
			}
			
			int tmp = polyIndex;
			
			Debug.Log (maxTris + " "+polyIndex+" "+polys.Length+" "+ntris+" "+(ntris*3) + " " + cont.nverts);*/
			int startIndex = vertexIndex;
			for (int j=0;j<ntris*3; polyIndex++, j++) {
				//@Error sometimes
				polys[polyIndex] = tris[j]+startIndex;
			}
			
			/*int tmp2 = polyIndex;
			if (tmp+ntris*3 != tmp2) {
				Debug.LogWarning (tmp+" "+(tmp+ntris*3)+" "+tmp2+" "+ntris*3);
			}*/
			
			for (int j=0;j<cont.nverts; vertexIndex++, j++) {
				verts[vertexIndex] = new Int3(cont.verts[j*4],cont.verts[j*4+1],cont.verts[j*4+2]);
			}
		}
		
		mesh = new VoxelMesh ();
		//yield break;
		Int3[] trimmedVerts = new Int3[vertexIndex];
		for (int i=0;i<vertexIndex;i++) {
			trimmedVerts[i] = verts[i];
		}
		
		int[] trimmedTris = new int[polyIndex];
#if ASTAR_MEMCPY
		System.Buffer.BlockCopy (polys, 0, trimmedTris, 0, polyIndex*sizeof(int));
#else
		for (int i=0;i<polyIndex;i++) {
			trimmedTris[i] = polys[i];
		}
#endif
		
		
		mesh.verts = trimmedVerts;
		mesh.tris = trimmedTris;
		
		/*for (int i=0;i<mesh.tris.Length/3;i++) {
			
			int p = i*3;
			
			int p1 = mesh.tris[p];
			int p2 = mesh.tris[p+1];
			int p3 = mesh.tris[p+2];
			
			//Debug.DrawLine (ConvertPosCorrZ (mesh.verts[p1].x,mesh.verts[p1].y,mesh.verts[p1].z),ConvertPosCorrZ (mesh.verts[p2].x,mesh.verts[p2].y,mesh.verts[p2].z),Color.yellow);
			//Debug.DrawLine (ConvertPosCorrZ (mesh.verts[p1].x,mesh.verts[p1].y,mesh.verts[p1].z),ConvertPosCorrZ (mesh.verts[p3].x,mesh.verts[p3].y,mesh.verts[p3].z),Color.yellow);
			//Debug.DrawLine (ConvertPosCorrZ (mesh.verts[p3].x,mesh.verts[p3].y,mesh.verts[p3].z),ConvertPosCorrZ (mesh.verts[p2].x,mesh.verts[p2].y,mesh.verts[p2].z),Color.yellow);

			//Debug.DrawLine (ConvertPosCorrZ (verts[p1],0,verts[p1+2]),ConvertPosCorrZ (verts[p2],0,verts[p2+2]),Color.blue);
			//Debug.DrawLine (ConvertPosCorrZ (verts[p1],0,verts[p1+2]),ConvertPosCorrZ (verts[p3],0,verts[p3+2]),Color.blue);
			//Debug.DrawLine (ConvertPosCorrZ (verts[p2],0,verts[p2+2]),ConvertPosCorrZ (verts[p3],0,verts[p3+2]),Color.blue);

		}*/
	}
	
	public void RemoveDegenerateSegments(List<int> simplified) {
		
		// Remove adjacent vertices which are equal on xz-plane,
		// or else the triangulator will get confused
		for (int i = 0; i < simplified.Count/4; i++) {
			
			int ni = i+1;
			if (ni >= (simplified.Count/4))
				ni = 0;
				
			if (simplified[i*4+0] == simplified[ni*4+0] &&
				simplified[i*4+2] == simplified[ni*4+2])
			{
				// Degenerate segment, remove.
				simplified.RemoveRange (i,4);
				
				/*for (int j = i; j < simplified.Count/4-1; ++j)
				{
					simplified[j*4+0] = simplified[(j+1)*4+0];
					simplified[j*4+1] = simplified[(j+1)*4+1];
					simplified[j*4+2] = simplified[(j+1)*4+2];
					simplified[j*4+3] = simplified[(j+1)*4+3];
				}
				simplified.resize(simplified.Count-4);*/
			}
		}
	}
	
	// Returns T iff (v_i, v_j) is a proper internal
	// diagonal of P.
	public static bool Diagonal(int i, int j, int n, int[] verts, int[] indices)
	{
		return InCone(i, j, n, verts, indices) && Diagonalie(i, j, n, verts, indices);
	}
	
	public static bool InCone (int i, int j, int n, int[] verts, int[] indices) {
		int pi = (indices[i] & 0x0fffffff) * 4;
		int pj = (indices[j] & 0x0fffffff) * 4;
		int pi1 = (indices[Next(i, n)] & 0x0fffffff) * 4;
		int pin1 = (indices[Prev(i, n)] & 0x0fffffff) * 4;
	
		// If P[i] is a convex vertex [ i+1 left or on (i-1,i) ].
		if (LeftOn(pin1, pi, pi1, verts))
			return Left(pi, pj, pin1,verts) && Left(pj, pi, pi1,verts);
		// Assume (i-1,i,i+1) not collinear.
		// else P[i] is reflex.
		return !(LeftOn(pi, pj, pi1,verts) && LeftOn(pj, pi, pin1,verts));
	}
	
	// Returns true iff c is strictly to the left of the directed
	// line through a to b.
	public static bool Left(int a, int b, int c, int[] verts)
	{
		return Area2(a, b, c, verts) < 0;
	}

	public static bool LeftOn(int a, int b, int c, int[] verts)
	{
		return Area2(a, b, c, verts) <= 0;
	}
	
	public static bool Collinear(int a, int b, int c, int[] verts)
	{
		return Area2(a, b, c, verts) == 0;
	}

	public static int Area2 (int a, int b, int c, int[] verts)
	{
		return (verts[b] - verts[a]) * (verts[c+2] - verts[a+2]) - (verts[c+0] - verts[a+0]) * (verts[b+2] - verts[a+2]);
	}
	
	// Returns T iff (v_i, v_j) is a proper internal *or* external
	// diagonal of P, *ignoring edges incident to v_i and v_j*.
	static bool Diagonalie(int i, int j, int n, int[] verts, int[] indices)
	{
		int d0 = (indices[i] & 0x0fffffff) * 4;
		int d1 = (indices[j] & 0x0fffffff) * 4;
	
		// For each edge (k,k+1) of P
		for (int k = 0; k < n; k++)
		{
			int k1 = Next(k, n);
			// Skip edges incident to i or j
			if (!((k == i) || (k1 == i) || (k == j) || (k1 == j)))
			{
				int p0 = (indices[k] & 0x0fffffff) * 4;
				int p1 = (indices[k1] & 0x0fffffff) * 4;
	
				if (Vequal(d0, p0, verts) || Vequal(d1, p0, verts) || Vequal(d0, p1, verts) || Vequal(d1, p1, verts))
					continue;
				
				if (Intersect(d0, d1, p0, p1, verts))
					return false;
			}
		}
		return true;
	}
	
	//	Exclusive or: true iff exactly one argument is true.
	//	The arguments are negated to ensure that they are 0/1
	//	values.  Then the bitwise Xor operator may apply.
	//	(This idea is due to Michael Baldwin.)
	public static bool Xorb(bool x, bool y)
	{
		return !x ^ !y;
	}

	//	Returns true iff ab properly intersects cd: they share
	//	a point interior to both segments.  The properness of the
	//	intersection is ensured by using strict leftness.
	public static bool IntersectProp(int a, int b, int c, int d, int[] verts)
	{
		// Eliminate improper cases.
		if (Collinear(a,b,c, verts) || Collinear(a,b,d, verts) ||
			Collinear(c,d,a, verts) || Collinear(c,d,b, verts))
			return false;
		
		return Xorb(Left(a,b,c, verts), Left(a,b,d, verts)) && Xorb(Left(c,d,a, verts), Left(c,d,b, verts));
	}
	
	// Returns T iff (a,b,c) are collinear and point c lies 
	// on the closed segement ab.
	static bool Between(int a, int b, int c, int[] verts)
	{
		if (!Collinear(a, b, c, verts))
			return false;
		// If ab not vertical, check betweenness on x; else on y.
		if (verts[a+0] != verts[b+0])
			return	((verts[a+0] <= verts[c+0]) && (verts[c+0] <= verts[b+0])) || ((verts[a+0] >= verts[c+0]) && (verts[c+0] >= verts[b+0]));
		else
			return	((verts[a+2] <= verts[c+2]) && (verts[c+2] <= verts[b+2])) || ((verts[a+2] >= verts[c+2]) && (verts[c+2] >= verts[b+2]));
	}

	// Returns true iff segments ab and cd intersect, properly or improperly.
	static bool Intersect(int a, int b, int c, int d, int[] verts)
	{
		if (IntersectProp(a, b, c, d,verts))
			return true;
		else if (Between(a, b, c,verts) || Between(a, b, d,verts) ||
				 Between(c, d, a,verts) || Between(c, d, b,verts))
			return true;
		else
			return false;
	}
	
	static bool Vequal(int a, int b, int[] verts)
	{
		return verts[a+0] == verts[b+0] && verts[a+2] == verts[b+2];
	}

	/*public static int Next (int i, int n) {
		i++;
		if (i >= n) {
			return 0;
		}
		return i;
	}
	
	public static int Prev (int i, int n) {
		i--;
		if (i < 0) {
			return n-1;
		}
		return i;
	}*/
	
	public static int Prev(int i, int n) { return i-1 >= 0 ? i-1 : n-1; }
	public static int Next(int i, int n) { return i+1 < n ? i+1 : 0; }

	
	public int Triangulate(int n, int[] verts, ref int[] indices, ref int[] tris) {
		
		int ntris = 0;
		int[] dst = tris;
		
		int dstIndex = 0;
		
		// The last bit of the index is used to indicate if the vertex can be removed.
		for (int i = 0; i < n; i++)
		{
			int i1 = Next (i, n);
			int i2 = Next (i1, n);
			if (Diagonal (i, i2, n, verts, indices)) {
				indices[i1] |= 0x40000000;
			}
		}
		
		//yield return 0;
		
		while (n > 3) {
			
			#if ASTARDEBUG
			for (int j=0;j<n;j++) {
				DrawLine (Prev(j,n),j,indices,verts,Color.red);
			}
			#endif
			
			int minLen = -1;
			int mini = -1;
			
			for (int q = 0; q < n; q++) {
				
				int q1 = Next (q, n);
				if ((indices[q1] & 0x40000000) != 0)
				{
					int p0 = (indices[q] & 0x0fffffff) * 4;
					int p2 = (indices[Next(q1, n)] & 0x0fffffff) * 4;
					
					int dx = verts[p2+0] - verts[p0+0];
					int dz = verts[p2+2] - verts[p0+2];
					
					#if ASTARDEBUG
					DrawLine (q,Next(q1, n),indices,verts,Color.blue);
					#endif
					
					//Squared distance
					int len = dx*dx + dz*dz;
					
					if (minLen < 0 || len < minLen)
					{
						minLen = len;
						mini = q;
					}
				}
			}
			
			if (mini == -1)
			{
				Debug.LogError ("This should not happen");
				// Should not happen.
	/*			printf("mini == -1 ntris=%d n=%d\n", ntris, n);
				for (int i = 0; i < n; i++)
				{
					printf("%d ", indices[i] & 0x0fffffff);
				}
				printf("\n");*/
				//yield break;
				return -ntris;
			}
			
			int i = mini;
			int i1 = Next(i, n);
			int i2 = Next(i1, n);
			
			#if ASTARDEBUG
			//yield return 0;
			for (int j=0;j<n;j++) {
				DrawLine (Prev(j,n),j,indices,verts,Color.red);
			}
			
			DrawLine (i,i2,indices,verts,Color.magenta);
			//yield return 0;
			for (int j=0;j<n;j++) {
				DrawLine (Prev(j,n),j,indices,verts,Color.red);
			}
			#endif
			
			dst[dstIndex] = indices[i] & 0x0fffffff;
			dstIndex++;
			dst[dstIndex] = indices[i1] & 0x0fffffff;
			dstIndex++;
			dst[dstIndex] = indices[i2] & 0x0fffffff;
			dstIndex++;
			ntris++;
			
			// Removes P[i1] by copying P[i+1]...P[n-1] left one index.
			n--;
			for (int k = i1; k < n; k++) {
				indices[k] = indices[k+1];
			}
			
			if (i1 >= n) i1 = 0;
			i = Prev(i1,n);
			// Update diagonal flags.
			if (Diagonal(Prev(i, n), i1, n, verts, indices)) {
				//DrawLine (Prev(i,n),i1,indices,verts,Color.green);
				indices[i] |= 0x40000000;
			} else {
				//DrawLine (Prev(i,n),i1,indices,verts,Color.white);
				indices[i] &= 0x0fffffff;
			}
			if (Diagonal(i, Next(i1, n), n, verts, indices)) {
				//DrawLine (Next(i1, n),i,indices,verts,Color.green);
				indices[i1] |= 0x40000000;
			} else {
				indices[i1] &= 0x0fffffff;
			}
			//yield return 0;
		}
		
		//yield return 0;
		// Append the remaining triangle.
		/**dst++ = indices[0] & 0x0fffffff;
		*dst++ = indices[1] & 0x0fffffff;
		*dst++ = indices[2] & 0x0fffffff;
		ntris++;*/
		
		dst[dstIndex] = indices[0] & 0x0fffffff;
		dstIndex++;
		dst[dstIndex] = indices[1] & 0x0fffffff;
		dstIndex++;
		dst[dstIndex] = indices[2] & 0x0fffffff;
		dstIndex++;
		ntris++;
		
		return ntris;
	}
	
	public void DrawLine (int a, int b, int[] indices, int[] verts, Color col) {
		int p1 = (indices[a] & 0x0fffffff) * 4;
		int p2 = (indices[b] & 0x0fffffff) * 4;
		
		Debug.DrawLine (ConvertPosCorrZ (verts[p1+0],verts[p1+1],verts[p1+2]),ConvertPosCorrZ (verts[p2+0],verts[p2+1],verts[p2+2]),col);
	}
	
	public void SimplifyContour(List<int> verts, List<int> simplified, float maxError, int maxEdgeLenght, int buildFlags) {
		
		// Add initial points.
		bool hasConnections = false;
		for (int i = 0; i < verts.Count; i += 4) {
			
			if ((verts[i+3] & ContourRegMask) != 0) {
				
				hasConnections = true;
				break;
			}
		}
		
		if (hasConnections) {
			
			// The contour has some portals to other regions.
			// Add a new point to every location where the region changes.
			for (int i = 0, ni = verts.Count/4; i < ni; i++) {
				
				int ii = (i+1) % ni;
				bool differentRegs = (verts[i*4+3] & ContourRegMask) != (verts[ii*4+3] & ContourRegMask);
				bool areaBorders = (verts[i*4+3] & RC_AREA_BORDER) != (verts[ii*4+3] & RC_AREA_BORDER);
				
				if (differentRegs || areaBorders)
				{
					simplified.Add(verts[i*4+0]);
					simplified.Add(verts[i*4+1]);
					simplified.Add(verts[i*4+2]);
					simplified.Add(i);
				}
			}
		}
		
		
		if (simplified.Count == 0) {
			
			// If there is no connections at all,
			// create some initial points for the simplification process. 
			// Find lower-left and upper-right vertices of the contour.
			int llx = verts[0];
			int lly = verts[1];
			int llz = verts[2];
			int lli = 0;
			int urx = verts[0];
			int ury = verts[1];
			int urz = verts[2];
			int uri = 0;
			
			for (int i = 0; i < verts.Count; i += 4)
			{
				int x = verts[i+0];
				int y = verts[i+1];
				int z = verts[i+2];
				if (x < llx || (x == llx && z < llz))
				{
					llx = x;
					lly = y;
					llz = z;
					lli = i/4;
				}
				if (x > urx || (x == urx && z > urz))
				{
					urx = x;
					ury = y;
					urz = z;
					uri = i/4;
				}
			}
			
			simplified.Add(llx);
			simplified.Add(lly);
			simplified.Add(llz);
			simplified.Add(lli);
			
			simplified.Add(urx);
			simplified.Add(ury);
			simplified.Add(urz);
			simplified.Add(uri);
		}
		
		// Add points until all raw points are within
		// error tolerance to the simplified shape.
		int pn = verts.Count/4;
		
		//Use the max squared error instead
		maxError *= maxError;
		
		for (int i = 0; i < simplified.Count/4; )
		{
			
			int ii = (i+1) % (simplified.Count/4);
			
			int ax = simplified[i*4+0];
			int az = simplified[i*4+2];
			int ai = simplified[i*4+3];
			
			int bx = simplified[ii*4+0];
			int bz = simplified[ii*4+2];
			int bi = simplified[ii*4+3];
			
			// Find maximum deviation from the segment.
			float maxd = 0;
			int maxi = -1;
			int ci, cinc, endi;
			
			// Traverse the segment in lexilogical order so that the
			// max deviation is calculated similarly when traversing
			// opposite segments.
			if (bx > ax || (bx == ax && bz > az))
			{
				cinc = 1;
				ci = (ai+cinc) % pn;
				endi = bi;
			}
			else
			{
				cinc = pn-1;
				ci = (bi+cinc) % pn;
				endi = ai;
			}
			
			// Tessellate only outer edges or edges between areas.
			if ((verts[ci*4+3] & ContourRegMask) == 0 ||
				(verts[ci*4+3] & RC_AREA_BORDER) == RC_AREA_BORDER) {
					
				while (ci != endi)
				{
					float d2 = Mathfx.DistancePointSegment (verts[ci*4+0], verts[ci*4+2]/voxelArea.width, ax, az/voxelArea.width, bx, bz/voxelArea.width);
					//float d2 = Mathfx.DistancePointSegment2 (verts[ci*4+0], verts[ci*4+2]/voxelArea.width, ax, az/voxelArea.width, bx, bz/voxelArea.width);
					
					//if (Mathf.Abs (d2-d3) > 0.5F) {
					//	Debug.Log (d2+" "+d3);
					//}
					
					if (d2 > maxd)
					{
						maxd = d2;
						maxi = ci;
					}
					ci = (ci+cinc) % pn;
				}
			}
			
			// If the max deviation is larger than accepted error,
			// add new point, else continue to next segment.
			if (maxi != -1 && maxd > maxError)
			{
				// Add space for the new point.
				//simplified.resize(simplified.size()+4);
				simplified.AddRange (new int[4]);
				
				int n = simplified.Count/4;
				
				for (int j = n-1; j > i; --j)
				{
					simplified[j*4+0] = simplified[(j-1)*4+0];
					simplified[j*4+1] = simplified[(j-1)*4+1];
					simplified[j*4+2] = simplified[(j-1)*4+2];
					simplified[j*4+3] = simplified[(j-1)*4+3];
				}
				// Add the point.
				simplified[(i+1)*4+0] = verts[maxi*4+0];
				simplified[(i+1)*4+1] = verts[maxi*4+1];
				simplified[(i+1)*4+2] = verts[maxi*4+2];
				simplified[(i+1)*4+3] = maxi;
			}
			else
			{
				i++;
			}
		}
		
		
		
		//Split too long edges
		
		float maxEdgeLen = maxEdgeLength / cellSize;
		
		if (maxEdgeLen > 0 && (buildFlags & (RC_CONTOUR_TESS_WALL_EDGES|RC_CONTOUR_TESS_AREA_EDGES)) != 0) {
			
			for (int i = 0; i < simplified.Count/4; ) {
				
				if (simplified.Count/4 > 200) {
					break;
				}
				
				int ii = (i+1) % (simplified.Count/4);
				
				int ax = simplified[i*4+0];
				int az = simplified[i*4+2];
				int ai = simplified[i*4+3];
				
				int bx = simplified[ii*4+0];
				int bz = simplified[ii*4+2];
				int bi = simplified[ii*4+3];
				
				// Find maximum deviation from the segment.
				int maxi = -1;
				int ci = (ai+1) % pn;

				// Tessellate only outer edges or edges between areas.
				bool tess = false;
				
				// Wall edges.
				if ((buildFlags & RC_CONTOUR_TESS_WALL_EDGES) == 1 && (verts[ci*4+3] & ContourRegMask) == 0)
					tess = true;
				// Edges between areas.
				if ((buildFlags & RC_CONTOUR_TESS_AREA_EDGES) == 1 && (verts[ci*4+3] & RC_AREA_BORDER) == 1)
					tess = true;
				
				if (tess) {
					
					int dx = bx - ax;
					int dz = (bz/voxelArea.width) - (az/voxelArea.width);
					if (dx*dx + dz*dz > maxEdgeLen*maxEdgeLen) {
						
						// Round based on the segments in lexilogical order so that the
						// max tesselation is consistent regardles in which direction
						// segments are traversed.
						if (bx > ax || (bx == ax && bz > az))
						{
							int n = bi < ai ? (bi+pn - ai) : (bi - ai);
							maxi = (ai + n/2) % pn;
						}
						else
						{
							int n = bi < ai ? (bi+pn - ai) : (bi - ai);
							maxi = (ai + (n+1)/2) % pn;
						}
					}
					
					
				}
				
				// If the max deviation is larger than accepted error,
				// add new point, else continue to next segment.
				if (maxi != -1)
				{
					// Add space for the new point.
					//simplified.resize(simplified.size()+4);
					simplified.AddRange (new int[4]);
					
					int n = simplified.Count/4;
					for (int j = n-1; j > i; --j)
					{
						simplified[j*4+0] = simplified[(j-1)*4+0];
						simplified[j*4+1] = simplified[(j-1)*4+1];
						simplified[j*4+2] = simplified[(j-1)*4+2];
						simplified[j*4+3] = simplified[(j-1)*4+3];
					}
					// Add the point.
					simplified[(i+1)*4+0] = verts[maxi*4+0];
					simplified[(i+1)*4+1] = verts[maxi*4+1];
					simplified[(i+1)*4+2] = verts[maxi*4+2];
					simplified[(i+1)*4+3] = maxi;
				}
				else
				{
					++i;
				}
			}
		}
		
		for (int i = 0; i < simplified.Count/4; i++)
		{
			// The edge vertex flag is take from the current raw point,
			// and the neighbour region is take from the next raw point.
			int ai = (simplified[i*4+3]+1) % pn;
			int bi = simplified[i*4+3];
			simplified[i*4+3] = (verts[ai*4+3] & ContourRegMask) | (verts[bi*4+3] & RC_BORDER_VERTEX);
		}
	}
	public void WalkContour (int x, int z, int i, int[] flags, List<int> verts) {
		
		// Choose the first non-connected edge
		int dir = 0;
		
		while ((flags[i] & (1 << dir)) == 0) {
			dir++;
		}
		
		int startDir = dir;
		int startI = i;
		
		int area = voxelArea.areaTypes[i];
		
		int iter = 0;
		
		#if ASTARDEBUG
		Vector3 previousPos;
		Vector3 currentPos;
		
		previousPos = ConvertPos (
			x,
			0,
			z
		);
		
		Vector3 previousPos2 = ConvertPos (
			x,
			0,
			z
		);
		#endif
		
		while (iter++ < 40000) {
			
			//Are we facing a region edge
			if ((flags[i] & (1 << dir)) != 0) {
				
				#if ASTARDEBUG
				Vector3 pos = ConvertPos (x,0,z)+new Vector3 ((voxelArea.DirectionX[dir] != 0) ? Mathf.Sign(voxelArea.DirectionX[dir]) : 0,0,(voxelArea.DirectionZ[dir]) != 0 ? Mathf.Sign(voxelArea.DirectionZ[dir]) : 0)*0.6F;
				int dir2 = (dir+1) & 0x3;
				//pos += new Vector3 ((voxelArea.DirectionX[dir2] != 0) ? Mathf.Sign(voxelArea.DirectionX[dir2]) : 0,0,(voxelArea.DirectionZ[dir2]) != 0 ? Mathf.Sign(voxelArea.DirectionZ[dir2]) : 0)*1.2F;
				
				//Debug.DrawLine (ConvertPos (x,0,z),pos,Color.cyan);
				// Debug.DrawLine (previousPos2,pos,Color.blue);
				previousPos2 = pos;
				#endif
				//Choose the edge corner
				
				bool isBorderVertex = false;
				bool isAreaBorder = false;
				
				int px = x;
				int py = GetCornerHeight (x,z,i,dir,ref isBorderVertex);
				int pz = z;
				
				switch(dir)
				{
					case 0: pz += voxelArea.width;; break;
					case 1: px++; pz += voxelArea.width; break;
					case 2: px++; break;
				}
				
				/*case 1: px++; break;
					case 2: px++; pz += voxelArea.width; break;
					case 3: pz += voxelArea.width; break;
				*/
				
				int r = 0;
				CompactVoxelSpan s = voxelArea.compactSpans[i];
				
				if (s.GetConnection (dir) != NotConnected)
				{
					int nx = x + voxelArea.DirectionX[dir];
					int nz = z + voxelArea.DirectionZ[dir];
					int ni = (int)voxelArea.compactCells[nx+nz].index + s.GetConnection (dir);
					r = (int)voxelArea.compactSpans[ni].reg;
					
					if (area != voxelArea.areaTypes[ni]) {
						isAreaBorder = true;
					}
				}
				
				if (isBorderVertex) {
					r |= RC_BORDER_VERTEX;
				}
				if (isAreaBorder) {
					r |= RC_AREA_BORDER;
				}
				
				verts.Add(px);
				verts.Add(py);
				verts.Add(pz);
				verts.Add(r);
				
				//Debug.DrawRay (previousPos,new Vector3 ((dir == 1 || dir == 2) ? 1 : 0, 0, (dir == 0 || dir == 1) ? 1 : 0),Color.cyan);
				
				flags[i] &= ~(1 << dir); // Remove visited edges
				
				dir = (dir+1) & 0x3;  // Rotate CW
				
			} else {
				int ni = -1;
				int nx = x + voxelArea.DirectionX[dir];
				int nz = z + voxelArea.DirectionZ[dir];
				
				CompactVoxelSpan s = voxelArea.compactSpans[i];
				
				if (s.GetConnection (dir) != NotConnected)
				{
					CompactVoxelCell nc = voxelArea.compactCells[nx+nz];
					ni = (int)nc.index + s.GetConnection(dir);
				}
				
				if (ni == -1)
				{
					Debug.LogError ("This should not happen");
					return;
				}
				x = nx;
				z = nz;
				i = ni;
				
				dir = (dir+3) & 0x3;	// Rotate CCW
				
				#if ASTARDEBUG
				currentPos = ConvertPos (
					x,
					0,
					z
				);
				
				Debug.DrawLine (previousPos+Vector3.up*0,currentPos,Color.blue);
				previousPos = currentPos;
				#endif
			}
			
			if (startI == i && startDir == dir)
			{
				break;
			}
		}
		
		#if ASTARDEBUG
		Color col = new Color (Random.value,Random.value,Random.value);
		
		Vector3 offset = new Vector3 (0,Random.value*10,0);
		for (int q=0, j = (verts.Count/4)-1;q<(verts.Count/4);j=q, q++) {
			
			int i4 = q*4;
			int j4 = j*4;
			
			Vector3 p1 = ConvertPosWithoutOffset (
				verts[i4+0],
				verts[i4+1],
				verts[i4+2]
			);
			
			Vector3 p2 = ConvertPosWithoutOffset (
				verts[j4+0],
				verts[j4+1],
				verts[j4+2]
			);
			
			Debug.DrawLine (p1,p2,col);
			
		}
		#endif
		
	}
	
	public Vector3 ConvertPos (int x, int y, int z) {
		Vector3 p = Vector3.Scale (
			new Vector3 (
				x+0.5F,
				y,
				(z/(float)voxelArea.width)+0.5F
			)
			,CellScale)
			+voxelOffset;
		return p;
	}
	
	public Vector3 ConvertPosCorrZ (int x, int y, int z) {
		Vector3 p = Vector3.Scale (
			new Vector3 (
				x,
				y,
				z
			)
			,CellScale)
			+voxelOffset;
		return p;
	}
	
	public Vector3 ConvertPosWithoutOffset (int x, int y, int z) {
		Vector3 p = Vector3.Scale (
			new Vector3 (
				x,
				y,
				(z/(float)voxelArea.width)
			)
			,CellScale)
			+voxelOffset;
		return p;
	}
	
	public int GetCornerHeight (int x, int z, int i, int dir, ref bool isBorderVertex) {
		
		CompactVoxelSpan s = voxelArea.compactSpans[i];
		
		int ch = (int)s.y;
		
		//dir + clockwise direction
		int dirp = (dir+1) & 0x3;
		//int dirp = (dir+3) & 0x3;
		
		uint[] regs = new uint[4];
		
		regs[0] = (uint)voxelArea.compactSpans[i].reg | ((uint)voxelArea.areaTypes[i] << 16);
		
		if (s.GetConnection (dir) != NotConnected) {
			int nx = x + voxelArea.DirectionX[dir];
			int nz = z + voxelArea.DirectionZ[dir];
			int ni = (int)voxelArea.compactCells[nx+nz].index + s.GetConnection(dir);
			
			CompactVoxelSpan ns = voxelArea.compactSpans[ni];
			
			ch = Mathfx.Max (ch,(int)ns.y);
			regs[1] = (uint)ns.reg | ((uint)voxelArea.areaTypes[ni] << 16);
			
			if (ns.GetConnection (dirp) != NotConnected) {
				int nx2 = nx + voxelArea.DirectionX[dirp];
				int nz2 = nz + voxelArea.DirectionZ[dirp];
				int ni2 = (int)voxelArea.compactCells[nx2+nz2].index + ns.GetConnection(dirp);
				
				CompactVoxelSpan ns2 = voxelArea.compactSpans[ni2];
				
				ch = Mathfx.Max (ch,(int)ns2.y);
				regs[2] = (uint)ns2.reg | ((uint)voxelArea.areaTypes[ni2] << 16);
			}
		}
		
		if (s.GetConnection (dirp) != NotConnected) {
			int nx = x + voxelArea.DirectionX[dirp];
			int nz = z + voxelArea.DirectionZ[dirp];
			int ni = (int)voxelArea.compactCells[nx+nz].index + s.GetConnection(dirp);
			
			CompactVoxelSpan ns = voxelArea.compactSpans[ni];
			
			ch = Mathfx.Max (ch,(int)ns.y);
			regs[3] = (uint)ns.reg | ((uint)voxelArea.areaTypes[ni] << 16);
			
			if (ns.GetConnection (dir) != NotConnected) {
				int nx2 = nx + voxelArea.DirectionX[dir];
				int nz2 = nz + voxelArea.DirectionZ[dir];
				int ni2 = (int)voxelArea.compactCells[nx2+nz2].index + ns.GetConnection(dir);
				
				CompactVoxelSpan ns2 = voxelArea.compactSpans[ni2];
				
				ch = Mathfx.Max (ch,(int)ns2.y);
				regs[2] = (uint)ns2.reg | ((uint)voxelArea.areaTypes[ni2] << 16);
			}
		}
		
		// Check if the vertex is special edge vertex, these vertices will be removed later.
		for (int j = 0; j < 4; ++j) {
			int a = j;
			int b = (j+1) & 0x3;
			int c = (j+2) & 0x3;
			int d = (j+3) & 0x3;
			
			// The vertex is a border vertex there are two same exterior cells in a row,
			// followed by two interior cells and none of the regions are out of bounds.
			bool twoSameExts = (regs[a] & regs[b] & BorderReg) != 0 && regs[a] == regs[b];
			bool twoInts = ((regs[c] | regs[d]) & BorderReg) == 0;
			bool intsSameArea = (regs[c]>>16) == (regs[d]>>16);
			bool noZeros = regs[a] != 0 && regs[b] != 0 && regs[c] != 0 && regs[d] != 0;
			if (twoSameExts && twoInts && intsSameArea && noZeros)
			{
				isBorderVertex = true;
				break;
			}
		}
		
		return ch;
	}
		/*
	if (rcGetCon(s, dirp) != RC_NOT_CONNECTED)
	{
		const int ax = x + rcGetDirOffsetX(dirp);
		const int ay = y + rcGetDirOffsetY(dirp);
		const int ai = (int)chf.cells[ax+ay*chf.width].index + rcGetCon(s, dirp);
		const rcCompactSpan& as = chf.spans[ai];
		ch = rcMax(ch, (int)as.y);
		regs[3] = chf.spans[ai].reg | (chf.areas[ai] << 16);
		if (rcGetCon(as, dir) != RC_NOT_CONNECTED)
		{
			const int ax2 = ax + rcGetDirOffsetX(dir);
			const int ay2 = ay + rcGetDirOffsetY(dir);
			const int ai2 = (int)chf.cells[ax2+ay2*chf.width].index + rcGetCon(as, dir);
			const rcCompactSpan& as2 = chf.spans[ai2];
			ch = rcMax(ch, (int)as2.y);
			regs[2] = chf.spans[ai2].reg | (chf.areas[ai2] << 16);
		}
	}

	// Check if the vertex is special edge vertex, these vertices will be removed later.
	for (int j = 0; j < 4; ++j)
	{
		const int a = j;
		const int b = (j+1) & 0x3;
		const int c = (j+2) & 0x3;
		const int d = (j+3) & 0x3;
		
		// The vertex is a border vertex there are two same exterior cells in a row,
		// followed by two interior cells and none of the regions are out of bounds.
		const bool twoSameExts = (regs[a] & regs[b] & RC_BORDER_REG) != 0 && regs[a] == regs[b];
		const bool twoInts = ((regs[c] | regs[d]) & RC_BORDER_REG) == 0;
		const bool intsSameArea = (regs[c]>>16) == (regs[d]>>16);
		const bool noZeros = regs[a] != 0 && regs[b] != 0 && regs[c] != 0 && regs[d] != 0;
		if (twoSameExts && twoInts && intsSameArea && noZeros)
		{
			isBorderVertex = true;
			break;
		}
	}
	
	return ch;*/
	
	
	public void BuildRegions () {
		
		int w = voxelArea.width;
		int d = voxelArea.depth;
		
		int wd = w*d;
		
		int expandIterations = 8;
		
		int borderSize = 3;
		
		int spanCount = voxelArea.compactSpans.Length;
		
		List<int> stack = Pathfinding.Util.ListPool<int>.Claim(1024);
		
		//new List<int>(1024);
		//List<int> visited = new List<int>(1024);
		
		ushort regionId = 1;
		
		ushort[] srcReg = new ushort[spanCount];
		ushort[] srcDist = new ushort[spanCount];
		ushort[] dstReg = new ushort[spanCount];
		ushort[] dstDist = new ushort[spanCount];
		
		MarkRectWithRegion (0, borderSize, 0, d, 	(ushort)(regionId | BorderReg), srcReg);	regionId++;
		MarkRectWithRegion (w-borderSize, w, 0, d, 	(ushort)(regionId | BorderReg), srcReg);	regionId++;
		MarkRectWithRegion (0, w, 0, borderSize, 	(ushort)(regionId | BorderReg), srcReg);	regionId++;
		MarkRectWithRegion (0, w, d-borderSize, d, 	(ushort)(regionId | BorderReg), srcReg);	regionId++;
		
		uint level = (uint)((voxelArea.maxDistance+1) & ~1);
		
		int count = 0;
		
		while (level > 0) {
			level = level >= 2 ? level-2 : 0;
			
			AstarProfiler.StartProfile ("--Expand Regions");
			if (ExpandRegions (expandIterations, level, srcReg, srcDist, dstReg, dstDist, stack) != srcReg) {
				ushort[] tmp = srcReg;
				srcReg = dstReg;
				dstReg = tmp;
				
				tmp = srcDist;
				srcDist = dstDist;
				dstDist = tmp;
			}
			
			AstarProfiler.EndProfile ("--Expand Regions");
			
			AstarProfiler.StartProfile ("--Mark Regions");
			// Mark new regions with IDs.
			for (int z=0, pz = 0;z < wd;z += w, pz++) {
				for (int x=0;x < voxelArea.width;x++) {
				
					CompactVoxelCell c = voxelArea.compactCells[z+x];
					
					for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
						
						if (voxelArea.dist[i] < level || srcReg[i] != 0 || voxelArea.areaTypes[i] == UnwalkableArea)
							continue;
							
						if (FloodRegion(x, z, i, level, regionId, srcReg, srcDist, stack))
							regionId++;
					}
				}
			}
			
			AstarProfiler.EndProfile ("--Mark Regions");
			
			count++;
			
			//if (count == 10) {
			//	return;
			//}
		}
		
		if (ExpandRegions (expandIterations*8, 0, srcReg, srcDist, dstReg, dstDist, stack) != srcReg) {
			ushort[] tmp = srcReg;
			srcReg = dstReg;
			dstReg = tmp;
			
			tmp = srcDist;
			srcDist = dstDist;
			dstDist = tmp;
		}
		
		// Filter out small regions.
		voxelArea.maxRegions = regionId;
		
		//FilterSmallRegions (srcReg,100, voxelArea.maxRegions);
		                    
		// Write the result out.
		for (int i = 0; i < voxelArea.compactSpans.Length; i++) {
			voxelArea.compactSpans[i].reg = srcReg[i];
		}
		
		Pathfinding.Util.ListPool<int>.Release (stack);
	}
	
	/** Filters out or merges small regions.
	 * \todo This function has not been translated entirely (don't use it) */
	public void FilterSmallRegions (ushort[] reg, int minRegionSize, int maxRegions) {
		
		/*int maxID = 0;
		for (int i=0;i<reg.Length;i++) {
			
			maxID = Mathf.Max (maxID,(int)reg[i]);
		}*/
		
		int[] counter = new int[maxRegions+1];
		int nReg = counter.Length;
		
		for (int i=0;i<reg.Length;i++) {
			int r = (int)reg[i];
			if (r >= nReg) {
				continue;
			}
			
			counter[r]++;
		}
		
		for (int i=0;i<reg.Length;i++) {
			int r = (int)reg[i];
			if (r >= nReg) {
				continue;
			}
			
			if (counter[r] < minRegionSize) {
				reg[i] = 0;
			}
		}
		
	}
	
	public Vector3 ConvertPosition (int x,int z, int i) {
		CompactVoxelSpan s = voxelArea.compactSpans[i];
		return new Vector3 (x*cellSize,s.y*cellHeight,(z/(float)voxelArea.width)*cellSize)+voxelOffset;
	}
	
	public ushort[] ExpandRegions (int maxIterations, uint level, ushort[] srcReg, ushort[] srcDist, ushort[] dstReg, ushort[] dstDist, List<int> stack) {
		
		AstarProfiler.StartProfile ("---Expand 1");
		int w = voxelArea.width;
		int d = voxelArea.depth;
		
		int wd = w*d;
		
		// Find cells revealed by the raised level.
		stack.Clear ();
		
		for (int z=0, pz = 0;z < wd;z += w, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[z+x];
				
				for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
					
					if (voxelArea.dist[i] >= level && srcReg[i] == 0 && voxelArea.areaTypes[i] != UnwalkableArea) {
						stack.Add (x);
						stack.Add (z);
						stack.Add (i);
						
						//Debug.DrawRay (ConvertPosition(x,z,i),Vector3.up*0.5F,Color.cyan);
					}
				}
			}
		}
		
		AstarProfiler.EndProfile ("---Expand 1");
		AstarProfiler.StartProfile ("---Expand 2");
		
		int iter = 0;
		
		if (stack.Count > 0) while (true) {
			
			int failed = 0;
			
			AstarProfiler.StartProfile ("---- Copy");	
			
#if ASTAR_MEMCPY
			System.Buffer.BlockCopy (srcReg, 0, dstReg, 0, srcReg.Length*2);
			System.Buffer.BlockCopy (srcDist, 0, dstDist, 0, dstDist.Length*2);
#else
			//NOTE: half of execution time is spent here, optimize!!
			for (int i=0;i<srcReg.Length;i++) {
				dstReg[i] = srcReg[i];
			}
			
			for (int i=0;i<srcDist.Length;i++) {
				dstDist[i] = srcDist[i];
			}
#endif
			
			AstarProfiler.EndProfile ("---- Copy");
			
			for (int j=0;j<stack.Count;j += 3) {
				
				int x = stack[j];
				int z = stack[j+1];
				int i = stack[j+2];
				
				if (i < 0) {
					//Debug.DrawRay (ConvertPosition(x,z,i),Vector3.up*2,Color.blue);
					failed++;
					continue;
				}
				
				ushort r = srcReg[i];
				ushort d2 = 0xffff;
				
				CompactVoxelSpan s = voxelArea.compactSpans[i];
				int area = voxelArea.areaTypes[i];
				
				for (int dir = 0; dir < 4; dir++) {
					
					if (s.GetConnection (dir) == NotConnected) { continue; }
					
					int nx = x + voxelArea.DirectionX[dir];
					int nz = z + voxelArea.DirectionZ[dir];
					
					int ni = (int)voxelArea.compactCells[nx+nz].index + s.GetConnection (dir);
					
					if (area != voxelArea.areaTypes[ni]) { continue; }
					
					if (srcReg[ni] > 0 && (srcReg[ni] & BorderReg) == 0) {
						if ((int)srcDist[ni]+2 < (int)d2)
						{
							r = srcReg[ni];
							d2 = (ushort)(srcDist[ni]+2);
						}
					}
				}
				
				if (r != 0) {
					
					stack[j+2] = -1; // mark as used
					dstReg[i] = r;
					dstDist[i] = d2;
					
				} else {
					failed++;
					//Debug.DrawRay (ConvertPosition(x,z,i),Vector3.up*2,Color.red);
				}
				
			}
			
			// Swap source and dest.
			ushort[] tmp = srcReg;
			srcReg = dstReg;
			dstReg = tmp;
			
			tmp = srcDist;
			srcDist = dstDist;
			dstDist = tmp;
			
			if (failed*3 == stack.Count) {
				//Debug.Log("Failed count broke "+failed);
				break;
			}
			
			if (level > 0) {
				iter++;

				if (iter >= maxIterations) {
					//Debug.Log("Iterations broke");
					break;
				}
			}
		}
		
		AstarProfiler.EndProfile ("---Expand 2");
		
		return srcReg;
		
	}
	
	public bool FloodRegion (int x, int z, int i, uint level, ushort r, ushort[] srcReg, ushort[] srcDist, List<int> stack) {
	
		int area = voxelArea.areaTypes[i];
	
		// Flood fill mark region.
		stack.Clear ();
		
		stack.Add (x);
		stack.Add (z);
		stack.Add (i);
		
		srcReg[i] = r;
		srcDist[i] = 0;
		
		int lev = (int)(level >= 2 ? level-2 : 0);
		
		int count = 0;
		
		while (stack.Count > 0) {
			
			//Similar to the Pop operation of an array, but Pop is not implemented in List<>
			int ci = stack[stack.Count-1]; stack.RemoveAt(stack.Count-1);
			int cz = stack[stack.Count-1]; stack.RemoveAt(stack.Count-1);
			int cx = stack[stack.Count-1]; stack.RemoveAt(stack.Count-1);
			
			CompactVoxelSpan cs = voxelArea.compactSpans[ci];
			
			//Debug.DrawRay (new Vector3( cx*cellSize, cs.y*cellHeight, ((float)cz/(float)w)*cellSize)+min,Vector3.up, Color.cyan);
			
			// Check if any of the neighbours already have a valid region set.
			ushort ar = 0;
			
			for (int dir = 0; dir < 4; dir++)
			{
				// 8 connected
				if (cs.GetConnection (dir) != NotConnected)
				{
					int ax = cx + voxelArea.DirectionX[dir];
					int az = cz + voxelArea.DirectionZ[dir];
					
					int ai = (int)voxelArea.compactCells[ax+az].index + cs.GetConnection (dir);
					
					if (voxelArea.areaTypes[ai] != area)
						continue;
						
					ushort nr = srcReg[ai];
					
					if ((nr & BorderReg) == BorderReg) // Do not take borders into account.
						continue;
						
					if (nr != 0 && nr != r)
						ar = nr;
					
					CompactVoxelSpan aspan = voxelArea.compactSpans[ai];
					
					int dir2 = (dir+1) & 0x3;
					if (aspan.GetConnection (dir2) != NotConnected)
					{
						int ax2 = ax + voxelArea.DirectionX[dir2];
						int az2 = az + voxelArea.DirectionZ[dir2];
						
						int ai2 = (int)voxelArea.compactCells[ax2+az2].index + aspan.GetConnection(dir2);
						
						if (voxelArea.areaTypes[ai2] != area)
							continue;
							
						nr = srcReg[ai2];
						if (nr != 0 && nr != r) {
							ar = nr;
						}
					}				
				}
			}
			
			if (ar != 0)
			{
				srcReg[ci] = 0;
				continue;
			}
			count++;
			
			// Expand neighbours.
			for (int dir = 0; dir < 4; ++dir)
			{
				if (cs.GetConnection (dir) != NotConnected)
				{
					int ax = cx + voxelArea.DirectionX[dir];
					int az = cz + voxelArea.DirectionZ[dir];
					int ai = (int)voxelArea.compactCells[ax+az].index + cs.GetConnection (dir);
					
					if (voxelArea.areaTypes[ai] != area)
						continue;
						
					if (voxelArea.dist[ai] >= lev && srcReg[ai] == 0)
					{
						srcReg[ai] = r;
						srcDist[ai] = 0;
						
						stack.Add(ax);
						stack.Add(az);
						stack.Add(ai);
					}
				}
			}
		}
		
		return count > 0;
	}
	
						
	public void MarkRectWithRegion (int minx, int maxx, int minz, int maxz, ushort region, ushort[] srcReg) {
		int md = maxz * voxelArea.width;
			
		for (int z= minz*voxelArea.width;z < md;z += voxelArea.width) {
			for (int x=minx;x < maxx;x++) {
				CompactVoxelCell c = voxelArea.compactCells[z+x];
				
				for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
					
					//voxelArea.compactSpans[i].area = area;
					
					if (voxelArea.areaTypes[i] != UnwalkableArea) {
						srcReg[i] = region;
					}
				}
			}
		}
	}

	public void ErodeWalkableArea (int radius) {
		
		ushort[] src = new ushort[voxelArea.compactSpans.Length];
		
#if ASTAR_MEMCPY
		Pathfinding.Util.Memory.MemSet<ushort> (src, 0xffff, sizeof(ushort));
#else
		for (int i=0;i<src.Length;i++) {
			src[i] = 0xffff;
		}
#endif
		
		CalculateDistanceField (src);
		
		for (int i=0;i<src.Length;i++) {
			if (src[i] < radius) {
				voxelArea.areaTypes[i] = UnwalkableArea;
			}
		}
	}
	
	public void BuildDistanceField () {
		AstarProfiler.StartProfile ("Build Distance Field");
		
		ushort[] src = new ushort[voxelArea.compactSpans.Length];
		
#if ASTAR_MEMCPY
		Pathfinding.Util.Memory.MemSet<ushort> (src, 0xffff, sizeof(ushort));
#else
		for (int i=0;i<src.Length;i++) {
			src[i] = 0xffff;
		}
#endif
		voxelArea.maxDistance = CalculateDistanceField (src);
		
		ushort[] dst = new ushort[voxelArea.compactSpans.Length];
		
		dst = BoxBlur (src,dst);
		
		voxelArea.dist = dst;
		
		AstarProfiler.EndProfile ("Build Distance Field");
	}
	
	public ushort CalculateDistanceField (ushort[] src) {
		
		int wd = voxelArea.width*voxelArea.depth;
		
		for (int z=0;z < wd;z += voxelArea.width) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					int area = voxelArea.areaTypes[i];
					
					int nc = 0;
					for (int d = 0; d < 4; d++) {
						if (s.GetConnection (d) != NotConnected) {
							
							int nx = x+voxelArea.DirectionX[d];
							int nz = z+voxelArea.DirectionZ[d];
							
							int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (d));
							
							if (area == voxelArea.areaTypes[ni]) {
								nc++;
							}
						}
					}
					
					if (nc != 4) {
						src[i] = 0;
					}
				}
			}
		}
		
		//Pass 1
		
		for (int z=0;z < wd;z += voxelArea.width) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					if (s.GetConnection (0) != NotConnected) {
						// (-1,0)
						int nx = x+voxelArea.DirectionX[0];
						int nz = z+voxelArea.DirectionZ[0];
							
						int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (0));
						
						if (src[ni]+2 < src[i]) {
							src[i] = (ushort)(src[ni]+2);
						}
						
						CompactVoxelSpan ns = voxelArea.compactSpans[ni];
						
						if (ns.GetConnection (3) != NotConnected) {
							// (-1,0) + (0,-1) = (-1,-1)
							int nnx = nx+voxelArea.DirectionX[3];
							int nnz = nz+voxelArea.DirectionZ[3];
							
							int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (3));
							
							if (src[nni]+3 < src[i]) {
								src[i] = (ushort)(src[nni]+3);
							}
						}
					}
					
					if (s.GetConnection (3) != NotConnected) {
						// (0,-1)
						int nx = x+voxelArea.DirectionX[3];
						int nz = z+voxelArea.DirectionZ[3];
							
						int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (3));
						
						if (src[ni]+2 < src[i]) {
							src[i] = (ushort)(src[ni]+2);
						}
						
						CompactVoxelSpan ns = voxelArea.compactSpans[ni];
						
						if (ns.GetConnection (2) != NotConnected) {
							
							// (0,-1) + (1,0) = (1,-1)
							int nnx = nx+voxelArea.DirectionX[2];
							int nnz = nz+voxelArea.DirectionZ[2];
							
							int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (2));
							
							if (src[nni]+3 < src[i]) {
								src[i] = (ushort)(src[nni]+3);
							}
						}
					}
				}
			}
		}
		
		//Pass 2
		
		for (int z= wd-voxelArea.width;z >= 0;z -= voxelArea.width) {
			for (int x= voxelArea.width-1;x >= 0;x--) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					if (s.GetConnection (2) != NotConnected) {
						// (-1,0)
						int nx = x+voxelArea.DirectionX[2];
						int nz = z+voxelArea.DirectionZ[2];
							
						int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (2));
						
						if (src[ni]+2 < src[i]) {
							src[i] = (ushort)(src[ni]+2);
						}
						
						CompactVoxelSpan ns = voxelArea.compactSpans[ni];
						
						if (ns.GetConnection (1) != NotConnected) {
							// (-1,0) + (0,-1) = (-1,-1)
							int nnx = nx+voxelArea.DirectionX[1];
							int nnz = nz+voxelArea.DirectionZ[1];
							
							int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (1));
							
							if (src[nni]+3 < src[i]) {
								src[i] = (ushort)(src[nni]+3);
							}
						}
					}
					
					if (s.GetConnection (1) != NotConnected) {
						// (0,-1)
						int nx = x+voxelArea.DirectionX[1];
						int nz = z+voxelArea.DirectionZ[1];
							
						int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (1));
						
						if (src[ni]+2 < src[i]) {
							src[i] = (ushort)(src[ni]+2);
						}
						
						CompactVoxelSpan ns = voxelArea.compactSpans[ni];
						
						if (ns.GetConnection (0) != NotConnected) {
							
							// (0,-1) + (1,0) = (1,-1)
							int nnx = nx+voxelArea.DirectionX[0];
							int nnz = nz+voxelArea.DirectionZ[0];
							
							int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (0));
							
							if (src[nni]+3 < src[i]) {
								src[i] = (ushort)(src[nni]+3);
							}
						}
					}
				}
			}
		}
		
		ushort maxDist = 0;
		
		for (int i=0;i<voxelArea.compactSpans.Length;i++) {
			maxDist = Mathfx.Max (src[i],maxDist);
		}
		
		return maxDist;
	}
	
	public ushort[] BoxBlur (ushort[] src, ushort[] dst) {
		
		ushort thr = 20;
		
		int wd = voxelArea.width*voxelArea.depth;
		
		for (int z= wd-voxelArea.width;z >= 0;z -= voxelArea.width) {
			for (int x= voxelArea.width-1;x >= 0;x--) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					ushort cd = src[i];
					
					if (cd < thr) {
						dst[i] = cd;
						continue;
					}
					
					int total = (int)cd;
					
					for (int d=0; d < 4; d++) {
						
						if (s.GetConnection (d) != NotConnected) {
							int nx = x+voxelArea.DirectionX[d];
							int nz = z+voxelArea.DirectionZ[d];
							
							int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (d));
							
							total += (int)src[ni];
							
							CompactVoxelSpan ns = voxelArea.compactSpans[ni];
							
							int d2 = (d+1) & 0x3;
							
							if (ns.GetConnection (d2) != NotConnected) {
								int nnx = nx+voxelArea.DirectionX[d2];
								int nnz = nz+voxelArea.DirectionZ[d2];
							
								int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (d2));
								total += (int)src[nni];
							} else {
								total += cd;
							}
						} else {
							total += cd*2;
						}
					}
					dst[i] = (ushort)((total+5)/9F);
				}
			}
		}
		return dst;
	}
	
	public void BuildContours (float maxError, int maxEdgeLength, VoxelContourSet cset, int buildFlags) {
		
		int w = voxelArea.width;
		int d = voxelArea.depth;
		
		int wd = w*d;
		
		//cset.bounds = voxelArea.bounds;
		
		int maxContours = Mathf.Max (8/*Max Regions*/,8);
		
		//cset.conts = new VoxelContour[maxContours];
		List<VoxelContour> contours = new List<VoxelContour>(maxContours);
		
		//cset.nconts = 0;
		
		int[] flags = new int[voxelArea.compactSpans.Length];
		
		// Mark boundaries. (@?)
		for (int z=0;z < wd;z += voxelArea.width) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					
					int res = 0;
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					if (s.reg == 0 || (s.reg & BorderReg) == BorderReg) {
						flags[i] = 0;
						continue;
					}
					
					for (int dir=0;dir < 4; dir++) {
						int r = 0;
						
						if (s.GetConnection (dir) != NotConnected) {
							int nx = x + voxelArea.DirectionX[dir];
							int nz = z + voxelArea.DirectionZ[dir];
							
							int ni = (int)voxelArea.compactCells[nx+nz].index + s.GetConnection (dir);
							r = voxelArea.compactSpans[ni].reg;
							
							
						}
						
						//@TODO - Why isn't this inside the previous IF
						if (r == s.reg) {
							res |= (1 << dir);
							
						}
					}
					
					//Inverse, mark non connected edges.
					flags[i] = res ^ 0xf;
						
				}
				
			}
		}
		
		
		List<int> verts = new List<int> (256);
		List<int> simplified = new List<int> (64);
		
		for (int z=0;z < wd;z += voxelArea.width) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					
					//CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					if (flags[i] == 0 || flags[i] == 0xf)
					{
						flags[i] = 0;
						continue;
					}
					
					int reg = voxelArea.compactSpans[i].reg;
					
					if (reg == 0 || (reg & BorderReg) == BorderReg) {
						continue;
					}
					
					int area = voxelArea.areaTypes[i];
					
					verts.Clear ();
					simplified.Clear ();
					
					WalkContour(x, z, i, flags, verts);
					
					SimplifyContour(verts, simplified, maxError, maxEdgeLength, buildFlags);
					RemoveDegenerateSegments (simplified);
					
					VoxelContour contour = new VoxelContour ();
					contour.verts = simplified.ToArray ();
					contour.rverts = verts.ToArray ();
					contour.nverts = simplified.Count/4;
					contour.reg = reg;
					contour.area = area;
					
					contours.Add (contour);
					
					#if ASTARDEBUG
					for (int q=0, j = (simplified.Count/4)-1;q<(simplified.Count/4);j=q, q++) {
			
						int i4 = q*4;
						int j4 = j*4;
						
						Vector3 p1 = Vector3.Scale (
						new Vector3 (
							simplified[i4+0],
							simplified[i4+1],
							(simplified[i4+2]/(float)voxelArea.width)
						),
						CellScale)
						+voxelOffset;
						
						Vector3 p2 = Vector3.Scale (
						new Vector3 (
							simplified[j4+0],
							simplified[j4+1],
							(simplified[j4+2]/(float)voxelArea.width)
						)
						,CellScale)
						+voxelOffset;
						
						
						if (CalcAreaOfPolygon2D(contour.verts, contour.nverts) > 0) {
							Debug.DrawLine (p1,p2,Mathfx.IntToColor (reg,0.5F));
						} else {
							Debug.DrawLine (p1,p2,Color.red);
							
						}
						
					}
					#endif
				}
			}
		}
		
		// Check and merge droppings.
		// Sometimes the previous algorithms can fail and create several contours
		// per area. This pass will try to merge the holes into the main region.
		for (int i = 0; i < contours.Count; i++)
		{
			VoxelContour cont = contours[i];
			// Check if the contour is would backwards.
			if (CalcAreaOfPolygon2D(cont.verts, cont.nverts) < 0)
			{
				// Find another contour which has the same region ID.
				int mergeIdx = -1;
				for (int j = 0; j < contours.Count; j++)
				{
					if (i == j) continue;
					if (contours[j].nverts > 0 && contours[j].reg == cont.reg)
					{
						// Make sure the polygon is correctly oriented.
						if (CalcAreaOfPolygon2D(contours[j].verts, contours[j].nverts) > 0)
						{
							mergeIdx = j;
							break;
						}
					}
				}
				if (mergeIdx == -1)
				{
					Debug.LogError ("rcBuildContours: Could not find merge target for bad contour "+i+".");
				}
				else
				{
					Debug.LogWarning ("Fixing contour");
					VoxelContour mcont = contours[mergeIdx];
					// Merge by closest points.
					int ia = 0, ib = 0;
					GetClosestIndices(mcont.verts, mcont.nverts, cont.verts, cont.nverts, ref ia, ref ib);
					
					if (ia == -1 || ib == -1)
					{
						Debug.LogWarning ("rcBuildContours: Failed to find merge points for "+i+" and "+mergeIdx+".");
						continue;
					}
					
					int p4 = ia*4;
					int p42 = ib*4;
						
					Vector3 p12 = Vector3.Scale (
						new Vector3 (
							mcont.verts[p4+0],
							mcont.verts[p4+1],
							(mcont.verts[p4+2]/(float)voxelArea.width)
						),
						CellScale)
						+voxelOffset;
						
						Vector3 p22 = Vector3.Scale (
						new Vector3 (
							cont.verts[p42+0],
							cont.verts[p42+1],
							(cont.verts[p42+2]/(float)voxelArea.width)
						)
						,CellScale)
						+voxelOffset;
					
					Debug.DrawLine (p12,p22,Color.green);
					
					if (!MergeContours(ref mcont, ref cont, ia, ib))
					{
						Debug.LogWarning ("rcBuildContours: Failed to merge contours "+i+" and "+mergeIdx+".");
						continue;
					}
					
					contours[mergeIdx] = mcont;
					contours[i] = cont;
					
					#if ASTARDEBUG
					Debug.Log (mcont.nverts);
					
					for (int q=0, j = (mcont.nverts)-1;q<(mcont.nverts);j=q, q++) {
						int i4 = q*4;
						int j4 = j*4;
						
						Vector3 p1 = Vector3.Scale (
						new Vector3 (
							mcont.verts[i4+0],
							mcont.verts[i4+1],
							(mcont.verts[i4+2]/(float)voxelArea.width)
						),
						CellScale)
						+voxelOffset;
						
						Vector3 p2 = Vector3.Scale (
						new Vector3 (
							mcont.verts[j4+0],
							mcont.verts[j4+1],
							(mcont.verts[j4+2]/(float)voxelArea.width)
						)
						,CellScale)
						+voxelOffset;
						
						Debug.DrawLine (p1,p2,Color.red);
					//}
					}
					#endif
				}
			}
		}
	
		cset.conts = contours.ToArray ();
	}
	
	public int CalcAreaOfPolygon2D (int[] verts, int nverts) {
		int area = 0;
		for (int i = 0, j = nverts-1; i < nverts; j=i++) {
			int vi = i*4;
			int vj = j*4;
			area += verts[vi+0] * (verts[vj+2]/voxelArea.width) - verts[vj+0] * (verts[vi+2]/voxelArea.width);
		}
		
		return (area+1) / 2;
	}
		
	void GetClosestIndices(int[] vertsa, int nvertsa,
							  int[] vertsb, int nvertsb,
							  ref int ia, ref int ib) {
							  	
		int closestDist = 0xfffffff;
		ia = -1;
		ib = -1;
		for (int i = 0; i < nvertsa; i++)
		{
			//in is a keyword in C#, so I can't use that as a variable name
			int in2 = (i+1) % nvertsa;
			int ip = (i+nvertsa-1) % nvertsa;
			int va = i*4;
			int van = in2*4;
			int vap = ip*4;
			
			for (int j = 0; j < nvertsb; ++j)
			{
				int vb = j*4;
				// vb must be "infront" of va.
				if (Ileft(vap,va,vb,vertsa,vertsa,vertsb) && Ileft(va,van,vb,vertsa,vertsa,vertsb))
				{
					int dx = vertsb[vb+0] - vertsa[va+0];
					int dz = (vertsb[vb+2]/voxelArea.width) - (vertsa[va+2]/voxelArea.width);
					int d = dx*dx + dz*dz;
					if (d < closestDist)
					{
						ia = i;
						ib = j;
						closestDist = d;
					}
				}
			}
		}
	}
	
	public static bool MergeContours(ref VoxelContour ca, ref VoxelContour cb, int ia, int ib) {
		
		int maxVerts = ca.nverts + cb.nverts + 2;
		int[] verts = new int[maxVerts*4];
		
		//if (!verts)
		//	return false;
	
		int nv = 0;
	
		// Copy contour A.
		for (int i = 0; i <= ca.nverts; i++)
		{
			int dst = nv*4;
			int src = ((ia+i) % ca.nverts)*4;
			verts[dst+0] = ca.verts[src+0];
			verts[dst+1] = ca.verts[src+1];
			verts[dst+2] = ca.verts[src+2];
			verts[dst+3] = ca.verts[src+3];
			nv++;
		}
	
		// Copy contour B
		for (int i = 0; i <= cb.nverts; i++)
		{
			int dst = nv*4;
			int src = ((ib+i) % cb.nverts)*4;
			verts[dst+0] = cb.verts[src+0];
			verts[dst+1] = cb.verts[src+1];
			verts[dst+2] = cb.verts[src+2];
			verts[dst+3] = cb.verts[src+3];
			nv++;
		}
		
		//rcFree(ca.verts);
		ca.verts = verts;
		ca.nverts = nv;
	
		//rcFree(cb.verts);
		cb.verts = new int[0];
		cb.nverts = 0;
		
		return true;
	}

	public static bool Ileft(int a, int b, int c, int[] va, int[] vb, int[] vc)
	{
		return (vb[b+0] - va[a+0]) * (vc[c+2] - va[a+2]) - (vc[c+0] - va[a+0]) * (vb[b+2] - va[a+2]) <= 0;
	}
		
	/** \todo Complete the ErodeVoxels function translation */
	[System.Obsolete ("This function is not complete and should not be used")]
	public void ErodeVoxels (int radius) {
		
		if (radius > 255) {
			Debug.LogError ("Max Erode Radius is 255");
			radius = 255;
		}
		
		int wd = voxelArea.width*voxelArea.depth;
		
		int[] dist = new int[voxelArea.compactSpans.Length];
		
		for (int i=0;i<dist.Length;i++) {
			dist[i] = 0xFF;
		}
		
		for (int z=0;z < wd;z += voxelArea.width) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
					
					if (voxelArea.areaTypes[i] != UnwalkableArea) {
						
						CompactVoxelSpan s = voxelArea.compactSpans[i];
						int nc = 0;
						for (int dir=0;dir<4;dir++) {
							if (s.GetConnection (dir) != NotConnected)
								nc++;
						}
						//At least one missing neighbour
						if (nc != 4) {
							dist[i] = 0;
						}
					}
				}
			}
		}
		
		//int nd = 0;
		
		//Pass 1
		
		/*for (int z=0;z < wd;z += voxelArea.width) {
			for (int x=0;x < voxelArea.width;x++) {
				
				CompactVoxelCell c = voxelArea.compactCells[x+z];
				
				for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
					CompactVoxelSpan s = voxelArea.compactSpans[i];
					
					if (s.GetConnection (0) != NotConnected) {
						// (-1,0)
						int nx = x+voxelArea.DirectionX[0];
						int nz = z+voxelArea.DirectionZ[0];
						
						int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (0));
						CompactVoxelSpan ns = voxelArea.compactSpans[ni];
						
						if (dist[ni]+2 < dist[i]) {
							dist[i] = (ushort)(dist[ni]+2);
						}
						
						if (ns.GetConnection (3) != NotConnected) {
							// (-1,0) + (0,-1) = (-1,-1)
							int nnx = nx+voxelArea.DirectionX[3];
							int nnz = nz+voxelArea.DirectionZ[3];
							
							int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (3));
							
							if (src[nni]+3 < src[i]) {
								src[i] = (ushort)(src[nni]+3);
							}
						}
					}
					
					if (s.GetConnection (3) != NotConnected) {
						// (0,-1)
						int nx = x+voxelArea.DirectionX[3];
						int nz = z+voxelArea.DirectionZ[3];
							
						int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (3));
						
						if (src[ni]+2 < src[i]) {
							src[i] = (ushort)(src[ni]+2);
						}
						
						CompactVoxelSpan ns = voxelArea.compactSpans[ni];
						
						if (ns.GetConnection (2) != NotConnected) {
							
							// (0,-1) + (1,0) = (1,-1)
							int nnx = nx+voxelArea.DirectionX[2];
							int nnz = nz+voxelArea.DirectionZ[2];
							
							int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (2));
							
							if (src[nni]+3 < src[i]) {
								src[i] = (ushort)(src[nni]+3);
							}
						}
					}
				}
			}
		}*/
	}
	
	public void FilterLowHeightSpans (uint voxelWalkableHeight, float cs, float ch, Vector3 min) {
		int wd = voxelArea.width*voxelArea.depth;
		
		//Filter all ledges
		for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				for (VoxelSpan s = voxelArea.cells[z+x].firstSpan; s != null; s = s.next) {
					
					uint bottom = s.top;
					uint top = s.next != null ? s.next.bottom : VoxelArea.MaxHeight;
					
					if (top - bottom < voxelWalkableHeight) {
						s.area = 0;
					}
				}
			}
		}
		
	}
	
	//Code almost completely ripped from Recast
	public void FilterLedges (uint voxelWalkableHeight, int voxelWalkableClimb, float cs, float ch, Vector3 min) {
		
		int wd = voxelArea.width*voxelArea.depth;
		
		//Filter all ledges
		for (int z=0, pz = 0;z < wd;z += voxelArea.width, pz++) {
			for (int x=0;x < voxelArea.width;x++) {
				
				for (VoxelSpan s = voxelArea.cells[z+x].firstSpan; s != null; s = s.next) {
					
					//Skip non-walkable spans
					if (s.area == 0) {
						continue;
					}
					
					int bottom = (int)s.top;
					int top = s.next != null ? (int)s.next.bottom : VoxelArea.MaxHeightInt;
					
					int minHeight = VoxelArea.MaxHeightInt;
					
					int aMinHeight = (int)s.top;
					int aMaxHeight = (int)s.top;
					
					for (int d = 0; d < 4; d++) {
						
						int nx = x+voxelArea.DirectionX[d];
						int nz = z+voxelArea.DirectionZ[d];
						
						//Skip out-of-bounds points
						if (nx < 0 || nz < 0 || nz >= wd || nx >= voxelArea.width) {
							s.area = 0;
							break;
							
							//If a node's neighbour is out of bounds it shouldn't be walkable at all
							/*minHeight = (int)Mathf.Min (minHeight, -voxelWalkableClimb - bottom);
							
							if (minHeight < -voxelWalkableClimb) {
								s.area = 0;
								break;
							}
							Debug.Log ("Got Here");
							continue;*/
						}
						
						VoxelSpan nsx = voxelArea.cells[nx+nz].firstSpan;
						
						int nbottom = -voxelWalkableClimb;
						
						int ntop = nsx != null ? (int)nsx.bottom : VoxelArea.MaxHeightInt;
						
						if (Mathfx.Min (top,ntop) - Mathfx.Max (bottom,nbottom) > voxelWalkableHeight) {
							minHeight = Mathfx.Min (minHeight, nbottom - bottom);
						}
						
						//Loop through spans
						for (VoxelSpan ns = nsx; ns != null; ns = ns.next) {
							nbottom = (int)ns.top;
							ntop = ns.next != null ? (int)ns.next.bottom : VoxelArea.MaxHeightInt;
							
							if (Mathfx.Min (top, ntop) - Mathfx.Max (bottom, nbottom) > voxelWalkableHeight) {
								minHeight = Mathfx.Min (minHeight, nbottom - bottom);
								
								if (Mathfx.Abs (nbottom - bottom) <= voxelWalkableClimb) {
									if (nbottom < aMinHeight) { aMinHeight = nbottom; }
									if (nbottom > aMaxHeight) { aMaxHeight = nbottom; }
								}
							}
						}
					}
					
					if (minHeight < -voxelWalkableClimb) {
						s.area = 0;
					} else
					
					if ((aMaxHeight - aMinHeight) > voxelWalkableClimb) {
						s.area = 0;
					}
					
				}
			}
		}
	}
}
