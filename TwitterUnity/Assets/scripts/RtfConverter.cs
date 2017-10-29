﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twitter;
using System.IO;

namespace Twitter
{
    [System.Serializable]
    public class RtfData
    {
        public long user_id;
        public string data;
    };
    public class RtfConverter :MonoBehaviour
    {

        public RtfData data;
        Tweet tweet_;

        [SerializeField] List<int> cmd_num = new List<int>();
        bool iscrate = true;
        string font_size_r = " \\fs";
        string center_r = "\\par\\pard\\sa200\\sl276\\slmult1\\qc ";
        string init_r = "\\par\\pard\\sa200\\sl276\\slmult1 \\fs22 ";

        char font_size_t = 'f';
        char center_t = 'c';
        char init_t = 'i';
        [SerializeField] string[] s;
        public string tmp = "";

        enum cmdid { add, end, creat, creat_center,other };
        [SerializeField]　cmdid cm;

        public List<RtfData> rtf_list = new List<RtfData>();

        [SerializeField] bool istext_cmd = false;

        protected string rtfinit = "{\\rtf1\\ansi\\ansicpg932\\deff0{\\fonttbl{\\f0\\fnil\\fcharset128 \\'82\\'6c\\'82\\'72 \\'96\\'be\\'92\\'a9;}}{\\colortbl ;\\red255\\green0\\blue0;}{\\*\\generator Msftedit 5.41.21.2510;}\\viewkind4\\uc1\\pard\\sa200\\sl276\\slmult1\\lang17\\f0\\fs22";
        protected string rtfinit_c = "{\\rtf1\\ansi\\ansicpg932\\deff0{\\fonttbl{\\f0\\fnil\\fcharset128 \\'82\\'6c\\'82\\'72 \\'96\\'be\\'92\\'a9;}}{\\colortbl ;\\red255\\green0\\blue0;}{\\*\\generator Msftedit 5.41.21.2510;}\\viewkind4\\uc1\\pard\\sa200\\sl276\\slmult1\\qc\\lang17\\f0\\fs22";
        public void Tweet_in(Tweet t)
        {
            tweet_ = t;
            s = TweetSpliter(t.text);
            TweetToRtf(s, istext_cmd, t.user.id);

        }

        public void debug_tweet(string s)
        {

            TweetToRtf(TweetSpliter(s), istext_cmd, 2949334176);
        }

        IEnumerator post(RtfData r, Tweet tweet)
        {
            string url = "http://sada-913.xyz/Unity/TwitterToRTF/rtfdl.php";
            WWWForm form = new WWWForm();
            form.AddField("id", r.user_id.ToString());
            form.AddField("data", r.data);


            WWW www = new WWW(url, form);
            yield return www;
            DM("レポートはこちら" + "http://sada-913.xyz/Unity/TwitterToRTF/" + r.user_id + ".rtf", tweet);
           // Reply("レポートはこちら" + "http://sada-913.xyz/Unity/TwitterToRTF/" + r.user_id + ".rtf", tweet);
        }
        public string[] TweetSpliter(string text)
        {
            int n = 0;
            string[] s = text.Split(' ');

            TweetCmdChecker(s[1]);
            for(int i = 0; i< s.Length;i++)
            {
                if (s[i][0] == '-')
                    n++;
            }

            if(n >= 2)
            {
                istext_cmd = true;
            }

            return s;
        }

        public void TweetCmdChecker(string cmd)
        {
            if (cmd == "-a")
            {
                cm = cmdid.add;
            }
            else if (cmd == "-e")
            {
                cm = cmdid.end;
            }
            else if (cmd == "-n")
            {
                cm = cmdid.creat;
            }
            else if(cmd == "-nc" )
            {
                cm = cmdid.creat;
            }
            else
                cm = cmdid.other;


        }

        public string TweetToRtf(string[] text, bool text_cmd,long usr_id)
        {


            CREAT:
            if (cm == cmdid.creat)
            {
                RtfData rtf = new RtfData { user_id = usr_id, data = rtfinit };
                if(text[1] == "-nc" || text[1] == "-cn")
                {
                    rtf.data = rtfinit_c;
                }
                if(rtf_list.Count != 0)
                {
                    for(int i = 0;i<rtf_list.Count;i++)
                    {
                       if(rtf_list[i].user_id ==usr_id)
                       {
                            cm = cmdid.add;
                            goto Add;
                       }
                    }
                }
                iscrate = true;
                if (istext_cmd)
                {
                    for(int i = 2;i < text.Length; i++)
                    {
                        if(text[i][0] == '-')
                        {
                            cmd_num.Add(i);//rtfコマンドの場所の添字int
                        }

                    }
                    
                    for(int i = 0;i<cmd_num.Count;i++)
                    {
                        //コマンドチェックして.rtfの中身を返す関数
                        rtf.data += CmdToRtf(text[cmd_num[i]], text, cmd_num[i], ref i,ref rtf);
                        Debug.Log("Cのi" + i);
                        
                    }
                }
                else
                {
                    if (text.Length >= 3)
                        for (int i = 3; i <= text.Length; i++)
                            rtf.data += text[i - 1];
                }
                tmp = rtf.data;
                rtf_list.Add(rtf);

                Debug.Log(rtf_list[0].data + "         -n");
                //デバッグ
            }

            Add:
            if(cm == cmdid.add)
            {
                RtfData rtf = new RtfData { user_id = -1, data = null };
                int rtf_data_num =-1;
                for(int i = 0;i<rtf_list.Count;i++)
                {
                    if(rtf_list[i].user_id == usr_id)
                    {
                        rtf_data_num = i;
                        break;
                    }
                }

                if (rtf_data_num == -1)
                {
                    cm = cmdid.creat;
                    goto CREAT;
                }

                iscrate = false;
                if (istext_cmd)
                {
                    for (int i = 2; i < text.Length; i++)
                    {
                        if (text[i][0] == '-')
                        {
                            cmd_num.Add(i);//rtfコマンドの場所の添字int
                        }

                    }

                    for (int i = 0; i < cmd_num.Count; i++)
                    {
                        //コマンドチェックして.rtfの中身を返す関数
                        rtf_list[rtf_data_num].data += CmdToRtf(text[cmd_num[i]], text, cmd_num[i],ref i,ref rtf);

                    }
                }
                else
                {
                    if (text.Length >= 3)
                        for (int i = 3; i <= text.Length; i++)
                            rtf_list[rtf_data_num].data += text[i - 1];
                }
                tmp = rtf_list[rtf_data_num].data;
                Debug.Log(rtf_list[rtf_data_num].user_id);
                Debug.Log(rtf_list[rtf_data_num].data + "        -a");
            }
            else if(cm == cmdid.end)
            {
                RtfData rtf = new RtfData { user_id = -1, data = null };
                int rtf_data_num = -1;
                for (int i = 0; i < rtf_list.Count; i++)
                {
                    if (rtf_list[i].user_id == usr_id)
                    {
                        rtf_data_num = i;
                        break;
                    }
                }

                if(rtf_data_num  == -1)
                {
                    cm = cmdid.creat;
                    goto CREAT;
                }

                iscrate = false;
                if (istext_cmd)
                {
                    for (int i = 2; i < text.Length; i++)
                    {
                        if (text[i][0] == '-')
                        {
                            cmd_num.Add(i);//rtfコマンドの場所の添字int
                        }

                    }

                    for (int i = 0; i < cmd_num.Count; i++)
                    {
                        //コマンドチェックして.rtfの中身を返す関数
                        rtf_list[rtf_data_num].data += CmdToRtf(text[cmd_num[i]], text, cmd_num[i],ref i,ref rtf);

                    }
                }
                else
                {
                    if(text.Length >= 3)
                        for(int i = 3;i<=text.Length ;i++)
                            rtf_list[rtf_data_num].data += text[i-1];
                        //Debug.Log(text[2]);
                }

                rtf_list[rtf_data_num].data += "}";


                tmp = rtf_list[rtf_data_num].data;
                Debug.Log(rtf_list[rtf_data_num].user_id);
                Debug.Log(rtf_list[rtf_data_num].data);
                data = rtf_list[rtf_data_num];
                StartCoroutine(post(data, tweet_));

                rtf_list.RemoveAt(rtf_data_num);
            }
            

            return tmp;

        }
        public string CmdToRtf(string cmd,string[] text,int string_num_soezi,ref int j,ref　RtfData rtf )
        {

            Debug.Log("添字は"+string_num_soezi);
            Debug.Log(" CtoRの" + j);
            bool check = false ;
            string rtftext = "";
            if(cmd[1] == font_size_t)
            {
                string n = "";
                int font_num;
                for (int i = 2; i < cmd.Length; i++)
                {
                    n += cmd[i];
                }
                font_num = int.Parse(n);
                //Debug.Log(font_num);
                rtftext += font_size_r + font_num.ToString();
            }
            else if(iscrate && cmd[1] == center_t && (string_num_soezi == 2 ) )
            {
                Debug.Log("最初のセンター");

                    rtf.data = rtfinit_c;
            }
            //ここそのうち変える
            else if(cmd[1] == center_t)
            {
                rtftext += center_r;
            }
            else if(cmd[1] == init_t)
            {
                rtftext += init_r;
            }

            if (text.Length > string_num_soezi + 1)
            {
                if (text[string_num_soezi + 1][0] == '-')
                {
                    //cmd_num.Remove(string_num_soezi);
                    int a = 0;
                    j++;
                    return CmdToRtf(text[string_num_soezi + 1], text, string_num_soezi + 1, ref a,ref rtf);
                }

                if (check == false)
                    rtftext += text[string_num_soezi + 1];
            }
            if(cmd[1] == center_t)
            {
                rtftext += "\\par";
            }
            cmd_num.Clear();
            return rtftext;
        }


        /// <summary>
        /// リプ
        /// </summary>
        /// <param name="text"></param>
        public void Reply(string text, Tweet tweet)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["status"] =  text+" " +"@" + tweet.user.screen_name;  // 前半部分がリプの相手後半内容
            parameters["in_reply_to_status_id"] = tweet.id_str;
            StartCoroutine(Twitter.Client.Post("statuses/update", parameters, this.Callback));
        }

        public void DM(string text, Tweet tweet)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["text"] = text;  // 前半部分がリプの相手後半内容
            parameters["user_id"] = tweet.user.id_str;
            StartCoroutine(Twitter.Client.Post("direct_messages/new", parameters, this.DMCallback));
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
                Debug.Log(tweet.text);
            }
            else
            {
                Debug.Log(response);
            }
        }

        void DMCallback(bool success, string response)
        {
            if (success)
            {
                DirectMessage DM = JsonUtility.FromJson<DirectMessage> (response); // 投稿したツイートが返ってくる
                Debug.Log(DM.text);
            }
            else
            {
                Debug.Log(response);
            }
        }

    }

    
}
