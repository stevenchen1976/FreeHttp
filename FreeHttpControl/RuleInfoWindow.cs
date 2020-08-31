﻿using FreeHttp.FiddlerHelper;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FreeHttp.FreeHttpControl
{
    public partial class RuleInfoWindow : CBalloon.CBalloonBase
    {

        public RuleInfoWindow(ListViewItem yourListViewItem)
        {
            InitializeComponent();
            //this.Width= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width/2;
            myListViewItem = yourListViewItem;
            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
        }

        

        //RichTextBox rtb_errorList = new RichTextBox();
        //rtb_errorList.AppendText("test");
        //rtb_errorList.Dock = DockStyle.Fill;
        //this.Controls.Add(rtb_errorList);

        ListViewItem myListViewItem;
        Timer timer;
        Rectangle lastListViewItemRectangle;
        Point nowLocation = Point.Empty;


        private void analyzeCaseData()
        {
            //System.Drawing.Image myBit = Image.FromFile(@"D:/123.png", false);
            //Bitmap myBit = Resources.MyResource.show;
            //Graphics GraphicsMyg = rtb_Content.CreateGraphics();
            //GraphicsMyg.DrawImage(myBit,0, 0, myBit.Width/10, myBit.Height/10);
            //GraphicsMyg.ResetTransform();
        }

        private void LoadRuleInfo(IFiddlerHttpTamper ruleInfo )
        {
            Action<FiddlerRequestChange> GetFiddlerRequestChangeAddition = (nowFiddlerRequestChange) =>
            {
                if ((nowFiddlerRequestChange.ParameterPickList != null && nowFiddlerRequestChange.ParameterPickList.Count > 0) || nowFiddlerRequestChange.IsHasParameter)
                {
                    rtb_ruleInfo.AddRtbStr("Addition ", Color.Red, true, new Font(FontFamily.GenericMonospace, 14));
                    if (nowFiddlerRequestChange.IsHasParameter)
                    {
                        rtb_ruleInfo.AddRtbStr("Has Parameter: ", Color.Blue, false);
                        rtb_ruleInfo.AppendText("true");
                        rtb_ruleInfo.AppendText("\r\n");
                    }
                    if (nowFiddlerRequestChange.ParameterPickList != null && nowFiddlerRequestChange.ParameterPickList.Count > 0)
                    {
                        foreach (var tempRequest in nowFiddlerRequestChange.ParameterPickList)
                        {
                            rtb_ruleInfo.AddRtbStr("Request Parameter Pick: ", Color.Blue, false);
                            rtb_ruleInfo.AppendText(tempRequest.ToString());
                            rtb_ruleInfo.AppendText("\r\n");
                        }
                    }
                }
            };
            Action<FiddlerResponseChange> GetFiddlerResponseChangeAddition = (nowFiddlerResponseChange) =>
            {
                if ((nowFiddlerResponseChange.ParameterPickList != null && nowFiddlerResponseChange.ParameterPickList.Count > 0) || nowFiddlerResponseChange.IsHasParameter || nowFiddlerResponseChange.LesponseLatency > 0)
                {
                    rtb_ruleInfo.AddRtbStr("Addition ", Color.Red, true, new Font(FontFamily.GenericMonospace, 14));
                    if (nowFiddlerResponseChange.IsHasParameter)
                    {
                        rtb_ruleInfo.AddRtbStr("Has Parameter: ", Color.Blue, false);
                        rtb_ruleInfo.AppendText("true");
                        rtb_ruleInfo.AppendText("\r\n");
                    }
                    if (nowFiddlerResponseChange.LesponseLatency > 0)
                    {
                        rtb_ruleInfo.AddRtbStr("ResponseLatency: ", Color.Blue, false);
                        rtb_ruleInfo.AppendText(nowFiddlerResponseChange.LesponseLatency + "ms");
                        rtb_ruleInfo.AppendText("\r\n");
                    }
                    if (nowFiddlerResponseChange.ParameterPickList != null && nowFiddlerResponseChange.ParameterPickList.Count > 0)
                    {
                        foreach (var tempResponse in nowFiddlerResponseChange.ParameterPickList)
                        {
                            rtb_ruleInfo.AddRtbStr("Response Parameter Pick: ", Color.Blue, false);
                            rtb_ruleInfo.AppendText(tempResponse.ToString());
                            rtb_ruleInfo.AppendText("\r\n");
                        }
                    }
                }
            };

            pb_ruleIcon.Image = myListViewItem.ImageList.Images[myListViewItem.ImageIndex];
            rtb_ruleInfo.AddRtbStr("Filter ", Color.Red, true, new Font(FontFamily.GenericMonospace, 14));
            if (ruleInfo.HttpFilter.UriMatch!=null)
            {
                rtb_ruleInfo.AddRtbStr("Uri: ", Color.Blue, false);
                rtb_ruleInfo.AppendText(ruleInfo.HttpFilter.UriMatch.ToString());
                rtb_ruleInfo.AppendText("\r\n");
            }
            if (ruleInfo.HttpFilter.HeadMatch != null && ruleInfo.HttpFilter.HeadMatch.HeadsFilter.Count>0)
            {
                foreach (var tempHeaderFilter in ruleInfo.HttpFilter.HeadMatch.HeadsFilter)
                {
                    rtb_ruleInfo.AddRtbStr("Header: ", Color.Blue, false);
                    rtb_ruleInfo.AppendText(string.Format("{0} [contain] {1}", tempHeaderFilter.Key, tempHeaderFilter.Value));
                    rtb_ruleInfo.AppendText("\r\n");
                }
            }
            if (ruleInfo.HttpFilter.BodyMatch != null)
            {
                rtb_ruleInfo.AddRtbStr("Entity: ", Color.Blue, false);
                rtb_ruleInfo.AppendText(ruleInfo.HttpFilter.BodyMatch.ToString());
                rtb_ruleInfo.AppendText("\r\n");
            }
            rtb_ruleInfo.AddRtbStr("Action ", Color.Red, true, new Font(FontFamily.GenericMonospace, 14));

            switch (ruleInfo.TamperProtocol)
            {
                case TamperProtocalType.Http:
                    if(ruleInfo is FiddlerRequestChange)
                    {
                        lb_ruleId.Text = string.Format("Http Request Tamper Rule {0}", myListViewItem.SubItems[0].Text);

                        FiddlerRequestChange nowFiddlerRequestChange = ruleInfo as FiddlerRequestChange;
                        if (nowFiddlerRequestChange.IsRawReplace)
                        {
                            rtb_ruleInfo.AddRtbStr("Request Replace", Color.Blue, true);
                            rtb_ruleInfo.AppendText(nowFiddlerRequestChange.HttpRawRequest.OriginSting);
                            rtb_ruleInfo.AppendText("\r\n");
                        }
                        else
                        {
                            if (nowFiddlerRequestChange.UriModific != null && nowFiddlerRequestChange.UriModific.ModificMode != HttpHelper.ContentModificMode.NoChange)
                            {
                                rtb_ruleInfo.AddRtbStr("Request Uri Modific: ", Color.Blue, false);
                                rtb_ruleInfo.AppendText(nowFiddlerRequestChange.UriModific.ToString());
                                rtb_ruleInfo.AppendText("\r\n");
                            }
                            if (nowFiddlerRequestChange.HeadDelList != null && nowFiddlerRequestChange.HeadDelList.Count > 0)
                            {
                                foreach (var tempHeaderDel in nowFiddlerRequestChange.HeadDelList)
                                {
                                    rtb_ruleInfo.AddRtbStr("Request Head Delete: ", Color.Blue, false);
                                    rtb_ruleInfo.AppendText(tempHeaderDel);
                                }
                            }
                            if (nowFiddlerRequestChange.HeadAddList != null && nowFiddlerRequestChange.HeadAddList.Count > 0)
                            {
                                foreach (var tempHeaderAdd in nowFiddlerRequestChange.HeadAddList)
                                {
                                    rtb_ruleInfo.AddRtbStr("Request Head Add: ", Color.Blue, false);
                                    rtb_ruleInfo.AppendText(tempHeaderAdd);
                                    rtb_ruleInfo.AppendText("\r\n");
                                }
                            }
                            if (nowFiddlerRequestChange.BodyModific != null && nowFiddlerRequestChange.BodyModific.ModificMode != HttpHelper.ContentModificMode.NoChange)
                            {
                                rtb_ruleInfo.AddRtbStr("Request Entity Modific: ", Color.Blue, false);
                                rtb_ruleInfo.AppendText(nowFiddlerRequestChange.BodyModific.ToString());
                                rtb_ruleInfo.AppendText("\r\n");
                            }
                        }
                        GetFiddlerRequestChangeAddition(nowFiddlerRequestChange);
                    }
                    else if(ruleInfo is FiddlerResponseChange)
                    {
                        lb_ruleId.Text = string.Format("Http Response Tamper Rule {0}", myListViewItem.SubItems[0].Text);

                        FiddlerResponseChange nowFiddlerResponseChange = ruleInfo as FiddlerResponseChange;
                        if (nowFiddlerResponseChange.IsRawReplace)
                        {
                            rtb_ruleInfo.AddRtbStr("Request Replace", Color.Blue, true);
                            rtb_ruleInfo.AppendText(nowFiddlerResponseChange.HttpRawResponse.OriginSting);
                            rtb_ruleInfo.AppendText("\r\n");
                        }
                        else
                        {
                            if (nowFiddlerResponseChange.HeadDelList != null && nowFiddlerResponseChange.HeadDelList.Count > 0)
                            {
                                foreach (var tempHeaderDel in nowFiddlerResponseChange.HeadDelList)
                                {
                                    rtb_ruleInfo.AddRtbStr("Response Head Delete: ", Color.Blue, false);
                                    rtb_ruleInfo.AppendText(tempHeaderDel);
                                }
                            }
                            if (nowFiddlerResponseChange.HeadAddList != null && nowFiddlerResponseChange.HeadAddList.Count > 0)
                            {
                                foreach (var tempHeaderAdd in nowFiddlerResponseChange.HeadAddList)
                                {
                                    rtb_ruleInfo.AddRtbStr("Response Head Add: ", Color.Blue, false);
                                    rtb_ruleInfo.AppendText(tempHeaderAdd);
                                    rtb_ruleInfo.AppendText("\r\n");
                                }
                            }
                            if (nowFiddlerResponseChange.BodyModific != null && nowFiddlerResponseChange.BodyModific.ModificMode != HttpHelper.ContentModificMode.NoChange)
                            {
                                rtb_ruleInfo.AddRtbStr("Response Entity Modific: ", Color.Blue, false);
                                rtb_ruleInfo.AppendText(nowFiddlerResponseChange.BodyModific.ToString());
                                rtb_ruleInfo.AppendText("\r\n");
                            }
                        }

                        GetFiddlerResponseChangeAddition(nowFiddlerResponseChange);
                    }
                    break;
                case TamperProtocalType.WebSocket:
                    if (ruleInfo is FiddlerRequestChange)
                    {
                        lb_ruleId.Text = string.Format("Websocket Send Tamper Rule {0}", myListViewItem.SubItems[0].Text);

                        FiddlerRequestChange nowFiddlerWebSocketRequestChange = ruleInfo as FiddlerRequestChange;
                        if (nowFiddlerWebSocketRequestChange.BodyModific != null && nowFiddlerWebSocketRequestChange.BodyModific.ModificMode != HttpHelper.ContentModificMode.NoChange)
                        {
                            rtb_ruleInfo.AddRtbStr("Socket Payload Modific: ", Color.Blue, false);
                            rtb_ruleInfo.AppendText(nowFiddlerWebSocketRequestChange.BodyModific.ToString());
                            rtb_ruleInfo.AppendText("\r\n");
                        }
                        GetFiddlerRequestChangeAddition(nowFiddlerWebSocketRequestChange);
                    }
                    else if (ruleInfo is FiddlerResponseChange)
                    {
                        lb_ruleId.Text = string.Format("Websocket Receive Tamper Rule {0}", myListViewItem.SubItems[0].Text);

                        FiddlerResponseChange nowFiddlerWebSocketResponseChange = ruleInfo as FiddlerResponseChange;
                        if (nowFiddlerWebSocketResponseChange.BodyModific != null && nowFiddlerWebSocketResponseChange.BodyModific.ModificMode != HttpHelper.ContentModificMode.NoChange)
                        {
                            rtb_ruleInfo.AddRtbStr("Socket Payload Modific: ", Color.Blue, false);
                            rtb_ruleInfo.AppendText(nowFiddlerWebSocketResponseChange.BodyModific.ToString());
                            rtb_ruleInfo.AppendText("\r\n");
                        }
                        GetFiddlerResponseChangeAddition(nowFiddlerWebSocketResponseChange);
                    }

                    break;
                default:
                    break;
            }
            if(!string.IsNullOrEmpty(ruleInfo.HttpFilter.Name))
            {
                lb_ruleId.Text += string.Format(" ({0})", ruleInfo.HttpFilter.Name);
            }
                  
        }

        private void MyCBalloon_Load(object sender, EventArgs e)
        {
            if (myListViewItem != null && myListViewItem.Tag is IFiddlerHttpTamper)
            {
                IFiddlerHttpTamper nowRule = myListViewItem.Tag as IFiddlerHttpTamper;
                LoadRuleInfo(nowRule );
                
                lastListViewItemRectangle = myListViewItem.Bounds;
                timer.Start();
            }
            else
            {
                MessageBox.Show("can not find rule data ", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if(myListViewItem==null || myListViewItem.ListView==null)
            {
                Close();
                return;
            }
            if(lastListViewItemRectangle != myListViewItem.Bounds)
            {
                lastListViewItemRectangle = myListViewItem.Bounds;
                Form mainForm = this.Owner.Owner;
                Point myPosition = new Point(myListViewItem.Bounds.X, myListViewItem.Bounds.Y);
                myPosition = myListViewItem.ListView.PointToScreen(myPosition);
                myPosition = mainForm.PointToClient(myPosition);
                myPosition.Offset(40, 10);
                this.UpdateBalloonPosition(myPosition);
            }
        }

        private void pictureBox_close_Click(object sender, EventArgs e)
        {
            this.Owner.Activate();
            this.Close();
        }

        public void RefreshRuleInfo()
        {
            if (myListViewItem != null && myListViewItem.Tag is IFiddlerHttpTamper)
            {
                IFiddlerHttpTamper nowRule = myListViewItem.Tag as IFiddlerHttpTamper;
                LoadRuleInfo(nowRule);
            }
        }
        public new void Close()
        {
            if(timer!=null)
            {
                timer.Stop();
                timer.Dispose();
            }
            myListViewItem = null;
            base.Close();


        }

        private void rtb_ruleInfo_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            var richTextBox = (RichTextBox)sender;
            //    richTextBox.Width = e.NewRectangle.Width;
            int lineheight = richTextBox.Font.Height;
            int nHeight = lineheight;
            if (e.NewRectangle.Height < lineheight)
                nHeight = lineheight;
            else
                nHeight = e.NewRectangle.Height + lineheight;
            richTextBox.Height = nHeight;
        }
    }
}