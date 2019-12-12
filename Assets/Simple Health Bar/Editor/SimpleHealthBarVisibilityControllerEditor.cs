/* Written by Kaz Crowe */
/* SimpleHealthBarVisibilityControllerEditor.cs */
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

[CustomEditor( typeof( SimpleHealthBarVisibilityController ) )]
public class SimpleHealthBarVisibilityControllerEditor : Editor
{
	SimpleHealthBarVisibilityController targ;

	List<string> names = new List<string>();
	List<SerializedProperty> keepVisible = new List<SerializedProperty>();
	List<SerializedProperty> triggerValue = new List<SerializedProperty>();
	List<float> testValue = new List<float>();

	AnimBool UpdateVisibilityOnActivity;
	AnimBool ShowChildHealthBars;
	SerializedProperty updateVisibility;

	SerializedProperty idleSeconds;
	SerializedProperty enableDuration;
	SerializedProperty disableDuration;
	SerializedProperty enabledAlpha;
	SerializedProperty disabledAlpha;
	SerializedProperty initialState, barName;
	AnimBool ExampleCode, DuplicateBarName;
	enum FunctionList
	{
		EnableVisibility,
		DisableVisibility
	}
	FunctionList functionList = FunctionList.EnableVisibility;
	string exampleCode;


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

		EditorGUILayout.BeginVertical( "Box" );

		// ----- < BAR NAME > ----- //
		if( barName.stringValue == string.Empty && Event.current.type == EventType.Repaint )
		{
			GUIStyle style = new GUIStyle( GUI.skin.textField );
			style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
			EditorGUILayout.TextField( new GUIContent( "Bar Name", "The unique name to be used in reference to this bar." ), "Bar Name", style );
		}
		else
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( barName, new GUIContent( "Bar Name", "The unique name to be used in reference to this bar." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DuplicateBarName.target = GetDuplicateBarName();
				ExampleCode.target = targ.barName != string.Empty && !GetDuplicateBarName();
				GenerateExampleCode();
			}
		}
		// ----- < END BAR NAME > ----- //

		// ----- < NAME ERRORS > ----- //
		if( EditorGUILayout.BeginFadeGroup( DuplicateBarName.faded ) )
			EditorGUILayout.HelpBox( "The bar name \"" + targ.barName + "\" is already being used in this scene. Please make each Ultimate Health Bar has a unique name.", MessageType.Warning );
		EditorGUILayout.EndFadeGroup();
		
		if( EditorGUILayout.BeginFadeGroup( ExampleCode.faded ) )
		{
			GUILayout.Space( 1 );
			EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );
			EditorGUI.BeginChangeCheck();
			functionList = ( FunctionList )EditorGUILayout.EnumPopup( "Function", functionList );
			if( EditorGUI.EndChangeCheck() )
				GenerateExampleCode();

			EditorGUILayout.TextField( exampleCode );

			GUILayout.Space( 1 );
		}
		EditorGUILayout.EndFadeGroup();
		// ----- < END NAME ERRORS > ----- //

		EditorGUILayout.EndVertical();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( updateVisibility, new GUIContent( "Update Visibility", "Determines how the script calculates the visibility of the Ultimate Health Bars." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			UpdateVisibilityOnActivity.target = GetUpdateVisibilityOnStatusUpdated();
			ShowChildHealthBars.target = GetUpdateVisibilityOnStatusUpdated();
		}

		if( EditorGUILayout.BeginFadeGroup( UpdateVisibilityOnActivity.faded ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( idleSeconds, new GUIContent( "Idle Seconds", "Time in seconds to wait before fading out." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( idleSeconds.floatValue < 0 )
					idleSeconds.floatValue = 0;

				serializedObject.ApplyModifiedProperties();
			}
		}
		EditorGUILayout.EndFadeGroup();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( initialState, new GUIContent( "Initial State", "The initial state of the visibility." ) );
		EditorGUILayout.PropertyField( enableDuration, new GUIContent( "Enable Duration", "Time in seconds before the alpha reaches full enabled." ) );
		EditorGUILayout.PropertyField( disableDuration, new GUIContent( "Disable Duration", "Time in seconds before the alpha reaches full disabled." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			if( enableDuration.floatValue < 0 )
				enableDuration.floatValue = 0;

			if( disableDuration.floatValue < 0 )
				disableDuration.floatValue = 0;

			serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.Slider( enabledAlpha, 0.0f, 1.0f, new GUIContent( "Enabled Alpha", "The target alpha when the visibility is enabled." ) );
		EditorGUILayout.Slider( disabledAlpha, 0.0f, 1.0f, new GUIContent( "Disabled Alpha", "The target alpha when the visibility is disabled." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			targ.GetComponent<CanvasGroup>().alpha = enabledAlpha.floatValue;
			EditorUtility.SetDirty( targ.gameObject );
		}

		EditorGUILayout.Space();

		if( EditorGUILayout.BeginFadeGroup( UpdateVisibilityOnActivity.faded ) )
		{
			EditorGUILayout.BeginVertical( "Toolbar" );
			EditorGUILayout.LabelField( "Simple Health Bars", EditorStyles.boldLabel );
			EditorGUILayout.EndVertical();

			for( int i = 0; i < targ.healthBarInformation.Count; i++ )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( names[ i ], EditorStyles.boldLabel );
				EditorGUI.BeginDisabledGroup( Application.isPlaying );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( keepVisible[ i ] );
				if( keepVisible[ i ].boolValue == true )
					EditorGUILayout.Slider( triggerValue[ i ], 0.0f, 1.0f );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginChangeCheck();
				testValue[ i ] = EditorGUILayout.Slider( new GUIContent( "Test Value", "The test value for the Ultimate Health Bar." ), testValue[ i ], 0.0f, 100.0f );
				if( EditorGUI.EndChangeCheck() )
				{
					if( targ.healthBarInformation[ i ].healthBar.barImage != null )
					{
						targ.healthBarInformation[ i ].healthBar.barImage.enabled = false;
						targ.healthBarInformation[ i ].healthBar.UpdateBar( testValue[ i ], 100 );
						targ.healthBarInformation[ i ].healthBar.barImage.enabled = true;

						EditorUtility.SetDirty( targ.healthBarInformation[ i ].healthBar.barImage );
					}
				}

				EditorGUILayout.EndVertical();
			}
		}
		EditorGUILayout.EndFadeGroup();

		EditorGUILayout.Space();

		Repaint();
	}

	void StoreReferences ()
	{
		targ = ( SimpleHealthBarVisibilityController )target;

		FindUltimateHealthBars();

		keepVisible = new List<SerializedProperty>();
		triggerValue = new List<SerializedProperty>();
		for( int i = 0; i < targ.healthBarInformation.Count; i++ )
		{
			names.Add( targ.healthBarInformation[ i ].healthBar.gameObject.name );
			keepVisible.Add( serializedObject.FindProperty( string.Format( "healthBarInformation.Array.data[{0}].keepVisible", i ) ) );
			triggerValue.Add( serializedObject.FindProperty( string.Format( "healthBarInformation.Array.data[{0}].triggerValue", i ) ) );
			testValue.Add( targ.healthBarInformation[ i ].healthBar.GetCurrentCalculatedFraction * 100 );
		}

		UpdateVisibilityOnActivity = new AnimBool( GetUpdateVisibilityOnStatusUpdated() );
		ShowChildHealthBars = new AnimBool( GetUpdateVisibilityOnStatusUpdated() );
		updateVisibility = serializedObject.FindProperty( "updateVisibility" );
		idleSeconds = serializedObject.FindProperty( "idleSeconds" );
		enableDuration = serializedObject.FindProperty( "enableDuration" );
		disableDuration = serializedObject.FindProperty( "disableDuration" );
		enabledAlpha = serializedObject.FindProperty( "enabledAlpha" );
		disabledAlpha = serializedObject.FindProperty( "disabledAlpha" );
		initialState = serializedObject.FindProperty( "initialState" );
		barName = serializedObject.FindProperty( "barName" );

		DuplicateBarName = new AnimBool( GetDuplicateBarName() );
		ExampleCode = new AnimBool( targ.barName != string.Empty && !GetDuplicateBarName() );

		GenerateExampleCode();

		if( !targ.GetComponent<CanvasGroup>() )
			targ.gameObject.AddComponent<CanvasGroup>();
	}

	void FindUltimateHealthBars ()
	{
		SimpleHealthBar[] healthBars;
		healthBars = targ.GetComponentsInChildren<SimpleHealthBar>();

		List<SimpleHealthBarVisibilityController.HealthBarInformation> unusedList = new List<SimpleHealthBarVisibilityController.HealthBarInformation>();
		for( int i = 0; i < targ.healthBarInformation.Count; i++ )
			unusedList.Add( targ.healthBarInformation[ i ] );

		for( int i = 0; i < healthBars.Length; i++ )
		{
			bool newHealthBar = true;
			for( int n = 0; n < targ.healthBarInformation.Count; n++ )
			{
				if( healthBars[ i ] == targ.healthBarInformation[ n ].healthBar )
				{
					newHealthBar = false;
					unusedList.Remove( targ.healthBarInformation[ n ] );
				}
			}
			if( newHealthBar )
			{
				SimpleHealthBarVisibilityController.HealthBarInformation tempHealthBar = new SimpleHealthBarVisibilityController.HealthBarInformation();
				tempHealthBar.healthBar = healthBars[ i ];
				targ.healthBarInformation.Add( tempHealthBar );
			}
		}
		
		for( int i = 0; i < unusedList.Count; i++ )
			targ.healthBarInformation.Remove( unusedList[ i ] );
	}

	bool GetUpdateVisibilityOnStatusUpdated ()
	{
		return targ.updateVisibility == SimpleHealthBarVisibilityController.UpdateVisibility.OnActivity;
	}

	bool GetDuplicateBarName ()
	{
		SimpleHealthBarVisibilityController[] allControllers = FindObjectsOfType<SimpleHealthBarVisibilityController>();
		for( int i = 0; i < allControllers.Length; i++ )
		{
			if( targ != allControllers[ i ] && targ.barName == allControllers[ i ].barName && allControllers[ i ].barName != string.Empty )
				return true;
		}

		return false;
	}

	void GenerateExampleCode ()
	{
		switch( functionList )
		{
			default:
			case FunctionList.EnableVisibility:
			{
				exampleCode = "SimpleHealthBarVisibilityController.EnableVisibility( \"" + targ.barName + "\" );";
			}
			break;
			case FunctionList.DisableVisibility:
			{
				exampleCode = "SimpleHealthBarVisibilityController.DisableVisibility( \"" + targ.barName + "\" );";
			}
			break;
		}
	}
}