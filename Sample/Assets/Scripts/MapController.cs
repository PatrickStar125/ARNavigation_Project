using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;

namespace ARNavigation
{
    public class MapController : MonoBehaviour
    {
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        public Button btnSave;
        /// <summary>
        /// 显示文本
        /// </summary>
        public Text text;
        public ARSession session;
        /// <summary>
        /// 稀疏空间工作框架
        /// </summary>
        public SparseSpatialMapWorkerFrameFilter mapWorker;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public SparseSpatialMapController map;

        void Start()
        {
            game = FindObjectOfType<GameController>();
            btnSave.interactable = false;
            session.WorldRootController.TrackingStatusChanged += OnTrackingStatusChanged;
            if (session.WorldRootController.TrackingStatus == MotionTrackingStatus.Tracking)
            {
                btnSave.interactable = true;
            }
            else
            {
                btnSave.interactable = false;
            }
        }
        /// <summary>
        /// 跟踪状态事件
        /// </summary>
        /// <param name="status"></param>
        private void OnTrackingStatusChanged(MotionTrackingStatus status)
        {
            if (status == MotionTrackingStatus.Tracking)
            {
                btnSave.interactable = true;
                text.text = "进入跟踪状态。";
            }
            else
            {
                btnSave.interactable = false;
                text.text = "跟踪状态异常";
            }
        }
        /// <summary>
        /// 保存地图
        /// </summary>
        public void SaveMap()
        {
            btnSave.interactable = false;
            //地图保存结果反馈
            mapWorker.BuilderMapController.MapHost += (mapInfo, isSuccess, error) =>
            {
                if (isSuccess)
                {
                    game.SaveMapID(mapInfo.ID);
                    game.SaveMapName(mapInfo.Name);
                    text.text = "地图保存成功。";
                }
                else
                {
                    text.text = "地图保存出错：" + error;
                    btnSave.interactable = true;
                }
            };
            try
            {
                //保存地图
                mapWorker.BuilderMapController.Host(game.inputName, null);
                text.text = "开始保存地图，请稍等。";
            }
            catch (Exception ex)
            {
                text.text = "保存出错：" + ex.Message;
                btnSave.interactable = true;
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
    }
}

