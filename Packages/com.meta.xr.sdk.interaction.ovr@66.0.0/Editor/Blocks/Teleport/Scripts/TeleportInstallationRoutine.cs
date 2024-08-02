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

using Oculus.Interaction.Editor.QuickActions;
using Oculus.Interaction.Locomotion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Meta.XR.BuildingBlocks.Editor
{
    public class TeleportInstallationRoutine : InstallationRoutine
    {
        protected override bool UsesPrefab => false;

        public enum InteractableVariant
        {
            Hotspot,
            NavMesh,
            PhysicsLayerBlocker
        }

        [SerializeField]
        [Variant(Description = "Select the type of Teleport interactable that you want to create:\n" +
            "[Hotspot]: Creates a Hotspot that snaps the feet of the player upon selection.\n" +
            "[Nav Mesh]: Allows the player to teleport around the Unity Walkable NavMesh. Bake your NavMesh in the Window/AI/Navigation menu.\n" +
            "[Physics Layer Blocker]: Prevents the Teleport arc from passing throught any collider in the \"Default\", \"UI\", and \"Ignore Raycast\" layers")]
        public InteractableVariant variant = InteractableVariant.Hotspot;

        public override List<GameObject> Install(BlockData blockData, GameObject selectedObject)
        {
            if (!InteractableInjectors.TryGetValue(variant, out TeleportCreationData creationData))
            {
                throw new KeyNotFoundException(nameof(variant));
            }

            if (selectedObject == null)
            {
                // Install on empty GameObject
                selectedObject = new GameObject(creationData.defaultName);
            }

            if (selectedObject == null)
            {
                throw new ArgumentNullException(nameof(selectedObject));
            }

            IEnumerable<GameObject> createdObjects = creationData.interactionCreator(selectedObject);
            List<GameObject> blocks = createdObjects
                .Where(block => block.GetComponent<TeleportInteractable>() != null)
                .ToList();
            blocks.ForEach(block => block.name = $"{Utils.BlockPublicTag} {name}");

            Undo.RegisterFullObjectHierarchyUndo(selectedObject, $"Installing {nameof(TeleportInteractable)} on {selectedObject.name}");

            return blocks;
        }

        private class TeleportCreationData
        {
            public string defaultName;

            public delegate IEnumerable<GameObject> CreateInteractionDelegate(GameObject root);
            public CreateInteractionDelegate interactionCreator;

            public TeleportCreationData(string defaultName, CreateInteractionDelegate creator)
            {
                this.defaultName = defaultName;
                this.interactionCreator = creator;
            }
        }

        private static readonly Dictionary<InteractableVariant, TeleportCreationData>
           InteractableInjectors = new Dictionary<InteractableVariant, TeleportCreationData>
           {
                {
                    InteractableVariant.Hotspot,
                    new TeleportCreationData("Teleport Hotspot",
                        (selectedObject) =>  QuickActionsWizard.CreateWithDefaults<TeleportWizard>(selectedObject, true, (wizard) =>
                        {
                            wizard.InjectOptionalDeviceTypes(DeviceTypes.All);
                            wizard.InjectOptionalHotspotType(null, TeleportWizard.TeleportHotspotSnapType.SnapPosition, null);
                        }))
                },
                {
                    InteractableVariant.NavMesh,
                    new TeleportCreationData("Teleport NavMesh",
                        (selectedObject) => {
                            var walkable = QuickActionsWizard.CreateWithDefaults<TeleportWizard>(selectedObject, true, (wizard) =>
                            {
                                wizard.InjectOptionalDeviceTypes(DeviceTypes.All);
                                wizard.InjectOptionalNavMeshType("Walkable");
                            });
                            var nonWalkable = QuickActionsWizard.CreateWithDefaults<TeleportWizard>(selectedObject, true, (wizard) =>
                            {
                                wizard.InjectOptionalAllowTeleport(false);
                                wizard.InjectOptionalNavMeshType("Non Walkable");
                            });

                            return walkable.Concat(nonWalkable);
                        })
                },
                {
                    InteractableVariant.PhysicsLayerBlocker,
                    new TeleportCreationData("Teleport Physics Blocker",
                        (selectedObject) =>  QuickActionsWizard.CreateWithDefaults<TeleportWizard>(selectedObject, true, (wizard) =>
                        {
                            wizard.InjectOptionalDeviceTypes(DeviceTypes.All);
                            wizard.InjectOptionalAllowTeleport(false);
                            wizard.InjectOptionalPhysicsLayerType(LayerMask.GetMask("Default", "UI", "Ignore Raycast"));
                        }))
                }
           };
    }
}
