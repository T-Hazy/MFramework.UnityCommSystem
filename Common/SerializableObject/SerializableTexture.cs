using System;
using MFramework.CommSystem;
using UnityEngine;
using TextureFormat = UnityEngine.TextureFormat;

namespace MFramework.Network.Core
{
    public class SerializableTexture : SerializableObject
    {
        public TextureFormatType EncodingType { get; private set; }
        public Texture2D Texture { get; private set; }
        public byte[] TextureData { get; private set; }

        public SerializableTexture(byte[] data) : base(data) {
        }

        protected override int GetFieldsContainerSize() {
            return sizeof(TextureFormatType) + sizeof(int) + TextureData.Length;
        }

        protected override void SerializeFields() {
            //编码类型
            SerializeInt((int)EncodingType);
            //纹理
            SerializeBytes(TextureData);
        }

        protected override void DeserializeFields() {
            EncodingType = (TextureFormatType)DeserializeInt();
            TextureData = DeserializeBytes();
            //TODO 使用Texture Async Loader来异步创建纹理
            if (TextureData != null) {
                Texture = new Texture2D(1, 1);
                Texture.LoadImage(TextureData);
            }
        }

        public SerializableTexture(Texture2D texture, TextureFormatType encodingType) {
            Texture = MakeTextureAvailable(texture);
            EncodingType = encodingType;
            switch (EncodingType) {
                case TextureFormatType.PNG:
                    TextureData = Texture.EncodeToPNG();
                    break;
                case TextureFormatType.JPG:
                    TextureData = Texture.EncodeToJPG();
                    break;
                case TextureFormatType.EXR:
                    TextureData = Texture.EncodeToEXR();
                    break;
                case TextureFormatType.TGA:
                    TextureData = Texture.EncodeToTGA();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 使纹理可用
        /// </summary>
        /// <param name="texture">原始纹理</param>
        /// <param name="makeReadable">是否使纹理可读</param>
        /// <param name="makeUncompressed">是否解压纹理</param>
        /// <returns>处理后的纹理</returns>
        private static Texture2D MakeTextureAvailable(Texture2D texture, bool makeReadable = true,
            bool makeUncompressed = true) {
            if (!makeReadable && !makeUncompressed) return texture;
            // 创建一个临时的 RenderTexture
            RenderTexture tempRT = RenderTexture.GetTemporary(texture.width, texture.height, 0,
                RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // 将原始纹理拷贝到临时的 RenderTexture
            Graphics.Blit(texture, tempRT);

            // 绑定 RenderTexture 为活动 RenderTexture
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tempRT;

            // 创建一个新的 Texture2D 并从 RenderTexture 中读取像素
            Texture2D availableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            availableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            availableTexture.Apply();

            // 恢复先前的 RenderTexture
            RenderTexture.active = previous;

            // 释放临时的 RenderTexture
            RenderTexture.ReleaseTemporary(tempRT);

            return availableTexture;
        }
    }
}