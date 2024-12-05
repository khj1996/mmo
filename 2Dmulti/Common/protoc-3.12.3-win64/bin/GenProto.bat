protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../GameServer/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto


XCOPY /Y Protocol.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../GameServer/DummyClient/Packet"
XCOPY /Y Protocol.cs "../../../GameServer/GameServer/Packet"
XCOPY /Y Protocol.cs "../../../GameServer/LoginServer/Packet"
XCOPY /Y ClientPacketManager.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y ClientPacketManager.cs "../../../GameServer/DummyClient/Packet"
XCOPY /Y ServerPacketManager.cs "../../../GameServer/GameServer/Packet"
XCOPY /Y ServerPacketManager.cs "../../../GameServer/LoginServer/Packet"