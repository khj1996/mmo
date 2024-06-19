namespace PacketGenerator
{
    class Program
    {
        //클리언트 패킷에 해당하는 핸들러 스트링
        static string clientRegister;
        //서버 패킷에 해당하는 핸들러 스트링
        static string serverRegister;

        static void Main(string[] args)
        {
            //프로토 버프 생성을 위한 파일
            string file = "../../../Common/protoc-3.12.3-win64/bin/Protocol.proto";
            
            if (args.Length >= 1)
                file = args[0];

            bool startParsing = false;
            foreach (string line in File.ReadAllLines(file))
            {
                //이넘타입 시작하는 부분부터 데이터 생성 시작
                if (!startParsing && line.Contains("enum MsgId"))
                {
                    startParsing = true;
                    continue;
                }

                if (!startParsing)
                    continue;

                if (line.Contains("}"))
                    break;

                string[] names = line.Trim().Split(" =");
                if (names.Length == 0)
                    continue;

                string name = names[0];
                if (name.StartsWith("S_"))
                {
                    string[] words = name.Split("_");

                    string msgName = "";
                    foreach (string word in words)
                        msgName += FirstCharToUpper(word);

                    string packetName = $"S_{msgName.Substring(1)}";
                    clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
                }
                else if (name.StartsWith("C_"))
                {
                    string[] words = name.Split("_");

                    string msgName = "";
                    foreach (string word in words)
                        msgName += FirstCharToUpper(word);

                    string packetName = $"C_{msgName.Substring(1)}";
                    serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
                }
            }

            string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
            File.WriteAllText("ClientPacketManager.cs", clientManagerText);
            
            string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
            File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
        }
    }
}