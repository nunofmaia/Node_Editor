using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class ComposedNode : Node
{
    public Node_Canvas_Object canvas;

    void CopyNodes(List<Node> nodes)
    {
        nodes.ForEach(n => this.Nodes.Add(n.Clone()));
    }

    public sealed override void AddInput(NodeInput input) { }
    public sealed override void AddOutput(NodeOutput output) { }

    protected void Init()
    {
        InitBase();
        InitPorts();
    }
    
    protected void LoadCanvas(string name)
    {
        string path = "Assets/Node_Editor/Saves/" + name + ".asset";
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath (path);
		if (objects.Length == 0)
        {
            return;
        }
		Node_Canvas_Object newNodeCanvas = null;
		
		for (int cnt = 0; cnt < objects.Length; cnt++) 
		{ // We only have to search for the Node Canvas itself in the mess, because it still hold references to all of it's nodes and their connections
			object obj = objects [cnt];
			if (obj.GetType () == typeof (Node_Canvas_Object)) 
				newNodeCanvas = obj as Node_Canvas_Object;
		}
		if (newNodeCanvas == null)
        {
            return;
        }

		canvas = newNodeCanvas;
    }

    protected void InitPorts()
    {
        if (canvas != null)
        {
            CopyNodes(canvas.nodes);
            foreach (var node in Nodes)
            {
                List<NodeInput> inputs = new List<NodeInput>();
                foreach (var input in node.Inputs)
                {
                    if (input.connection == null)
                    {
                        NodeInput inp = Object.Instantiate(input) as NodeInput;
                        inp.body = this;
                        inp.name = input.name;
                        Inputs.Add(inp);
                        inputs.Add(input);
                    }
                }
                
                inputs.ForEach(i => node.Inputs.Remove(i));
                Inputs.ForEach(i => node.Inputs.Add(i));
                
                List<NodeOutput> outputs = new List<NodeOutput>();
                foreach (var output in node.Outputs)
                {
                    if (output.connections.Count == 0)
                    {
                        NodeOutput outp = Object.Instantiate(output) as NodeOutput;
                        outp.body = this;
                        outp.name = output.name;
                        Outputs.Add(outp);
                        outputs.Add(output);
                    }
                }
                
                outputs.ForEach(o => node.Outputs.Remove(o));
                Outputs.ForEach(o => node.Outputs.Add(o));
            }
        }
    }
}
