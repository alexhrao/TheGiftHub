using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;

namespace GiftServer
{
    namespace Data
    {
        public class UserEvent : ISynchronizable
        {
            public long EventUserID;
            public User user;
            public DefaultEvent defaultEvent;
            public string Name;
            public string Descrption;
            public int Day;
            public int Month;
            public int Year;
            public bool IsRecurring;
            public EventFuture[] Futures;

            public UserEvent() { }
            public UserEvent(long EventUserID)
            {
                this.EventUserID = EventUserID;
            }

            public bool Create()
            {
                return false;
            }
            public bool Update()
            {
                return false;
            }
            public bool Delete()
            {
                return false;
            }
        }
    }
}
