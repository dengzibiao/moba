/// <summary>
/// KGFICustomInspectorGUI is used for drawing custom gui elements below the default inspector.
/// </summary>
public interface KGFICustomInspectorGUI
{
	/// <summary>
	/// Draw custom gui
	/// </summary>
	/// <param name="theObject"></param>
	/// <param name="theIsPrefab"></param>
	void DrawInspectorGUI(object theObject,bool theIsPrefab);
}
