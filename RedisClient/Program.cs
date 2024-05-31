using System;
using System.Collections.Generic;

namespace RedisClient
{
    internal class Program
    {
        static void Main(string[] rawArgs)
        {
            if (rawArgs.Length == 0)
            {
                Console.WriteLine();
                Console.WriteLine("语法：RedisClient.exe <command> [args...] [-options...]");
                Console.WriteLine();
                Console.WriteLine("options: ");
                Console.WriteLine("  -host: Redis 服务器地址");
                Console.WriteLine("  -port: Redis 服务器端口");
                Console.WriteLine("  -password: Redis 服务器密码");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();
                return;
            }

            string host = "49.235.205.146";
            int port = 6379;
            //string password = "";
            string password = "u3T1BVytcXQqpGoh";
            //string type = "json";
            string command;
            var commandArgs = new List<string>();

            #region 解析输入参数

            var args = ParseArgs(rawArgs);
            command = args[0];

            int j = 0;
            while (j < args.Count - 1)
            {
                j++;
                if (!args[j].StartsWith("-"))
                    commandArgs.Add(args[j]);
                else break;
            }

            for (int i = j; i < args.Count; i++)
            {
                if (args[i] == "-host" && i < args.Count - 1)
                {
                    host = args[i + 1];
                }
                else if (args[i] == "-port" && i < args.Count - 1)
                {
                    int.TryParse(args[i + 1], out port);
                }
                else if (args[i] == "-password" && i < args.Count - 1)
                {
                    password = args[i + 1];
                }
            }

            #endregion

            try
            {
                using (var redis = new CSRedis.RedisClient(host, port))
                {
                    if (!string.IsNullOrEmpty(password))
                        redis.Connected += (s, e) => redis.Auth(password); // set AUTH, CLIENT NAME, etc
                    // connection will retry 3 times with 200ms in between before throwing an Exception
                    redis.ReconnectAttempts = 3;
                    redis.ReconnectWait = 200;

                    //string ping = redis.Ping();
                    //Log(ping);
                    //string echo = redis.Echo("hello world");
                    //Log(echo);
                    //DateTime time = redis.Time();
                    //Log(time.ToString());

                    //command = "json.set";
                    //commandArgs = new List<string>
                    //{
                    //    "PK2200",
                    //    ".",
                    //    "{\"PMF03147A04C0A-T\": {\"Cutoff_T\": 0.3,\"Cutoff_B\": 0.0,\"MFD1310_T\": 0.0,\"MFD1310_B\": 0.0,\"MFD1550_T\": 0.0,\"MFD1550_B\": 0.0,\"MFD850_T\": 0.0,\"MFD850_B\": 0.0,\"MFD980_T\": 0.0,\"MFD980_B\": 0.0,\"Attenuation850\": 8.0,\"Attenuation980\": 0.0,\"Attenuation1060\": 0.0,\"Attenuation1240\": 0.0,\"Attenuation1310\": 0.0,\"Attenuation1380\": 0.0,\"Attenuation1550\": 0.0}}"
                    //};

                    string response = redis.Call(command, commandArgs.ToArray()).ToString();
                    Log(response);
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }

        static void Log(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(message);
            Console.WriteLine();
        }

        /// <summary>
        /// 处理命令行传参中的引号
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static List<string> ParseArgs(string[] args)
        {
            List<string> parsedArgs = new List<string>();
            bool inQuote = false;
            string currentArg = string.Empty;

            foreach (var arg in args)
            {
                if (inQuote)
                {
                    currentArg += " " + arg;

                    if (arg.EndsWith("\"") || arg.EndsWith("'"))
                    {
                        inQuote = false;
                        parsedArgs.Add(currentArg.Trim('"', '\''));
                        currentArg = string.Empty;
                    }
                }
                else
                {
                    if (arg.StartsWith("\"") || arg.StartsWith("'"))
                    {
                        inQuote = true;
                        currentArg = arg;
                    }
                    else
                    {
                        parsedArgs.Add(arg);
                    }
                }
            }

            // 如果在引号内结束解析，则最后一个参数没有加入已解析的列表中
            if (!string.IsNullOrEmpty(currentArg))
            {
                parsedArgs.Add(currentArg.Trim('"', '\''));
            }

            return parsedArgs;
        }
    }
}