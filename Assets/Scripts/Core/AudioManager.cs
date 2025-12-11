using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 音频管理器
    /// 管理全局音量设置
    /// 所有场景共享同一音量设置
    /// </summary>
    public class AudioManager : MonoBehaviour, IController
    {
        /// <summary>
        /// 音频管理器单例实例
        /// </summary>
        public static AudioManager Instance;
        
        /// <summary>
        /// 主音量，范围0-1
        /// </summary>
        [Header("音量设置")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        
        /// <summary>
        /// 音量保存的PlayerPrefs键名
        /// </summary>
        private const string VOLUME_PREF_KEY = "MasterVolume";
        
        private void Awake()
        {
            // 实现单例模式
            if (Instance == null)
            {
                Instance = this;
                // 确保场景切换时不被销毁
                DontDestroyOnLoad(gameObject);
                // 加载保存的音量设置
                LoadVolume();
            }
            else
            {
                // 如果已经存在实例，销毁当前对象
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 设置主音量
        /// </summary>
        /// <param name="volume">音量值，范围0-1</param>
        public void SetVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            // 保存音量设置
            SaveVolume();
            // 应用音量到所有音频源
            ApplyVolumeToAllAudioSources();
        }
        
        /// <summary>
        /// 获取当前主音量
        /// </summary>
        /// <returns>当前音量值，范围0-1</returns>
        public float GetVolume()
        {
            return masterVolume;
        }
        
        /// <summary>
        /// 从PlayerPrefs加载音量设置
        /// </summary>
        private void LoadVolume()
        {
            masterVolume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 1f);
            // 应用音量到所有音频源
            ApplyVolumeToAllAudioSources();
        }
        
        /// <summary>
        /// 将音量设置保存到PlayerPrefs
        /// </summary>
        private void SaveVolume()
        {
            PlayerPrefs.SetFloat(VOLUME_PREF_KEY, masterVolume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// 应用音量到所有音频源
        /// </summary>
        private void ApplyVolumeToAllAudioSources()
        {
            // 查找场景中所有的AudioSource组件
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                // 设置音频源的音量
                audioSource.volume = masterVolume;
            }
        }
        
        /// <summary>
        /// 当场景加载完成时，应用音量设置
        /// </summary>
        private void OnLevelWasLoaded(int level)
        {
            ApplyVolumeToAllAudioSources();
        }
        
        /// <summary>
        /// 获取游戏架构实例
        /// </summary>
        /// <returns>游戏架构实例</returns>
        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }
    }
}