using System;

namespace RedisApp2.Models
{
    public class User
    {
        public  String username;
        public String password;



        public override string ToString()
        {
            return username + " " + password;
        }
    }

    
}
