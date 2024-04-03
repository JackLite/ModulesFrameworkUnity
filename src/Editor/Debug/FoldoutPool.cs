using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug
{
    public class FoldoutPool
    {
        private readonly LinkedList<FoldoutWrapper> _foldouts = new();
        private readonly int _incCount;
        private readonly VisualElement _parent;
        public FoldoutPool (int size, VisualElement parent)
        {
            _incCount = size / 2 + 1;
            _parent = parent;
            Fill(size);
        }
        
        public void Return(FoldoutWrapper wrapper)
        {
            wrapper.foldout.style.opacity = 0;
            _foldouts.AddFirst(wrapper);
        }
        
        public FoldoutWrapper Pop()
        {
            if(_foldouts.First == null)
                Fill(_incCount);
            var fd = _foldouts.First.Value;
            fd.foldout.style.opacity = 1;
            fd.foldout.SetValueWithoutNotify(false);
            fd.Reset();
            _foldouts.RemoveFirst();
            return fd;
        }
        
        private void Fill(int size)
        {
            for (int i = 0; i < size; i++)
            {
                var foldout = new Foldout();
                foldout.style.opacity = 0;
                _foldouts.AddLast(new FoldoutWrapper(foldout));
                _parent.Add(foldout);
            }
        }
    }
}