/* Written by Kaz Crowe */
/* SimpleHealthBar.cs */
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu( "UI/Simple Health Bar/Simple Health Bar" )]
public class SimpleHealthBar : MonoBehaviour
{	
	// COLOR OPTIONS //
	public Image barImage;
	public enum ColorMode
	{
		Single,
		Gradient
	}
	public ColorMode colorMode;
	public Color barColor = Color.white;
	public Gradient barGradient = new Gradient();

	// TEXT OPTIONS //
	public enum DisplayText
	{
		Disabled,
		Percentage,
		CurrentValue,
		CurrentAndMaxValues
	}
	public DisplayText displayText;
	public Text barText;
	public string additionalText = string.Empty;

	// SMOOTH FILL //
	public bool smoothFill = false;
	public float smoothFillDuration = 1.0f;
	public bool isSmoothing = false;
	bool _resetSmoothing = false;
	public bool resetSmoothing
	{
		get
		{
			if( _resetSmoothing == true )
			{
				_resetSmoothing = false;
				return true;
			}
			return _resetSmoothing;
		}
	}

	// DRAMATIC FILL //
	public Image dramaticImage;
	public Color dramaticColor = Color.white;
	public enum DramaticFill
	{
		Disabled,
		Increase,
		Decrease
	}
	public DramaticFill dramaticFill = DramaticFill.Disabled;
	bool isUpdating = false;
	float _secondsDelay = 0.1f;
	public float secondsDelay = 0.1f;
	public float resetSensitivity = 0.1f;
	float previousFillAmt = 0.0f;
	public float fillSpeed = 0.5f;

	// FILL CONSTRAINT //
	public bool fillConstraint = false;
	public float fillConstraintMin = 0.0f;
	public float fillConstraintMax = 1.0f;

	// PRIVATE VARIABLES AND GET FUNCTIONS //
	float _currentFraction = 1.0f;
	/// <summary>
	/// Returns the percentage value that was calculated when the bar was updated. This number will not be current with the Smooth Fill option.
	/// </summary>
	public float GetCurrentFraction
	{
		get
		{
			return _currentFraction;
		}
	}

	/// <summary>
	/// The stored max value that the user entered.
	/// </summary>
	float maxValue = 0.0f;

	/// <summary>
	/// This float stores the target amount of fill. This value is current with Fill Constraints.
	/// </summary>
	float targetFill = 0.0f;

	/// <summary>
	/// This value calculates the current value of the bar. The number is current with the smooth fill option and fill constraints.
	/// </summary>
	public float GetCurrentCalculatedFraction
	{
		get
		{
			if( barImage == null )
				return 0.0f;

			float tempFloat = 0.0f;
			if( fillConstraint == true )
				tempFloat = ( barImage.fillAmount - fillConstraintMin ) / ( fillConstraintMax - fillConstraintMin );
			else
				tempFloat = barImage.fillAmount;

			return tempFloat;
		}
	}

	public Action OnBarUpdated;

	// ----- < SCRIPT REFERENCE > ----- //
	static Dictionary<string, SimpleHealthBar> SimpleHealthBars = new Dictionary<string, SimpleHealthBar>();
	public string barName = string.Empty;

	void Awake ()
	{
		// If the barName is assigned...
		if( barName != string.Empty )
		{
			// Check to see if the SimpleHealthBars dictionary already contains this name, and if so, remove the current one.
			if( SimpleHealthBars.ContainsKey( barName ) )
				SimpleHealthBars.Remove( barName );

			// Register this UltimateHealthBar into the dictionary.
			SimpleHealthBars.Add( barName, GetComponent<SimpleHealthBar>() );
		}
	}

	/// <summary>
	/// Displays the text.
	/// </summary>
	void DisplayTextHandler ()
	{
		// If the user does not want text to be displayed, or the text component is null, then return.
		if( displayText == DisplayText.Disabled || barText == null )
			return;

		// Switch statement for the displayText option. Each option will display the correct text for the set option.
		switch( displayText )
		{
			case DisplayText.Percentage:
			{
				barText.text = additionalText + ( GetCurrentCalculatedFraction * 100 ).ToString( "F0" ) + "%";
			}break;
			case DisplayText.CurrentValue:
			{
				barText.text = additionalText + ( GetCurrentCalculatedFraction * maxValue ).ToString( "F0" );
			}break;
			case DisplayText.CurrentAndMaxValues:
			{
				barText.text = additionalText + ( GetCurrentCalculatedFraction * maxValue ).ToString( "F0" ) + " / " + maxValue.ToString();
			}break;
		}
	}

	/// <summary>
	/// Update the color of the bar according to the gradient.
	/// </summary>
	void UpdateGradient ()
	{
		// If the color mode is set to Gradient, then apply the current gradient color.
		if( colorMode == ColorMode.Gradient )
			barImage.color = barGradient.Evaluate( GetCurrentCalculatedFraction );
	}

	/// <summary>
	/// Updates the options.
	/// </summary>
	void UpdateOptions ()
	{
		UpdateGradient();
		DisplayTextHandler();

		if( !Application.isPlaying )
			return;

		if( OnBarUpdated != null )
			OnBarUpdated();

		if( dramaticFill != DramaticFill.Disabled )
			UpdateDramaticFill();
	}

	/// <summary>
	/// Smoothly transitions from the current fill amount to the target fill amount.
	/// </summary>
	IEnumerator SmoothFill ()
	{
		// Set isSmoothing to true.
		isSmoothing = true;

		// Configure the speed, as well as the current fill of the image.
		float speed = 1.0f / smoothFillDuration;
		float currentFill = barImage.fillAmount;
		
		// Loop for the duration of the smooth fill.
		for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed )
		{
			// If this needs to be reset...
			if( resetSmoothing == true )
			{
				// Reconfigure the current fill and reset t.
				currentFill = barImage.fillAmount;
				t = 0.0f;
			}

			// Lerp the fill amount from the current fill to the target.
			barImage.fillAmount = Mathf.Lerp( currentFill, targetFill, t );

			// Call the UpdateOptions each frame that this bar is updated to update options.
			UpdateOptions();

			yield return null;
		}
		// If this is not needing to reset the smoothing, then finalize the fill to be the stored target fill.
		if( resetSmoothing == false )
			barImage.fillAmount = targetFill;
		// Else, restart this coroutine.
		else
			StartCoroutine( "SmoothFill" );
		
		// Call the UpdateOptions each frame that this bar is updated to update options.
		UpdateOptions();

		// Set isSmoothing to false so that the script knows that this coroutine is not running any more.
		isSmoothing = false;
	}

	void UpdateDramaticFill ()
	{
		switch( dramaticFill )
		{
			case DramaticFill.Decrease:
			{
				// If the official bar's fill amount is higher than this image...
				if( barImage.fillAmount > dramaticImage.fillAmount )
				{
					// Apply the same fill amount so that it will keep up visually, then return.
					dramaticImage.fillAmount = barImage.fillAmount;
					
					if( isUpdating == true )
						isUpdating = false;
					return;
				}

				// If the bar is not currently updating, then start the update.
				if( isUpdating == false )
					StartCoroutine( "UpdateFillDecrease" );
				// Else if the bar is currently updating and the difference between fills is less than the reset sensitivity, then reset the wait seconds.
				else if( isUpdating == true && ( dramaticImage.fillAmount - previousFillAmt ) < resetSensitivity )
					_secondsDelay = secondsDelay;

				// Store the previous fill amount.
				previousFillAmt = barImage.fillAmount;
			}break;
			case DramaticFill.Increase:
			{
				if( barImage.fillAmount < dramaticImage.fillAmount && barImage.fillAmount > targetFill )
					dramaticImage.fillAmount = barImage.fillAmount;
				else if( barImage.fillAmount != targetFill )
					dramaticImage.fillAmount = targetFill;
			}
			break;
		}
	}

	/// <summary>
	/// Coroutine to update the fill over time.
	/// </summary>
	IEnumerator UpdateFillDecrease ()
	{
		// Set isUpdating to true so that other functions can check if this is running.
		isUpdating = true;

		// Apply the wait time.
		_secondsDelay = secondsDelay;

		// This loop will continue while the local fill amount is greater than the target fill amount.
		while( dramaticImage.fillAmount >= barImage.fillAmount && isUpdating == true )
		{
			// If the wait seconds are greater than zero, then decrease the time.
			if( _secondsDelay > 0 )
				_secondsDelay -= Time.deltaTime;
			// Else, reduce the fill amount by the configured fill speed.
			else
				dramaticImage.fillAmount -= fillSpeed * Time.deltaTime;

			yield return null;
		}

		// If isUpdating is true, then this coroutine finished, so apply the final amount.
		if( isUpdating == true )
			dramaticImage.fillAmount = barImage.fillAmount;

		// Set isUpdating to false so that other function will know that this function is not running now.
		isUpdating = false;
	}

	/// <summary>
	/// Checks to see if the targeted name and health bar have been registered to the dictionary.
	/// </summary>
	/// <param name="name">The targeted Simple Health Bar name.</param>
	static bool UltimateHealthBarRegistered ( string name )
	{
		// If an Simple Health Bar has not been registered with the name parameter, then return.
		if( !SimpleHealthBars.ContainsKey( name ) )
		{
			Debug.LogError( "Simple Health Bar - No Health Bar has been registered with the name: " + name + "." );
			return false;
		}

		return true;
	}

	#region PUBLIC FUNCTIONS
	/// <summary>
	/// Updates the health bar with the current and max values.
	/// </summary>
	/// <param name="currentValue">The current value of the bar.</param>
	/// <param name="maxValue">The maximum value of the bar.</param>
	public void UpdateBar ( float currentValue, float maxValue )
	{
		// If the bar image is left unassigned, then return.
		if( barImage == null )
			return;
			
		// Fix the value to be a percentage.
		_currentFraction = currentValue / maxValue;

		// If the value is greater than 1 or less than 0, then fix the values to being min/max.
		if( _currentFraction < 0 || _currentFraction > 1 )
			_currentFraction = _currentFraction < 0 ? 0 : 1;

		// Store the target amount of fill according to the users options.
		targetFill = fillConstraint == true ? Mathf.Lerp( fillConstraintMin, fillConstraintMax, _currentFraction ) : _currentFraction;

		// Store the values so that other functions used can reference the maxValue.
		this.maxValue = maxValue;

		// If the user is not using the Smooth Fill option, or this function is being called inside Edit Mode...
		if( smoothFill == false || Application.isPlaying == false )
		{
			// Then just apply the target fill amount.
			barImage.fillAmount = targetFill;

			// Call the functions for the options.
			UpdateOptions();
		}
		// Else the user is using Smooth Fill while in Play Mode.
		else
		{
			// If the Smooth Fill function is already running, then just reset the values.
			if( isSmoothing == true )
				_resetSmoothing = true;
			// Else Start the Coroutine to show the smooth fill.
			else
				StartCoroutine( "SmoothFill" );
		}
	}

	/// <summary>
	/// Updates the color of the bar with the target color.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the bar.</param>
	public void UpdateColor ( Color targetColor )
	{
		// If the color is not set to single, then return.
		if( colorMode != ColorMode.Single || barImage == null )
			return;

		// Set the bar color to the new target color and apply it to the bar.
		barColor = targetColor;
		barImage.color = barColor;
	}

	/// <summary>
	/// Updates the gradient of the bar with the target gradient.
	/// </summary>
	/// <param name="targetGradient">The target gradient to apply to the bar.</param>
	public void UpdateColor ( Gradient targetGradient )
	{
		// If the color is not set to gradient, then return.
		if( colorMode != ColorMode.Gradient || barImage == null )
			return;

		barGradient = targetGradient;
		UpdateGradient();
	}

	/// <summary>
	/// Updates the color of the text associated with the bar.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the text component.</param>
	public void UpdateTextColor ( Color targetColor )
	{
		// If the user is not wanting the text to be displayed, or the text component is not assigned, then return.
		if( displayText == DisplayText.Disabled || barText == null)
			return;

		// Set the text color to the new target color and apply it to the text component.
		barText.color = targetColor;
	}
	#endregion

	#region PUBLIC STATIC FUNCTIONS
	/// <summary>
	/// Returns the Simple Health Bar component that has been registered with the statusName.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	public static SimpleHealthBar GetSimpleHealthBar ( string barName )
	{
		if( !UltimateHealthBarRegistered( barName ) )
			return null;

		return SimpleHealthBars[ barName ];
	}

	/// <summary>
	/// Updates the targeted health bar with the current and max values.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="currentValue">The current value of the bar.</param>
	/// <param name="maxValue">The max value of the bar.</param>
	public static void UpdateBar ( string barName, float currentValue, float maxValue )
	{
		if( !UltimateHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateBar( currentValue, maxValue );
	}

	/// <summary>
	/// Updates the color of the targeted health bar with the target color.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="targetColor">The target color to apply to the image.</param>
	public static void UpdateColor ( string barName, Color targetColor )
	{
		if( !UltimateHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateColor( targetColor );
	}

	/// <summary>
	/// Updates the color of the targeted health bar with the target gradient.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="targetGradient">The target gradient to apply to the image.</param>
	public static void UpdateColor ( string barName, Gradient targetGradient )
	{
		if( !UltimateHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateColor( targetGradient );
	}

	/// <summary>
	/// Updates the color of the text component associated with the targeted health bar.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="targetColor">The target color to apply to the text.</param>
	public static void UpdateTextColor ( string barName, Color targetColor )
	{
		if( !UltimateHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateTextColor( targetColor );
	}
	#endregion
}