using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Threading.Tasks;

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

        async public static Task Reset()
        {
            await Task.Run(() =>
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "CALL gift_registry_db_test.setup();";
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }
    }
}