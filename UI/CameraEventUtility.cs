using System;
using Argyle.Events;
using Argyle.UnclesToolkit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Argyle.Utilities.UI
{
	/// <summary>
	/// For triggering code to run at very specific frame timing.
	/// If you want to run per-camera render events, this script must sit on the same gameobject as the camera in question.
	/// </summary>
	public class CameraEventUtility : ArgyleComponent
	{
		[Tooltip("Subscribe to this channel any method to be run immediately before frame renders.")]
		public UnityEvent PreFrameChannel;
		[Tooltip("Subscribe to this channel any method to be run immediately after frame renders.")]
		public UnityEvent PostFrameChannel;
		[Tooltip("Subscribe to this channel any method to be run immediately before camera renders.")]
		public UnityEvent PreCameraChannel;
		[Tooltip("Subscribe to this channel any method to be run immediately after camera renders.")]
		public UnityEvent PostCameraChannel;

		/// <summary>
		/// The camera associated with this component. Will run camera events at this cameras render time. 
		/// </summary>
		public Camera Camera
		{
			get
			{
				if (_camera == null)
					_camera = GetComponent<Camera>();

				return _camera;
			}
		}
		private Camera _camera;
		

		private void OnEnable()
		{
			RenderPipelineManager.beginFrameRendering += OnFrameStart;
			RenderPipelineManager.endFrameRendering += OnFrameStop;

			RenderPipelineManager.beginCameraRendering += OnCameraStart;
			RenderPipelineManager.endCameraRendering += OnCameraStop;
		}

		private void OnDisable()
		{
			RenderPipelineManager.beginFrameRendering -= OnFrameStart;
			RenderPipelineManager.endFrameRendering -= OnFrameStop;
			
			RenderPipelineManager.beginCameraRendering -= OnCameraStart;
			RenderPipelineManager.endCameraRendering -= OnCameraStop;
		}

		private void OnFrameStart(ScriptableRenderContext context, Camera[] cameras)
		{
			PreFrameChannel.Invoke();
		}

		private void OnFrameStop(ScriptableRenderContext context, Camera[] cameras)
		{
			PostFrameChannel.Invoke();
		}

		private void OnCameraStart(ScriptableRenderContext context, Camera renderCamera)
		{
			if(renderCamera == Camera)
				PreCameraChannel.Invoke();
		}

		private void OnCameraStop(ScriptableRenderContext context, Camera renderCamera)
		{
			if(renderCamera == Camera)
				PostCameraChannel.Invoke();
		}
	}
}