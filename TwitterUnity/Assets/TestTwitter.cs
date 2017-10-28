using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twitter;
using UnityEngine.UI;

namespace Twitter
{
    public class TestTwitter : MonoBehaviour
    {
        Stream stream;
        int i = 0;
        public string search_srting;
        [SerializeField] Text text;
        [SerializeField] RawImage usr_imag;
        [SerializeField] RawImage[] image;
        [SerializeField] bool isFilter = false;
        [SerializeField] Texture tex;
        [SerializeField] Image im;


        private void Update()
        {
            if (EventHandler.isOauth && i == 0)
            {
                i = 1;
                if(isFilter)
                    Filter(search_srting);
                else
                    UserStream();
            }
            //認証が終わっていたら実行
        }


        /// <summary>
        /// ツイート
        /// </summary>
        /// <param name="text"></param>
        public void Tweet(string text)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["status"] = text;  // ツイートするテキスト
            StartCoroutine(Twitter.Client.Post("statuses/update", parameters, this.Callback));
        }


        /// <summary>
        /// フィルター
        /// </summary>
        /// <param name="search"></param>
        public void Filter(string search)
        {
            stream = new Stream(StreamType.PublicFilter);
            Dictionary<string, string> streamParameters = new Dictionary<string, string>();

            List<string> tracks = new List<string>();
            tracks.Add(search);
            Twitter.FilterTrack filterTrack = new Twitter.FilterTrack(tracks);
            streamParameters.Add(filterTrack.GetKey(), filterTrack.GetValue());
            StartCoroutine(stream.On(streamParameters, OnStream_user));
        }


        /// <summary>
        /// ユーザーストリーム
        /// </summary>
        public void UserStream()
        {
            stream = new Stream(StreamType.User);
            Dictionary<string, string> streamParameters = new Dictionary<string, string>();

            StartCoroutine(stream.On(streamParameters, OnStream_user));
        }


        /// <summary>
        /// 投稿したツイートのコールバック
        /// </summary>
        /// <param name="success"></param>
        /// <param name="response"></param>
        void Callback(bool success, string response)
        {
            if (success)
            {
                Tweet tweet = JsonUtility.FromJson<Tweet>(response); // 投稿したツイートが返ってくる
            }
            else
            {
                Debug.Log(response);
            }
        }


        /// <summary>
        /// ストリームのコールバック
        /// </summary>
        /// <param name="response"></param>
        /// <param name="messageType"></param>
        void OnStream_user(string response, StreamMessageType messageType)
        {
            try
            {
                if (messageType == StreamMessageType.Tweet)
                {
                    Tweet tweet = JsonUtility.FromJson<Tweet>(response);
                    text.text = (tweet.user.name + "\n" + tweet.text);
                    StartCoroutine(GetImage(tweet.user.profile_image_url,usr_imag));
                    if (tweet.entities.media != null)
                        StartCoroutine(GetImage(tweet.entities.media[0].media_url, image[0]));
                    else
                        image[0].texture = tex;
                    Debug.Log(tweet.entities.media);
                    //ファボテスト
                   // Fav(tweet);


                }
                else if (messageType == StreamMessageType.StreamEvent)
                {
                    StreamEvent streamEvent = JsonUtility.FromJson<StreamEvent>(response);
                    Debug.Log(streamEvent.event_name); // Response Key 'event' is replaced 'event_name' in this library.

                }
                else if (messageType == StreamMessageType.FriendsList)
                {
                    FriendsList friendsList = JsonUtility.FromJson<FriendsList>(response);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        IEnumerator GetImage(string url,RawImage image)
        {
            float width, height;
            float aspect = 1f;
            WWW www = new WWW(url);

            // 画像ダウンロード完了を待機
            yield return www;

            // webサーバから取得した画像をRaw Imagで表示する
            image.texture = www.texture;
            image.SetNativeSize();
            height = www.texture.height;
            width = www.texture.width;

            if(height > 300)
            {
                aspect = 300 / height;
            }
            else if(width > 530)
            {
                aspect = 530 / width;
            }
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(width * aspect, height *aspect);

        }

        



        /// <summary>
        /// ふぁぼる
        /// </summary>
        /// <param name="tweet"></param>
        void Fav(Tweet tweet)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["id"] = tweet.id_str; // ファボするツイートのID
            StartCoroutine(Twitter.Client.Post("favorites/create", parameters, this.FavCallback));
        }
        /// <summary>
        /// ファボのコールバック
        /// </summary>
        /// <param name="success"></param>
        /// <param name="response"></param>
        void FavCallback(bool success, string response)
        {
            if (success)
            {
                Debug.Log("Fav Done");
            }
            else
            {
                Debug.Log(response);
            }
        }
        /// <summary>
        /// リツイートする
        /// </summary>
        /// <param name="tweet"></param>
        void Retweet(Tweet tweet)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["id"] = tweet.id_str; // リツイートするツイートのID
            StartCoroutine(Twitter.Client.Post("statuses/retweet/" + tweet.id_str, parameters, this.RetweetCallback));
        }
        /// <summary>
        /// リツイートのコールバック
        /// </summary>
        /// <param name="success"></param>
        /// <param name="response"></param>
        void RetweetCallback(bool success, string response)
        {
            if (success)
            {
                Debug.Log("Retweet Done");
            }
            else
            {
                Debug.Log(response);
            }
        }

    }
}