﻿using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using PromotItLibrary.Classes;
using PromotItLibrary.Classes.Users;
using PromotItLibrary.Enums;
using PromotItLibrary.Interfaces.LinkedList;
using PromotItLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotItLibrary.Patterns.LinkedLists.LinkedLists_MySql_State
{
    public class LinkedListTweet_MySql : ILinkedListTweet
    {

        private readonly MySQL _mySQL;
        private readonly Tweet _tweet;

        public LinkedListTweet_MySql(Tweet tweet, MySQL mySQL) 
        {
            _mySQL = mySQL;
            _tweet = tweet;
        }

        public async Task<List<Tweet>> GetAllTweets_ListAsync(Modes mode = null)
        {
            List<Tweet> tweetList = new List<Tweet>();
            _mySQL.Quary("SELECT campaign_hashtag,activist_user_name,retweets FROM tweets");
            using MySqlDataReader results = await _mySQL.GetQueryMultyResultsAsync();
            while (results != null && results.Read()) //for 1 result: if (mdr.Read())
            {
                try
                {
                    tweetList.Add
                        (
                            new Tweet()
                            {
                                Campaign = new Campaign() { Hashtag = results.GetString("campaign_hashtag"), },
                                ActivistUser = new ActivistUser() { UserName = results.GetString("activist_user_name"), },
                                Retweets = int.Parse(results.GetString("retweets")),
                            }
                        );
                }
                catch { };
            }
            return tweetList;
        }


    }
}
