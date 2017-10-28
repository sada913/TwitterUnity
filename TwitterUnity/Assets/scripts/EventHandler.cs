using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twitter;
namespace Twitter
{

    public class EventHandler : MonoBehaviour
    {
        public static bool isOauth = false;
        public TestTwitter twitter;
        [SerializeField] string api_s;
        [SerializeField] string token;
        [SerializeField] string token_s;


        void Start()
        {
            Twitter.Oauth.consumerKey = "nifATKyTGVVmhyTkVSQGcEfmt";
            Twitter.Oauth.consumerSecret = "wLABePmX4iekNmR3Sn9ipW4YwRNZ1JXhtXJCDsN7YHG7mKNWOR";
            Twitter.Oauth.accessToken = "837528735100239873-bS9fDIXeQNycXmW5gmdT106Ojzy2btM";
            Twitter.Oauth.accessTokenSecret = "WBBP9wkpGG7RE4bvAuueUKhMOqnzhXqY0ckl3H0DCRNEp";
            isOauth = true;

            
            
        }

        // Update is called once per frame
        void Update()
        {

        }


    }

}