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

using Oculus.Interaction.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
    public class HandTrackingConfidenceProvider : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IInteractor))]
        private UnityEngine.Object _interactor;
        private IInteractor Interactor;

        [SerializeField, Interface(typeof(IHand))]
        private UnityEngine.Object _hand;
        private IHand Hand { get; set; }

        private static Dictionary<int, HandTrackingConfidenceProvider> _interactorTrackingConfidence;

        protected bool _started;

        #region Editor Callbacks

        protected virtual void Reset()
        {
            _interactor = this.GetComponent<IInteractor>() as UnityEngine.Object;
            _hand = this.GetComponent<IHand>() as UnityEngine.Object;
        }

        #endregion

        protected virtual void Awake()
        {
            if (_interactorTrackingConfidence == null)
            {
                _interactorTrackingConfidence = new Dictionary<int, HandTrackingConfidenceProvider>();
            }

            Interactor = _interactor as IInteractor;
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            this.AssertAspect(Interactor, nameof(_interactor));
            this.AssertAspect(Hand, nameof(_hand));

            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                int key = Interactor.Identifier;
                if (_interactorTrackingConfidence != null && !_interactorTrackingConfidence.ContainsKey(key))
                {
                    _interactorTrackingConfidence.Add(key, this);
                }
                else
                {
                    Debug.LogError($"This interactor was already added to {nameof(HandTrackingConfidenceProvider)}. "
                        + $"Ensure each interactor is paired just once" );
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                int key = Interactor.Identifier;
                if (_interactorTrackingConfidence != null && _interactorTrackingConfidence.ContainsKey(key))
                {
                    _interactorTrackingConfidence.Remove(Interactor.Identifier);
                }
            }
        }

        public static bool TryGetTrackingConfidence(int key, out bool isTrackingHighConfidence)
        {
            if (_interactorTrackingConfidence != null && _interactorTrackingConfidence.ContainsKey(key))
            {
                isTrackingHighConfidence = _interactorTrackingConfidence[key].Hand.IsHighConfidence;
                return true;
            }
            isTrackingHighConfidence = true;
            return false;
        }

        #region Inject

        public void InjectAllHandTrackingConfidenceProvider(IInteractor interactor, IHand hand)
        {
            InjectInteractor(interactor);
            InjectHand(hand);
        }

        public void InjectInteractor(IInteractor interactor)
        {
            _interactor = interactor as UnityEngine.Object;
            Interactor = interactor;
        }
        public void InjectHand(IHand hand)
        {
            _hand = hand as UnityEngine.Object;
            Hand = hand;
        }

        #endregion
    }
}
