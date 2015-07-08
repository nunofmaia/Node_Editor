using UnityEngine;

public static class Styles
{
	const string textures = "Assets/Plugins/Node_Editor/Textures/";
	public static GUIStyle NodeHeader()
	{
		GUIStyle style = new GUIStyle();
		
		style.alignment = TextAnchor.MiddleLeft;
		style.padding = new RectOffset(10, 10, 0, 0);
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
		style.normal.background = NodeEditor.ColorToTex(new Color(0.98f, 0.98f, 0.98f));
		
		return style;
	}
	
	public static GUIStyle NodeBody()
	{
		GUIStyle style = new GUIStyle();
		
		style.normal.background = NodeEditor.ColorToTex(Color.white);
		//  style.padding = new RectOffset(15, 15, 0, 0);
		
		return style;
	}
}
