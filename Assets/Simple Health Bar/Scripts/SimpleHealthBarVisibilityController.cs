/* Written by Kaz Crowe */
/* SimpleHealthBarVisibilityController.cs */
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu( "UI/Simple Health Bar/Visibility Controller" )]
public class SimpleHealthBarVisibilityController : MonoBehaviour
{
	[Serializable]
	public class HealthBarInformation
	{
		public SimpleHealthBar healthBar;
		public bool forceVisible = false;
		public float triggerValue = 0.25f;
		public bool keepVisible = false;
		public event Action UpdateStatusBar;
		public event Action UpdateVisibility;


		public void OnStatusUpdated ()
		{
			if( keepVisible == false )
			{
				// Then just inform the Ultimate Health Bar that the bar has simply been updated.
				if( UpdateStatusBar != null )
					UpdateStatusBar();
				return;
			}
			
			if( healthBar.GetCurrentCalculatedFraction <= triggerValue && forceVisible == false )
			{
				// Set forceVisible to true so the UltimateHealthBarVisibilityController knows and then inform that a change to the visibility may be needed.
				forceVisible = true;
				if( UpdateVisibility != null )
					UpdateVisibility();
				return;
			}

			// If the current fraction is greater than the trigger and forceVisible is currently true...
			if( healthBar.GetCurrentCalculatedFraction > triggerValue && forceVisible == true )
			{
				// Set forceVisible to false so the UltimateHealthBarVisibilityController knows and then inform that a change may be needed.
				forceVisible = false;
				if( UpdateVisibility != null )
					UpdateVisibility();
				return;
			}

			// If nothing else has triggered, then simply update the bar.
			if( UpdateStatusBar != null )
				UpdateStatusBar();
		}
	}
	public List<HealthBarInformation> healthBarInformation = new List<HealthBarInformation>();

	public enum UpdateVisibility
	{
		Manually,
		OnActivity
	}
	public UpdateVisibility updateVisibility;

	public float idleSeconds;
	public float enableDuration = 1.0f, disableDuration = 1.0f;
	public float enabledAlpha = 1.0f, disabledAlpha = 0.0f;
	float enabledSpeed = 1.0f, disabledSpeed = 1.0f;
	bool isFading = false, isCountingDown = false;
	float countdownTime = 0.0f;
	CanvasGroup statusBarGroup;
	bool forceVisible = false;
	public bool initialState = true;
	bool currentState = true;
	public bool CurrentState
	{
		get
		{
			return currentState;
		}
	}

	static Dictionary<string, SimpleHealthBarVisibilityController> VisibilityControllers = new Dictionary<string, SimpleHealthBarVisibilityController>();
	public string barName = string.Empty;


	public void Awake ()
	{
		// If the barName is assigned...
		if( barName != string.Empty )
		{
			// Check to see if the VisibilityControllers dictionary already contains this name, and if so, remove the current one.
			if( VisibilityControllers.ContainsKey( barName ) )
				VisibilityControllers.Remove( barName );

			// Register this UltimateHealthBarVisibilityController into the dictionary.
			VisibilityControllers.Add( barName, GetComponent<SimpleHealthBarVisibilityController>() );
		}

		// Loop through each Ultimate Health Bar...
		for( int i = 0; i < healthBarInformation.Count; i++ )
		{
			// If the user wants to update the visibility using the OnBarUpdated option...
			if( updateVisibility == UpdateVisibility.OnActivity )
			{
				//healthBarInformation[ i ].healthBar.OnBarUpdated += UpdateStatusBar;
				healthBarInformation[ i ].healthBar.OnBarUpdated += healthBarInformation[ i ].OnStatusUpdated;
				healthBarInformation[ i ].UpdateStatusBar += UpdateStatusBar;
				healthBarInformation[ i ].UpdateVisibility += UpdateStatusBarVisibility;
			}
		}

		// Get the CanvasGroup component
		statusBarGroup = GetComponent<CanvasGroup>();

		// If the Canvas Group is null, then add the component and assign the variable again.
		if( statusBarGroup == null )
		{
			gameObject.AddComponent( typeof( CanvasGroup ) );
			statusBarGroup = GetComponent<CanvasGroup>();
		}

		// Configure the different fade speeds.
		enabledSpeed = 1.0f / enableDuration;
		disabledSpeed = 1.0f / disableDuration;

		// Set the current state to the initial state value.
		currentState = initialState;

		// If the current state is false, then set the canvas group to the disabled alpha.
		if( currentState == false )
			statusBarGroup.alpha = disabledAlpha;
		// Else apply the enabled alpha.
		else
			statusBarGroup.alpha = enabledAlpha;
			
		// If the user wants to update the visibility when the bar has been updated, and the initial state is true, then start the countdown.
		if( updateVisibility == UpdateVisibility.OnActivity && initialState == true )
			StartCoroutine( "ShowStatusBarCountdown" );
	}

	/// <summary>
	/// This function is subscribed to the UltimateStatus class function UpdateStatusBar. This function will be called each time that a bar is updated. Therefore, even if the certain bar will not keep the bar visible, it can still update the visibility when it has been modified.
	/// </summary>
	void UpdateStatusBar ()
	{
		// If the user is not using the OnBarUpdated option, or the Ultimate Health Bar is already forcing visible, then return.
		if( updateVisibility != UpdateVisibility.OnActivity || forceVisible == true )
			return;
		
		// If the countdown is already running, then reset the countdown time.
		if( isCountingDown == true )
			countdownTime = idleSeconds;
		// Else start the countdown.
		else
			StartCoroutine( "ShowStatusBarCountdown" );

		// If the bar is currently fading out...
		if( isFading == true && currentState == false )
		{
			// Then set isFading to false, and stop the coroutine.
			isFading = false;
			StopCoroutine( "FadeOutHandler" );
		}
		// Show the bar.
		EnableVisibility();
	}

	/// <summary>
	/// This function is subscribed to the UltimateStatus class function: UpdateStatusBarVisibility. This function is only called when the bar has triggered it's keep visible value.
	/// </summary>
	void UpdateStatusBarVisibility ()
	{
		// If the user doesn't have the timeout option enabled, return.
		if( updateVisibility != UpdateVisibility.OnActivity )
			return;

		// Loop through each Ultimate Health Bar...
		for( int i = 0; i < healthBarInformation.Count; i++ )
		{
			// If the current bar has the keepVisible option enabled, and it is currently wanted to force the visibility...
			if( healthBarInformation[ i ].keepVisible == true && healthBarInformation[ i ].forceVisible == true )
			{
				// Stop counting down.
				isCountingDown = false;
				StopCoroutine( "ShowStatusBarCountdown" );

				// Show the bar.
				EnableVisibility();

				// Set forceVisible to true and return.
				forceVisible = true;
				return;
			}
		}

		// Set force visible to false.
		forceVisible = false;

		// Start the countdown and show the bar.
		StartCoroutine( "ShowStatusBarCountdown" );
		EnableVisibility();
	}

	/// <summary>
	/// This function is used as a local controlled Update function for count down the time that this bar has been idle.
	/// </summary>
	IEnumerator ShowStatusBarCountdown ()
	{
		// Set isCountingDown to true for checks.
		isCountingDown = true;

		// Set the starting time.
		countdownTime = idleSeconds;

		// If the current state is false, then add the duration of the enable to make idle time correct.
		if( currentState == false )
			countdownTime += enableDuration;

		// While the countdownTime is greater than zero, continue counting down.
		while( countdownTime > 0 )
		{
			countdownTime -= Time.deltaTime;
			yield return null;
		}

		// Once the countdown is complete, set isCountingDown to false and hide the bar.
		isCountingDown = false;
		DisableVisibility();
	}

	/// <summary>
	/// This function handles the fading in of the Ultimate Health Bar.
	/// </summary>
	IEnumerator FadeInHandler ()
	{
		// Set isFading to true so that other functions will know that this coroutine is running.
		isFading = true;

		// Store the current value of the Canvas Group's alpha.
		float currentAlpha = statusBarGroup.alpha;

		// Loop for the duration of the enabled duration variable.
		for( float t = 0.0f; t < 1.0f && isFading == true; t += Time.deltaTime * enabledSpeed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( enabledSpeed ) )
				break;

			// Apply the alpha to the CanvasGroup.
			statusBarGroup.alpha = Mathf.Lerp( currentAlpha, enabledAlpha, t );
			yield return null;
		}
		// If the coroutine was not interrupted, then apply the final value.
		if( isFading == true )
			statusBarGroup.alpha = enabledAlpha;

		// Set isFading to false so that other functions know that this coroutine is not running anymore.
		isFading = false;
	}
	
	/// <summary>
	/// For details on this coroutine, see the FadeInHandler() function above.
	/// </summary>
	IEnumerator FadeOutHandler ()
	{
		isFading = true;
		float currentAlpha = statusBarGroup.alpha;
		for( float t = 0.0f; t < 1.0f && isFading == true; t += Time.deltaTime * disabledSpeed )
		{
			if( float.IsInfinity( disabledSpeed ) )
				break;

			statusBarGroup.alpha = Mathf.Lerp( currentAlpha, disabledAlpha, t );

			yield return null;
		}
		if( isFading == true )
			statusBarGroup.alpha = disabledAlpha;

		isFading = false;
	}

	/// <summary>
	/// Shows the bar.
	/// </summary>
	public void EnableVisibility ()
	{
		// If the current state is already true, or the user does not want to update the visibility ever, then return.
		if( currentState == true )
			return;

		// Set the current state to true.
		currentState = true;
		
		// If there is no CanvasGroup, then return.
		if( statusBarGroup == null )
			return;

		// If the bar is currently fading, then stop the FadeOutHandler.
		if( isFading == true )
			StopCoroutine( "FadeOutHandler" );

		// Start the Fade In routine.
		StartCoroutine( "FadeInHandler" );
	}

	/// <summary>
	/// Hides the bar.
	/// </summary>
	public void DisableVisibility ()
	{
		// If the current state is already false, or the user does not want to update the visibility ever, then return.
		if( currentState == false )
			return;

		// Set the current state to false.
		currentState = false;

		// If the statusBarGroup isn't assigned, return.
		if( statusBarGroup == null )
			return;

		// If the bar is currently fading, then stop the coroutine.
		if( isFading == true )
			StopCoroutine( "FadeInHandler" );

		// Start the Fade Out routine.
		StartCoroutine( "FadeOutHandler" );
	}

	public bool GetCurrentState ()
	{
		return CurrentState;
	}

	static bool VisibilityControllerRegistered ( string name )
	{
		// If an visibility controller has not been registered with the name parameter, then return.
		if( !VisibilityControllers.ContainsKey( name ) )
		{
			Debug.LogError( "Ultimate Health Bar Visibility Controller - No controller has been registered with the name: " + name + "." );
			return false;
		}

		return true;
	}

	/// <summary>
	/// Returns the Ultimate Health Bar Visibility Controller component that has been registered with the targeted name.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static SimpleHealthBarVisibilityController GetUltimateHealthBarVisibilityController ( string name )
	{
		if( !VisibilityControllerRegistered( name ) )
			return null;

		return VisibilityControllers[ name ];
	}

	/// <summary>
	/// Enables the visibility of the bars.
	/// </summary>
	/// <param name="name">The registered name of the targeted Ultimate Health Bar Visibility Controller.</param>
	public static void EnableVisibility ( string name )
	{
		if( !VisibilityControllerRegistered( name ) )
			return;

		VisibilityControllers[ name ].EnableVisibility();
	}

	/// <summary>
	/// Disables the visibility of the bars.
	/// </summary>
	/// <param name="name">The registered name of the targeted Ultimate Health Bar Visibility Controller.</param>
	public static void DisableVisibility ( string name )
	{
		if( !VisibilityControllerRegistered( name ) )
			return;

		VisibilityControllers[ name ].DisableVisibility();
	}

	/// <summary>
	/// Returns the current visibility state.
	/// </summary>
	/// <param name="name">The registered name of the targeted Ultimate Health Bar Visibility Controller.</param>
	public static bool GetCurrentState ( string name )
	{
		if( !VisibilityControllerRegistered( name ) )
			return false;

		return VisibilityControllers[ name ].CurrentState;
	}
}