using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug
{
    public class FoldoutWrapper
    {
        public readonly Foldout foldout;
        public Type type;

        private bool _firstTime;

        public event Action<Type> OnShowedFirstTime;

        public FoldoutWrapper(Foldout foldout)
        {
            this.foldout = foldout;
            foldout.RegisterValueChangedCallback(Callback);
        }

        private void Callback(ChangeEvent<bool> evt)
        {
            if (evt.newValue && !_firstTime)
            {
                _firstTime = true;
                OnShowedFirstTime?.Invoke(type);
            }
        }

        public void Reset()
        {
            _firstTime = false;
            OnShowedFirstTime = null;
            foldout.Clear();
        }
    }
}