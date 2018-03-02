using UnityEngine;
using System.Collections;
using Pathfinding;

/** \astarpro */
public class VoxelArea {
	
	public static uint MaxHeight = 65536;
	public static int MaxHeightInt = 65536;
	
	public int width = 0;
	public int height = 0;
	public int depth = 0;
	
	public VoxelCell[] cells;
	
	public CompactVoxelSpan[] compactSpans;
	public CompactVoxelCell[] compactCells;
	
	public int[] areaTypes;
	
	public ushort[] dist;
	public ushort maxDistance;
	
	public int maxRegions = 0;
	
	public VoxelArea (Bounds b) {
		width = Mathf.RoundToInt (b.size.x);
		height = Mathf.RoundToInt (b.size.y);
		depth = Mathf.RoundToInt (b.size.z);
		
		int wd = width*depth;
		cells = new VoxelCell[wd];
		compactCells = new CompactVoxelCell[wd];
		//for (int x=0;x<wd;x++) {
			//cells[x] = new VoxelCell ();
			//cells[x].lockObject = new System.Object ();
			//compactCells[x] = new CompactVoxelCell ();
			//compactSpans[x] = new CompactVo
			//cells[x].firstSpan = new VoxelSpan (0,(uint)height+1);
		//}
		
		DirectionX = new int[4] {-1,0,1,0};
		DirectionZ = new int[4] {0,width,0,-width};
		
		VectorDirection = new Vector3[4] {Vector3.left, Vector3.forward,Vector3.right, Vector3.back};
	}

	public int GetSpanCountAll () {
		int count = 0;
		
		int wd = width*depth;
		
		for (int x=0;x<wd;x++) {
			
			for (VoxelSpan s = cells[x].firstSpan; s != null; s = s.next) {
				count++;
			}
		}
		return count;
	}
	
	public int GetSpanCount () {
		int count = 0;
		
		int wd = width*depth;
		
		for (int x=0;x<wd;x++) {
			
			for (VoxelSpan s = cells[x].firstSpan; s != null; s = s.next) {
				if (s.area != 0) {
					count++;
				}
			}
		}
		return count;
	}
	
	public int[] DirectionX;
	public int[] DirectionZ;
	
	public Vector3[] VectorDirection;
}

public class VoxelContourSet {
	public VoxelContour[] conts;		// Pointer to all contours.
	//public int nconts;				// Number of contours.
	public Bounds bounds;	// Bounding box of the heightfield.
	//public float cellSize, cellHeight;			// Cell size and height.
}

public struct VoxelContour {
	public int nverts;
	public int[] verts;		// Vertex coordinates, each vertex contains 4 components.
	public int[] rverts;		// Raw vertex coordinates, each vertex contains 4 components.
	
	public int reg;			// Region ID of the contour.
	public int area;			// Area ID of the contour.
}

public struct VoxelMesh {
	public Int3[] verts;
	public int[] tris;
}

public struct VoxelCell {
	
	public VoxelSpan firstSpan;
	
	//public System.Object lockObject;
	
	public void AddSpan (uint bottom, uint top, int area, int voxelWalkableClimb) {
		VoxelSpan span = new VoxelSpan (bottom,top,area);
		
		//lock (lockObject) {
			
			if (firstSpan == null) {
				firstSpan = span;
				return;
			}
		
		
			
			VoxelSpan prev = null;
			
			VoxelSpan cSpan = firstSpan;
			
			//for (VoxelSpan cSpan = firstSpan; cSpan != null; cSpan = cSpan.next) {
			while (cSpan != null) {
				if (cSpan.bottom > span.top) {
					break;
					
				} else if (cSpan.top < span.bottom) {
					prev = cSpan;
					cSpan = cSpan.next;
				} else {
					if (cSpan.bottom < bottom) {
						span.bottom = cSpan.bottom;
					}
					if (cSpan.top > top) {
						span.top = cSpan.top;
					}
					
					//1 is flagMergeDistance, when a walkable flag is favored before an unwalkable one
					if (Mathfx.Abs ((int)span.top - (int)cSpan.top) <= voxelWalkableClimb) {
						span.area = Mathfx.Max (span.area,cSpan.area);
					}
					
					VoxelSpan next = cSpan.next;
					if (prev != null) {
						prev.next = next;
					} else {
						firstSpan = next;
					}
					cSpan = next;
					
					/*cSpan.bottom = cSpan.bottom < bottom ? cSpan.bottom : bottom;
					cSpan.top = cSpan.top > top ? cSpan.top : top;
					
					if (cSpan.bottom < span.bottom) {
						span.bottom = cSpan.bottom;
					}
					if (cSpan.top > span.top) {
						span.top = cSpan.top;
					}
					
					span.area = Mathfx.Min (span.area,cSpan.area);
					VoxelSpan next = cSpan.next;
					
					if (prev != null) {
						prev.next = next;
					} else {
						firstSpan = next;
					}
					cSpan = next;*/
				}
			}
			
			if (prev != null) {
				span.next = prev.next;
				prev.next = span;
			} else {
				span.next = firstSpan;
				firstSpan = span;
			}
		//}
	}
	
	public void FilterWalkable (uint walkableHeight) {
		VoxelSpan prev = null;
		
		for (VoxelSpan cSpan = firstSpan; cSpan != null; cSpan = cSpan.next) {
			if (cSpan.area == 1) {
				if (prev != null) {
					prev.next = cSpan.next;
				} else {
					firstSpan = cSpan.next;
				}
			} else {
				if (cSpan.next != null) {
					cSpan.top = cSpan.next.bottom;
				} else {
					cSpan.top = VoxelArea.MaxHeight;
				}
				
				uint val = cSpan.top-cSpan.bottom;
				if (cSpan.top < cSpan.bottom) {
					Debug.Log ((cSpan.top-cSpan.bottom));
				}
				
				if (val < walkableHeight) {
					if (prev != null) {
						prev.next = cSpan.next;
					} else {
						firstSpan = cSpan.next;
					}
				} else {
					prev = cSpan;
					continue;
				}
			}
			
		}
	}
}

public struct CompactVoxelCell {
	public uint index;
	public uint count;
	
	public CompactVoxelCell (uint i, uint c) {
		index = i;
		count = c;
	}
}

public struct CompactVoxelSpan {
	public ushort y;
	public uint con;
	public uint h;
	public int reg;
	
	public CompactVoxelSpan (ushort bottom, uint height) {
		con = 24;
		y = bottom;
		h = height;
		reg = 0;
	}
	
	/*public CompactVoxelSpan (uint bottom, uint top) {
		con = 24;
		y = (ushort)bottom;
		h = top-bottom;
		area = 1;
	}*/
	
	public void SetConnection (int dir, uint value) {
		int shift = dir*6;
		con  = (uint) ( (con & ~(0x3f << shift)) | ((value & 0x3f) << shift) );
	}

	public int GetConnection (int dir) {
        return ((int)con >> dir*6) & 0x3f;
	}
	
	//const unsigned int shift = (unsigned int)dir*6;
      //  s.con = (con & ~(63 << shift)) | (((uint)value & 63) << shift);
}

public class VoxelSpan {
	public uint bottom;
	public uint top;
	
	public VoxelSpan next;
	
	/*Area
	0 is an unwalkable span (triangle face down)
	1 is a walkable span (triangle face up)
	*/
	public int area;
	//public VoxelSpan () {
	//}
	
	public VoxelSpan (uint b, uint t,int area) {
		bottom = b;
		top = t;
		this.area = area;
	}
}
