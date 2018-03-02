using UnityEngine;
using UnityEditor;
using System.Collections;
using Pathfinding;

[CustomGraphEditor (typeof(RecastGraph),"RecastGraph")]
/** \astarpro */
public class RecastGraphEditor : GraphEditor {
#if UNITY_3_5
	public GameObject meshRenderer;
	public MeshFilter meshFilter;
#endif
	public Mesh navmeshRender;
	public Renderer navmeshRenderer;
	
	/** Material to use for navmeshes in the editor */
	public static Material navmeshMaterial;
	public static bool tagMaskFoldout = false;
	
	public override void OnEnable () {
#if UNITY_3_5
		CreateDebugMesh ();
#else
		UpdateDebugMesh (editor.script);
#endif
		
		//Get a callback when scanning has finished
		AstarPath.OnLatePostScan += UpdateDebugMesh;
	}
	
	public override void OnDestroy () {
#if UNITY_3_5
		if (meshRenderer != null) {
			GameObject.DestroyImmediate (meshRenderer);
		}
#else
		//Avoid memory leak
		Mesh.DestroyImmediate (navmeshRender);
#endif
	}
	
	public override void OnDisable () {
		AstarPath.OnLatePostScan -= UpdateDebugMesh;
		
#if UNITY_3_5
		if (meshRenderer != null) {
			//GameObject.DestroyImmediate (meshRenderer);
		}
#endif
	}
	
#if UNITY_3_5
	public void CreateDebugMesh () {
		RecastGraph graph = target as RecastGraph;
		
		meshRenderer = GameObject.Find ("RecastGraph_"+graph.guid.ToString ());
		
		if (meshRenderer == null || meshFilter == null || navmeshRender == null || navmeshRenderer == null) {
			
			if (meshRenderer == null) {
				meshRenderer = new GameObject ("RecastGraph_"+graph.guid.ToString ());
				meshRenderer.hideFlags = /*HideFlags.NotEditable |*/ HideFlags.DontSave;
			}
			
			if (meshRenderer.GetComponent<NavMeshRenderer>() == null) {
				meshRenderer.AddComponent<NavMeshRenderer>();
			}
			
			MeshFilter filter;
			if ((filter = meshRenderer.GetComponent<MeshFilter>()) == null) {
				filter = meshRenderer.AddComponent<MeshFilter>();
			}
			
			navmeshRenderer = meshRenderer.GetComponent<MeshRenderer>();
			if (navmeshRenderer == null) {
				navmeshRenderer = meshRenderer.AddComponent<MeshRenderer>();
				navmeshRenderer.castShadows = false;
				navmeshRenderer.receiveShadows = false;
			}
			
			if (filter.sharedMesh == null) {
				navmeshRender = new Mesh ();
				filter.sharedMesh = navmeshRender;
			} else {
				navmeshRender = filter.sharedMesh;
			}
			
			navmeshRender.name = "Navmesh_"+graph.guid.ToString ();
		}
		
		if (navmeshMaterial == null) {
			navmeshMaterial = AssetDatabase.LoadAssetAtPath (AstarPathEditor.editorAssets + "/Materials/Navmesh.mat",typeof(Material)) as Material;
			if (navmeshMaterial == null) {
				Debug.LogWarning ("Could not find navmesh material at path "+AstarPathEditor.editorAssets + "/Materials/Navmesh.mat");
			}
			navmeshRenderer.material = navmeshMaterial;
		}
	}
#endif
	
	public void UpdateDebugMesh (AstarPath astar) {
#if UNITY_3_5
		CreateDebugMesh ();
		
		meshRenderer.transform.position = Vector3.zero;
		meshRenderer.transform.localScale = Vector3.one;
#endif
		
		RecastGraph graph = target as RecastGraph;
		
		if (graph != null && graph.nodes != null && graph.vectorVertices != null) {
			if (navmeshRender == null) navmeshRender = new Mesh();
			
			navmeshRender.Clear ();
			
			navmeshRender.vertices = graph.vectorVertices;
			
			int[] tris = new int[graph.nodes.Length*3];
			Color[] vColors = new Color[graph.vectorVertices.Length];
			
			for (int i=0;i<graph.nodes.Length;i++) {
				MeshNode node = graph.nodes[i] as MeshNode;
				tris[i*3] = node.v1;
				tris[i*3+1] = node.v2;
				tris[i*3+2] = node.v3;
				Color col = Mathfx.IntToColor (node.area,1F);
				vColors[node.v1] = col;
				vColors[node.v2] = col;
				vColors[node.v3] = col;
			}
			navmeshRender.triangles = tris;
			navmeshRender.colors = vColors;
			
			//meshRenderer.transform.position = graph.forcedBoundsCenter-graph.forcedBoundsSize*0.5F;
			//meshRenderer.transform.localScale = Int3.Precision*Voxelize.CellScale;
			navmeshRender.RecalculateNormals ();
			navmeshRender.RecalculateBounds ();
			
			if (navmeshMaterial == null) {
				navmeshMaterial = AssetDatabase.LoadAssetAtPath (AstarPathEditor.editorAssets + "/Materials/Navmesh.mat",typeof(Material)) as Material;
			}
			
			navmeshRender.hideFlags = HideFlags.HideAndDontSave;
			
#if UNITY_3_5
			navmeshRenderer.material = navmeshMaterial;
#endif
		}
	}
	
	public override void OnSceneGUI (NavGraph target) {
#if UNITY_3_5
		if (navmeshRenderer != null) {
			navmeshRenderer.enabled = editor.script.showNavGraphs;
		}
#else
		if (editor.script.showNavGraphs) {
			//navmeshMaterial, 0
			if (navmeshRender != null && navmeshMaterial != null) { 
				//Render the navmesh-mesh. The shader has three passes
				if (navmeshMaterial.SetPass (0)) Graphics.DrawMeshNow (navmeshRender, Matrix4x4.identity);
				if (navmeshMaterial.SetPass (1)) Graphics.DrawMeshNow (navmeshRender, Matrix4x4.identity);
				if (navmeshMaterial.SetPass (2)) Graphics.DrawMeshNow (navmeshRender, Matrix4x4.identity);
				
			}
		}
#endif
	}
	
	public override void OnDrawGizmos () {
	}
	public override void OnInspectorGUI (NavGraph target) {
		RecastGraph graph = target as RecastGraph;
		
		bool preEnabled = GUI.enabled;
		//if (graph.forceBounds) {
		
		graph.useCRecast = GUILayout.Toolbar (graph.useCRecast ?1:0,new GUIContent[2] {
			new GUIContent ("C# Recast","I have translated a portion of Recast to C#, this can be used in a webplayer but is more limited than the C++ version"),
			new GUIContent ("C++ Recast","Use the original C++ version of Recast, faster scanning times and has more features than the C# version, but it can only be used in the editor or on standalone applications (note that you can still scan the graph in the editor and then cache the startup if you want to build for a webplayer)"
			                +"\nTake a look in the docs on RecastGraph.useCRecast for more information on the special considerations when using this mode")}) == 1;
		
		if (graph.useCRecast) {
			BuildTarget bt = EditorUserBuildSettings.activeBuildTarget;
			if (bt != BuildTarget.StandaloneOSXIntel) {
				if (GUILayout.Button ("Note that the C++ version of Recast does not work in your selected build target ("+bt+")\n" +
					"Change build target to standalone (osx) if you want to be able to use C++\n" +
					"Click here for more info",AstarPathEditor.helpBox)) {
					Application.OpenURL (AstarPathEditor.GetURL ("cRecastHelp"));
				}
			} else {
				if (GUILayout.Button ("Note the special considerations when using C++ Recast\nClick here for more info",AstarPathEditor.helpBox)) {
					Application.OpenURL (AstarPathEditor.GetURL ("cRecastHelp"));
				}
			}
			
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				GUILayout.Label ("C++ Recast can currently not be used on Windows",AstarPathEditor.helpBox);
			}
		}
		
		System.Int64 estWidth = Mathf.RoundToInt (Mathf.Ceil (graph.forcedBoundsSize.x / graph.cellSize));
		System.Int64 estDepth = Mathf.RoundToInt (Mathf.Ceil (graph.forcedBoundsSize.z / graph.cellSize));
		
		if (estWidth*estDepth >= 1024*1024 || estDepth >= 1024*1024 || estWidth >= 1024*1024) {
			GUIStyle helpBox = GUI.skin.FindStyle ("HelpBox");
			if (helpBox == null) helpBox = GUI.skin.FindStyle ("Box");
			
			Color preColor = GUI.color;
			if (estWidth*estDepth >= 2048*2048 || estDepth >= 2048*2048 || estWidth >= 2048*2048) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			
			GUILayout.Label ("Warning : Might take some time to calculate",helpBox);
			GUI.color = preColor;
		}
		
		GUI.enabled = false;
		EditorGUILayout.LabelField ("Width (samples)",estWidth.ToString ());
		
		EditorGUILayout.LabelField ("Depth (samples)",estDepth.ToString ());
		/*} else {
			GUI.enabled = false;
			EditorGUILayout.LabelField ("Width (samples)","undetermined");
			EditorGUILayout.LabelField ("Depth (samples)","undetermined");
		}*/
		GUI.enabled = preEnabled;
		
		graph.cellSize = EditorGUILayout.FloatField (new GUIContent ("Cell Size","Size of one voxel in world units"),graph.cellSize);
		if (graph.cellSize < 0.001F) graph.cellSize = 0.001F;
		
		graph.cellHeight = EditorGUILayout.FloatField (new GUIContent ("Cell Height","Height of one voxel in world units"),graph.cellHeight);
		if (graph.cellHeight < 0.001F) graph.cellHeight = 0.001F;
		
		graph.walkableHeight = EditorGUILayout.FloatField (new GUIContent ("Walkable Height","Minimum distance to the roof for an area to be walkable"),graph.walkableHeight);
		graph.walkableClimb = EditorGUILayout.FloatField (new GUIContent ("Walkable Climb","How high can the character climb"),graph.walkableClimb);
		graph.characterRadius = EditorGUILayout.FloatField (new GUIContent ("Character Radius","Radius of the character, it's good to add some margin though"),graph.characterRadius);
		
		if(graph.useCRecast) {
			graph.regionMinSize = EditorGUILayout.IntField (new GUIContent ("Min Region Size","The lowest number of voxles in one area for it not to be deleted"),graph.regionMinSize);
		}
		
		graph.maxSlope = EditorGUILayout.Slider (new GUIContent ("Max Slope","Approximate maximum slope"),graph.maxSlope,0F,90F);
		graph.maxEdgeLength = EditorGUILayout.FloatField (new GUIContent ("Max Edge Length","Maximum length of one edge in the completed navmesh before it is split. A lower value can often yield better quality graphs"),graph.maxEdgeLength);
		graph.maxEdgeLength = graph.maxEdgeLength < graph.cellSize ? graph.cellSize : graph.maxEdgeLength;
		
		/*if (!graph.useCRecast) {
			graph.erosionRadius = EditorGUILayout.IntSlider ("Erosion radius",graph.erosionRadius,0,256);
		}*/
		
		graph.contourMaxError = EditorGUILayout.FloatField (new GUIContent ("Max edge error","Amount of simplification to apply to edges"),graph.contourMaxError);
		
		graph.rasterizeTerrain = EditorGUILayout.Toggle (new GUIContent ("Rasterize Terrain","Should a rasterized terrain be included"), graph.rasterizeTerrain);
		if (graph.rasterizeTerrain) {
			EditorGUI.indentLevel++;
			graph.rasterizeTrees = EditorGUILayout.Toggle (new GUIContent ("Rasterize Trees", "Rasterize tree colliders on terrains. " +
				"If the tree prefab has a collider, that collider will be rasterized. " +
				"Otherwise a simple box collider will be used and the script will " +
				"try to adjust it to the tree's scale, it might not do a very good job though so " +
				"an attached collider is preferable."), graph.rasterizeTrees);
			if (graph.rasterizeTrees) {
				EditorGUI.indentLevel++;
				graph.colliderRasterizeDetail = EditorGUILayout.FloatField (new GUIContent ("Collider Detail", "Controls the detail of the generated collider meshes. Increasing does not necessarily yield better navmeshes, but lowering will speed up scan"), graph.colliderRasterizeDetail);
				EditorGUI.indentLevel--;
			}
			
			graph.terrainSampleSize = EditorGUILayout.IntField (new GUIContent ("Terrain Sample Size","Size of terrain samples. A lower value is better, but slower"), graph.terrainSampleSize);
			graph.terrainSampleSize = graph.terrainSampleSize < 1 ? 1 : graph.terrainSampleSize;//Clamp to at least 1
			EditorGUI.indentLevel--;
		}
		
		graph.rasterizeMeshes = EditorGUILayout.Toggle (new GUIContent ("Rasterize Meshes", "Should meshes be rasterized and used for building the navmesh"), graph.rasterizeMeshes);
		graph.rasterizeColliders = EditorGUILayout.Toggle (new GUIContent ("Rasterize Colliders", "Should colliders be rasterized and used for building the navmesh"), graph.rasterizeColliders);
		if (graph.rasterizeColliders) {
			EditorGUI.indentLevel++;
			graph.colliderRasterizeDetail = EditorGUILayout.FloatField (new GUIContent ("Collider Detail", "Controls the detail of the generated collider meshes. Increasing does not necessarily yield better navmeshes, but lowering will speed up scan"), graph.colliderRasterizeDetail);
			EditorGUI.indentLevel--;
		}
		
		graph.mask = EditorGUILayoutx.LayerMaskField ("Layer Mask",graph.mask);
		
		graph.includeOutOfBounds = EditorGUILayout.Toggle (new GUIContent ("Include out of bounds","Should voxels out of bounds, on the Y axis below the graph, be included or not"),graph.includeOutOfBounds);
		Separator ();
		
		graph.forcedBoundsCenter = EditorGUILayout.Vector3Field ("Center",graph.forcedBoundsCenter);
		graph.forcedBoundsSize = EditorGUILayout.Vector3Field ("Size",graph.forcedBoundsSize);
		
		if (GUILayout.Button (new GUIContent ("Snap bounds to scene","Will snap the bounds of the graph to exactly contain all active meshes in the scene"))) {
			graph.SnapForceBoundsToScene ();
			GUI.changed = true;
		}
		
		Separator ();
		
		tagMaskFoldout = EditorGUILayoutx.UnityTagMaskList (new GUIContent("Tag Mask"), tagMaskFoldout, graph.tagMask);
		
		Separator ();
		
		graph.showMeshOutline = EditorGUILayout.Toggle (new GUIContent ("Show mesh outline","Toggles gizmos for drawing an outline of the mesh"),graph.showMeshOutline);
		
		graph.accurateNearestNode = EditorGUILayout.Toggle (new GUIContent ("Accurate Nearest Node Queries","More accurate nearest node queries. See docs for more info"),graph.accurateNearestNode);
		
		if (GUILayout.Button ("Export to file")) {
			ExportToFile (graph);
		}
		
		/*graph.replaceMesh = (Mesh)ObjectField (new GUIContent ("Replacement Mesh","If you make edits to the mesh manually, you can drop the new mesh file here to import it"), graph.replaceMesh,typeof(Mesh),false);
		
		if (graph.replaceMesh != null) {
			HelpBox ("Note: Graph will be replaced by the mesh");
		}*/
		//graph.mask = 1 << EditorGUILayout.LayerField ("Mask",(int)Mathf.Log (graph.mask,2));
	}
	
	/** Exports the INavmesh graph to a file */
	public void ExportToFile (NavGraph target) {
		
		INavmesh graph = (INavmesh)target;
		if (graph == null) return;
		
		Int3[] vertices = graph.vertices;
		
		if (vertices == null || target.nodes == null) {
			if (EditorUtility.DisplayDialog	 ("Scan graph before exporting?","The graph does not contain any mesh data. Do you want to scan it?","Ok","Cancel")) {
				AstarPath.MenuScan ();
			} else {
				return;
			}
		}
		
		vertices = graph.vertices;
		
		if (vertices == null || target.nodes == null) {
			Debug.LogError ("Graph still does not contain any nodes or vertices. Canceling");
			return;
		}
		
		string path = EditorUtility.SaveFilePanel ("Export .obj","","navmesh.obj","obj");
		
		if (path == "") return;
		
		//Generate .obj
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		string name = System.IO.Path.GetFileNameWithoutExtension (path);
		
		sb.Append ("g ").Append(name).AppendLine();
		
		//Write vertices	
		for (int i=0;i<vertices.Length;i++) {
			Vector3 v = (Vector3)vertices[i];
			sb.Append(string.Format("v {0} {1} {2}\n",-v.x,v.y,v.z));
		}
		
		//Define single texture coordinate to zero
		sb.Append ("vt 0\n");
		
		//Write triangles
		for (int i=0;i<target.nodes.Length;i++) {
			MeshNode node = target.nodes[i] as MeshNode;
			if (node == null) {
				Debug.LogError ("Node could not be casted to MeshNode. Node was null or no MeshNode");
				return;
			}
			sb.Append(string.Format("f {0}/0 {1}/0 {2}/0\n", node.v1+1,node.v2+1,node.v3+1));
		}
		
		string obj = sb.ToString();
		
		using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path)) 
		{
			sw.Write(obj);
		}
	}
}