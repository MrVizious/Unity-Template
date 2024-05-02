using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using ExtensionMethods;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TypeReferences;
using UnityEngine;

namespace DesignPatterns
{

    public abstract class MonoBehaviourStateMachine<T> : MonoBehaviour, IStateMachine<T> where T : MonoBehaviourState<T>
    {
        protected Stack<T> _stateStack = new Stack<T>();
        [ShowInInspector]
        public virtual Stack<T> stateStack
        {
            get { return _stateStack; }
            protected set { _stateStack = value; }
        }
        [ShowInInspector]
        public virtual T currentState
        {
            get;
            protected set;
        }
        [ShowInInspector]
        public virtual T previousState
        {
            get
            {
                if (stateStack == null || stateStack.Count <= 0) return null;
                return stateStack.Peek();
            }
        }

        protected virtual void Update()
        {
            currentState?.UpdateExecution();
        }
        protected virtual void FixedUpdate()
        {
            currentState?.FixedUpdateExecution();
        }

        [Button]
        public virtual T ChangeToState(string type)
        {
            return ChangeToState(Type.GetType(type));
        }
        [Button]
        public virtual T ChangeToState(Type t)
        {
            Debug.Log("Type is " + t.ToString());
            if (!typeof(T).IsAssignableFrom(t)) throw new ArgumentException("Type " + t + " is not a subtype of " + typeof(T).Name);
            T newState = (T)this.GetOrAddComponent(t);
            return ChangeToState(newState);
        }
        [Button]
        public virtual T ChangeToState(T newState)
        {
            // null check
            if (newState == null) throw new ArgumentNullException("New State to substitute into is null!");

            Type type = newState.GetType();
            T existingInstance = (T)this.GetComponent(type);
            if (existingInstance != null && !existingInstance.Equals(newState)) Destroy(existingInstance);

            // no current state
            if (currentState == null)
            {
                currentState = newState;
                currentState.Enter(this);
                return currentState;
            }

            // trying to change into current state
            if (currentState.Equals(newState)) return currentState;

            // previous state is same as new one
            if (stateStack.Count > 0 && previousState.Equals(newState))
            {
                currentState?.Exit();
                stateStack.Pop();
                stateStack.Push(currentState);
                currentState = newState;
                currentState.Enter(this);
                return currentState;
            }

            currentState?.Exit();
            stateStack.Push(currentState);
            currentState = newState;
            currentState.Enter(this);
            return currentState;
        }

        [Button]
        public virtual T ChangeToPreviousState()
        {
            if (stateStack == null || stateStack.Count <= 0)
            {
                Debug.LogError("Can't go to previous state because it doesn't exist");
                return null;
            }
            currentState?.Exit();
            currentState = stateStack.Pop();
            currentState?.Enter(this);
            return currentState;
        }
        [Button]
        public virtual T SubstituteStateWith(T newState)
        {
            if (newState == null) throw new ArgumentNullException("New State to substitute into is null!");
            if (previousState.Equals(newState))
            {
                return ChangeToPreviousState();
            }
            currentState?.Exit();
            currentState = newState;
            currentState.Enter(this);
            return currentState;
        }
    }

}