using GiftServer.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace GiftServerTests
{
    [TestClass]
    public class TestManager
    {
        public static byte[] Image
        {
            get
            {
                string[] imgBytes = ("89 50 4e 47 0d 0a 1a 0a 00 00 00 0d 49 48 44 52 00 00 00 01 00 00 00 " +
                    "01 01 00 00 00 00 37 6e f9 24 00 00 00 10 49 44 41 54 78 " +
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
        [TestMethod]
        public void IntegrityCheck()
        {
            // Check integrity of all data:
            // Check all users have preferences:

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                List<ulong> uids = new List<ulong>();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    // Get all users, then get all preferences
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT UserID FROM users;";
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uids.Add(Convert.ToUInt64(reader["UserID"]));
                        }
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT UserID FROM preferences;";
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Find UserID in list, delete
                            uids.Remove(Convert.ToUInt64(reader["UserID"]));
                        }
                    }
                }
                Assert.IsTrue(uids.Count == 0, "User(s) don't have preferences!");
            }

            // Check that each gift with reservation can be seen by reserver
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                // First get all the reservations. Then, for each reservation, check that the Gift and the Reserver can see each other
                List<Reservation> res = new List<Reservation>();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ReservationID FROM reservations;";
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Reservation(Convert.ToUInt64(reader["ReservationID"])));
                        }
                    }
                }
                // for each reservation, get gift and reserver
                foreach (var r in res)
                {
                    ulong groupId = 0;
                    User reserver = r.User;
                    Gift gift = r.Gift;
                    // Check that the gift is shared in AT LEAST one group the reserver is in
                    foreach (var g in gift.Groups)
                    {
                        // see if any of user's groups have this ID:
                        if (reserver.Groups.Exists(group => group.Equals(g)))
                        {
                            // break, good to go!
                            groupId = g.ID;
                            break;
                        }
                    }
                    Assert.AreNotEqual(0UL, groupId, "Gift " + gift.ID + ", reserved by User " + reserver.ID + ", is not " +
                        "shared correctly. The gift is not shared in a group the reserver is a member of!");
                    // If it is true, check that the owner is in THAT group!
                    User owner = gift.Owner;
                    Assert.IsTrue(owner.Groups.Exists(g => g.ID == groupId), "Owner " + owner.ID + 
                        " of gift " + gift.ID + " is not in group " + groupId);
                }
            }

            // Check that, for every gift, all group's its shared in are groups the user is a part of
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    List<Gift> gifts = new List<Gift>();
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT GiftID FROM gifts;";
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            gifts.Add(new Gift(Convert.ToUInt64(reader["GiftID"])));
                        }
                    }
                    // For each gift, check all groups and owners:
                    foreach (var gift in gifts)
                    {
                        User owner = gift.Owner;
                        List<Group> giftGroups = gift.Groups;
                        List<Group> ownerGroups = owner.Groups;
                        foreach (var group in giftGroups)
                        {
                            Assert.IsTrue(ownerGroups.Exists(g => g.Equals(group)), "Group " + 
                                group.ID + " can see Gift " + gift.ID + ", but owner " + owner.ID + " is not part of that group");
                        }
                    }
                }
            }
            // For each event, check that all groups it shares are shared by owner as well
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    List<Event> events = new List<Event>();
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT EventID FROM user_events;";
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new Event(Convert.ToUInt64(reader["EventID"])));
                        }
                    }
                    // For each gift, check all groups and owners:
                    foreach (var e in events)
                    {
                        User owner = e.User;
                        List<Group> eGroups = e.Groups;
                        List<Group> ownerGroups = owner.Groups;
                        foreach (var group in eGroups)
                        {
                            Assert.IsTrue(ownerGroups.Exists(g => g.Equals(group)), "Group " +
                                group.ID + " can see Event " + e.ID + ", but owner " + owner.ID + " is not part of that group");
                        }
                    }
                }
            }
        }

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            Reset().Wait();
        }
        [ClassCleanup]
        public static void Cleanup()
        {
            Reset().Wait();
        }
    }
}