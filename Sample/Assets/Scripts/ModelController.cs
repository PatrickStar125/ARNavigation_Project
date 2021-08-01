using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using easyar;

namespace ARNavigation
{
    public class ModelController : MonoBehaviour
    {
        /// <summary>
        /// 选中屏幕中模型
        /// </summary>
        private UIControlObject uiControl;
        /// <summary>
        /// 界面控制
        /// </summary>
        private GameController game;
        /// <summary>
        /// 摄像头位置
        /// </summary>
        public Transform frontCamera;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public Transform ssMap;
        /// <summary>
        /// 生成模型
        /// </summary>
        public GameObject blueBox;
        /// <summary>
        /// 添加界面
        /// </summary>
        public GameObject addUI;
        /// <summary>
        /// 管理界面
        /// </summary>
        public GameObject saveUI;
        /// <summary>
        /// 文本提示
        /// </summary>
        public Text textShow;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public SparseSpatialMapController map;
        /// <summary>
        /// 稀疏空间地图定位
        /// </summary>
        public SparseSpatialMapWorkerFrameFilter mapWorker;
        /// <summary>
        /// 按钮
        /// </summary>
        public Button btnAdd;
        public Button btnSave;
        public Button btnClose;
        /// <summary>
        /// 输入框
        /// </summary>
        public InputField inputField;
        /// <summary>
        /// 选择点类型
        /// </summary>
        public Dropdown dropdown;
        /// <summary>
        /// 储存动态点
        /// </summary>
        public Transform KPSave;
        /// <summary>
        /// 所选模型
        /// </summary>
        private Transform selected;
        /// <summary>
        /// KeyPoint预制件
        /// </summary>
        public KPDate prefab;

        /// <summary>
        /// 保存当前模型位置为关键点
        /// </summary>
        public void Save()
        {
            KPDate kp = Instantiate(prefab, KPSave);
            KeyPoint point = new KeyPoint();
            point.pointType = dropdown.value;
            point.position = selected.localPosition;
            point.name = inputField.text;
            kp.keyPoint = point;
            kp.name = inputField.text;
            btnSave.interactable = false;
            btnClose.interactable = true;
            textShow.text = "已保存为关键点";
        }

        /// <summary>
        /// 保存所有关键点
        /// </summary>
        public void SaveALL()
        {
            if (KPSave.childCount == 0)
                return;
            string[] jsons = new string[KPSave.childCount];
            for (int i = 0; i < KPSave.childCount; i++)
            {
                KeyPoint keyPoint = new KeyPoint();
                keyPoint.name = KPSave.GetChild(i).GetComponent<KPDate>().keyPoint.name;
                keyPoint.position = KPSave.GetChild(i).GetComponent<KPDate>().keyPoint.position;
                keyPoint.pointType = KPSave.GetChild(i).GetComponent<KPDate>().keyPoint.pointType;
                jsons[i] = JsonUtility.ToJson(keyPoint);
            }
            if (game)
            {
                game.SaveKeyPoint(jsons);
            }
        }
        /// <summary>
        /// 删除所选关键点
        /// </summary>
        public void Delete()
        {
            var go = uiControl.selected.gameObject;
            uiControl.ClearSelected();
            Destroy(go);
            textShow.text = "已删除";
            btnClose.interactable = true;
            btnSave.interactable = false;
        }
        /// <summary>
        /// 打开管理界面
        /// </summary>
        public void Open()
        {
            addUI.SetActive(false);
            saveUI.SetActive(true);
        }
        /// <summary>
        /// 添加模型作为关键点
        /// </summary>
        public void Add()
        {
            var tf = Instantiate(blueBox, ssMap).transform;
            tf.position = frontCamera.position;
            tf.name = inputField.text;
            btnSave.interactable = true;
            btnAdd.interactable = false;
            Open();
        }
        /// <summary>
        /// 返回到菜单
        /// </summary>
        public void Back()
        {
            SaveALL();
            game.BackMenu();
        }
        /// <summary>
        /// 关闭管理界面
        /// </summary>
        public void Close()
        {
            addUI.SetActive(true);
            saveUI.SetActive(false);
        }
        /// <summary>
        /// 点击物体处理
        /// </summary>
        /// <param name="ray"></param>
        private void TouchedObject(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                addUI.SetActive(false);
                saveUI.SetActive(true);
                uiControl.SetSelected(hit.transform);
                textShow.text = "选中物体";
                selected = hit.transform;
            }
        }
        /// <summary>
        /// 加载稀疏空间地图
        /// </summary>
        private void LoadMap()
        {
            //设置地图ID和名字
            map.MapManagerSource.ID = game.GetMapID();
            map.MapManagerSource.Name = game.GetMapName();
            //地图获取反馈
            map.MapLoad += (map, status, error) =>
            {
                if (status)
                {
                    textShow.text = "地图加载完成";
                }
                else
                {
                    textShow.text = "Error:" + error;
                }
            };
            //地图定位成功
            map.MapLocalized += () =>
            {
                textShow.text = "定位成功";
            };
            //停止定位
            map.MapStopLocalize += () =>
            {
                textShow.text = "停止定位";
                btnAdd.interactable = false;
            };
            textShow.text = "开始地图本地化";
            //本地化地图
            mapWorker.Localizer.startLocalization();
        }
        /// <summary>
        /// 检测框内有无输入
        /// </summary>
        public void ChangedInput()
        {
            if (string.IsNullOrEmpty(inputField.text))
                btnAdd.interactable = false;
            else
                btnAdd.interactable = true;
        }
        /// <summary>
        /// 加载关键点
        /// </summary>
        private void Load()
        {
            if (game)
            {
                var list = game.LoadKeyPoint();
                foreach (var item in list)
                {
                    KPDate kp = Instantiate(prefab, KPSave);
                    kp.keyPoint = JsonUtility.FromJson<KeyPoint>(item);
                    kp.name = kp.keyPoint.name;
                    var tf = Instantiate(blueBox, ssMap).transform;
                    tf.position = kp.keyPoint.position;
                    tf.name = kp.keyPoint.name;
                }
            }
        }

        void Start()
        {
            game = FindObjectOfType<GameController>();
            uiControl = FindObjectOfType<UIControlObject>();
            btnAdd.interactable = false;
            Close();
            LoadMap();
            Load();
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetMouseButtonDown(0)
                && !EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    TouchedObject(ray);
                }
            }
            else
            {
                if (Input.touchCount == 1
                && Input.touches[0].phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    TouchedObject(ray);
                }
            }
        }
    }
}