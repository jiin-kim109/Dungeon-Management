/* Written by Kaz Crowe */
/* SimpleHealthBarWindow.cs */
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

public class SimpleHealthBarWindow : EditorWindow
{
	static string version = "1.0.1f";// ALWAYS UDPATE
	static int importantChanges = 0;// UPDATE ON IMPORTANT CHANGES
	static string menuTitle = "Main Menu";

	// LAYOUT STYLES //
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();

	GUILayoutOption[] buttonSize = new GUILayoutOption[] { GUILayout.Width( 200 ), GUILayout.Height( 35 ) }; 
	GUILayoutOption[] docSize = new GUILayoutOption[] { GUILayout.Width( 300 ), GUILayout.Height( 330 ) };
	GUISkin style;
	Texture2D scriptReference;
	Texture2D ujPromo, ubPromo;

	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Main Menu" };
	static PageInformation howTo = new PageInformation() { pageName = "How To" };
	static PageInformation overview = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_SimpleHealthBar = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_VisibilityController = new PageInformation() { pageName = "Overview" };
	static PageInformation overview_FollowCameraRotation = new PageInformation() { pageName = "Overview" };
	static PageInformation documentation = new PageInformation() { pageName = "Documentation" };
	static PageInformation docs_SimpleHealthBar = new PageInformation() { pageName = "Documentation" };
	static PageInformation docs_VisibilityController = new PageInformation() { pageName = "Documentation" };
	static PageInformation otherProducts = new PageInformation() { pageName = "Other Products" };
	static PageInformation feedback = new PageInformation() { pageName = "Feedback" };
	static PageInformation changeLog = new PageInformation() { pageName = "Change Log" };
	static PageInformation versionChanges = new PageInformation() { pageName = "Version Changes" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You" };
	static PageInformation settings = new PageInformation() { pageName = "Window Settings" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();

	enum FontSize
	{
		Small,
		Medium,
		Large
	}
	FontSize fontSize = FontSize.Small;
	bool configuredFontSize = false;

	
	class DocumentationInfo
	{
		public string functionName = "";
		public AnimBool showMore = new AnimBool( false );
		public string[] parameter;
		public string returnType = "";
		public string description = "";
		public string codeExample = "";
	}
	#region Simple Health Bar Documentation
	DocumentationInfo p_UpdateBar = new DocumentationInfo()
	{
		functionName = "UpdateBar()",
		description = "Updates the Simple Health Bar with the current and max values.",
		codeExample = "healthBar.UpdateBar( health, maxHealth );"
	};
	DocumentationInfo p_UpdateColor = new DocumentationInfo()
	{
		functionName = "UpdateColor()",
		showMore = new AnimBool( false ),
		parameter = new string[ 2 ]
		{
			"Color targetColor - The target color to apply to the image.",
			"Gradient targetGradient - The target gradient to apply to the image."
		},
		description = "Updates the image component with the Color parameter. This function also has an override to apply a new Gradient to the image as well.",
		codeExample = "healthBar.UpdateColor( Color.white );"
	};
	DocumentationInfo p_UpdateTextColor = new DocumentationInfo()
	{
		functionName = "UpdateTextColor()",
		showMore = new AnimBool( false ),
		parameter = new string[ 1 ]
		{
			"Color targetColor - The target color to apply to the text.",
		},
		description = "Updates the color of the text component.",
		codeExample = "healthBar.UpdateTextColor( Color.red );"
	};

	// STATIC //
	DocumentationInfo s_GetSimpleHealthBar = new DocumentationInfo()
	{
		functionName = "GetSimpleHealthBar()",
		showMore = new AnimBool( false ),
		parameter = new string[ 1 ]
		{
			"string barName - The name that the targeted Simple Health Bar has been registered with."
		},
		returnType = "SimpleHealthBar",
		description = "Returns the Simple Health Bar component that has been registered with the targeted status name.",
		codeExample = "SimpleHealthBar healthBar = SimpleHealthBar.GetSimpleHealthBar( \"Health\" );"
	};
	DocumentationInfo s_UpdateBar = new DocumentationInfo()
	{
		functionName = "UpdateBar()",
		showMore = new AnimBool( false ),
		parameter = new string[ 1 ]
		{
			"string barName - The name that the targeted Simple Health Bar has been registered with."
		},
		description = "Updates the targeted Simple Health Bar with the current and max value parameters.",
		codeExample = "SimpleHealthBar.UpdateBar( \"Health\", health, maxHealth );"
	};
	DocumentationInfo s_UpdateColor = new DocumentationInfo()
	{
		functionName = "UpdateColor()",
		showMore = new AnimBool( false ),
		parameter = new string[ 3 ]
		{
			"string barName - The name that the targeted Simple Health Bar has been registered with.",
			"Color targetColor - The target color to apply to the image.",
			"Gradient targetGradient - The target gradient to apply to the image."
		},
		description = "Updates the image component of the targeted Simple Health Bar. This function has an override so it's possible to update a Color or Gradient to the image.",
		codeExample = "SimpleHealthBar.UpdateColor( \"Health\", Color.white );"
	};
	DocumentationInfo s_UpdateTextColor = new DocumentationInfo()
	{
		functionName = "UpdateTextColor()",
		showMore = new AnimBool( false ),
		parameter = new string[ 2 ]
		{
			"string barName - The name that the targeted Simple Health Bar has been registered with.",
			"Color targetColor - The target color to apply to the text."
		},
		description = "Updates the color of the text component associated with the targeted Simple Health Bar.",
		codeExample = "SimpleHealthBar.UpdateTextColor( \"Health\", Color.red );"
	};
	#endregion

	#region Simple Health Bar Visibility Controller Documentation
	DocumentationInfo p_EnableVisibility = new DocumentationInfo()
	{
		functionName = "EnableVisibility()",
		description = "Enables the Simple Health Bars visually.",
		codeExample = "healthBarVisibility.EnableVisibility();"
	};
	DocumentationInfo p_DisableVisibility = new DocumentationInfo()
	{
		functionName = "DisableVisibility()",
		description = "Disables the Simple Health Bars visually.",
		codeExample = "healthBarVisibility.DisableVisibility();"
	};
	DocumentationInfo p_GetCurrentState = new DocumentationInfo()
	{
		functionName = "GetCurrentState()",
		returnType = "bool",
		description = "Returns the current state of visibility for the Simple Health Bars.",
		codeExample = "bool visibilityState = healthBarVisibility.GetCurrentState();"
	};
	// static
	DocumentationInfo s_GetSimpleHealthBarVisibilityController = new DocumentationInfo()
	{
		functionName = "GetSimpleHealthBarVisibilityController()",
		returnType = "SimpleHealthBarVisibilityController",
		parameter = new string[ 1 ]
		{
			"string name - The name that the targeted Simple Health Bar Visibility Controller has been registered with."
		},
		description = "Returns the Simple Health Bar Visibility Controller component that has been registered with the targeted name.",
		codeExample = "SimpleHealthBarVisibilityController.GetSimpleHealthBarVisibilityController( \"Health\" );"
	};
	DocumentationInfo s_EnableVisibility = new DocumentationInfo()
	{
		functionName = "EnableVisibility()",
		parameter = new string[ 1 ]
		{
			"string name - The name that the targeted Simple Health Bar Visibility Controller has been registered with."
		},
		description = "Enables the targeted Simple Health Bars visually.",
		codeExample = "SimpleHealthBarVisibilityController.EnableVisibility( \"Health\" );"
	};
	DocumentationInfo s_DisableVisibility = new DocumentationInfo()
	{
		functionName = "DisableVisibility()",
		parameter = new string[ 1 ]
		{
			"string name - The name that the targeted Simple Health Bar Visibility Controller has been registered with."
		},
		description = "Disables the targeted Simple Health Bars visually.",
		codeExample = "SimpleHealthBarVisibilityController.DisableVisibility( \"Health\" );"
	};
	DocumentationInfo s_GetCurrentState = new DocumentationInfo()
	{
		functionName = "GetCurrentState()",
		returnType = "bool",
		parameter = new string[ 1 ]
		{
			"string name - The name that the targeted Simple Health Bar Visibility Controller has been registered with."
		},
		description = "Returns the current state of visibility for the Simple Health Bars.",
		codeExample = "bool visibilityState = SimpleHealthBarVisibilityController.GetCurrentState( \"Health\" );"
	};
	#endregion

	[MenuItem( "Window/Tank and Healer Studio/Simple Health Bar PRO", false, 0 )]
	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<SimpleHealthBarWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 500, 500 );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	public static void OpenDocumentation ()
	{
		InitializeWindow();

		if( !pageHistory.Contains( documentation ) )
			NavigateForward( documentation );
	}

	void OnEnable ()
	{
		style = ( GUISkin )Resources.Load( "SimpleHealthBarEditorSkin" );

		scriptReference = ( Texture2D )Resources.Load( "SHBP_ScriptRef" );
		ujPromo = ( Texture2D ) Resources.Load( "UJ_Promo" );
		ubPromo = ( Texture2D ) Resources.Load( "UB_Promo" );

		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainMenu;
		howTo.targetMethod = HowTo;
		overview.targetMethod = OverviewPage;
		documentation.targetMethod = DocumentationPage;
		otherProducts.targetMethod = OtherProducts;
		feedback.targetMethod = Feedback;
		changeLog.targetMethod = ChangeLog;
		versionChanges.targetMethod = VersionChanges;
		thankYou.targetMethod = ThankYou;
		settings.targetMethod = WindowSettings;

		overview_SimpleHealthBar.targetMethod = Overview_UltimateHealthBar;
		overview_VisibilityController.targetMethod = Overview_VisibilityController;
		overview_FollowCameraRotation.targetMethod = Overview_FollowCameraRotation;

		docs_SimpleHealthBar.targetMethod = Docs_UltimateHealthBar;
		docs_VisibilityController.targetMethod = Docs_VisibilityController;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;
	}
	
	void OnGUI ()
	{
		if( style == null )
		{
			GUILayout.BeginVertical( "Box" );
			GUILayout.FlexibleSpace();
			ErrorScreen();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			return;
		}

		GUI.skin = style;

		paragraphStyle = GUI.skin.GetStyle( "ParagraphStyle" );
		itemHeaderStyle = GUI.skin.GetStyle( "ItemHeader" );
		sectionHeaderStyle = GUI.skin.GetStyle( "SectionHeader" );

		if( !configuredFontSize )
		{
			configuredFontSize = true;
			if( paragraphStyle.fontSize == 14 )
				fontSize = FontSize.Large;
			else if( paragraphStyle.fontSize == 12 )
				fontSize = FontSize.Medium;
			else
				fontSize = FontSize.Small;
		}
		
		GUILayout.BeginVertical( "Box" );
		
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField( "Simple Health Bar PRO", GUI.skin.GetStyle( "WindowTitle" ) );

		if( GUILayout.Button( "", GUI.skin.GetStyle( "SettingsButton" ) ) && currentPage != settings && !pageHistory.Contains( settings ) )
			NavigateForward( settings );

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 3 );
		
		if( GUILayout.Button( "Version " + version, GUI.skin.GetStyle( "VersionNumber" ) ) && currentPage != changeLog && !pageHistory.Contains( changeLog ) )
			NavigateForward( changeLog );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.Space( 12 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 5 );
		if( pageHistory.Count > 1 )
		{
			if( GUILayout.Button( "", GUI.skin.GetStyle( "BackButton" ), GUILayout.Width( 80 ), GUILayout.Height( 40 ) ) )
				NavigateBack();
			rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		}
		else
			GUILayout.Space( 80 );

		GUILayout.Space( 15 );
		EditorGUILayout.LabelField( menuTitle, GUI.skin.GetStyle( "MenuTitle" ) );
		GUILayout.FlexibleSpace();
		GUILayout.Space( 80 );
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if( currentPage.targetMethod != null )
			currentPage.targetMethod();

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Space( 25 );
		
		EditorGUILayout.EndVertical();

		Repaint();
	}

	void ErrorScreen ()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUIStyle errorStyle = new GUIStyle( GUI.skin.label );
		errorStyle.fixedHeight = 55;
		errorStyle.fixedWidth = 175;
		errorStyle.fontSize = 48;
		errorStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
		EditorGUILayout.LabelField( "ERROR", errorStyle );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 50 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "Could not find the needed GUISkin located in the Editor / Resources folder. Please ensure that the correct GUISkin, SimpleHealthBarEditorSkin, is in the right folder( Simple Health Bar / Editor / Resources ) before trying to access the Simple Health Bar Window.", EditorStyles.wordWrappedLabel );
		GUILayout.Space( 50 );
		EditorGUILayout.EndHorizontal();
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];
	}

	static void NavigateForward ( PageInformation menu )
	{
		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;
	}
	
	void MainMenu ()
	{
		mainMenu.scrollPosition = EditorGUILayout.BeginScrollView( mainMenu.scrollPosition, false, false, docSize );

		GUILayout.Space( 25 );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "How To", buttonSize ) )
			NavigateForward( howTo );

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Overview", buttonSize ) )
			NavigateForward( overview );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Documentation", buttonSize ) )
			NavigateForward( documentation );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Other Products", buttonSize ) )
			NavigateForward( otherProducts );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Feedback", buttonSize ) )
			NavigateForward( feedback );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndScrollView();
	}
	
	void HowTo ()
	{
		StartPage( howTo );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "In order to create a Simple Health Bar in your scene, first make sure there is Canvas in your scene. After there is a Canvas in your scene, simply find the prefab that you want. Prefabs are located at Assets / Simple Health Bar / Prefabs, and drag it into the Canvas in your scene.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The Simple Health Bar is very easy to reference into your own scripts. Simply assign a name to the Simple Health Bar at the top of the Simple Health Bar inspector, and then copy the example code provided. Below is the key function that you will be using to display your status values to your player. For this example, we will be using the name \"Health\" for our Simple Health bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Key Function:", itemHeaderStyle );
		EditorGUILayout.TextArea( "SimpleHealthBar.UpdateBar( \"Health\", currentValue, maxValue );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( scriptReference );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Simple Example", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "Since the most common use of these kinds of bars is for health, let's assume that we will be implementing a Simple Health Bar into our scene. In order to send the correct and current value to the Simple Health Bar, it is important to remember to only send information once it has finished being modified. For example, for a players health, you will only want to update the Simple Health Bar once the player has finished taking damage. For example:", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Coding Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "void TakeDamage ( int damage )\n{\n" + Indent + "health -= damage;\n\n" + Indent + "// Now is where you will want to update the Simple Health Bar. Only AFTER the value has been modified.\n" + Indent + "SimpleHealthBar.UpdateBar( \"Health\", health, maxHealth );\n}", GUI.skin.GetStyle( "TextArea" ) );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "<b>NOTE:</b> For more information about each function and how to use it, please see the Documentation section of this window.", paragraphStyle );

		EndPage();
	}
	
	void OverviewPage ()
	{
		StartPage( overview );

		EditorGUILayout.LabelField( "Simple Health Bar", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( Indent + "The Simple Health Bar component is the main component of this package. It is used to display the health, or any other value, to the user. Click the button below to learn more about this component.", paragraphStyle );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Learn More", buttonSize ) )
			NavigateForward( overview_SimpleHealthBar );
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Visibility Controller", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( Indent + "The Simple Health Bar Visibility Controller script keeps track of all the child Simple Health Bar components, and controls the visibility depending on the child Simple Health Bars. Click the button below to learn more about this component.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Learn More", buttonSize ) )
			NavigateForward( overview_VisibilityController );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Follow Camera Rotation", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( Indent + "The Follow Camera Rotation script is used to keep a Canvas facing a certain camera in the scene. This is useful for enemies health bars in your scene that you want to face the camera. Click the button below to learn more about this component.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Learn More", buttonSize ) )
			NavigateForward( overview_FollowCameraRotation );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void Overview_UltimateHealthBar ()
	{
		StartPage( overview_SimpleHealthBar );

		EditorGUILayout.LabelField( "Simple Health Bar Overview", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Bar Name", itemHeaderStyle );
		EditorGUILayout.LabelField( "The unique name to be used in reference to this bar. This string will be used to call functions on this particular Simple Health Bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Bar Image", itemHeaderStyle );
		EditorGUILayout.LabelField( "The image component to be used for this bar. When assigned, this image will be set to Image Type: Filled if has not been changed before. Remember to refer to the actual Image component to change the Method and Origin of the Image.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Color Mode", itemHeaderStyle );
		EditorGUILayout.LabelField( "The mode in which to display the color to the barImage component. The option <i>Single</i> will use a single color to apply to the Image component, and the option <i>Gradient</i> will allow you to set a custom gradient for the fill of the Image.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Display Text", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Display Text option allows you to determine how this bar will display text to the user, if at all. Once enabled, the sub-options will allow you to set the Text component to display the text to, the color of the text, and any additional text that you want added to the values.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Smooth Fill", itemHeaderStyle );
		EditorGUILayout.LabelField( "This option, when enabled, will fill the image component over a set period of time. This option makes the status feel much more fluid and smooth.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Dramatic Fill", itemHeaderStyle );
		EditorGUILayout.LabelField( "This option uses another Image component to display behind the Bar Image to draw more attention to the change in values of the bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Fill Constraint", itemHeaderStyle );
		EditorGUILayout.LabelField( "This option can be used to set limits to the Image fill. This can be used if you are not happy with the default fill that Unity uses, or for a semi round image that would not need to fill a complete 360°.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Test Value", itemHeaderStyle );
		EditorGUILayout.LabelField( "A simple slider to help see how the Simple Health Bar will look when it is being used.", paragraphStyle );

		EndPage();
	}
	
	void Overview_VisibilityController ()
	{
		StartPage( overview_VisibilityController );

		EditorGUILayout.LabelField( "Visibility Controller Overview", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Bar Name", itemHeaderStyle );
		EditorGUILayout.LabelField( "The name to register this component as.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Update Visibility", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines how the Visibility Controller will update the visibility of the child Simple Health Bars.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Idle Seconds", itemHeaderStyle );
		EditorGUILayout.LabelField( "This option is only available when using the <i>On Activity</i> setting of the Update Visibility option. It determines how many seconds that the health bars are idle before updating it's visibility.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Initial State", itemHeaderStyle );
		EditorGUILayout.LabelField( "The initial state for the visibility.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Enable Duration", itemHeaderStyle );
		EditorGUILayout.LabelField( "Time in seconds for the Visibility Controller to react enabled alpha.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Disable Duration", itemHeaderStyle );
		EditorGUILayout.LabelField( "Time in seconds for the Visibility Controller to react disabled alpha.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Enabled Alpha", itemHeaderStyle );
		EditorGUILayout.LabelField( "The targeted alpha when enabled.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Disabled Alpha", itemHeaderStyle );
		EditorGUILayout.LabelField( "The targeted alpha when disabled.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Simple Health Bars", itemHeaderStyle );
		EditorGUILayout.LabelField( "A list of all child Simple Health Bars.", paragraphStyle );
		EditorGUILayout.LabelField( "<i>Keep Visible:</i> Determines if this status should force the Visibility Controller to be enabled.", paragraphStyle );
		EditorGUILayout.LabelField( "<i>Trigger Value:</i> The value that will trigger the Visibility Controller to stay enabled.", paragraphStyle );
		EditorGUILayout.LabelField( "<i>Test Value:</i> A simple slider to allow you to see the changes to the Simple Health Bar.", paragraphStyle );

		EndPage();
	}
	
	void Overview_FollowCameraRotation ()
	{
		StartPage( overview_SimpleHealthBar );

		EditorGUILayout.LabelField( "Follow Camera Rotation Overview", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Find Camera By", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines how the Camera should be found. <i>Transform</i> allows you to simply assign a Transform for the camera, <i>Name</i> will find the camera by a specific name inside your scene, and <i>Tag</i> will find the camera by a specific tag that is being used on the camera.", paragraphStyle );

		EndPage();
	}
	
	void DocumentationPage ()
	{
		StartPage( documentation );

		EditorGUILayout.LabelField( "Simple Health Bar", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( Indent + "The Simple Health Bar component is the main component of this package. It is used to display the health, or any other value, to the user. Click the button below to learn more about this component.", paragraphStyle );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Learn More", buttonSize ) )
			NavigateForward( docs_SimpleHealthBar );
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Visibility Controller", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( Indent + "The Simple Health Bar Visibility Controller script keeps track of all the child Simple Health Bar components, and controls the visibility depending on the child Simple Health Bars. Click the button below to learn more about this component.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Learn More", buttonSize ) )
			NavigateForward( docs_VisibilityController );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}
	
	void Docs_UltimateHealthBar ()
	{
		StartPage( documentation );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Simple Health Bar Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Simple Health Bar. Each example provided relies on having a Simple Health Bar variable named 'healthBar' stored inside your script. When using any of the example code provided, make sure that you have a public Simple Health Bar variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "public SimpleHealthBar healthBar;", GUI.skin.textArea );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		ShowDocumentation( p_UpdateBar );
		ShowDocumentation( p_UpdateColor );
		ShowDocumentation( p_UpdateTextColor );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Simple Health Bar Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The following functions can be referenced from your scripts without the need for an assigned local Simple Health Bar variable. However, each function must have the targeted Simple Health Bar name in order to find the correct Simple Health Bar in the scene. Each example code provided uses the name 'Health' as the bar name.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		ShowDocumentation( s_GetSimpleHealthBar );
		ShowDocumentation( s_UpdateBar );
		ShowDocumentation( s_UpdateColor );
		ShowDocumentation( s_UpdateTextColor );
		
		GUILayout.Space( itemHeaderSpace );
		
		EndPage();
	}
	
	void Docs_VisibilityController ()
	{
		StartPage( documentation );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Visibility Controller Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Simple Health Bar Visibility Controller. Each example provided relies on having a Simple Health Bar Visibility Controller variable named 'healthBarVisibility' stored inside your script. When using any of the example code provided, make sure that you have a public Simple Health Bar Visibility Controller variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "public SimpleHealthBarVisibilityController healthBarVisibility;", GUI.skin.textArea );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		ShowDocumentation( p_EnableVisibility );
		ShowDocumentation( p_DisableVisibility );
		ShowDocumentation( p_GetCurrentState );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Visibility Controller Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The following functions can be referenced from your scripts without the need for an assigned local Simple Health Bar Visibility Controller variable. However, each function must have the targeted Simple Health Bar Visibility Controller name in order to find the correct Simple Health Bar Visibility Controller in the scene. Each example code provided uses the name 'Health' as the name.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		ShowDocumentation( s_GetSimpleHealthBarVisibilityController );
		ShowDocumentation( s_EnableVisibility );
		ShowDocumentation( s_DisableVisibility );
		ShowDocumentation( s_GetCurrentState );
		
		GUILayout.Space( itemHeaderSpace );
		
		EndPage();
	}
	
	void OtherProducts ()
	{
		StartPage( otherProducts );

		/* -------------- < ULTIMATE JOYSTICK > -------------- */
		if( ujPromo != null )
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Space( 15 );
			GUILayout.Label( ujPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			GUILayout.Space( paragraphSpace );
		}

		EditorGUILayout.LabelField( "Ultimate Joystick", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Ultimate Joystick is a simple, yet powerful input tool for the development of your mobile games. The Ultimate Joystick was created with the goal of giving developers an incredibly versatile joystick solution. Designed with this in mind, it is extremely fast and easy to implement into either new or existing scripts. You don't need to be a programmer to work with the Ultimate Joystick, and it is very easy to integrate into any type of character controller. Additionally, the Ultimate Joystick features a complete in-engine Documentation Window to help you understand exactly what you have at your disposal. In its entirety, Ultimate Joystick is an elegant and easy to utilize mobile joystick solution.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-joystick.html" );
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END ULTIMATE JOYSTICK > ------------ */

		GUILayout.Space( 25 );

		/* ------------ < ULTIMATE BUTTON > ------------ */
		if( ubPromo != null )
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Space( 15 );
			GUILayout.Label( ubPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			GUILayout.Space( paragraphSpace );
		}

		EditorGUILayout.LabelField( "Ultimate Button", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "Action buttons are a core element of UI, and as such they should be easy to customize and implement. The Ultimate Button is the embodiment of that very idea. This code package takes the best of Unity's Input and UnityEvent methods and pairs it with exceptional customization to give you the most versatile button for your mobile project. With Ultimate Button's easy size and placement options, custom style settings and functionality configurations, you'll have everything you need to create your custom buttons, whether they are simple or complex.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-button.html" );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* -------------- < END ULTIMATE BUTTON > --------------- */

		EndPage();
	}
	
	void Feedback ()
	{
		StartPage( feedback );

		EditorGUILayout.LabelField( "Having Problems?", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "If you experience any issues with the Simple Health Bar, please contact us right away! We will lend any assistance that we can to resolve any issues that you have.\n\n<b>Support Email:</b>.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( 25 );


		EditorGUILayout.LabelField( "Good Experiences?", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "If you have appreciated how easy the Simple Health Bar is to get into your project, leave us a comment and rating on the Unity Asset Store. We are very grateful for all positive feedback that we get.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Rate Us", buttonSize ) )
			Application.OpenURL( "https://www.assetstore.unity3d.com/#!/content/109385" );
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Show Us What You've Done!", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "If you have used any of the assets created by Tank & Healer Studio in your project, we would love to see what you have done. Contact us with any information on your game and we will be happy to support you in any way that we can!\n\n<b>Contact Us:</b>", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EndPage();
	}

	void ChangeLog ()
	{
		StartPage( changeLog );

		EditorGUILayout.LabelField( "Version 1.0.1f", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Added complete In-Engine Documentation window.", paragraphStyle );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.0", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Initial Release.", paragraphStyle );

		EndPage();
	}
	
	void ThankYou ()
	{
		StartPage( thankYou );
		
		EditorGUILayout.LabelField( Indent + "The two of us at Tank & Healer Studio would like to thank you for purchasing the Simple Health Bar asset package from the Unity Asset Store. If you have any questions about the Simple Health Bar that are not covered in this Documentation Window, please don't hesitate to contact us at: ", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( Indent + "We hope that the Simple Health Bar will be a great help to you in the development of your game. After pressing the continue button below, you will be presented with helpful information on this asset to assist you in implementing Simple Health Bar into your project.\n", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", paragraphStyle, GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", buttonSize ) )
			NavigateBack();
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void VersionChanges ()
	{
		StartPage( versionChanges );

		EditorGUILayout.LabelField( Indent + "This page is used for major changes to the asset that needs to be brought to the attention of the user.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", buttonSize ) )
			NavigateBack();
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		EndPage();
	}

	void WindowSettings ()
	{
		StartPage( settings );

		EditorGUI.BeginChangeCheck();
		fontSize = ( FontSize )EditorGUILayout.EnumPopup( "Font Size", fontSize );
		if( EditorGUI.EndChangeCheck() )
		{
			switch( fontSize )
			{
				case FontSize.Small:
				default:
				{
					GUI.skin.textArea.fontSize = 11;
					paragraphStyle.fontSize = 11;
					itemHeaderStyle.fontSize = 11;
					sectionHeaderStyle.fontSize = 14;
				}
				break;
				case FontSize.Medium:
				{
					GUI.skin.textArea.fontSize = 12;
					paragraphStyle.fontSize = 12;
					itemHeaderStyle.fontSize = 12;
					sectionHeaderStyle.fontSize = 18;
				}
				break;
				case FontSize.Large:
				{
					GUI.skin.textArea.fontSize = 14;
					paragraphStyle.fontSize = 14;
					itemHeaderStyle.fontSize = 14;
					sectionHeaderStyle.fontSize = 20;
				}
				break;
			}
		}

		GUILayout.Space( 20 );
		
		EditorGUILayout.LabelField( "Example Text", sectionHeaderStyle );
		GUILayout.Space( paragraphSpace );
		EditorGUILayout.LabelField( "Example Text", itemHeaderStyle );
		GUILayout.Space( paragraphSpace );
		EditorGUILayout.LabelField( "This is an example paragraph to see the size of the text after modification.", paragraphStyle );

		EndPage();
	}

	void StartPage ( PageInformation pageInfo )
	{
		pageInfo.scrollPosition = EditorGUILayout.BeginScrollView( pageInfo.scrollPosition, false, false, docSize );
		GUILayout.Space( 15 );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	void ShowDocumentation ( DocumentationInfo info )
	{
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( info.functionName, itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) && ( info.showMore.faded == 0.0f || info.showMore.faded == 1.0f ) )
		{
			info.showMore.target = !info.showMore.target;
		}

		if( EditorGUILayout.BeginFadeGroup( info.showMore.faded ) )
		{
			if( info.parameter != null )
			{
				for( int i = 0; i < info.parameter.Length; i++ )
					EditorGUILayout.LabelField( Indent + "<i>Parameter:</i> " + info.parameter[ i ], paragraphStyle );
			}
			if( info.returnType != string.Empty )
				EditorGUILayout.LabelField( Indent + "<i>Return type:</i> " + info.returnType, paragraphStyle );

			EditorGUILayout.LabelField( Indent + "<i>Description:</i> " + info.description, paragraphStyle );

			if( info.codeExample != string.Empty )
				EditorGUILayout.TextArea( info.codeExample, GUI.skin.textArea );

			GUILayout.Space( paragraphSpace );
		}
		EditorGUILayout.EndFadeGroup();
	}

	string Indent
	{
		get
		{
			return "    ";
		}
	}

	[InitializeOnLoad]
	class SimpleHealthBarInitialLoad
	{
		static SimpleHealthBarInitialLoad ()
		{
			// If this is the first time that the user has downloaded this package...
			if( !EditorPrefs.HasKey( "SimpleHealthBarVersion" ) )
			{
				// Navigate to the Thank You page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "SimpleHealthBarVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
			else if( EditorPrefs.GetInt( "SimpleHealthBarVersion" ) < importantChanges )
			{
				// Navigate to the Version Changes page.
				NavigateForward( versionChanges );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "SimpleHealthBarVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;

			InitializeWindow();
		}
	}
}