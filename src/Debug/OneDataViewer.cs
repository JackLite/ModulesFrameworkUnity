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
        internal OneData ChangedData { get; private set; }

        public void Init(Type type, OneData oneData)
        {
            name = $"{type.Name} [Gen {_generation.ToString()}]";
            DataType = type;
            Data = oneData;
            ChangedData = oneData.Copy();
            _generation++;
        }

        public void UpdateData(object changed)
        {
            Data.SetDataObject(changed);
            ChangedData = Data.Copy();
        }
    }
}