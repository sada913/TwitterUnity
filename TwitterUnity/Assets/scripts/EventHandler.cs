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


        void Start()
        {
            Twitter.Oauth.consumerKey = "2cvM3kz0SjdFnqWr2F4fdlyFZ";
            Twitter.Oauth.consumerSecret = "cLCwgmyHoDrm81L9Sj3Th15RBOyB7g8jCfo9lAyye90EyXpPmI";
            Twitter.Oauth.accessToken = "2949334176-ot6ThM3aZ65a0tXwfKcLwwaSWoIEyZMm6maWfXt";
            Twitter.Oauth.accessTokenSecret = "SE0TpuotgwC3qNpieR6pUohcAncMIP6FmNHw3M5p8VcbN";
            isOauth = true;

            
            
        }

        // Update is called once per frame
        void Update()
        {

        }


    }

}