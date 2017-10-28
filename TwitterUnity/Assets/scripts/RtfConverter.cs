using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twitter;
using System.IO;

namespace Twitter
{

    public class RtfConverter :MonoBehaviour
    {
        [System.Serializable]
        public class RtfData
        {
            public long user_id;
            public string data;
        };

            string font_size_r = " \\fs";
            string center_r = "\\pard\\sa200\\sl276\\slmult1\\qc ";
            string init_r = "\\pard\\sa200\\sl276\\slmult1 ";

            char font_size_t = 'f';
            char center_t = 'c';
            char init_t = 'i';

        public string tmp = "";

        enum cmdid { add, end, creat };
        [SerializeField]　cmdid cm;

        public List<RtfData> rtf_list = new List<RtfData>();

        [SerializeField] bool istext_cmd = false;

        protected string rtfinit = "{\\rtf1\\ansi\\ansicpg932\\deff0{\\fonttbl{\\f0\\fnil\\fcharset128 \\'82\\'6c\\'82\\'72 \\'96\\'be\\'92\\'a9;}}{\\colortbl ;\\red255\\green0\\blue0;}{\\*\\generator Msftedit 5.41.21.2510;}\\viewkind4\\uc1\\pard\\sa200\\sl276\\slmult1\\lang17\\f0\\fs22";

        public IEnumerator Tweet_in(string text,long usr_id)
        {
            string[] s = TweetSpliter(text);
            TweetToRtf(s, istext_cmd, usr_id);

            yield return null;
        }
        public string[] TweetSpliter(string text)
        {
            string[] s = text.Split(' ');

            TweetCmdChecker(s[1]);

            if(s.Length > 3)
            {
                istext_cmd = true;
            }

            return s;
        }

        public void TweetCmdChecker(string cmd)
        {
            if(cmd == "-a")
            {
                cm = cmdid.add;
            }
            else if(cmd == "-e")
            { 
                cm = cmdid.end;
            }
            else if(cmd == "-n")
            {
                cm = cmdid.creat;
            }


        }

        public string TweetToRtf(string[] text, bool text_cmd,long usr_id)
        {
            List<int> cmd_num = new List<int>();

            CREAT:
            if (cm == cmdid.creat)
            {
                RtfData rtf = new RtfData { user_id = usr_id, data = rtfinit };
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
                        rtf.data += CmdToRtf(text[cmd_num[i]], text, cmd_num[i]);
                        
                    }
                }
                else
                {
                    if (text.Length != 2)
                        rtf.data += text[2];
                }
                tmp = rtf.data;
                rtf_list.Add(rtf);

                Debug.Log(rtf_list[0].data + "         -n");
                //デバッグ
            }

            Add:
            if(cm == cmdid.add)
            {
                
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
                        rtf_list[rtf_data_num].data += CmdToRtf(text[cmd_num[i]], text, cmd_num[i]);

                    }
                }
                else
                {
                    if (text.Length != 2)
                        rtf_list[rtf_data_num].data += text[2];
                }
                Debug.Log(rtf_list[rtf_data_num].user_id);
                Debug.Log(rtf_list[rtf_data_num].data + "        -a");
            }
            else if(cm == cmdid.end)
            {
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
                        rtf_list[rtf_data_num].data += CmdToRtf(text[cmd_num[i]], text, cmd_num[i]);

                    }
                }
                else
                {
                    if(text.Length != 2)
                        rtf_list[rtf_data_num].data += text[2];
                        //Debug.Log(text[2]);
                }

                rtf_list[rtf_data_num].data += "}";


                tmp = rtf_list[rtf_data_num].data;
                Debug.Log(rtf_list[rtf_data_num].user_id);
                Debug.Log(rtf_list[rtf_data_num].data);
                rtf_list.RemoveAt(rtf_data_num);
            }

            return tmp;

        }
        public string CmdToRtf(string cmd,string[] text,int string_num_soezi)
        {
            
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
            else if(cmd[1] == center_t)
            {
                rtftext += center_r;
            }
            else if(cmd[1] == init_t)
            {
                rtftext += init_r;
            }

            rtftext += text[string_num_soezi + 1];

            return rtftext;
        }

    }

    
}
