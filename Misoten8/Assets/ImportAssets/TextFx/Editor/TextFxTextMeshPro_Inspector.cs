using UnityEngine;
using UnityEditor;
using System.Collections;
using TMPro.EditorUtilities;

namespace TextFx
{
	[CustomEditor(typeof(TextFxTextMeshPro)), CanEditMultipleObjects]
	public class TextFxTextMeshPro_Inspector : TMP_EditorPanel {

		TextFxTextMeshPro tmpEffect;
		TextFxAnimationManager animationManager;

		new void OnEnable()
		{
			tmpEffect = (TextFxTextMeshPro) target;
			animationManager = tmpEffect.AnimationManager;

			EditorApplication.update += UpdateManager;

			base.OnEnable ();
		}

		new void OnDisable()
		{
			EditorApplication.update -= UpdateManager;

			base.OnDisable ();
		}

		void UpdateManager()
		{
			TextFxBaseInspector.UpdateManager (animationManager);
		}

		public override void OnInspectorGUI ()
		{
			// Draw TextFx inspector section
			TextFxBaseInspector.DrawTextFxInspectorSection(this, animationManager, ()=> {
				RefreshTextCurveData();
			});

			// Draw default NGUI inspector content
			base.OnInspectorGUI();


		}

		new void OnSceneGUI()
		{
			if (tmpEffect.RenderToCurve && tmpEffect.BezierCurve.EditorVisible)
			{
				tmpEffect.OnSceneGUIBezier (animationManager.Transform.position, animationManager.Scale * animationManager.AnimationInterface.MovementScale);

				if (GUI.changed)
				{
					RefreshTextCurveData ();
				}
			}
		}

		void RefreshTextCurveData()
		{
			animationManager.CheckCurveData ();

			// Update mesh values to latest using new curve offset values
			tmpEffect.ForceUpdateCachedVertData();


			tmpEffect.UpdateTextFxMesh();
		}
	}
}
