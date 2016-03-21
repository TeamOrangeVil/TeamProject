/******************************************************************************
 * Spine Runtimes Software License
 * Version 2.3
 * 
 * Copyright (c) 2013-2015, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable and
 * non-transferable license to use, install, execute and perform the Spine
 * Runtimes Software (the "Software") and derivative works solely for personal
 * or internal use. Without the written permission of Esoteric Software (see
 * Section 2 of the Spine Software License Agreement), you may not (a) modify,
 * translate, adapt or otherwise create derivative works, improvements of the
 * Software or develop new applications using the Software or (b) remove,
 * delete, alter or obscure any trademarks or any copyright, trademark, patent
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoneFollower))]
public class BoneFollowerInspector : Editor {
	SerializedProperty boneName, skeletonRenderer, followZPosition, followBoneRotation;
	BoneFollower component;
	bool needsReset;

	void OnEnable () {
		skeletonRenderer = serializedObject.FindProperty("skeletonRenderer");
		boneName = serializedObject.FindProperty("boneName");
		followBoneRotation = serializedObject.FindProperty("followBoneRotation");
		followZPosition = serializedObject.FindProperty("followZPosition");
		component = (BoneFollower)target;
		component.skeletonRenderer.Initialize(false);
	}
		
	override public void OnInspectorGUI () {
		if (needsReset) {
			component.Reset();
			component.DoUpdate();
			needsReset = false;
			SceneView.RepaintAll();
		}
		serializedObject.Update();

		// FindRenderer()
		if (skeletonRenderer.objectReferenceValue == null) {
			SkeletonRenderer parentRenderer = SkeletonUtility.GetInParent<SkeletonRenderer>(component.transform);

			if (parentRenderer != null) {
				skeletonRenderer.objectReferenceValue = (UnityEngine.Object)parentRenderer;
			}
		}

		EditorGUILayout.PropertyField(skeletonRenderer);

		if (component.valid) {
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(boneName);
			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
				needsReset = true;
				serializedObject.Update();
			}
				
			EditorGUILayout.PropertyField(followBoneRotation);
			EditorGUILayout.PropertyField(followZPosition);
		} else {
			GUILayout.Label("INVALID");
		}

		if (serializedObject.ApplyModifiedProperties() ||
			(Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
	    ) {
			component.Reset();
		}
	}
}
