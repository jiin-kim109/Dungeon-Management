/* Written by Kaz Crowe */
/* FollowCameraRotationEditor.cs */
using UnityEngine;
using UnityEditor;
using SimpleHealthBarUtility;
using UnityEditor.AnimatedValues;

[CustomEditor( typeof( FollowCameraRotation ) )]
public class FollowCameraRotationEditor : Editor
{
	FollowCameraRotation targ;

	AnimBool FindByTransform, FindByName, FindByTag;
	SerializedProperty findBy;
	SerializedProperty targetName;
	SerializedProperty cameraTransform;


	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();

		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	// Function called for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( findBy, new GUIContent( "Find Camera By", "Determines how the script will find the camera transform component." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			FindByTransform.target = targ.findBy == FollowCameraRotation.FindBy.Transform;
			FindByName.target = targ.findBy == FollowCameraRotation.FindBy.Name;
			FindByTag.target = targ.findBy == FollowCameraRotation.FindBy.Tag;
		}

		if( EditorGUILayout.BeginFadeGroup( FindByTransform.faded ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( cameraTransform, new GUIContent( "Camera Transform", "The camera's Transform component." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();

		if( EditorGUILayout.BeginFadeGroup( FindByName.faded ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( targetName, new GUIContent( "Camera Name", "The name of the camera in the scene." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();

		if( EditorGUILayout.BeginFadeGroup( FindByTag.faded ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( targetName, new GUIContent( "Camera Tag", "The tag of the camera in the scene." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();

		EditorGUILayout.Space();

		Repaint();
	}

	void StoreReferences ()
	{
		targ = ( FollowCameraRotation )target;

		FindByTransform = new AnimBool( targ.findBy == FollowCameraRotation.FindBy.Transform );
		FindByName = new AnimBool( targ.findBy == FollowCameraRotation.FindBy.Name );
		FindByTag = new AnimBool( targ.findBy == FollowCameraRotation.FindBy.Tag );

		findBy = serializedObject.FindProperty( "findBy" );
		targetName = serializedObject.FindProperty( "targetName" );
		cameraTransform = serializedObject.FindProperty( "cameraTransform" );
	}
}