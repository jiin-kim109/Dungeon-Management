/* Written by Kaz Crowe */
/* SimpleHealthBarEditor.cs */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

[CanEditMultipleObjects]
[CustomEditor( typeof( global::SimpleHealthBar ) )]
public class SimpleHealthBarEditor : Editor
{
	SimpleHealthBar targ;

	// ----->>> IMAGE //
	AnimBool ImageAssigned, ImageUnassigned;
	AnimBool ImageFilledWarning;
	SerializedProperty barImage;

	// ----->>> COLOR //
	AnimBool ImageColorWarning;
	SerializedProperty colorMode, barColor, barGradient;

	// ----->>> TEXT //
	AnimBool DisplayTextOption;
	Color textColor;
	SerializedProperty barText;
	SerializedProperty displayText, additionalText;

	// ----->>> SMOOTH FILL //
	AnimBool SmoothFillOption;
	SerializedProperty smoothFill, smoothFillDuration;

	// DRAMATIC FILL //
	AnimBool DramaticFillOption, DramImageUnassigned, DramImageAssigned;
	AnimBool DramOptionDecrease;
	SerializedProperty dramaticStatusImage, dramaticStatusColor;
	SerializedProperty dramaticFill, secondsDelay, resetSensitivity, fillSpeed;

	// ----->>> FILL CONSTRAINT //
	AnimBool FillConstraintOption;
	SerializedProperty fillConstraint, fillConstraintMin, fillConstraintMax;

	// ---- < SCRIPT REFERENCE > ---- //
	AnimBool DuplicateBarName;
	AnimBool ExampleCode;
	SerializedProperty barName;
	enum FunctionList
	{
		GetUltimateHealthBar,
		UpdateBar,
		UpdateColor,
		UpdateTextColor
	}
	FunctionList functionList = FunctionList.UpdateBar;
	string exampleCode;

	// ----->>> TEST VALUE //
	float testValue = 100.0f;


	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();

		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;

		if( targ != null && targ.barImage != null )
		{
			float tempFloat = 0.0f;
			if( targ.fillConstraint == true )
				tempFloat = ( targ.barImage.fillAmount - targ.fillConstraintMin ) / ( targ.fillConstraintMax - targ.fillConstraintMin );
			else
				tempFloat = targ.barImage.fillAmount;

			testValue = tempFloat * 100.0f;
		}
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

		// ----- < BAR IMAGE > ----- //
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( barImage, new GUIContent( "Bar Image", "The image component to be used for this bar." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			if( targ.barImage != null && targ.barImage.type != Image.Type.Filled )
			{
				targ.barImage.type = Image.Type.Filled;
				targ.barImage.fillMethod = Image.FillMethod.Horizontal;
				EditorUtility.SetDirty( targ.barImage );
			}
			if( targ.barImage != null )
			{
				barColor.colorValue = targ.barImage.color;
				serializedObject.ApplyModifiedProperties();
			}
			targ.UpdateBar( testValue, 100.0f );

			ImageFilledWarning.target = GetBarImageWarning();
			ImageAssigned.target = GetImageAssigned();
			ImageUnassigned.target = GetImageUnassigned();
		}

		if( EditorGUILayout.BeginFadeGroup( ImageUnassigned.faded ) )
		{
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.HelpBox( "Image is unassigned.", MessageType.Warning );
			if( GUILayout.Button( "Find", EditorStyles.miniButton ) )
			{
				barImage.objectReferenceValue = targ.GetComponent<Image>();
				serializedObject.ApplyModifiedProperties();
				if( targ.barImage != null )
				{
					if( targ.barImage.type != Image.Type.Filled )
					{
						targ.barImage.type = Image.Type.Filled;
						targ.barImage.fillMethod = Image.FillMethod.Horizontal;
						EditorUtility.SetDirty( targ.barImage );
					}

					barColor.colorValue = targ.barImage.color;
					serializedObject.ApplyModifiedProperties();
				}

				ImageFilledWarning.target = GetBarImageWarning();
				ImageAssigned.target = GetImageAssigned();
				ImageUnassigned.target = GetImageUnassigned();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		// ----- < END BAR IMAGE > ----- //

		if( EditorGUILayout.BeginFadeGroup( ImageAssigned.faded ) )
		{
			// ----- < BAR IMAGE ERROR > ----- //
			if( EditorGUILayout.BeginFadeGroup( ImageFilledWarning.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "Invalid Image Type: " + targ.barImage.type.ToString(), MessageType.Warning );
				if( GUILayout.Button( "Fix", EditorStyles.miniButton ) )
				{
					targ.barImage.type = Image.Type.Filled;
					EditorUtility.SetDirty( targ.barImage );

					ImageFilledWarning.target = GetBarImageWarning();
				}
				EditorGUILayout.EndVertical();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END BAR IMAGE ERROR > ----- //

			// ----- < BAR COLORS > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( colorMode, new GUIContent( "Color Mode", "The mode in which to display the color to the barImage component." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				UpdateStatusColor();
				ImageColorWarning.target = GetColorWarning();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel = 1;
			if( targ.colorMode == SimpleHealthBar.ColorMode.Single )
				EditorGUILayout.PropertyField( barColor, new GUIContent( "Color", "The color of this barImage." ) );
			else
				EditorGUILayout.PropertyField( barGradient, new GUIContent( "Gradient", "The color gradient of this barImage." ) );
			EditorGUI.indentLevel = 0;
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				UpdateStatusColor();
				ImageColorWarning.target = GetColorWarning();
			}

			if( GetColorWarning() )
				ImageColorWarning.target = GetColorWarning();

			if( EditorGUILayout.BeginFadeGroup( ImageColorWarning.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "Image color has been modified incorrectly.", MessageType.Warning );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Update Image", EditorStyles.miniButtonLeft ) )
				{
					targ.barImage.color = barColor.colorValue;
					EditorUtility.SetDirty( targ.barImage );
					ImageColorWarning.target = GetColorWarning();
				}
				if( GUILayout.Button( "Update Script", EditorStyles.miniButtonRight ) )
				{
					barColor.colorValue = targ.barImage.color;
					serializedObject.ApplyModifiedProperties();
					ImageColorWarning.target = GetColorWarning();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END BAR COLORS > ----- //

			EditorGUILayout.Space();

			// ------- < TEXT OPTIONS > ------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( displayText, new GUIContent( "Display Text", "Determines how this bar will display text to the user." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DisplayTextOption.target = targ.displayText != SimpleHealthBar.DisplayText.Disabled;

				targ.UpdateBar( testValue, 100.0f );
				if( barText.objectReferenceValue != null )
					EditorUtility.SetDirty( targ.barText );
			}

			if( EditorGUILayout.BeginFadeGroup( DisplayTextOption.faded ) )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( barText, new GUIContent( "Bar Text", "The Text component to be used for the text." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateTextColor( textColor );
					targ.UpdateBar( testValue, 100.0f );
					if( barText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.BeginChangeCheck();
				textColor = EditorGUILayout.ColorField( new GUIContent( "Text Color", "The color of the Text component." ), textColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateTextColor( textColor );
					if( barText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( additionalText, new GUIContent( "Additional Text", "Additional text to be displayed before the current information." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateBar( testValue, 100.0f );
					if( barText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.indentLevel = 2;
				switch( targ.displayText )
				{
					case SimpleHealthBar.DisplayText.Percentage:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue + "%" );
					}
					break;
					case SimpleHealthBar.DisplayText.CurrentValue:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue );
					}
					break;
					case SimpleHealthBar.DisplayText.CurrentAndMaxValues:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue + " / 100" );
					}
					break;
					default:
					{
						EditorGUILayout.LabelField( "Text Preview: Default" );
					}
					break;
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END TEXT OPTIONS > ----- //

			// ----- < SMOOTH FILL OPTIONS > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( smoothFill, new GUIContent( "Smooth Fill", "Determines if the fill amount should be applied over time or not." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SmoothFillOption.target = targ.smoothFill;
			}
			if( EditorGUILayout.BeginFadeGroup( SmoothFillOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( smoothFillDuration, new GUIContent( "Fill Duration", "Determines how long it takes for the fill amount to reach the target fill amount." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( smoothFillDuration.floatValue < 0 )
						smoothFillDuration.floatValue = 0;

					serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END SMOOTH FILL OPTIONS > ----- //

			// DRAMATIC FILL //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( dramaticFill, new GUIContent( "Dramatic Fill", "Determines whether an image should be used for a dramatic fill or not." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DramaticFillOption.target = targ.dramaticFill != SimpleHealthBar.DramaticFill.Disabled;
				if( targ.dramaticFill != SimpleHealthBar.DramaticFill.Disabled )
					DramOptionDecrease.target = GetDramaticFillDecrease();

				if( targ.dramaticFill == SimpleHealthBar.DramaticFill.Decrease && targ.smoothFill == true )
				{
					smoothFill.boolValue = false;
					serializedObject.ApplyModifiedProperties();
					SmoothFillOption.target = targ.smoothFill;
				}
				if( targ.dramaticFill == SimpleHealthBar.DramaticFill.Increase && targ.smoothFill == false )
				{
					smoothFill.boolValue = true;
					serializedObject.ApplyModifiedProperties();
					SmoothFillOption.target = targ.smoothFill;
				}
				if( targ.dramaticFill == SimpleHealthBar.DramaticFill.Disabled && targ.dramaticImage != null )
					targ.dramaticImage.gameObject.SetActive( false );
				else if( targ.dramaticFill != SimpleHealthBar.DramaticFill.Disabled && targ.dramaticImage != null )
					targ.dramaticImage.gameObject.SetActive( true );
			}
			if( EditorGUILayout.BeginFadeGroup( DramaticFillOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( dramaticStatusImage, new GUIContent( "Image", "The image component to use for the dramatic fill." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					DramImageUnassigned.target = targ.dramaticImage == null;
					DramImageAssigned.target = targ.dramaticImage != null;
					targ.dramaticImage.color = targ.dramaticColor;
					EditorUtility.SetDirty( targ.dramaticImage );
				}

				if( EditorGUILayout.BeginFadeGroup( DramImageUnassigned.faded ) )
				{
					EditorGUILayout.BeginVertical( "Box" );
					EditorGUI.indentLevel = 0;
					EditorGUILayout.HelpBox( "The Dramatic Fill option uses a separate barImage to display under the barImage. Click the button below to create the needed image.", MessageType.None );
					if( GUILayout.Button( "Generate Image", EditorStyles.miniButton ) )
					{
						GameObject obj = new GameObject( targ != null && targ.barName != string.Empty ? "Dramatic " + targ.barName + " Fill" : "Dramatic Fill" );
						obj.layer = LayerMask.NameToLayer( "UI" );
						obj.AddComponent<CanvasRenderer>();
						obj.transform.SetParent( targ.gameObject.transform.parent );
						obj.transform.SetSiblingIndex( targ.transform.GetSiblingIndex() );

						Image img = obj.AddComponent<Image>();
						if( targ != null && targ.barImage != null && targ.barImage.sprite != null )
							img.sprite = targ.barImage.sprite;
						img.type = Image.Type.Filled;
						img.fillMethod = Image.FillMethod.Horizontal;

						RectTransform rt = obj.GetComponent<RectTransform>();
						rt.anchorMax = new Vector2( 1, 1 );
						rt.anchorMin = new Vector2( 0, 0 );
						rt.offsetMin = new Vector2( 0, 0 );
						rt.offsetMax = new Vector2( 0, 0 );

						EditorUtility.SetDirty( obj );

						targ.dramaticImage = img;
						EditorUtility.SetDirty( targ );

						DramImageUnassigned.target = targ.dramaticImage == null;
						DramImageAssigned.target = targ.dramaticImage != null;

						targ.dramaticImage.color = targ.dramaticColor;
						EditorUtility.SetDirty( targ.dramaticImage );
					}
					EditorGUI.indentLevel = 1;
					EditorGUILayout.EndVertical();
				}
				if( ImageAssigned.faded == 1.0f && DramaticFillOption.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();

				if( EditorGUILayout.BeginFadeGroup( DramImageAssigned.faded ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( dramaticStatusColor, new GUIContent( "Color", "The color to apply to the dramatic fill image." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();
						targ.dramaticImage.color = targ.dramaticColor;
						EditorUtility.SetDirty( targ.dramaticImage );
					}

					if( EditorGUILayout.BeginFadeGroup( DramOptionDecrease.faded ) )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( secondsDelay, new GUIContent( "Delay", "Tooltip" ) );
						if( EditorGUI.EndChangeCheck() )
						{
							if( secondsDelay.floatValue < 0 )
								secondsDelay.floatValue = 0;

							serializedObject.ApplyModifiedProperties();
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( resetSensitivity, 0.0f, 1.0f, new GUIContent( "Reset Sensitivity", "Tooltip" ) );
						EditorGUILayout.Slider( fillSpeed, 0.0f, 1.0f, new GUIContent( "Fill Speed", "Amount of fill per second." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
					if( ImageAssigned.faded == 1.0f && DramaticFillOption.faded == 1.0f && DramImageAssigned.faded == 1.0f )
						EditorGUILayout.EndFadeGroup();
				}
				if( ImageAssigned.faded == 1.0f && DramaticFillOption.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();

				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// END DRAMATIC FILL //

			// ----- < FILL CONSTRAINT > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( fillConstraint, new GUIContent( "Fill Constraint", "Determines whether or not the barImage fill should be constrained." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				FillConstraintOption.target = targ.fillConstraint;
			}

			if( EditorGUILayout.BeginFadeGroup( FillConstraintOption.faded ) )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( fillConstraintMin, 0.0f, targ.fillConstraintMax, new GUIContent( "Fill Minimum", "The minimum fill amount." ) );
				EditorGUILayout.Slider( fillConstraintMax, targ.fillConstraintMin, 1.0f, new GUIContent( "Fill Maximum", "The maximum fill amount." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					if( targ.barImage != null )
					{
						targ.barImage.enabled = false;
						targ.UpdateBar( testValue, 100.0f );
						targ.barImage.enabled = true;
					}
				}

				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// --- < END FILL CONSTRAINT > --- //

			// ----- < TEST VALUE > ----- //
			EditorGUI.BeginChangeCheck();
			testValue = EditorGUILayout.Slider( new GUIContent( "Test Value" ), testValue, 0.0f, 100.0f );
			if( EditorGUI.EndChangeCheck() )
			{
				if( targ.barImage != null )
				{
					targ.barImage.enabled = false;
					targ.UpdateBar( testValue, 100.0f );
					targ.barImage.enabled = true;

					EditorUtility.SetDirty( targ.barImage );
				}
			}
			// ----- < END TEST VALUE > ----- //
		}
		EditorGUILayout.EndFadeGroup();

		EditorGUILayout.Space();

		Repaint();
	}

	void StoreReferences ()
	{
		targ = ( SimpleHealthBar ) target;

		ImageAssigned = new AnimBool( GetImageAssigned() );
		ImageUnassigned = new AnimBool( GetImageUnassigned() );
		ImageFilledWarning = new AnimBool( GetBarImageWarning() );
		barImage = serializedObject.FindProperty( "barImage" );

		// ----->>> COLOR //
		ImageColorWarning = new AnimBool( GetColorWarning() );
		colorMode = serializedObject.FindProperty( "colorMode" );
		barColor = serializedObject.FindProperty( "barColor" );
		barGradient = serializedObject.FindProperty( "barGradient" );

		// ----->>> TEXT //
		DisplayTextOption = new AnimBool( targ.displayText != SimpleHealthBar.DisplayText.Disabled );
		textColor = targ.barText != null ? targ.barText.color : Color.white;
		barText = serializedObject.FindProperty( "barText" );
		displayText = serializedObject.FindProperty( "displayText" );
		additionalText = serializedObject.FindProperty( "additionalText" );

		// ----->>> SMOOTH FILL //
		SmoothFillOption = new AnimBool( targ.smoothFill );
		smoothFill = serializedObject.FindProperty( "smoothFill" );
		smoothFillDuration = serializedObject.FindProperty( "smoothFillDuration" );

		// DRAMATIC FILL //
		DramaticFillOption = new AnimBool( targ.dramaticFill != SimpleHealthBar.DramaticFill.Disabled );
		DramImageUnassigned = new AnimBool( targ.dramaticImage == null );
		DramImageAssigned = new AnimBool( targ.dramaticImage != null );
		DramOptionDecrease = new AnimBool( GetDramaticFillDecrease() );
		dramaticStatusImage = serializedObject.FindProperty( "dramaticImage" );
		dramaticStatusColor = serializedObject.FindProperty( "dramaticColor" );
		dramaticFill = serializedObject.FindProperty( "dramaticFill" );
		secondsDelay = serializedObject.FindProperty( "secondsDelay" );
		resetSensitivity = serializedObject.FindProperty( "resetSensitivity" );
		fillSpeed = serializedObject.FindProperty( "fillSpeed" );

		// ----->>> FILL CONSTRAINT //
		FillConstraintOption = new AnimBool( targ.fillConstraint );
		fillConstraint = serializedObject.FindProperty( "fillConstraint" );
		fillConstraintMin = serializedObject.FindProperty( "fillConstraintMin" );
		fillConstraintMax = serializedObject.FindProperty( "fillConstraintMax" );

		// ---- < SCRIPT REFERENCE > ---- //
		DuplicateBarName = new AnimBool( GetDuplicateBarName() );
		ExampleCode = new AnimBool( targ.barName != string.Empty && !GetDuplicateBarName() );
		barName = serializedObject.FindProperty( "barName" );

		GenerateExampleCode();
	}

	void UpdateStatusColor ()
	{
		// If the image component is null, then return.
		if( targ.barImage == null )
			return;

		// Switch statement for the color mode option. Each case handles the color according to the option.
		switch( targ.colorMode )
		{
			case SimpleHealthBar.ColorMode.Single:
			{
				targ.barImage.color = targ.barColor;
			} break;
			case SimpleHealthBar.ColorMode.Gradient:
			{
				targ.barImage.color = targ.barGradient.Evaluate( targ.GetCurrentFraction );
			} break;
		}
		EditorUtility.SetDirty( targ.barImage );
	}

	bool GetImageAssigned ()
	{
		if( targ.barImage != null )
			return true;
		return false;
	}

	bool GetImageUnassigned ()
	{
		if( targ.barImage == null )
			return true;
		return false;
	}

	bool GetBarImageWarning ()
	{
		if( targ.barImage != null && targ.barImage.type != Image.Type.Filled )
			return true;
		return false;
	}

	bool GetColorWarning ()
	{
		if( Application.isPlaying == true )
			return false;

		if( targ.barImage == null )
			return false;

		if( targ.colorMode == SimpleHealthBar.ColorMode.Single && targ.barImage.color != targ.barColor )
			return true;

		return false;
	}
	
	bool GetDramaticFillDecrease ()
	{
		return targ.dramaticFill == SimpleHealthBar.DramaticFill.Decrease;
	}

	bool GetDuplicateBarName ()
	{
		SimpleHealthBar[] allStatusBars = FindObjectsOfType<SimpleHealthBar>();
		for( int i = 0; i < allStatusBars.Length; i++ )
		{
			if( targ != allStatusBars[ i ] && targ.barName == allStatusBars[ i ].barName && allStatusBars[ i ].barName != string.Empty )
				return true;
		}

		return false;
	}

	void GenerateExampleCode ()
	{
		switch( functionList )
		{
			default:
			case FunctionList.GetUltimateHealthBar:
			{
				exampleCode = "SimpleHealthBar.GetSimpleHealthBar( \"" + targ.barName + "\" );";
			}
			break;
			case FunctionList.UpdateBar:
			{
				exampleCode = "SimpleHealthBar.UpdateBar( \"" + targ.barName + "\", current, max );";
			}
			break;
			case FunctionList.UpdateColor:
			{
				exampleCode = "SimpleHealthBar.UpdateColor( \"" + targ.barName + "\", newColor );";
			}
			break;
			case FunctionList.UpdateTextColor:
			{
				exampleCode = "SimpleHealthBar.UpdateTextColor( \"" + targ.barName + "\", newTextColor );";
			}
			break;
		}
	}
}