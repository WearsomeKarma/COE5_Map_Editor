using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using StbImageSharp;

namespace ConquestOfElysium5_MapEditor.Core
{
    public class Texture : IDisposable
    {
        public readonly int Handle;
        public readonly int Width;
        public readonly int Height;

        internal Texture(int handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
        }

        public void Unload()
        {
            Dispose();
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }

        public static Texture Load(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException("File does not exist.");

            int texture_handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture_handle);

            int width, height;

            using (Stream stream = File.OpenRead(path))
            {
                ImageResult img = ImageResult.FromStream(stream);

                PixelInternalFormat internal_format;
                PixelFormat pixel_format;

                switch (img.Comp)
                {
                    case ColorComponents.Grey:
                        internal_format = PixelInternalFormat.Luminance;
                        pixel_format = PixelFormat.Luminance;
                        break;
                    case ColorComponents.GreyAlpha:
                        internal_format = PixelInternalFormat.LuminanceAlpha;
                        pixel_format = PixelFormat.LuminanceAlpha;
                        break;
                    case ColorComponents.RedGreenBlue:
                        internal_format = PixelInternalFormat.Rgb;
                        pixel_format = PixelFormat.Rgb;
                        break;
                    default:
                    case ColorComponents.Default:
                    case ColorComponents.RedGreenBlueAlpha:
                        internal_format = PixelInternalFormat.Rgba;
                        pixel_format = PixelFormat.Rgba;
                        break;
                }

                GL.TexImage2D
                (
                    TextureTarget.Texture2D,
                    0, internal_format,
                    width = img.Width, height = img.Height,
                    0,
                    pixel_format,
                    PixelType.UnsignedByte,
                    img.Data
                );
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return new Texture(texture_handle, width, height);
        }
    }
}
