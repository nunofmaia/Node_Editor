using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static class ConnectionTypes
{
	// Static consistent information about types
	public static Dictionary<string, TypeData> types = new Dictionary<string, TypeData> ();

	/// <summary>
	/// Fetches every Type Declaration in the assembly
	/// </summary>
	public static void FetchTypes () 
	{ // Search the current and (if the NodeEditor is packed into a .dll) the calling one
		Assembly assembly = Assembly.GetExecutingAssembly ();
		foreach (Type type in assembly.GetTypes ().Where (T => T.IsClass && !T.IsAbstract && T.GetInterfaces ().Contains (typeof (ITypeDeclaration)))) 
		{
			ITypeDeclaration typeDecl = assembly.CreateInstance (type.Name) as ITypeDeclaration;
			Texture2D InputKnob = UnityEditor.AssetDatabase.LoadAssetAtPath (NodeEditor.editorPath + typeDecl.InputKnob_TexPath, typeof(Texture2D)) as Texture2D;
			Texture2D OutputKnob = UnityEditor.AssetDatabase.LoadAssetAtPath (NodeEditor.editorPath + typeDecl.OutputKnob_TexPath, typeof(Texture2D)) as Texture2D;
			types.Add (typeDecl.name, new TypeData (typeDecl.col, InputKnob, OutputKnob));
		}

		if (assembly != Assembly.GetCallingAssembly ())
		{
			assembly = Assembly.GetCallingAssembly ();
			foreach (Type type in assembly.GetTypes ().Where (T => T.IsClass && !T.IsAbstract && T.GetInterfaces ().Contains (typeof (ITypeDeclaration)))) 
			{
				ITypeDeclaration typeDecl = assembly.CreateInstance (type.Name) as ITypeDeclaration;
				Texture2D InputKnob = UnityEditor.AssetDatabase.LoadAssetAtPath (NodeEditor.editorPath + typeDecl.InputKnob_TexPath, typeof(Texture2D)) as Texture2D;
				Texture2D OutputKnob = UnityEditor.AssetDatabase.LoadAssetAtPath (NodeEditor.editorPath + typeDecl.OutputKnob_TexPath, typeof(Texture2D)) as Texture2D;
				types.Add (typeDecl.name, new TypeData (typeDecl.col, InputKnob, OutputKnob));
			}
		}
	}
}

public struct TypeData 
{
	public Color col;
	public Texture2D InputKnob;
	public Texture2D OutputKnob;
	
	public TypeData (Color color, Texture2D inKnob, Texture2D outKnob) 
	{
		col = color;
		InputKnob = NodeEditor.Tint (inKnob, color);
		OutputKnob = NodeEditor.Tint (outKnob, color);
	}
}

public interface ITypeDeclaration
{
	string name { get; }
	Color col { get; }
	string InputKnob_TexPath { get; }
	string OutputKnob_TexPath { get; }
}

// TODO: Node Editor: Built-In Connection Types
public class FloatType : ITypeDeclaration 
{
	public string name { get { return "Float"; } }
	public Color col { get { return Color.cyan; } }
	public string InputKnob_TexPath { get { return "Textures/handle.png"; } }
	public string OutputKnob_TexPath { get { return "Textures/handle.png"; } }
}

public class StructType : ITypeDeclaration 
{
	public string name { get { return "Struct"; } }
	public Color col { get { return Color.green; } }
	public string InputKnob_TexPath { get { return "Textures/handle.png"; } }
	public string OutputKnob_TexPath { get { return "Textures/handle.png"; } }
}