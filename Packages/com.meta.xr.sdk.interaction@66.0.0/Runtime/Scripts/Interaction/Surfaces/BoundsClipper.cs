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

using UnityEngine;

namespace Oculus.Interaction.Surfaces
{
    /// <summary>
    /// Clips a <cref"PlaneSurface" />. Each BoundsClipper represents a bounding box, similar to Unity’s BoxCollider.
    /// </summary>
    public class BoundsClipper : MonoBehaviour, IBoundsClipper
    {
        /// <summary>
        /// The position offset from transform origin in local space.
        /// </summary>
        [Tooltip("The offset of the bounding box center relative to " +
            "the transform origin, in local space.")]
        [SerializeField]
        private Vector3 _position = Vector3.zero;

        /// <summary>
        /// The size of the bounding box in local space.
        /// </summary>
        [Tooltip("The size of the bounding box in local space.")]
        [SerializeField]
        private Vector3 _size = Vector3.one;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector3 Size
        {
            get => _size;
            set => _size = value;
        }

        public bool GetLocalBounds(Transform localTo, out Bounds bounds)
        {
            Vector3 localPos = localTo.InverseTransformPoint(
                transform.TransformPoint(Position));
            Vector3 localSize = localTo.InverseTransformVector(
                transform.TransformVector(_size));
            bounds = new Bounds(localPos, localSize);
            return isActiveAndEnabled;
        }
    }
}
