using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CalcNode : Node
{
    public enum CalcType { Add, Substract, Multiply, Divide }
    public CalcType type = CalcType.Add;

    public const string ID = "calcNode";
    public override string GetID { get { return ID; } }

    public const int width = 100;
    public const int height = 60;

    public float Input1Val = 1f;
    public float Input2Val = 1f;

    public static CalcNode Create(Vector2 position)
    {
        CalcNode node = CreateInstance<CalcNode>();
        Rect NodeRect = new Rect(position.x, position.y, width, height);
        node.name = "Calc Node";
        node.rect = NodeRect;

        NodeInput.Create(node, "x", TypeOf.Float);
        NodeInput.Create(node, "y", TypeOf.Float);

        NodeOutput.Create(node, "out", TypeOf.Float);

        node.InitBase();
        return node;
    }

    public override void SideGUI()
    {
        if (Inputs[0].connection != null)
            GUILayout.Label(Inputs[0].name);
        else
            Input1Val = EditorGUILayout.FloatField(Input1Val);
        // --
        if (Inputs[1].connection != null)
            GUILayout.Label(Inputs[1].name);
        else
            Input2Val = EditorGUILayout.FloatField(Input2Val);

        type = (CalcType)EditorGUILayout.EnumPopup(new GUIContent("Calculation Type", "The type of calculation performed on Input 1 and Input 2"), type);

        //  if (GUI.changed)
        //  	Node_Editor.editor.RecalculateFrom (this);
    }

    public override bool Calculate()
    {
        //  if (Inputs [0].connection != null && Inputs [0].connection.value != null) 
        //  	Input1Val = (float)Inputs [0].connection.value;
        //  if (Inputs [1].connection != null && Inputs [1].connection.value != null) 
        //  	Input2Val = (float)Inputs [1].connection.value;
        if (allInputsReady())
        {
			Input1Val = (float)Receive(InputPort("x"));
			Input2Val = (float)Receive(InputPort("y"));
			
            switch (type)
            {
                case CalcType.Add:
                    Send(Input1Val + Input2Val, Outputs[0]);
                    break;
                case CalcType.Substract:
                    Send(Input1Val - Input2Val, Outputs[0]);
                    break;
                case CalcType.Multiply:
                    Send(Input1Val * Input2Val, Outputs[0]);
                    break;
                case CalcType.Divide:
                    Send(Input1Val / Input2Val, Outputs[0]);
                    break;
            }
        }



        return true;
    }
}
