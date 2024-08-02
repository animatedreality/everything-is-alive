/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEditor;
using UnityEngine;
using System.ComponentModel;
using Oculus.Interaction.Grab;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using System.Linq;

namespace Oculus.Interaction.Editor.QuickActions
{
    internal class GrabWizard : QuickActionsWizard
    {
        private const string MENU_NAME = MENU_FOLDER +
            "Add Grab Interaction";

        [MenuItem(MENU_NAME, priority = MenuOrder.ORDER_GRAB)]
        private static void OpenWizard()
        {
            ShowWindow<GrabWizard>(Selection.gameObjects[0]);
        }

        [MenuItem(MENU_NAME, true)]
        static bool Validate()
        {
            return Selection.gameObjects.Length == 1;
        }

        #region Fields

        [SerializeField]
        [DeviceType, WizardSetting]
        [InspectorName("Add Required Interactor(s)")]
        [Tooltip("The interactors required for the new interactable will be " +
            "added for the device types selected here, if not already present.")]
        private DeviceTypes _deviceTypes = DeviceTypes.All;

        [SerializeField]
        [InspectorName("Supported Grab Types")]
        [Tooltip("The types of grab that will be supported by this interactable.")]
        [WizardSetting]
        [DefaultValue(GrabTypeFlags.All)]
        private GrabTypeFlags _grabTypeFlags;

        [SerializeField]
        [WizardSetting]
        [BooleanDropdown(False = "Do Not Generate Collider", True = "Generate Collider")]
        [Tooltip("If a collider is not present under the provided RigidBody, " +
            "a collider will be auto-generated.")]
        private bool _autoGenerateCollider = true;

        [SerializeField]
        [Tooltip("The transform to be moved when grabbing the object.")]
        [WizardDependency(FindMethod = nameof(FindTransform), FixMethod = nameof(FixTransform))]
        private Transform _targetTransform;

        [SerializeField]
        [Tooltip("The rigidbody representing the physics object that will be moved.")]
        [WizardDependency(FindMethod = nameof(FindRigidbody), FixMethod = nameof(FixRigidbody))]
        private Rigidbody _rigidbody;

        #endregion Fields

        private void FindTransform()
        {
            _targetTransform = Target.GetComponent<Transform>();
        }

        private void FixTransform()
        {
            FindTransform();
        }

        private void FindRigidbody()
        {
            _rigidbody = Target.GetComponent<Rigidbody>();
        }

        private void FixRigidbody()
        {
            _rigidbody = AddComponent<Rigidbody>(Target);
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }

        private bool HasCollider()
        {
            return _rigidbody != null && _rigidbody.gameObject.
                GetComponentInChildren<Collider>() != null;
        }

        private void GenerateCollider(GameObject root)
        {
            Collider collider;
            if (Utils.TryEncapsulateRenderers(root,
                out Bounds localBounds))
            {
                var boxCollider = AddComponent<BoxCollider>(root);
                boxCollider.center = localBounds.center;
                boxCollider.size = localBounds.size;
                collider = boxCollider;
            }
            else
            {
                collider = AddComponent<SphereCollider>(root);
            }
            collider.isTrigger = true;
        }

        protected override void Create()
        {
            GameObject obj = Templates.CreateFromTemplate(
                Target.transform, Templates.HandGrabInteractable);

            Transform transform = obj.transform;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;

            HandGrabInteractable handInteractable =
                obj.GetComponent<HandGrabInteractable>();

            handInteractable.InjectRigidbody(_rigidbody);
            handInteractable.InjectSupportedGrabTypes(_grabTypeFlags);

            GrabInteractable grabInteractable =
                obj.GetComponent<GrabInteractable>();

            grabInteractable.InjectRigidbody(_rigidbody);

            Grabbable grabbable = obj.GetComponent<Grabbable>();
            grabbable.InjectOptionalTargetTransform(_targetTransform);
            grabbable.InjectOptionalRigidbody(_rigidbody);

            if (!HasCollider() && _autoGenerateCollider)
            {
                GenerateCollider(_rigidbody.gameObject);
            }

            InteractorUtils.AddInteractorsToRig(
                InteractorTypes.Grab, _deviceTypes);
        }

        protected override IEnumerable<MessageData> GetMessages()
        {
            IEnumerable<MessageData> messages = Enumerable.Empty<MessageData>();
            if (_rigidbody != null && !HasCollider() && !_autoGenerateCollider)
            {
                messages = messages.Append(new MessageData(MessageType.Warning,
                    "No collider(s) detected in the target rigidbody hierarchy. Grab " +
                    "interaction uses physics collisions to find interactables in the scene, " +
                    "and will not work without a collider(s) present.",
                    new ButtonData("Enable Auto\nGeneration", () => _autoGenerateCollider = true)));
            }
            return messages;
        }
    }
}
