using UnityEngine;

namespace VSMenuHelper
{
    internal class SpriteImporter
    {
        public static Texture2D LoadTexture(string FilePath)
        {
            Texture2D texture;

            if (!File.Exists(FilePath))
            {
                throw new ArgumentException("FilePath does not exist.");
            }

            byte[] imageBytes = File.ReadAllBytes(FilePath);
            texture = new Texture2D(2, 2);
            if (!ImageConversion.LoadImage(texture, imageBytes))
            {
                throw new Exception("ImageConversion.LoadImage failed");
            }

            return texture;

        }

        // Won't throw an exception
        public static Texture2D? TryLoadTexture(string FilePath)
        {
            try
            {
                Texture2D texture;

                if (File.Exists(FilePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(FilePath);
                    texture = new Texture2D(2, 2);
                    if (ImageConversion.LoadImage(texture, imageBytes))
                        return texture;
                }
            }
            catch { }
            return null;
        }

        public static Sprite LoadSprite(string FilePath)
        {
            Texture2D texture = LoadTexture(FilePath);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static Sprite LoadSprite(string FilePath, Rect rect, Vector2 pivot)
        {
            Texture2D texture = LoadTexture(FilePath);
            return Sprite.Create(texture, rect, pivot);
        }

        public static Sprite? TryLoadSprite(string FilePath)
        {
            try
            {
                Texture2D? texture = TryLoadTexture(FilePath);
                if (texture == null) return null;
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            catch { }
            return null;
        }

        public static Sprite? TryLoadSprite(string FilePath, Rect rect, Vector2 pivot)
        {
            try
            {
                Texture2D? texture = TryLoadTexture(FilePath);
                if (texture == null) return null;
                return Sprite.Create(texture, rect, pivot);
            }
            catch { }
            return null;
        }
    }
}
