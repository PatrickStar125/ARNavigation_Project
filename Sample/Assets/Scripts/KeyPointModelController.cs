using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using easyar;

namespace ARNavigation
{
    public class KeyPointModelController : MonoBehaviour
    {
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        /// <summary>
        /// 被选中的游戏对象
        /// </summary>
        private Transform selected;
        /// <summary>
        /// 添加按钮
        /// </summary>
        public Button btnAdd;
        /// <summary>
        /// 提示信息文本
        /// </summary>
        public Text textInfo;
        /// <summary>
        /// 关键点名称输入框
        /// </summary>
        public InputField inputField;
        /// <summary>
        /// 关键点类型下拉列表
        /// </summary>
        public Dropdown dropdown;
        /// <summary>
        /// 滚动视图容器
        /// </summary>
        public Transform svContent;
        /// <summary>
        /// 按钮预制件
        /// </summary>
        public SelectButton prefab;
        /// <summary>
        /// 删除按钮
        /// </summary>
        public Button btnDelete;
        /// <summary>
        /// 稀疏空间地图框架
        /// </summary>
        public SparseSpatialMapWorkerFrameFilter mapWorker;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public SparseSpatialMapController map;
        /// <summary>
        /// 添加界面
        /// </summary>
        public GameObject addUI;
        /// <summary>
        /// 调整模型界面
        /// </summary>
        public GameObject adjustUI;
        /// <summary>
        /// UI控制游戏对象
        /// </summary>
        private UIControlObject uiControl;
        /// <summary>
        /// 显示信息文本
        /// </summary>
        public Text textShow;
        /// <summary>
        /// 摄像头前方
        /// </summary>
        public Transform frontCamera;
        /// <summary>
        /// 添加的物体
        /// </summary>
        public GameObject blueBox;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public Transform ssMap;
        /// <summary>
        /// 储存模型信息预制件
        /// </summary>
        public KeyPointModel data;
        /// <summary>
        /// 储存模型信息储存位置
        /// </summary>
        public Transform DateSave;

        void Start()
        {
            game = FindObjectOfType<GameController>();
            Load();
            btnAdd.interactable = false;
            btnDelete.interactable = false;
            adjustUI.SetActive(false);
            addUI.SetActive(true);
            Close();
            LoadMap();
        }
        void Update()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                btnAdd.interactable = true;
            }
            else
            {
                btnAdd.interactable = false;
            }
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
        /// <summary>
        /// 本地化地图
        /// </summary>
        private void LoadMap()
        {
            //设置地图
            map.MapManagerSource.ID = game.GetMapID();
            map.MapManagerSource.Name = game.GetMapName();
            //地图获取反馈
            map.MapLoad += (map, status, error) =>
            {
                if (status)
                {
                    textInfo.text = "地图加载成功。";
                }
                else
                {
                    textInfo.text = "地图加载失败：" + error;
                }
            };
            //定位成功事件
            map.MapLocalized += () =>
            {
                textInfo.text = "稀疏空间定位成功。";
                btnAdd.interactable = true;
            };
            //停止定位事件
            map.MapStopLocalize += () =>
            {
                textInfo.text = "停止稀疏空间定位。";
                btnAdd.interactable = false;
            };
            textInfo.text = "开始本地化稀疏空间。";
            mapWorker.Localizer.startLocalization();    //本地化地图
        }
        /// <summary>
        /// 点击物体
        /// </summary>
        /// <param name="ray"></param>
        private void TouchedObject(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var tf = new GameObject().transform;
                tf.position = hit.transform.position;
                tf.parent = map.transform;
                selected = tf;
                addUI.SetActive(false);
                adjustUI.SetActive(true);
                uiControl.SetSelected(hit.transform);
                textShow.text = "选中物体";
            }
        }
        /// <summary>
        /// 返回
        /// </summary>
        public void Back()
        {
            if (game)
            {
                game.BackMenu();
            }
        }
        /// <summary>
        /// 关闭主界面
        /// </summary>
        public void Close()
        {
            adjustUI.SetActive(false);
            addUI.SetActive(true);
        }

        /// <summary>
        /// 添加模型
        /// </summary>
        public void AddModel()
        {
            Transform tf = Instantiate(blueBox, ssMap).transform;
            tf.position = frontCamera.position;

            KeyPointModel model = Instantiate(data, DateSave);
            model.GetComponentInChildren<Text>().text = inputField.text;
            model.keyPoint.name = inputField.text;
            model.keyPoint.position = tf.position;
            model.keyPoint.pointType = dropdown.value;
            model.model = tf;

            inputField.text = "";
            textInfo.text = "添加完成。";
            btnAdd.interactable = false;
        }
        /// <summary>
        /// 保存关键点
        /// </summary>
        public void Save()
        {
            string[] jsons = new string[ssMap.childCount - 1];
            for (int i = 0; i < ssMap.childCount; i++)
            {
                if (ssMap.GetChild(i).name != "PointCloudParticleSystem")
                {
                    DynamicObject dynamicObject = new DynamicObject();
                    dynamicObject.position = ssMap.GetChild(i).localPosition;
                    dynamicObject.rotation = ssMap.GetChild(i).localEulerAngles;
                    dynamicObject.scale = ssMap.GetChild(i).localScale;
                    jsons[i - 1] = JsonUtility.ToJson(dynamicObject);
                }
            }
            string[] jsons_model = new string[DateSave.childCount];
            for (int i = 0; i < DateSave.childCount; i++)
            {
                jsons_model[i] = JsonUtility.ToJson(DateSave.GetChild(i).GetComponent<KeyPointModel>());
            }
            if (game)
            {
                game.SaveKeyPoint(jsons);
                textInfo.text = "保存完成。";
            }
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
                    SelectButton btn = Instantiate(prefab, svContent);
                    btn.keyPoint = JsonUtility.FromJson<KeyPoint>(item);
                    btn.GetComponentInChildren<Text>().text = btn.keyPoint.name;
                }
            }
        }
        /// <summary>
        /// 关键点按钮点击
        /// </summary>
        /// <param name="btn">按钮</param>
        public void SelectButtonClicked(Transform btn)
        {
            selected = btn;
            textInfo.text = btn.GetComponentInChildren<Text>().text;
            btnDelete.interactable = true;
            btnAdd.interactable = false;
        }
        /// <summary>
        /// 删除关键点
        /// </summary>
        public void Delete()
        {
            Destroy(selected.gameObject);
            textInfo.text = "删除完成。";
            btnDelete.interactable = false;
        }
    }
}

