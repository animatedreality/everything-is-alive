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

using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction
{
    /// <summary>
    /// An ActiveState that is active when the controller matches the interaction profile.
    /// </summary>
    [Feature(Feature.Interaction)]
    public class OVRControllerMatchesProfileActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private OVRInput.Controller _controller;

        [SerializeField]
        private OVRInput.InteractionProfile _profile;

        public bool Active
        {
            get
            {
                OVRInput.Hand ovrHandedness = _controller.HasFlag(OVRInput.Controller.LTouch) || _controller.HasFlag(OVRInput.Controller.LHand) ? OVRInput.Hand.HandLeft : OVRInput.Hand.HandRight;
                OVRInput.InteractionProfile ovrProfile = OVRInput.GetCurrentInteractionProfile(ovrHandedness);
                return ovrProfile == _profile;
            }
        }

        #region Inject

        public void InjectAllOVRControllerSupportsPressure(OVRInput.Controller controller)
        {
            _controller = controller;
        }

        #endregion
    }
}
