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
        [SerializeField] string api;
        [SerializeField] string api_s;
        [SerializeField] string token;
        [SerializeField] string token_s;


        void Start()
        {
            Twitter.Oauth.consumerKey = api;
            Twitter.Oauth.consumerSecret = api_s;
            Twitter.Oauth.accessToken = token;
            Twitter.Oauth.accessTokenSecret = token_s;
            isOauth = true;

            
            
        }

        // Update is called once per frame
        void Update()
        {

        }


    }

}