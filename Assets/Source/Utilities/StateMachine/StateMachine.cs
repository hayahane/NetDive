using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetDive.Utilities.StateMachine
{
    public abstract class StateMachine<T> where T: class, IState
    {
        public String currentStateName;
        public T CurrentState { get; private set; }
        private readonly Dictionary<string, T> _statePool = new();

        public void TransitTo(T nextState)
        {
            CurrentState?.OnExit();
            CurrentState = nextState;
            CurrentState.OnEnter();
        }

        public void TransitTo(String stateName)
        {
            var isNameValid = _statePool.TryGetValue(stateName, out var nextState);
            if (!isNameValid)
            {
                Debug.LogError($"Can't find a state named {stateName}");
                return;
            }
#if UNITY_EDITOR
            //Debug.Log($"Change to state:{stateName}");
#endif
            currentStateName = stateName;
            TransitTo(nextState);
        }

        public bool AddState(T state, String name)
        {
            return _statePool.TryAdd(name, state);
        }

        public bool RemoveState(String name)
        {
            return _statePool.Remove(name);
        }

        #region Update State Machine
        
        public void Update()
        {
            CurrentState?.Update();
        }

        public void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }
        
        #endregion
    }
}
