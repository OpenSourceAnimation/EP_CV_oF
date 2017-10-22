﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using DlibFaceLandmarkDetector;

namespace DlibFaceLandmarkDetectorSample {

	public class FaceTrackCoords : MonoBehaviour {

        public Vector2 trackCoords = Vector2.zero;
		public float scaleCoords = 0f;
		WebCamTexture webCamTexture;
		WebCamDevice webCamDevice;
		Color32[] colors;
		public bool shouldUseFrontFacing = false;
		int width = 320;
		int height = 240;
		bool initDone = false;
		ScreenOrientation screenOrientation = ScreenOrientation.Unknown;
		FaceLandmarkDetector faceLandmarkDetector;
		Texture2D texture2D;
		bool flip;
           

		void Start() {
			faceLandmarkDetector = new FaceLandmarkDetector(Utils.getFilePath("shape_predictor_68_face_landmarks.dat"));
			StartCoroutine (init ());
   		}

		private IEnumerator init() {
			if (webCamTexture != null) {
				webCamTexture.Stop ();
				initDone = false;
			}
			
			// Checks how many and which cameras are available on the device
			for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++) {
				if (WebCamTexture.devices [cameraIndex].isFrontFacing == shouldUseFrontFacing) {
					Debug.Log (cameraIndex + " name " + WebCamTexture.devices [cameraIndex].name + " isFrontFacing " + WebCamTexture.devices [cameraIndex].isFrontFacing);
 					webCamDevice = WebCamTexture.devices [cameraIndex];
					webCamTexture = new WebCamTexture (webCamDevice.name, width, height);
					break;
				}
			}
			
			if (webCamTexture == null) {
				//			Debug.Log ("webCamTexture is null");
				if (WebCamTexture.devices.Length > 0) {
					webCamDevice = WebCamTexture.devices [0];
					webCamTexture = new WebCamTexture (webCamDevice.name, width, height);
				} else {
					webCamTexture = new WebCamTexture (width, height);
				}
			}
			
			Debug.Log ("width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);
			
			// Starts the camera
			webCamTexture.Play ();

			while (true) {
				//If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/)
				#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
					if (webCamTexture.width > 16 && webCamTexture.height > 16) {
				#else
				if (webCamTexture.didUpdateThisFrame) {
					#if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2                                    
						while (webCamTexture.width <= 16) {
							webCamTexture.GetPixels32 ();
							yield return new WaitForEndOfFrame ();
						} 
					#endif
				#endif
						
					Debug.Log ("width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);
					Debug.Log ("videoRotationAngle " + webCamTexture.videoRotationAngle + " videoVerticallyMirrored " + webCamTexture.videoVerticallyMirrored + " isFrongFacing " + webCamDevice.isFrontFacing);
						
					colors = new Color32[webCamTexture.width * webCamTexture.height];
					texture2D = new Texture2D (webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
					gameObject.GetComponent<Renderer> ().material.mainTexture = texture2D;
					updateLayout ();
					screenOrientation = Screen.orientation;
					initDone = true;
						
					break;
				} else {
					yield return 0;
				}
			}
		}
			
		private void updateLayout() {
			gameObject.transform.localRotation = new Quaternion (0, 0, 0, 0);
			gameObject.transform.localScale = new Vector3 (webCamTexture.width, webCamTexture.height, 1);
				
			if (webCamTexture.videoRotationAngle == 90 || webCamTexture.videoRotationAngle == 270) {
				gameObject.transform.eulerAngles = new Vector3 (0, 0, -90);
			}
				
			float width = 0;
			float height = 0;
			if (webCamTexture.videoRotationAngle == 90 || webCamTexture.videoRotationAngle == 270) {
				width = gameObject.transform.localScale.y;
				height = gameObject.transform.localScale.x;
			} else if (webCamTexture.videoRotationAngle == 0 || webCamTexture.videoRotationAngle == 180) {
				width = gameObject.transform.localScale.x;
				height = gameObject.transform.localScale.y;
			}
				
			float widthScale = (float)Screen.width / width;
			float heightScale = (float)Screen.height / height;
			if (widthScale < heightScale) {
				Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
			} else {
				Camera.main.orthographicSize = height / 2;
			}
				
			flip = true;
			if (webCamDevice.isFrontFacing) {
				if (webCamTexture.videoRotationAngle == 90) {
					flip = !flip;
				}
				if (webCamTexture.videoRotationAngle == 180) {
					flip = !flip;
				}
			} else {
				if (webCamTexture.videoRotationAngle == 180) {
					flip = !flip;
				} else if (webCamTexture.videoRotationAngle == 270) {
					flip = !flip;
				}
			}
			
			if (!flip) {
				gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x, -gameObject.transform.localScale.y, 1);
			}
		}
	
		void Update() {
			if (!initDone) return;
				
			if (screenOrientation != Screen.orientation) {
				screenOrientation = Screen.orientation;
				updateLayout();
			}
				
		#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
			if (webCamTexture.width > 16 && webCamTexture.height > 16) {
		#else
			if (webCamTexture.didUpdateThisFrame) {
		#endif
				webCamTexture.GetPixels32(colors);
				faceLandmarkDetector.SetImage<Color32> (colors, webCamTexture.width, webCamTexture.height, 4, flip);
			
				// detect face rects
				List<Rect> detectResult = faceLandmarkDetector.Detect();
			
				foreach (var rect in detectResult) {
					float x = (rect.x - (float) width/2f) * scaleCoords;
					float y = (rect.y - (float) height/2f) * scaleCoords;
					trackCoords = new Vector2(x,y);

					// detect landmark points
					List<Vector2> points = faceLandmarkDetector.DetectLandmark(rect);
				
					Debug.Log ("face point : " + points.Count);
					if (points.Count > 0) {
						//Debug.Log ("face points : x " + point.x + " y " + point.y);
						// draw landmark points
						faceLandmarkDetector.DrawDetectLandmarkResult<Color32>(colors, webCamTexture.width, webCamTexture.height, 4, flip, 0, 255, 0, 255);
					}
				
					// draw face rect
					faceLandmarkDetector.DrawDetectResult<Color32>(colors, webCamTexture.width, webCamTexture.height, 4, flip, 255, 0, 0, 255, 2);
				}

				texture2D.SetPixels32 (colors);
				texture2D.Apply ();
			}
		}

		void OnDisable() {
			webCamTexture.Stop ();
			faceLandmarkDetector.Dispose ();
		}

		public void OnBackButton() {
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				SceneManager.LoadScene ("DlibFaceLandmarkDetectorSample");
			#else
				Application.LoadLevel ("DlibFaceLandmarkDetectorSample");
			#endif
		}

		public void OnChangeCameraButton() {
			shouldUseFrontFacing = !shouldUseFrontFacing;
			StartCoroutine (init ());
		}

		float tween(float v1, float v2, float e) {
			v1 += (v2-v1)/e;
			return v1;
		}

	}

}
