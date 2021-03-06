using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

namespace ARNavigation.Dbg
{
    public class DbgNavigationController : MonoBehaviour
    {
        /// <summary>
        /// 返回界面
        /// </summary>
        public GameObject uiBack;
        /// <summary>
        /// 导航界面
        /// </summary>
        public GameObject uiNav;
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        /// <summary>
        /// 使用者位置
        /// </summary>
        public Transform player;
        /// <summary>
        /// 显示游戏对象数组
        /// </summary>
        public ShowChildObject[] staticObjects;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public Transform ssMap;
        /// <summary>
        /// 盒子预制件
        /// </summary>
        public Transform blueBox;
        /// <summary>
        /// 滚动视图内容游戏对象
        /// </summary>
        public Transform svContent;
        /// <summary>
        /// 目的地预制件
        /// </summary>
        public Transform targetPrefab;
        /// <summary>
        /// 选择按钮预制件
        /// </summary>
        public SelectButton selectButton;
        /// <summary>
        /// 路径预制件
        /// </summary>
        public Transform roadPrefab;
        /// <summary>
        /// 导航网络表面
        /// </summary>
        public NavMeshSurface surface;
        /// <summary>
        /// 导航代理
        /// </summary>
        public NavMeshAgent agent;
        /// <summary>
        /// 导航路径
        /// </summary>
        private NavMeshPath path;
        /// <summary>
        /// 目的地
        /// </summary>
        private Transform target;
        /// <summary>
        /// 线条渲染器
        /// </summary>
        public LineRenderer lineRenderer;
        /// <summary>
        /// 刷新频率
        /// </summary>
        public float refresh;
        void Start()
        {
            game = FindObjectOfType<GameController>();
            SetStaticObject();
            LoadTarget();
            LoadRoad();
            BakePath();
            ShowNav();
        }
        /// <summary>
        /// 设置静态游戏对象显示
        /// </summary>
        private void SetStaticObject()
        {
            foreach (var item in staticObjects)
            {
                item.SetVisible((item.transform.position - player.position).magnitude <= 2);
            }
        }
        /// <summary>
        /// 关闭导航界面
        /// </summary>
        public void Close()
        {
            uiBack.SetActive(true);
            uiNav.SetActive(false);
        }
        /// <summary>
        /// 显示导航界面
        /// </summary>
        public void ShowNav()
        {
            uiBack.SetActive(false);
            uiNav.SetActive(true);
        }
        /// <summary>
        /// 返回菜单
        /// </summary>
        public void Back()
        {
            game.BackDbgMenu();
        }
        /// <summary>
        /// 加载目的地
        /// </summary>
        private void LoadTarget()
        {
            if (game)
            {
                var list = game.LoadKeyPoint();
                foreach (var item in list)
                {
                    KeyPoint point = JsonUtility.FromJson<KeyPoint>(item);

                    if (point.pointType == 0)
                    {
                        var target = Instantiate(targetPrefab, ssMap);
                        target.localPosition = point.position;
                        target.GetComponent<ShowChildObject>().SetVisible(false);
                        var btn = Instantiate(selectButton, svContent);
                        btn.GetComponentInChildren<Text>().text = point.name;
                        btn.target = target;
                    }
                }
            }
        }
        /// <summary>
        /// 加载路径
        /// </summary>
        private void LoadRoad()
        {
            if (game)
            {
                var list = game.LoadRoad();
                foreach (var item in list)
                {
                    var road = JsonUtility.FromJson<Road>(item);
                    var tfRoad = Instantiate(roadPrefab, ssMap.Find("Roads"));

                    tfRoad.localPosition = (road.startPosition + road.endPosition) / 2;
                    tfRoad.LookAt(road.startPosition);
                    tfRoad.localScale = new Vector3(0.02f, 1f, (road.endPosition - road.startPosition).magnitude * 0.1f + 0.2f);
                }
            }
        }
        /// <summary>
        /// 烘焙路径
        /// </summary>
        private void BakePath()
        {
            agent.enabled = false;
            surface.BuildNavMesh();
            path = new NavMeshPath();
        }
        /// <summary>
        /// 显示导航路径
        /// </summary>
        private void DisplayPath()
        {
            agent.transform.position = player.position;
            agent.enabled = true;
            agent.CalculatePath(target.position, path);
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            agent.enabled = false;
        }
        /// <summary>
        /// 选择按钮点击
        /// </summary>
        /// <param name="btn">按钮</param>
        public void SelectButtonClicked(Transform btn)
        {
            CancelInvoke("DisplayPath");
            target = btn.GetComponent<SelectButton>().target;
            InvokeRepeating("DisplayPath", 0, refresh);
            Close();
        }
    }
}

