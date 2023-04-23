using System;
using Quick;
using TMPro;
using UnityEngine;

namespace TF.Runtime
{

    /// <summary>
    /// 所有场景物品的基类
    /// </summary>
    public abstract class ISceneObject : MonoBehaviour
    {
        public string id;
        public string desc;

        private ICommandReceiver _receiver;

        protected ICommandReceiver Receiver
        {
            get
            {
                if (_receiver == null)
                {
                    _receiver = new ICommandReceiver();
                }

                return _receiver;
            }
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        public ECommandReply Command<T>(T cmd, bool undo = false) where T : ICommand
        {
            return Receiver.Command(cmd, undo);
        }

        protected abstract void AddCommands();
    }
}