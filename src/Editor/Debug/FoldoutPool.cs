using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug
{
    public class FoldoutPool
    {
        private readonly LinkedList<Foldout> _foldouts = new();
        private readonly int _incCount;
        private readonly VisualElement _parent;
        public FoldoutPool (int size, VisualElement parent)
        {
            _incCount = size / 2 + 1;
            _parent = parent;
            Fill(size);
        }
        
        public void Return(Foldout foldout)
        {
            foldout.style.opacity = 0;
            _foldouts.AddFirst(foldout);
        }
        
        public Foldout Pop()
        {
            if(_foldouts.First == null)
                Fill(_incCount);
            var fd = _foldouts.First.Value;
            fd.style.opacity = 1;
            fd.value = false;
            _foldouts.RemoveFirst();
            return fd;
        }
        
        private void Fill(int size)
        {
            for (int i = 0; i < size; i++)
            {
                var foldout = new Foldout();
                foldout.style.opacity = 0;
                _foldouts.AddLast(foldout);
                _parent.Add(foldout);
            }
        }
    }
}