using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using LoginServer.Data;
using LoginServer.Game;

namespace LoginServer.DB
{
	//파티셜 클래스
	public partial class DbTransaction : JobSerializer
	{
		public static DbTransaction Instance { get; } = new DbTransaction();
	}
}
