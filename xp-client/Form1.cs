using Newtonsoft.Json;
using Parser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace xp_client
{
    public partial class Form1 : Form
    {
        private string _host = "127.0.0.1";
        private int _port = 6379;
        private string _password = "";
        private readonly List<string> _inputList = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region 读取配置文件

            try
            {
                serialPort1.PortName = ConfigurationManager.AppSettings["SerialPortName"];
                serialPort1.BaudRate = int.Parse(ConfigurationManager.AppSettings["SerialPortBaudRate"]);
                _host = ConfigurationManager.AppSettings["RedisHost"];
                _port = int.Parse(ConfigurationManager.AppSettings["RedisPort"]);
                _password = ConfigurationManager.AppSettings["RedisPassword"];
            }
            catch (Exception ex)
            {
                Log("配置文件错误："+ex.Message);
            }

            #endregion

            /*            #region redis 测试
                          // https://github.com/ctstone/csredis

                          string redisHost = "49.235.205.146";
                          using (var redis = new RedisClient(redisHost))
                          {
                              textBox1.Text = "Connected to Redis server at " + redisHost + Environment.NewLine;
                              string ping = redis.Ping();
                              string echo = redis.Echo("hello world");
                              DateTime time = redis.Time();
                              textBox1.Text += ping + Environment.NewLine + echo + Environment.NewLine + time.ToString() + Environment.NewLine;
                          }

                          #endregion
              */
            /*            #region HTTP Post 测试

                        textBox1.Text += Environment.NewLine;

                        var url = "http://rap2api.taobao.org/app/mock/245278/Select_WORKORDERH";
                        var json = "{\"Barcode\": \"value1\", \"key2\": \"value2\"}";
                        textBox1.Text += "网络请求测试" + Environment.NewLine;
                        textBox1.Text += url + Environment.NewLine;
                        textBox1.Text += json + Environment.NewLine;
                        var response = Helper.HttpPost(url, json);
                        textBox1.Text += "响应" + Environment.NewLine;
                        textBox1.Text += response + Environment.NewLine;

                        #endregion
            */
            if (serialPort1.IsOpen)
            {
                Log($"Close {serialPort1.PortName}");
                serialPort1.Close();
                button1.Text = "START";
                button1.BackColor = DefaultBackColor;
            }
            else
            {
                textBox1.Text = "";
                Log($"Open {serialPort1.PortName}");
                serialPort1.Open();
                button1.Text = "STOP";
                button1.BackColor = Color.GreenYellow;
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while (serialPort1.BytesToRead > 0)
            {
                var line = serialPort1.ReadLine();
                if (line.StartsWith("fiber")) _inputList.Clear();
                _inputList.Add(line.Trim());
            }

            Log(_inputList);
            DoWork(_inputList);
        }

        void DoWork(List<string> inputList)
        {
            if (inputList.Count < 2) return;
            try
            {
                using (var redis = new CSRedis.RedisClient(_host, _port))
                {
                    if (!string.IsNullOrEmpty(_password))
                        redis.Connected += (s, e) => redis.Auth(_password); // set AUTH, CLIENT NAME, etc
                    // connection will retry 3 times with 200ms in between before throwing an Exception
                    redis.ReconnectAttempts = 3;
                    redis.ReconnectWait = 200;

                    var items2200 = new Items2200();
                    string response;

                    // 获取盘号 FiberId
                    var array = inputList[0].Split(':');
                    if (array.Length < 2 || !array[0].StartsWith("fiber") || !array[1].Contains("-")) return;
                    var fiberId = array[1].Split('-')[0].Trim();
                    items2200.FiberId = fiberId;

                    #region 查询 Redis 中已有的值

                    var commandArgs = new List<string>
                    {
                        "PK2200",
                        $".{fiberId}"
                    };
                    var rawResponse = (byte[])redis.Call("json.get", commandArgs.ToArray());
                    if (rawResponse != null)
                    {
                        response = Encoding.ASCII.GetString(rawResponse);
                        items2200 = JsonConvert.DeserializeObject<Items2200>(response);
                    }

                    #endregion

                    if (inputList[0].StartsWith("fiber:   ")) // 2200衰减
                    {
                        Parser.Parser.Parse2200Attenuation(inputList, ref items2200);
                    }
                    else if (inputList[0].StartsWith("fiber:")) // 2200截止波长
                    {
                        Parser.Parser.Parse2200Cutoff(inputList, ref items2200);
                    }
                    else if (inputList[0].StartsWith("fiberMFD:")) // 2200模场直径
                    {
                        Parser.Parser.Parse2200MFD(inputList, ref items2200);
                    }

                    commandArgs = new List<string>
                    {
                        "PK2200",
                        ".",
                        //"{\"PMF03147A04C0A-T\": {\"Cutoff_T\": 0.3,\"Cutoff_B\": 0.0,\"MFD1310_T\": 0.0,\"MFD1310_B\": 0.0,\"MFD1550_T\": 0.0,\"MFD1550_B\": 0.0,\"MFD850_T\": 0.0,\"MFD850_B\": 0.0,\"MFD980_T\": 0.0,\"MFD980_B\": 0.0,\"Attenuation850\": 8.0,\"Attenuation980\": 0.0,\"Attenuation1060\": 0.0,\"Attenuation1240\": 0.0,\"Attenuation1310\": 0.0,\"Attenuation1380\": 0.0,\"Attenuation1550\": 0.0}}"
                        $"{{\"{fiberId}\": {JsonConvert.SerializeObject(items2200)}}}"
                    };

                    response = redis.Call("json.set", commandArgs.ToArray()).ToString();
                    Log($"Redis 命令响应：{response}");
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }

        private void Log(string message)
        {
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
                textBox1.Text += message + Environment.NewLine + Environment.NewLine;

                //自动滚动到最下行
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            });
        }

        private void Log(List<string> lines)
        {
            textBox1.Invoke((MethodInvoker)delegate
            {
                textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
                foreach (var line in lines)
                    textBox1.Text += line + Environment.NewLine;
                textBox1.Text += Environment.NewLine;

                //自动滚动到最下行
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            });
        }
    }
}