using System;
using ModulesFramework;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class OneDataViewer : MonoBehaviour
    {
        internal Type DataType { get; private set; }
        internal OneData Data { get; private set; }

        public void Init(Type type, OneData oneData)
        {
            DataType = type;
            Data = oneData;
        }
    }
}