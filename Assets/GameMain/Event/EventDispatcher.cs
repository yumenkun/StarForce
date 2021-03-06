using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using KEngine;

namespace Utils
{
    /// <summary>
    /// 事件处理类
    /// </summary>
#if UNITY_EDITOR &&!SCENE_DEBUG
#endif
    public class EventDispatcher
#if MEMORY_CHECK
        : MemoryObject
#endif
    {
        private static Type[] EventDefines = { typeof(ActorEventDefine) };

        static protected EventDispatcher sInstance = new EventDispatcher();
        static public EventDispatcher Instance { get { return sInstance; } }


        #region 内部实现
        private Dictionary<string, Delegate> m_theRouter = new Dictionary<string, Delegate>();

        //处理在派发事件时吧监听回调移除后还会回调的bug
        List<Delegate> m_tempDeleteCall = new List<Delegate>();
        int m_tempIsTriggerFlg = 0;

        public static void InitEventDefine()
        {
            foreach (Type t in EventDefines)
            {
                FieldInfo[] fis = t.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo fi in fis)
                {
                    fi.SetValue(t, fi.Name);
                }
            }
        }

        /// <summary>
        /// 清除事件
        /// </summary>
        public void Clear()
        {
            List<string> eventToRemove = new List<string>();

            foreach (KeyValuePair<string, Delegate> pair in m_theRouter)
            {
                eventToRemove.Add(pair.Key);
            }

            foreach (string Event in eventToRemove)
            {
                m_theRouter.Remove(Event);
            }

//            m_votes.Clear();
//            m_voteIdxById.Clear();
        }

        /// <summary>
        /// 处理增加监听器前的事项， 检查 参数等
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                m_theRouter.Add(eventType, null);
            }

#if COM_DEBUG || UNITY_EDITOR
            Delegate d = m_theRouter[eventType];
            if (d == null)
                return;

            if (d.GetType() != listenerBeingAdded.GetType())
            {
                throw new Exception(string.Format(
       "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
       eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }

            var callbacks = d.GetInvocationList();
            foreach (Delegate p in callbacks)
            {
                if (p == listenerBeingAdded)
                {
                    Debuger.LogError("重复添加监听回调 " + listenerBeingAdded.Method.ToString());
                }
            }
#endif
        }

        /// <summary>
        /// 移除监听器之前的检查
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingRemoved"></param>
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                return false;
            }

            m_tempDeleteCall.Add(listenerBeingRemoved);
            Delegate d = m_theRouter[eventType];
            if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new Exception(string.Format(
                    "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                    eventType, d.GetType(), listenerBeingRemoved.GetType()));
            }
            else
                return true;
        }

        /// <summary>
        /// 移除监听器之后的处理。删掉事件
        /// </summary>
        /// <param name="eventType"></param>
        private void OnListenerRemoved(string eventType)
        {
            if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
            {
                m_theRouter.Remove(eventType);
            }
        }

        void OnTriggerBegin()
        {
            m_tempIsTriggerFlg++;
            if(m_tempIsTriggerFlg == 1)
                m_tempDeleteCall.Clear();
            if (m_tempIsTriggerFlg > 10)
                Debuger.LogError("OnTriggerBegin 不应该走到这里");
        }

        void OnTriggerEnd()
        {
            m_tempIsTriggerFlg--;
            if (m_tempIsTriggerFlg == 0)
                m_tempDeleteCall.Clear();
            if (m_tempIsTriggerFlg < 0)
                Debuger.LogError("OnTriggerEnd 不应该走到这里");
        }
        //无效回调
        bool IsDeleteEvent(Delegate call )
        {
            return m_tempDeleteCall.Contains(call);
        }

        #endregion

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(string eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action)m_theRouter[eventType] + handler;
        }
        
        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] + handler;
        }

        public void On(string eventType, Action handler)
        {
            AddEventListener(eventType, handler);
        }

        public void On<T>(string eventType, Action<T> handler)
        {
            AddEventListener<T>(eventType, handler);
        }

        public void On<T, U>(string eventType, Action<T, U> handler)
        {
            AddEventListener<T, U>(eventType, handler);
        }  

        public void On<T, U, V>(string eventType, Action<T, U, V> handler)
        { 
            AddEventListener<T, U, V>(eventType, handler);
        }

        public void On<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            AddEventListener<T, U, V, W>(eventType, handler);
        }
    

        #endregion

        #region 移除监听器

        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(string eventType, Action handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        ///  触发事件， 不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void Trigger(string eventType)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }

            OnTriggerBegin();
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                if (IsDeleteEvent(callbacks[i]))
                    continue;
                Action callback = callbacks[i] as Action;

                if (callback == null)
                {
                    OnTriggerEnd();
                    throw new Exception(string.Format("TriggerEvent：{0} 参数不匹配 ", eventType));
                }

                try
                {

                    callback();
                }
                catch (Exception ex)
                {
                    Debuger.LogException(ex);
                }
            }
            OnTriggerEnd();
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void Trigger<T>(string eventType, T arg1)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            OnTriggerBegin();
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                if (IsDeleteEvent(callbacks[i]))
                    continue;
                Action<T> callback = callbacks[i] as Action<T>;

                if (callback == null)
                {
                    OnTriggerEnd();
                    throw new Exception(string.Format("TriggerEvent：{0} 参数不匹配 ", eventType));
                }

                try
                {
                    callback(arg1);
                }
                catch (Exception ex)
                {
                    Debuger.LogException(ex);
                }
            }
            OnTriggerEnd();
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void Trigger<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            OnTriggerBegin();
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                if (IsDeleteEvent(callbacks[i]))
                    continue;
                Action<T, U> callback = callbacks[i] as Action<T, U>;

                if (callback == null)
                {
                    OnTriggerEnd();
                    throw new Exception(string.Format("TriggerEvent：{0} 参数不匹配 ", eventType));
                }

                try
                {
                    callback(arg1, arg2);
                }
                catch (Exception ex)
                {
                    Debuger.LogException(ex);
                }
            }
            OnTriggerEnd();
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void Trigger<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            OnTriggerBegin();
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                if (IsDeleteEvent(callbacks[i]))
                    continue;
                Action<T, U, V> callback = callbacks[i] as Action<T, U, V>;

                if (callback == null)
                {
                    OnTriggerEnd();
                    throw new Exception(string.Format("TriggerEvent：{0} 参数不匹配 ", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    Debuger.LogException(ex);
                }
            }
            OnTriggerEnd();
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void Trigger<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            OnTriggerBegin();
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                if (IsDeleteEvent(callbacks[i]))
                    continue;
                Action<T, U, V, W> callback = callbacks[i] as Action<T, U, V, W>;

                if (callback == null)
                {
                    OnTriggerEnd();
                    throw new Exception(string.Format("TriggerEvent：{0} 参数不匹配 ", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    Debuger.LogException(ex);
                }
            }
            OnTriggerEnd();
        }


        //class TestData
        //{
        //    public Delegate[] d;
        //    public int cnt;
        //    public string objName;
        //}
        public string TestCheckEventCall()
        {
            string text = "";

            //相同角色事件的个数
            int minEventCnt = 3;
            Dictionary<string, Dictionary<string, int>> eventsMap = new Dictionary<string, Dictionary<string, int>>();

            //事件名,回调列表
            Dictionary<string, Delegate[]> eventsMap2 = new Dictionary<string, Delegate[]>();

            //对象名，事件名
            Dictionary<string, List<string>> eventTargetsMap = new Dictionary<string, List<string>>();



            foreach (KeyValuePair<string, Delegate> p in m_theRouter)
            {
                var callbacks = p.Value.GetInvocationList();
                if (callbacks.Length != 0)
                {
                    for (int i = 0; i < callbacks.Length; i++)
                    {
                        if (callbacks[i].Target == null)
                            continue;
                        string objName =callbacks[i].Target.ToString();
                        Dictionary<string, int> objCnt;
                        if (!eventsMap.TryGetValue(p.Key, out objCnt))
                        {
                            objCnt = new Dictionary<string,int>();
                            eventsMap.Add(p.Key, objCnt);
                        }

                        int cnt;
                        if (!objCnt.TryGetValue(objName, out cnt))
                            objCnt.Add(objName, 1);
                        else
                            objCnt[objName] = cnt + 1;

                        if(!eventTargetsMap.ContainsKey(objName))
                        {
                            eventTargetsMap.Add(objName, new List<string>());
                        }
                        eventTargetsMap[objName].Add(p.Key);
                    }
                    eventsMap2.Add(p.Key, callbacks);
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, int>> p in eventsMap)
            {
                foreach( KeyValuePair<string,int> kp in p.Value)
                {
                    //当相同对象事件的个数大于这个数才会显示
                    if (kp.Value > minEventCnt)
                    {
                        text += p.Key + " " + " " + kp.Key + " " + kp.Value+"\r\n";
                    }
                }
            }

            return text;
        }

        #endregion

        #region 否决器 由于有返回值，所以内部实现和监听器不太一样，删除要通过id
//        Dictionary<string, Dictionary<int, Func<object, bool>>> m_votes = new Dictionary<string, Dictionary<int, Func<object, bool>>>();
//        Dictionary<int, string> m_voteIdxById = new Dictionary<int, string>();
//        int m_voteCounter = 1;
//        const int Invalid_Vote_Id = -1;
//
//        public int AddVote(string eventType, Func<object, bool> onVote)
//        {
//            ++m_voteCounter;
//            var d =m_votes.GetNewIfNo(eventType);
//            d[m_voteCounter] = onVote;
//            m_voteIdxById[m_voteCounter] = eventType;
//            return m_voteCounter;
//        }
//        public void RemoveVote(int voteId)
//        {
//            string eventType;
//            if (!m_voteIdxById.TryGetValue(voteId, out eventType))
//                return;
//            m_voteIdxById.Remove(voteId);
//            var d = m_votes.Get(eventType);
//            if (d == null)
//                return;
//            d.Remove(voteId);
//        }
//
//        
//        public bool TriggerVote(string eventType,object param)
//        {
//            var d = m_votes.Get(eventType);
//            if (d == null)
//                return true;
//            foreach(var f in d.Values)
//            {
//                if (!f(param))
//                    return false;
//            }
//            return true;
//        }
        #endregion
    }
}