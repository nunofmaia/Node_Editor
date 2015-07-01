using UnityEngine;
using UnityEditor;

[System.Serializable]
public class InputNode : Node 
{
	public const string ID = "inputNode";
	public override string GetID { get { return ID; } }
	
	public const int width = 200;
	public const int height = 50;

	public float value = 1f;

	public static InputNode Create (Vector2 position) 
	{
		InputNode node = CreateInstance <InputNode> ();
		Rect NodeRect = new Rect(position.x, position.y, width, height);
		
		node.name = "Input Node";
		node.rect = NodeRect;
		
		NodeOutput.Create (node, "Value", TypeOf.Float);
		
		node.InitBase ();
		return node;
	}

	public override void NodeGUI () 
	{
		value = EditorGUILayout.FloatField (new GUIContent ("Value", "The input value of type float"), value);
		if (Event.current.type == EventType.Repaint) 
			Outputs [0].SetRect (GUILayoutUtility.GetLastRect ());

		if (GUI.changed)
			Node_Editor.editor.RecalculateFrom (this);
	}
	
	public override bool Calculate () 
	{
		Outputs [0].value = value;
		return true;
	}
}