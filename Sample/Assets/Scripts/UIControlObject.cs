using UnityEngine;
using UnityEngine.UI;

namespace ARNavigation
{
    /// <summary>
    /// 用UI控制游戏对象
    /// </summary>
    public class UIControlObject : MonoBehaviour
    {
        /// <summary>
        /// 动作按钮组
        /// </summary>
        public Button[] btnActions;
        /// <summary>
        /// 数值按钮组
        /// </summary>
        public Button[] btnNumbers;
        /// <summary>
        /// 显示文本
        /// </summary>
        public Text txtShow;
        /// <summary>
        /// 激活颜色
        /// </summary>
        public Color colorActive;
        /// <summary>
        /// 未激活颜色
        /// </summary>
        public Color colorInactive;
        /// <summary>
        /// 动作数值
        /// </summary>
        public float actionNumber;
        /// <summary>
        /// 选中对象
        /// </summary>
        public Transform selected;



        void Start()
        {
            //为数值按钮组添加事件响应
            foreach (Button btn in btnNumbers)
            {
                btn.onClick.AddListener(delegate ()
                {
                    OnClickNumber(btn.name);
                });
            }
            //为动作按钮组添加事件响应
            foreach (var btn in btnActions)
            {
                btn.onClick.AddListener(delegate ()
                {
                    OnClickAction(btn.name);
                });
            }
            //设置默认按钮显示
            SetButtonColor(btnNumbers, "One");
            //设置默认值
            actionNumber = 1;

            ClearSelected();
        }
        /// <summary>
        /// 数值按钮点击事件
        /// </summary>
        /// <param name="btnName">按钮名称</param>
        private void OnClickNumber(string btnName)
        {
            SetButtonColor(btnNumbers, btnName);
            switch (btnName)
            {
                case "Ten":
                    actionNumber = 10;
                    break;
                case "One":
                    actionNumber = 1;
                    break;
                case "OneTenth":
                    actionNumber = 0.1f;
                    break;
            }
        }
        /// <summary>
        /// 动作按钮点击事件
        /// </summary>
        /// <param name="btnName">按钮名称</param>
        private void OnClickAction(string btnName)
        {
            if (selected)   //确定有选中对象
            {
                var temp = Vector3.zero;
                switch (btnName)
                {
                    case "XAdd":
                        temp = new Vector3(actionNumber, 0, 0);
                        break;
                    case "XSub":
                        temp = new Vector3(-actionNumber, 0, 0);
                        break;
                    case "YAdd":
                        temp = new Vector3(0, actionNumber, 0);
                        break;
                    case "YSub":
                        temp = new Vector3(0, -actionNumber, 0);
                        break;
                    case "ZAdd":
                        temp = new Vector3(0, 0, actionNumber);
                        break;
                    case "ZSub":
                        temp = new Vector3(0, 0, -actionNumber);
                        break;
                }
                selected.localPosition = selected.localPosition + temp;
            }
            ShowSelectedInfo();
        }
        /// <summary>
        /// 设置按钮颜色
        /// </summary>
        /// <param name="buttons">按钮数组</param>
        /// <param name="btnName">激活的按钮名称</param>
        private void SetButtonColor(Button[] buttons, string btnName)
        {
            foreach (var btn in buttons)
            {
                Color temp = colorInactive;
                if (btn.name.Equals(btnName))
                {
                    temp = colorActive;
                }
                btn.GetComponent<Image>().color = temp;
            }
        }
        /// <summary>
        /// 设置选中对象
        /// </summary>
        /// <param name="tfSelected">对象的transform</param>
        public void SetSelected(Transform tfSelected)
        {
            selected = tfSelected;
            ShowSelectedInfo();
        }
        /// <summary>
        /// 清除选中对象
        /// </summary>
        public void ClearSelected()
        {
            selected = null;
            txtShow.text = "未选中";
        }
        /// <summary>
        /// 显示选中对象信息
        /// </summary>
        private void ShowSelectedInfo()
        {
            if (!selected)
            {
                return;
            }
            txtShow.text = string.Format(
                @"Name:{0}
                Position:{1}",
                selected.name,
                selected.localPosition).Replace(" ", "");
        }
    }
}


