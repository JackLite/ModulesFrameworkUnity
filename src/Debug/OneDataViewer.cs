using System;
using ModulesFramework;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class OneDataViewer : MonoBehaviour
    {
        private int _generation;
        internal Type DataType { get; private set; }
        internal OneData Data { get; private set; }

        internal event Action OnUpdate;

        public void Init(Type type, OneData oneData)
        {
            name = $"{type.Name} [Gen {_generation.ToString()}]";
            DataType = type;
            Data = oneData;
            oneData.Copy();
            _generation++;
        }

        public void UpdateData(object changed)
        {
            Data.SetDataObject(changed);
            Data.Copy();
        }

        internal void RiseUpdate()
        {
            OnUpdate?.Invoke();
        }
    }
}