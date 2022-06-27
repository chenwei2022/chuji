using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace QQ
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        DataOperator dataoper = new DataOperator();
        int messageindex = 0;
        int fid=0;
        int messagetype = 1;
        public static int Hid = 0;
        public static int Fhid = 0;
        public static string strflag="[离线]";
        ChatForm chatform;
        

        private void MainForm_Load(object sender, EventArgs e)
        {
            string sql = "select * from UserInfor where ID=" + PublicNum.LoginIdnum;
            SqlDataReader da = dataoper.GetDataReader(sql);
            if (da.Read())
            {
                NickName.Text = da["NickName"].ToString();
                Sign.Text = da["Sign"].ToString();
                Hid = int.Parse(da["HeadID"].ToString());//.....
                HeadPx.Image = imageHead.Images[Hid];
                ShowFriendList();
            }
            DataOperator.conn.Close();
            da.Close();
            ts_Message.Image = image_Message.Images[0];
        }

        private void ShowFriendList()
        {
            lvShowFriend.Items.Clear();
            string sql = "select FriendID,NickName,Flag,HeadID from UserInfor,Friend where Friend.HostID=" + PublicNum.LoginIdnum+"and Friend.FriendID=UserInfor.ID";
            SqlDataReader da =dataoper.GetDataReader(sql);
            int index = 0;
            while (da.Read())
            {
                if (da["Flag"].ToString() == "0")
                    strflag = "[离线]";
                else
                    strflag = "[在线]";
                lvShowFriend.Items.Add(da["FriendID"].ToString(),da["NickName"].ToString() + strflag,(int)da["HeadID"]);
                lvShowFriend.Items[index++].Group = lvShowFriend.Groups[0];
            }
            DataOperator.conn.Close();
            da.Close();
        }

        private void timer_Getmessage_Tick(object sender, EventArgs e)
        {
            string sql = "select Top 1 FromUserID,Message,MessageTypeID,MessageState from Message where ToUserID=" + PublicNum.LoginIdnum+ "and MessageState=0";
            SqlDataReader dr = dataoper.GetDataReader(sql);
            if (dr.Read())
            {
                if (lvShowFriend.SelectedItems.Count > 0)
                {
                    if (lvShowFriend.SelectedItems[0].Group == lvShowFriend.Groups[0]) 
                    {
                        ts_Delete.Visible = true;
                        ts_AddFriend.Visible = false;
                    }
                    else if (lvShowFriend.SelectedItems[0].Group == lvShowFriend.Groups[1]) 
                    {
                        ts_Delete.Visible = false;
                        ts_AddFriend.Visible = true;
                    }
                }
                fid = (int)dr["FromUserID"];
                messagetype = (int)dr["MessageTypeID"];
                DataOperator.conn.Close();
                dr.Close();
                if (messagetype==2)
                {
                    
                    SoundPlayer player = new SoundPlayer("system.wav");
                    player.Play();
                    timer_AddFriend_Remind.Start();
                    //MessageBox.Show("测试");
                }
                else if (messagetype == 1)
                {
                    if(!HasShowUser(fid))
                    {
                        UpdateStranger(fid);
                    }
                    
                    string s = "select HeadID from UserInfor where ID=" + fid;
                    Fhid=dataoper.ExecuteSql(s);
                    SoundPlayer player = new SoundPlayer("msg.wav");
                    player.Play();
                    timer_HeadTwinkle.Start();
                }
            }
            


        }

        private void UpdateStranger(int fid)
        {

            
        }

        private bool HasShowUser(int fid)
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < lvShowFriend.Groups[i].Items.Count; j++)
                {
                    if (Convert.ToInt32(lvShowFriend.Items[j].Name) == fid)
                        return true;
                }
            return false;
        }

        private void timer_HeadTwinkle_Tick(object sender, EventArgs e)
        {               
            for (int index = 0; index < lvShowFriend.Groups[0].Items.Count; index++)
            {                
                if (fid.ToString() == lvShowFriend.Groups[0].Items[index].Name)
                {
                    if (chatform != null /*&& chatform.chat_fid != 0*/)
                        lvShowFriend.Items[index].ImageIndex = Fhid;
                    else
                    {
                        if (lvShowFriend.Groups[0].Items[index].ImageIndex < 4)

                            lvShowFriend.Groups[0].Items[index].ImageIndex = 4;

                        else
                            lvShowFriend.Groups[0].Items[index].ImageIndex = Fhid;
                    }
                }
            }                       
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void 更新好友ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void timer_AddFriend_Remind_Tick(object sender, EventArgs e)
        {

            ts_Message.Image = image_Message.Images[messageindex=(messageindex==1?0:1)];
            //MessageBox.Show("测试");
        }
    }
}
