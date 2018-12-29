//using System;
//using System.Collections.Generic;
//using StarForce;
//using UnityEngine;
//
//namespace Battle
//{
//    public partial class PathMoveManage
//#if MEMORY_CHECK
//        : MemoryObject
//#endif
//    {
//        private Entity m_role;
//        //寻路组件
//        private UnityEngine.AI.NavMeshAgent m_navMeshAgent;
//        private UnityEngine.AI.NavMeshObstacle m_navMeshObstacle;
//        //当前寻路操作的最终目标点
//        private Vector3 m_targetPos;
//        //当与目标点的距离小于该值时则认为已到达
//        private float m_arrive;
//
//        public Vector3[] m_path { get; private set; }
//        public int m_pathIndex { get; private set; }
//        private float m_pathBeginTime;
//
//        List<Vector3> m_pathPlanPos = new List<Vector3>();
//
//        public Vector3 m_curTarget { get { return m_path.Length > m_pathIndex ? m_path[m_pathIndex] : m_role.transform.position; } }
//
//        public PathMoveManage(Entity role)
//        {
//            m_role = role;
//            m_path = new Vector3[0];
//        }
//        public void Init()
//        {
//            m_navMeshAgent = m_role.CachedTransform.GetComponent<UnityEngine.AI.NavMeshAgent>();
//            m_navMeshObstacle = m_role.CachedTransform.GetComponent<UnityEngine.AI.NavMeshObstacle>();
//            if (m_navMeshAgent == null)
//            {
//                return;
//            }
//            m_navMeshAgent.path.ClearCorners();
//            m_navMeshAgent.speed = 10; //m_role.m_attri.speed;
//        }
//        public void Clear()
//        {
//            if (m_role == null)
//            {
//                return;
//            }
//            m_targetPos = m_role.CachedTransform.position;
//            m_role = null;
//            m_targetPos = Vector3.zero;
//            m_navMeshAgent = null;
//        }
//
//        //是否需要重新寻路,如果目标点很近或者时间间隔太快则不需要
//        public bool IsNeedRefindPath(Vector3 pos)
//        {
//            if (Vector3.Distance(m_targetPos, pos) < 0.2f || (MainTime.timePass - m_pathBeginTime<0.3f))
//                return false;
//            return true;
//        }
//
//        //开始寻路
//        public bool StartPath(Vector3 pos, float arrive)
//        {
//            float cost;
//            return StartPath(pos, arrive, false, out cost);
//        }
//
//        //开始寻路
//        public bool StartPath(Vector3 pos, float arrive,bool pathPlan,out float cost)
//        {
//            if (m_navMeshObstacle != null)
//                m_navMeshObstacle.enabled = false;
//
//            m_pathBeginTime = MainTime.timePass;
//            m_targetPos = pos;
//            m_arrive = arrive;
//
//            //路径规划
//            m_pathPlanPos.Clear();
//            cost = ObjectHelp.GetDistance(m_role.m_modelManage.transform.position, pos);
//            if (pathPlan)
//                PathPlanMgr.instance.Plan(m_role.m_modelManage.transform.position, pos, ref m_pathPlanPos, ref cost);
//            else
//                m_pathPlanPos.Add(pos);
//
//
//            //路径第一个点为角色当前所在坐标点，需要将目标设为下一个点
//            m_pathIndex = 1;
//            Vector3[] path;
//            bool ret = CalculatePath(m_role.m_modelManage.transform.position, m_pathPlanPos[0], m_arrive, out path);
//            m_path = path;
//            return ret;
//        }
//        public void OnDrawGizmos()
//        {
//            if (m_pathIndex >= m_path.Length)
//                return;
//
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(m_role.m_modelManage.transform.position, m_path[m_pathIndex]);
//            for (int i = m_pathIndex+1; i < m_path.Length ; ++i)
//                Gizmos.DrawLine(m_path[i - 1], m_path[i]);
//
//            if (m_pathPlanPos.Count>1)
//            {
//                Gizmos.DrawLine(m_path[m_path.Length-1], m_pathPlanPos[1]);
//                for (int i = 2; i < m_pathPlanPos.Count; ++i)
//                    Gizmos.DrawLine(m_pathPlanPos[i - 1], m_pathPlanPos[i]);
//            }
//            
//        }
//        //寻路更新
//        public PathManage.State SmoothMove(Vector3 pos, float arrive)
//        {
//            if (!m_role.IsCanMove())
//            {
//                return PathManage.State.Arrive;
//            }
//
//            float tempMoveDis = m_role.m_attri.speed * Time.deltaTime;
//            //还距离目标多远就能结束
//            float targetNeedMoveDis = ObjectHelp.GetDistance(m_role.m_modelManage.transform.position, m_targetPos) - m_arrive;
//            //到达最终目标点，清空数据并尝试启动动态碰撞
//            if (m_navMeshAgent != null && (targetNeedMoveDis <= tempMoveDis))
//            {
//                //吧角色移动到目标点
//                m_role.m_moveManage.SetMovementImmediately(ObjectHelp.GetFowardNormalized(m_role.m_modelManage.transform.position, m_targetPos) * (targetNeedMoveDis + 0.01f));
//                m_role.m_aniManage.m_aniStateManage.PlayMoveAnimation(false);
//                m_path = new Vector3[0];
//                m_navMeshAgent.path.ClearCorners();
//                if (m_navMeshAgent.isOnNavMesh)
//                {
//                    m_navMeshAgent.Stop();
//                }
//                m_navMeshAgent.enabled = false;
//                if (m_navMeshObstacle != null)
//                {
//                    m_navMeshObstacle.enabled = true;
//                }
//                return PathManage.State.Arrive;
//            }
//
//            //沿路径点移动
//            if (m_pathIndex >= 0 && m_path.Length > 0)
//            {
//                if (tempMoveDis > ObjectHelp.GetDistance(m_role.m_modelManage.transform.position, m_path[m_pathIndex]))
//                {
//                    m_pathIndex++;
//                }
//                
//                if (m_pathIndex >= m_path.Length)
//                {
//                    m_pathPlanPos.RemoveAt(0);
//                    
//                    if(m_pathPlanPos.Count==0)
//                        return PathManage.State.Arrive;
//
//                    //路径规划启用的情况下，要继续寻路到下一个点
//                    m_pathIndex = 1;
//                    Vector3[] path;
//                    bool ret = CalculatePath(m_role.m_modelManage.transform.position, m_pathPlanPos[0], m_arrive, out path);
//                    m_path = path;
//
//                }
//                m_pathIndex = Mathf.Min(m_pathIndex, m_path.Length - 1);
//                Vector3 speed = ObjectHelp.GetForward(m_path[m_pathIndex], m_role.m_modelManage.transform.position).normalized * m_role.m_attri.speed;
//                m_role.m_moveManage.SetRunSpeed(speed);
//            }
//
//            m_role.m_aniManage.m_aniStateManage.PlayMoveAnimation(true);
//
//            //寻路过程逐步朝向目标点
//            if (m_pathIndex < m_path.Length && m_role.m_objPrototype.type != ObjectPrototype.ObjType.Player && ObjectHelp.GetForward(m_path[m_pathIndex], m_role.transform.position) != Vector3.zero)
//            {
//                Vector3 temp = m_path[m_pathIndex];
//                temp.y = m_role.transform.position.y;
//                Quaternion toFoward = Quaternion.LookRotation(temp - m_role.transform.position);
//                m_role.transform.rotation = Quaternion.Lerp(m_role.transform.rotation, toFoward, 2 * Mathf.PI * Time.deltaTime);
//            }
//            return PathManage.State.Moving;
//        }
//
//
//        public void MoveStop()
//        {
//            m_path = new Vector3[0];
//            m_targetPos = m_role.m_modelManage.transform.position;
//            if (m_navMeshAgent != null)
//            {
//                m_navMeshAgent.enabled = false;
//            }
//            if (m_navMeshObstacle != null)
//            { 
//                m_navMeshObstacle.enabled = true;
//            }
//        }
//        public bool CalculatePath(Vector3 startPos, Vector3 targetPos, float arrive, out Vector3[] result)
//        {
//            Vector3 tempPos = targetPos;
//            //将目标点往自身方向偏移一定距离，保证寻路方向
//            if (m_role.m_postureManage.GetPostureData().type == PosturePrototype.PostureType.Fly)
//            {
//                result = new Vector3[2] { startPos, tempPos };
//                return true;
//            }
//            //targetPos += (startPos- targetPos).normalized * arrive / 2;
//            UnityEngine.AI.NavMeshPath temp = new UnityEngine.AI.NavMeshPath();
//            bool ret = UnityEngine.AI.NavMesh.CalculatePath(startPos, targetPos, UnityEngine.AI.NavMesh.AllAreas, temp);
//            result = temp.corners;
//
//            //导航网格会寻路到最近的一个网格，有时候这个网格和目标高度差会很大，这时候需要返回寻路失败
//            if (ret && result.Length >= 1 && Math.Abs(result[result.Length-1].y-targetPos.y)>2)
//            {
//                return false;
//            }
//
//            if ((result.Length <= 1 || result[0] == result[result.Length - 1]) && m_navMeshAgent != null)
//            {
//                if (m_navMeshAgent.isOnNavMesh)
//                {
//                    m_navMeshAgent.enabled = true;
//                    if (m_navMeshAgent.isOnNavMesh)
//                    {
//                        m_navMeshAgent.SetDestination(targetPos);
//                        m_navMeshAgent.stoppingDistance = arrive / 2;
//                        result = m_navMeshAgent.path.corners;
//                        m_navMeshAgent.Stop();
//                    }
//                    m_navMeshAgent.enabled = false;
//                    return ret;
//                }
//                if (result.Length <= 1)
//                {
//                    result = new Vector3[2] { startPos, tempPos };
//                    return false;
//                }
//            }
//            return ret;
//        }
//    }
//}