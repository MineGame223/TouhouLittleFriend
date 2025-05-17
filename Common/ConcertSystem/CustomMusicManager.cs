using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using XPT.Core.Audio.MP3Sharp;
using NVorbis;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace TouhouPets
{
    /// <summary>
    /// 自制简易音乐播放器
    /// <br>感谢全知全能DeepSeek老师大恩大德提供的转码方法</br>
    /// </summary>
    public static class CustomMusicManager
    {
        public enum PlayModeID : int
        {
            /// <summary>
            /// 单曲循环
            /// </summary>
            SingleLoop,
            /// <summary>
            /// 随机循环
            /// </summary>
            RandomLoop,
            /// <summary>
            /// 列表循环
            /// </summary>
            ListLoop,
        }
        private enum ExitProcess : int
        {
            NotExited,
            PrintMessage,
            Exited,
        }
        /// <summary>
        /// 当前音乐的实例
        /// </summary>
        private static SoundEffectInstance _currentSoundInstance;
        /// <summary>
        /// 音量的额外百分比乘数
        /// </summary>
        private static float _soundFade = 0f;
        /// <summary>
        /// 已加载音乐的列表
        /// </summary>
        private static readonly List<SoundEffect> _loadedSounds = [];
        /// <summary>
        /// 已加载音乐的路径列表
        /// </summary>
        private static readonly List<string> _loadedSoundFiles = [];
        /// <summary>
        /// 一轮循环中播放过的音乐
        /// </summary>
        private static readonly List<int> _rolledSoundIndex = [];
        /// <summary>
        /// 上次播放的音乐索引值
        /// </summary>
        private static int _lastPlayedSound = 0;
        /// <summary>
        /// 是否未检测到任何音乐文件
        /// </summary>
        private static bool _noSoundFile = false;
        /// <summary>
        /// 是否开启后台播放
        /// </summary>
        private static bool _backgroundAudio = false;
        /// <summary>
        /// 退出播放的流程次序
        /// </summary>
        private static ExitProcess _exitPlay = ExitProcess.NotExited;
        /// <summary>
        /// 播放模式
        /// </summary>
        private static PlayModeID _playMode = PlayModeID.SingleLoop;
        /// <summary>
        /// 获取已加载音乐
        /// </summary>
        public static List<SoundEffect> LoadedSounds { get => _loadedSounds; }
        /// <summary>
        /// 设置是否开启后台播放
        /// </summary>
        public static bool EnableBackgroundAudio { get => _backgroundAudio; set => _backgroundAudio = value; }
        /// <summary>
        /// 设置播放模式
        /// </summary>
        public static PlayModeID PlayMode { get => _playMode; set => _playMode = value; }
        /// <summary>
        /// 获取是否未检测到任何音乐文件
        /// </summary>
        public static bool NoCustomMusic { get => _noSoundFile; }
        /// <summary>
        /// 指定文件夹的完整路径
        /// </summary>
        public static string FullPath { get => Path.Combine(ModLoader.ModPath, "TouhouPetCustomMusic"); }
        /// <summary>
        /// 控制台信息头
        /// </summary>
        public static string ConsoleMessageHead { get => $"[{DateTime.Now}] [TouhouPets]: "; }
        /// <summary>
        /// 确保文件夹存在
        /// </summary>
        public static void EnsureMusicFolder()
        {
            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }
        }
        /// <summary>
        /// 初始化并加载指定文件夹中的所有音频文件
        /// </summary>
        public static void Initialize(string musicsFolderPath)
        {
            if (!Directory.Exists(musicsFolderPath))
            {
                Console.WriteLine($"{ConsoleMessageHead}文件夹 {musicsFolderPath} 未找到！现已生成新的文件夹并跳过加载环节。");
                Directory.CreateDirectory(musicsFolderPath);
                return;
            }

            _rolledSoundIndex.Clear();
            _noSoundFile = false;

            // 清空已加载的音效
            foreach (var sound in _loadedSounds)
            {
                sound.Dispose();
            }
            _loadedSounds.Clear();
            _loadedSoundFiles.Clear();

            // 加载所有支持的音频文件
            IEnumerable<string> allFolder = Directory.EnumerateFiles(musicsFolderPath, ".", SearchOption.AllDirectories);
            string[] supportedExtensions = [".mp3", ".ogg", ".wav"];
            foreach (string file in allFolder)
            {
                string fileName = Path.GetFileName(file);
                string extension = Path.GetExtension(file).ToLower();
                if (!supportedExtensions.Contains(extension))
                    continue;

                try
                {
                    Console.WriteLine($"{ConsoleMessageHead}正在加载 {fileName}");
                    SoundEffect sound = LoadSoundEffect(file, extension);
                    if (sound != null)
                    {
                        _loadedSounds.Add(sound);
                        _loadedSoundFiles.Add(file);
                        Console.WriteLine($"{ConsoleMessageHead}已加载 {fileName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ConsoleMessageHead}加载音频 {file} 失败：{ex.Message}");
                }
            }

            if (_loadedSounds.Count == 0)
            {
                Console.WriteLine($"{ConsoleMessageHead}未找到任何支持的音频文件。");
                _noSoundFile = true;
            }
        }
        /// <summary>
        /// 根据文件扩展名加载音频
        /// </summary>
        private static SoundEffect LoadSoundEffect(string filePath, string extension)
        {
            switch (extension)
            {
                case ".wav":
                case ".ogg":
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        return LoadOggFile(filePath);
                    }

                case ".mp3":
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        return LoadMp3File(filePath);
                    }

                default:
                    throw new NotSupportedException($"不支持的格式: {extension}");
            }
        }
        /// <summary>
        /// 检测是否有任何可播放曲目
        /// </summary>
        /// <returns></returns>
        public static bool AnyFileFound()
        {
            int count = 0;
            for (int i = 0; i < _loadedSoundFiles.Count; i++)
            {
                if (!File.Exists(_loadedSoundFiles[i]))
                    count++;
            }
            if (count >= _loadedSoundFiles.Count)
            {
                Main.NewText(Language.GetTextValue("Mods.TouhouPets.NoAnyFile"), Color.Yellow);
                Main.LocalPlayer.GetModPlayer<ConcertPlayer>().customMode = false;
                Stop();
                PostStop();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 播放指定索引的音乐文件
        /// </summary>
        /// <param name="index">索引值</param>
        /// <param name="printMessage">是否打印播放信息</param>
        public static void PlayMusic(int index, bool printMessage = true)
        {
            _exitPlay = ExitProcess.NotExited;

            if (!AnyFileFound())
                return;

            // 先停止当前播放
            Stop();

            //根据列表查找文件
            string fileName = Path.GetFileName(_loadedSoundFiles[index]);
            if (!File.Exists(_loadedSoundFiles[index]))
            {
                Main.NewText(Language.GetTextValue("Mods.TouhouPets.FileNotExist", fileName), Color.Yellow);
                Console.WriteLine(ConsoleMessageHead + Language.GetTextValue("Mods.TouhouPets.FileNotExist", fileName));
                index++;
                if (index >= _loadedSoundFiles.Count - 1)
                {
                    index = 0;
                }
            }
            try
            {
                SoundEffect sound = _loadedSounds[index];
                _currentSoundInstance = sound.CreateInstance();
                _currentSoundInstance.Play();
                _lastPlayedSound = index;

                //若当前索引不包含在已播放列表中，则将其加入
                if (!_rolledSoundIndex.Contains(index))
                {
                    _rolledSoundIndex.Add(index);
                }

                if (printMessage)
                    Console.WriteLine($"{ConsoleMessageHead}正在播放：{fileName}");
            }
            catch (Exception ex)
            {
                Main.NewText(Language.GetTextValue("Mods.TouhouPets.FailedToPlay", fileName, ex.Message), Color.Yellow);
                Console.WriteLine(ConsoleMessageHead + Language.GetTextValue("Mods.TouhouPets.FailedToPlay", fileName, ex.Message));
            }
        }
        /// <summary>
        /// 停止播放之后的操作
        /// </summary>
        public static void PostStop()
        {
            _rolledSoundIndex.Clear();
            if (_exitPlay == ExitProcess.NotExited)
            {
                Console.WriteLine($"{ConsoleMessageHead}已退出播放。");
                _exitPlay = ExitProcess.PrintMessage;
            }
        }
        /// <summary>
        /// 实时更新的音乐状态
        /// </summary>
        public static void GuaranteedUpdate()
        {
            if (_currentSoundInstance == null)
                return;

            ConcertPlayer bp = Main.LocalPlayer.GetModPlayer<ConcertPlayer>();

            SetVolume();

            //当演出停止或不在自选模式时，退出播放
            if (!bp.ShouldBandPlaying || !bp.customMode)
            {
                StopGradually();
                PostStop();
                return;
            }

            //游戏窗口失焦且后台音频未开启时暂停
            if (Main.gameInactive && !_backgroundAudio)
            {
                _currentSoundInstance.Pause();
            }
            else if (_currentSoundInstance.State == SoundState.Paused)
            {
                _currentSoundInstance.Resume();
            }

            if (_currentSoundInstance.State == SoundState.Stopped)
            {
                RollListedMusic(false, true);
            }
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        public static void SetVolume()
        {
            if (_exitPlay <= 0)
            {
                //当月总出现前逐渐静音
                if (NPC.MoonLordCountdown > 0)
                {
                    //转换是必要的
                    float moonCDPercent = (float)NPC.MoonLordCountdown / (float)NPC.MaxMoonLordCountdown;
                    moonCDPercent -= 0.5f;
                    _soundFade = MathHelper.Clamp(moonCDPercent, 0f, 1f);
                }
                else
                {
                    _soundFade = MathHelper.Clamp(_soundFade + 0.01f, 0f, 1f);
                }
            }

            //同步游戏音乐音量
            _currentSoundInstance.Volume = Main.musicVolume * _soundFade;
        }
        /// <summary>
        /// 播放列表内的音乐
        /// </summary>
        /// <param name="manual">是否为手动切换</param>
        /// <param name="inLoop">是否处于自动循环中，是则单曲循环时不会频繁打印播放信息</param>
        public static void RollListedMusic(bool manual, bool inLoop)
        {
            if (_noSoundFile)
                return;

            //一次循环中可用曲目的列表
            List<int> rollableSound = [];

            //若已滚动曲目列表长度大于总曲目列表长度，则清空已滚动曲目列表
            if (_rolledSoundIndex.Count >= _loadedSounds.Count && _playMode != PlayModeID.SingleLoop)
            {
                _rolledSoundIndex.Clear();
                Console.WriteLine($"{ConsoleMessageHead}已完成一次列表播放。");
            }
            for (int i = 0; i < _loadedSounds.Count; i++)
            {
                //若 i 被包含在已滚动曲目列表中则跳过
                if (_rolledSoundIndex.Contains(i))
                    continue;

                //将余下的可用曲目加入列表
                rollableSound.Add(i);
            }
            switch (_playMode)
            {
                case PlayModeID.SingleLoop:

                    //手动切换时目标为当前循环曲目的下一首
                    int nextSound = _lastPlayedSound + 1;

                    //若超过列表边界则回归0
                    if (nextSound >= _loadedSounds.Count)
                    {
                        nextSound = 0;
                    }

                    //若为手动切换则按列表顺序选择音乐
                    int targetIndex = manual ? nextSound : _lastPlayedSound;
                    PlayMusic(targetIndex, !inLoop);
                    break;

                case PlayModeID.RandomLoop:
                    PlayMusic(Main.rand.NextFromCollection(rollableSound));
                    break;

                case PlayModeID.ListLoop:
                    PlayMusic(rollableSound.First());
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        public static void Stop()
        {
            _currentSoundInstance?.Stop();
            _currentSoundInstance?.Dispose();
            _currentSoundInstance = null;
        }
        /// <summary>
        /// 渐进式停止播放
        /// </summary>
        public static void StopGradually()
        {
            _soundFade = MathHelper.Clamp(_soundFade - 0.005f, 0f, 1f);
            if (_soundFade <= 0f && _exitPlay == ExitProcess.PrintMessage)
            {
                Stop();
                _exitPlay = ExitProcess.Exited;
            }
        }
        #region 加载非WAV文件
        /// <summary>
        /// 加载.ogg文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SoundEffect LoadOggFile(string filePath)
        {
            using VorbisReader vorbis = new(filePath);
            float[] samples = new float[vorbis.TotalSamples * vorbis.Channels];
            vorbis.ReadSamples(samples, 0, samples.Length);

            byte[] buffer = new byte[samples.Length * 2];
            for (int i = 0; i < samples.Length; i++)
            {
                short val = (short)(samples[i] * short.MaxValue);
                buffer[i * 2] = (byte)(val & 0xFF);
                buffer[i * 2 + 1] = (byte)((val >> 8) & 0xFF);
            }

            return new SoundEffect(
                buffer,
                vorbis.SampleRate,
                (AudioChannels)vorbis.Channels);
        }
        /// <summary>
        /// 加载.mp3文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SoundEffect LoadMp3File(string filePath)
        {
            // 使用MP3Sharp解码MP3文件
            using MP3Stream mp3Stream = new(filePath);
            using var memoryStream = new MemoryStream();

            // 将解码后的PCM数据写入内存流
            mp3Stream.CopyTo(memoryStream);

            // 重置流位置以便读取
            memoryStream.Position = 0;

            // 创建XNA SoundEffect
            return new SoundEffect(
                memoryStream.ToArray(),
                mp3Stream.Frequency,
                AudioChannels.Stereo);
        }
        #endregion
    }
}
