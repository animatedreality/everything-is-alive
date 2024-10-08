﻿/*
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

namespace Oculus.Interaction
{
    /// <summary>
    /// Moves the selected interactable 1 to 1 with the interactor. For example, if your interactor moves up and to the right, the selected interactable will also move up and to the right.
    /// </summary>
    public class MoveFromTargetProvider : MonoBehaviour, IMovementProvider
    {
        public IMovement CreateMovement()
        {
            return new MoveFromTarget();
        }
    }

    public class MoveFromTarget : IMovement
    {
        public Pose Pose { get; private set; } = Pose.identity;
        public bool Stopped => true;

        public void StopMovement()
        {
        }

        public void MoveTo(Pose target)
        {
            Pose = target;
        }

        public void UpdateTarget(Pose target)
        {
            Pose = target;
        }

        public void StopAndSetPose(Pose source)
        {
            Pose = source;
        }

        public void Tick()
        {
        }
    }
}
