/* Written by Kaz Crowe */
/* FollowCameraRotation.cs */
using UnityEngine;

namespace SimpleHealthBarUtility
{
	[AddComponentMenu( "UI/Simple Health Bar/Follow Camera Rotation" )]
	public class FollowCameraRotation : MonoBehaviour
	{
		public enum FindBy{ Transform, Name, Tag }
		public FindBy findBy = FindBy.Transform;
		public string targetName = "Main Camera";
		public Transform cameraTransform;
		RectTransform baseTransform;


		void Start ()
		{
			baseTransform = GetComponent<RectTransform>();

			if( baseTransform == null )
				Debug.LogError( "SimpleHealthBarUtility.FollowCameraRotation - There is no RectTransform attached to this object. Please make sure that this script is attached to a UI GameObject." );
		}

		void Update ()
		{
			if( baseTransform == null )
				return;

			// If the camera transform is null, then find the camera according to the user's options.
			if( cameraTransform == null )
				cameraTransform = FindCamera();
			// Else, make the canvas look at the camera.
			else
				baseTransform.LookAt( baseTransform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up );
		}

		/// <summary>
		/// This function will return the targeted camera according to the users options.
		/// </summary>
		Transform FindCamera ()
		{
			// Create a temporary transform component.
			Transform tempTrans = null;
		
			// If the user is wanting to find the camera by name, then use the GameObject.Find function.
			if( findBy == FindBy.Name && GameObject.Find( targetName ) )
				tempTrans = GameObject.Find( targetName ).GetComponent<Transform>();
			// Else if the user is wanting to find the camera by tag, then use the GameObject.FindGameObjectWithTag function.
			else if( findBy == FindBy.Tag && GameObject.FindGameObjectWithTag( targetName ) )
				tempTrans = GameObject.FindGameObjectWithTag( targetName ).GetComponent<Transform>();

			// If the temporary transform is still null, then inform the user.
			if( tempTrans == null )
				Debug.LogError( "SimpleHealthBarUtility.FollowCameraRotation - Could not locate the targeted camera for the gameObject: " + gameObject.name + ". Please make sure that the Transform/Name/Tag is correct for the FindBy type." );

			return tempTrans;
		}
	}
}