using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using easyar;

namespace ARNavigation
{
    public class KeyPointController : MonoBehaviour
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
        /// 提示信息文本
        /// </summary>
        public Text textInfo;
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


        void Start()
        {
            game = FindObjectOfType<GameController>();
            Load();
            btnDelete.interactable = false;
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
                selected = tf;
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
        /// 保存关键点
        /// </summary>
        public void Save()
        {
            string[] jsons = new string[svContent.childCount];
            for (int i = 0; i < svContent.childCount; i++)
            {
                jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().keyPoint);
            }
            if (game)
            {
                game.SaveKeyPoint(jsons);
                textInfo.text = "保存完成";
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
        }
        /// <summary>
        /// 删除关键点
        /// </summary>
        public void Delete()
        {
            Destroy(selected.gameObject);
            textInfo.text = "已删除所选关键点";
            btnDelete.interactable = false;
        }
    }
}

