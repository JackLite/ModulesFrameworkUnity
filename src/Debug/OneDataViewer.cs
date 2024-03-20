using System;
using System.Globalization;
using ModulesFramework;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class OneDataViewer : MonoBehaviour
    {
        internal Type DataType { get; private set; }
        internal OneData Data { get; private set; }

        internal event Action OnUpdate;

        public void Init(Type type, OneData oneData)
        {
            name = $"{type.Name} [Gen {oneData.generation.ToString(CultureInfo.InvariantCulture)}]";
            DataType = type;
            Data = oneData;
            oneData.Copy();
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