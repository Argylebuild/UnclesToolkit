using UnityEngine;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit.Control.MrtkPlaceholder
{
	public class Pile : ArgyleComponent
	{
		public UnityEvent<string> OnInteractionStarted;
		public UnityEvent<string> OnInteractionEnded;
		
		public Vector3 SliderStartPosition { get; set; }
		public Vector3 SliderEndPosition { get; set; }
		
		public float SliderValue { get; set; }
	}
	
	public class MixedRealityPointerEventData
	{
		public MixedRealityPointerEventData(UnityEngine.EventSystems.EventSystem current)
		{
		}
		
		//pointer 
		public PointerEventData Pointer { get; set; }
	}
	
	public class ManipulationEventData
	{
		public ManipulationEventData(UnityEngine.EventSystems.EventSystem current)
		{
		}
	}
	
	public class ManipulationHandler : ArgyleComponent
	{
		public bool enabled { get; set; }
		
		public UnityEvent<ManipulationEventData> OnManipulationStarted = new UnityEvent<ManipulationEventData>();
		public UnityEvent<ManipulationEventData> OnManipulationEnded = new UnityEvent<ManipulationEventData>();
	}
	
	public class ObjectManipulator : ArgyleComponent
	{
		public bool enabled { get; set; }
	}
	
	public class FocusEventData
	{
		public FocusEventData(UnityEngine.EventSystems.EventSystem current)
		{
		}
	}
	
	public interface IMixedRealityFocusHandler
	{
		void OnFocusEnter(FocusEventData eventData);
		void OnFocusExit(FocusEventData eventData);
	}
	
	public interface IMixedRealityPointerHandler
	{
		void OnPointerDown(MixedRealityPointerEventData eventData);
		void OnPointerUp(MixedRealityPointerEventData eventData);
	}
	
	public interface IMixedRealityInputHandler
	{
	}
	public class InputEventData
	{
		public InputEventData(UnityEngine.EventSystems.EventSystem current)
		{
		}
	}
	
	
	public class Interactable : ArgyleComponent
	{
		public UnityEvent OnClick;
		
		public void TriggerOnClick()
		{
			OnClick?.Invoke();
		}
		
		public void SetState(InteractableStates.InteractableStateEnum state, bool value)
		{
		}
		
		//oninputdown
		public void OnInputDown(InputEventData eventData)
		{
		}
		
		public bool IsToggled { get; set; }
	}
	
	public class InteractableStates
	{
		public enum InteractableStateEnum
		{
			Toggled
		}
	}

	public class PointerEventData
	{
		public PointerEventData(UnityEngine.EventSystems.EventSystem current)
		{
		}
		public PointerEventResult Result { get; set; }
		
	}
	
	public class PointerEventResult
	{
		public PointerEventResult(UnityEngine.EventSystems.EventSystem current)
		{
		}
		public PointerEventDetails Details { get; set; }
		
	}
	
	public class PointerEventDetails
	{
		public PointerEventDetails(UnityEngine.EventSystems.EventSystem current)
		{
		}
		public Vector3 Point { get; set; }
	}
	
	public class ScrollingObjectCollection : ArgyleComponent
	{
		public Transform transform { get; set; }
		public void MoveByTiers(int amount)
		{
		}
		
		public void MoveByPages(int amount)
		{
		}
		
		public void UpdateContent()
		{
		}
	}
	
	public class SolverHandler : ArgyleComponent
	{
		public Handedness CurrentTrackedHandedness { get; set; }
	}
	
	public class HandConstraintPalmUp : ArgyleComponent
	{
		public string Handedness { get; set; }
	}
	
	public enum Handedness
	{
		Left,
		Right
	}
	
	public class GridObjectCollection : ArgyleComponent
	{
		public Transform transform { get; set; }
		
		public void UpdateCollection()
		{
		}
	}
	
	public class FollowMeToggle : ArgyleComponent
	{
		public void SetFollowMeBehavior(bool _defaultFollowMe)
		{
		}
	}
	
	public class PinchSlider : ArgyleComponent
	{
		
		public UnityEvent<SliderEventData> OnValueUpdated;
		public UnityEvent<SliderEventData> OnInteractionEnded;
		public float SliderValue { get; set; }

		public GameObject ThumbVisuals;
	}
	
	public class SliderEventData
	{
		public SliderEventData(UnityEngine.EventSystems.EventSystem current)
		{
		}
		public float NewValue { get; set; }
	}
	
	public class CoreServices
	{
		public static InputSystem InputSystem { get; set; }
	}
	
	public class InputSystem
	{
		public InputSource[] DetectedInputSources { get; set; }
		public void RaisePointerUp(Pointer pointer, MixedRealityInputAction none)
		{
		}
	}
	
	public class MixedRealityInputAction
	{
	}
	
	public class InputSource
	{
		public Pointer[] Pointers { get; set; }
	}
	
	public class Pointer
	{
		public BaseCursor BaseCursor { get; set; }
	}
	
	public class BaseCursor
	{
		public bool IsVisible { get; set; }
	}
	
}