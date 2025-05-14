using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using XPT.Core.Audio.MP3Sharp;
using NVorbis;
using Terraria;

namespace TouhouPets
{
    public static class CustomMusicManager
    {
        private static SoundEffectInstance _currentSoundInstance;
        private static List<SoundEffect> _loadedSounds = [];
        private static List<string> _loadedSoundFiles = [];
        public static List<SoundEffect> LoadedSounds { get => _loadedSounds; }

        public static readonly string fullPath = Path.Combine(ModLoader.ModPath, "TouhouPetCustomMusic");
        public static void EnsureMusicFolder()
        {
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
        /// <summary>
        /// 初始化并加载 Musics 文件夹中的所有音频文件
        /// </summary>
        public static void Initialize(string musicsFolderPath)
        {
            if (!Directory.Exists(musicsFolderPath))
            {
                Console.WriteLine($"错误：文件夹 {musicsFolderPath} 不存在！现已生成新的文件夹。");
                if (!Directory.Exists(musicsFolderPath))
                {
                    Directory.CreateDirectory(musicsFolderPath);
                }
                return;
            }

            // 清空已加载的音效
            foreach (var sound in _loadedSounds)
            {
                sound.Dispose();
            }
            _loadedSounds.Clear();
            _loadedSoundFiles.Clear();

            // 加载所有支持的音频文件
            string[] supportedExtensions = [".mp3", ".ogg", ".wav"];
            foreach (string file in Directory.GetFiles(musicsFolderPath))
            {
                string extension = Path.GetExtension(file).ToLower();
                if (!supportedExtensions.Contains(extension))
                    continue;

                try
                {
                    Console.WriteLine($"正在加载音频: {Path.GetFileName(file)}");
                    SoundEffect sound = LoadSoundEffect(file, extension);
                    if (sound != null)
                    {
                        _loadedSounds.Add(sound);
                        _loadedSoundFiles.Add(file);
                        Console.WriteLine($"已加载: {Path.GetFileName(file)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载文件 {file} 失败: {ex.Message}");
                }
            }

            if (_loadedSounds.Count == 0)
            {
                Console.WriteLine("未找到任何支持的音频文件。");
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
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        return LoadOggFile(filePath);
                    }

                case ".mp3":
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        return LoadMp3File(filePath);
                    }

                default:
                    throw new NotSupportedException($"不支持的格式: {extension}");
            }
        }
        /// <summary>
        /// 播放指定索引的音乐文件
        /// </summary>
        public static void PlayMusic(int index)
        {
            string fileName = Path.GetFileName(_loadedSoundFiles[index]);
            if (!File.Exists(_loadedSoundFiles[index]))
            {
                ModUtils.WriteLineOrNewTextNotice($"错误：文件 {fileName} 不存在！");
                return;
            }
            // 先停止当前播放
            Stop();
            try
            {
                SoundEffect sound = LoadedSounds[index];
                _currentSoundInstance = sound.CreateInstance();
                _currentSoundInstance.IsLooped = true;
                _currentSoundInstance.Play();
                Console.WriteLine($"正在播放: {fileName}");
            }
            catch (Exception ex)
            {
                ModUtils.WriteLineOrNewTextNotice($"播放失败: {ex.Message}");
            }
        }
        public static void Update()
        {
            if (_currentSoundInstance == null)
            {
                return;
            }
            _currentSoundInstance.Volume = Main.musicVolume;
            if (Main.gamePaused || Main.GlobalTimerPaused)
            {
                _currentSoundInstance.Pause();
            }
            if (!Main.LocalPlayer.GetModPlayer<ConcertPlayer>().prismriverBand)
            {
                _currentSoundInstance.Stop();
            }
            if (_currentSoundInstance.State == SoundState.Playing)
            {
                Main.musicFade[Main.curMusic] = 0;
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
        /// 加载.ogg文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SoundEffect LoadOggFile(string path)
        {
            using (var vorbis = new VorbisReader(path))
            {
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
        }
        /// <summary>
        /// 加载.mp3文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SoundEffect LoadMp3File(string filePath)
        {
            // 使用MP3Sharp解码MP3文件
            using (var mp3Stream = new MP3Stream(filePath))
            using (var memoryStream = new MemoryStream())
            {
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
        }
    }
}
