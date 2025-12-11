using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Game.Core
{
    /// <summary>
    /// 简单场景加载器
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// 加载进度条
        /// </summary>
        public Slider LoadingSlider;

        /// <summary>
        /// 加载文本
        /// </summary>
        public TextMeshProUGUI LoadingText;

        /// <summary>
        /// 加载面板
        /// </summary>
        public GameObject LoadingPanel;

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneIndex">场景索引</param>
        public void LoadSceneAsync(int sceneIndex)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        /// <summary>
        /// 场景加载协程
        /// </summary>
        /// <param name="sceneReference">场景索引或名称</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator LoadSceneCoroutine(object sceneReference)
        {
            // 显示加载面板
            if (LoadingPanel != null)
            {
                LoadingPanel.SetActive(true);
            }

            AsyncOperation asyncOperation;

            // 根据场景引用类型选择加载方式
            if (sceneReference is int sceneIndex)
            {
                asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            }
            else if (sceneReference is string sceneName)
            {
                asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            }
            else
            {
                Debug.LogError("Invalid scene reference type!");
                yield break;
            }

            asyncOperation.allowSceneActivation = false;

            // 显示加载进度
            while (!asyncOperation.isDone)
            {
                // 更新加载进度
                float progress = asyncOperation.progress;

                if (LoadingSlider != null)
                {
                    LoadingSlider.value = progress;
                }

                if (LoadingText != null)
                {
                    LoadingText.text = $"加载中... {(int)(progress * 100)}%";
                }

                // 当加载进度达到0.9时，允许场景激活
                if (progress >= 0.9f)
                {
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            // 隐藏加载面板
            if (LoadingPanel != null)
            {
                LoadingPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneIndex">场景索引</param>
        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}