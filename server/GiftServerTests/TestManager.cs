using System;

namespace GiftServerTests
{
    public static class TestManager
    {
        public static byte[] Image
        {
            get
            {
                string[] imgBytes = ("89 50 4e 47 0d 0a 1a 0a 00 00 00 0d 49 48 44 52 00 00 00 01 00 00 00 01 01 00 00 00 00 37 6e f9 24 00 00 00 10 49 44 41 54 78 " +
                    "9c 62 60 01 00 00 00 ff ff 03 00 00 06 00 05 57 bf ab d4 00 00 00 00 49 45 4e 44 ae 42 60 82").Split(' ');
                byte[] img = new byte[imgBytes.Length];
                for (int i = 0; i < imgBytes.Length; i++)
                {
                    // Fill in corr:
                    img[i] = Convert.ToByte(imgBytes[i], 16);
                }
                return img;
            }
        }
    }
}