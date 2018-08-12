using UnityEngine;
using System.Collections;
using System;

using UnityEditor;
using Pipes;


namespace OSC
{
    public abstract class IChannel: MonoBehaviour
    {
        protected abstract bool Evaluate();
        public abstract float GetValue();
        protected abstract bool Changed();
    }

    public class ChannelIn : IChannel
    {

        [Header("Setup")]
        [SerializeField] public OSC.Receiver receiver;
        public string address;


        [Header("Value")]
        public float defaultValue = 0;
        public float value = 0;
        float prev = float.MinValue;
        bool changed = false;

        protected override bool Changed() { return changed; }

        public float GetOutput()
        {
            if (receiver != null)
            {
                object msgValue = receiver.GetValue(address);
                if (msgValue != null)
                {
                    if (msgValue.GetType() == typeof(int))
                    {
                        return (int)msgValue;
                    }
                    else if (msgValue.GetType() == typeof(float))
                    {
                        return (float)msgValue;
                    }
                }
            }
            return defaultValue;
        }
        protected void Start()
        {
            
            InitializeConnections();
            
        }
        protected void InitializeConnections()
        {
            receiver.AddChannel(address, defaultValue);
        }

        void Update()
        {
            changed = Evaluate();
        }

        protected override bool Evaluate()
        {
            object obj = receiver.GetValue(address);
            if (obj == null)
            {
                return false;
            }
            value = (float)obj;

            bool change = prev != value;
            if (change)
            {
                prev = value;
            }
            return change;
        }

        public override float GetValue()
        {
            return value;
        }
    }

}