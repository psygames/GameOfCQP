using Flexlive.CQP.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;

public class CQX
{

	//
	// Summary:
	//     代理类型。
	public static CQProxyType ProxyType { get; set; }

	//
	// Summary:
	//     获取 匿名代码。
	//
	// Parameters:
	//   ignore:
	//     是否不强制。
	//
	// Returns:
	//     CQ 匿名代码。
	public static string CQCode_Anonymous(bool ignore = false) { return CQ.CQCode_Anonymous(ignore); }
	//
	// Summary:
	//     获取 @指定QQ 的操作代码。
	//
	// Parameters:
	//   qqNumber:
	//     指定的QQ号码。
	//     当该参数为-1时，操作为 @全部成员。
	//
	// Returns:
	//     CQ @操作代码。
	public static string CQCode_At(long qqNumber) { return CQ.CQCode_At(qqNumber); }
	//
	// Summary:
	//     获取 指定的emoji表情代码。
	//
	// Parameters:
	//   id:
	//     emoji表情索引ID。
	//
	// Returns:
	//     CQ emoji表情代码。
	public static string CQCode_Emoji(int id) { return CQ.CQCode_Emoji(id); }
	//
	// Summary:
	//     获取 指定的表情代码。
	//
	// Parameters:
	//   id:
	//     表情索引ID。
	//
	// Returns:
	//     CQ 表情代码。
	public static string CQCode_Face(int id) { return CQ.CQCode_Face(id); }
	//
	// Summary:
	//     获取 发送图片代码。
	//
	// Parameters:
	//   fileName:
	//     图片路径。
	//
	// Returns:
	//     CQ 发送图片代码。
	public static string CQCode_Image(string fileName) { return CQ.CQCode_Image(fileName); }
	//
	// Summary:
	//     获取 发送音乐代码。
	//
	// Parameters:
	//   id:
	//     音乐索引ID。
	//
	// Returns:
	//     CQ 发送音乐代码。
	public static string CQCode_Music(int id) { return CQ.CQCode_Music(id); }
	//
	// Summary:
	//     获取 发送语音代码。
	//
	// Parameters:
	//   fileName:
	//     语音文件路径。
	//
	// Returns:
	//     CQ 发送语音代码。
	public static string CQCode_Record(string fileName) { return CQ.CQCode_Record(fileName); }
	//
	// Summary:
	//     获取 窗口抖动代码。
	//
	// Returns:
	//     CQ 窗口抖动代码。
	public static string CQCode_Shake() { return CQ.CQCode_Shake(); }
	//
	// Summary:
	//     获取 链接分享代码。
	//
	// Parameters:
	//   url:
	//     链接地址。
	//
	//   title:
	//     标题。
	//
	//   content:
	//     内容。
	//
	//   imageUrl:
	//     图片地址。
	//
	// Returns:
	//     CQ 链接分享代码。
	public static string CQCode_ShareLink(string url, string title, string content, string imageUrl)
	{
		return CQ.CQCode_ShareLink(url, title, content, imageUrl);
	}
	//
	// Summary:
	//     取Cookies。
	//
	// Returns:
	//     登录的Cookies。
	public static string GetCookies() { return CQ.GetCookies(); }
	//
	// Summary:
	//     获取酷Q插件App的目；如果是UDP方式，返回的则为酷Q主目录。
	public static string GetCQAppFolder() { return CQ.GetCQAppFolder(); }
	//
	// Summary:
	//     获取C#插件的应用目录。
	//
	// Returns:
	//     应用目录。
	public static string GetCSPluginsFolder() { return CQ.GetCSPluginsFolder(); }
	//
	// Summary:
	//     取取CsrfToken。
	//
	// Returns:
	//     登录的CsrfToken。
	public static int GetCsrfToken() { return CQ.GetCsrfToken(); }
	//
	// Summary:
	//     取群成员信息。
	//     多线程同步等待，采用阻塞线程的方式等待客户端返回群成员信息，响应时间较慢，建议使用缓存。
	//     缓存时长1天，超过1天的成员，在下次访问时会通过酷Q重新获取最新信息。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   qqNumber:
	//     被操作的QQ号码。
	//
	//   cache:
	//     是否使用缓存（使用缓存后，当后第一次访问会通过客户端读取，之后每次都通过缓存获得）。
	public static CQGroupMemberInfo GetGroupMemberInfo(long groupNumber, long qqNumber, bool cache = true)
	{
		return CQ.GetGroupMemberInfo(groupNumber, qqNumber, cache);
	}
	//
	// Summary:
	//     取登录昵称。
	//
	// Returns:
	//     登录的QQ号码昵称。
	public static string GetLoginName() { return CQ.GetLoginName(); }
	//
	// Summary:
	//     取登录QQ。
	//
	// Returns:
	//     登录的QQ号码
	public static long GetLoginQQ() { return CQ.GetLoginQQ(); }
	//
	// Summary:
	//     接收语音（返回保存在酷Q的data\record 目录下）。
	//
	// Parameters:
	//   fileName:
	//     保存的文件名。
	//
	//   postfixName:
	//     保存的文件格式。
	[Obsolete("该方法在本地C++模式下不被支持，请谨慎使用。")]
	public static void ReceiveVoice(string fileName, string postfixName)
	{
		CQ.ReceiveVoice(fileName, postfixName);
	}
	//
	// Summary:
	//     发送讨论组消息。
	//
	// Parameters:
	//   discussNumber:
	//     讨论组号码。
	//
	//   message:
	//     论组消息内容。
	public static void SendDiscussMessage(long discussNumber, string message)
	{
		CQ.SendDiscussMessage(discussNumber, FormatMsg(message));
	}
	//
	// Summary:
	//     发送群消息。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   message:
	//     群消息内容。
	public static void SendGroupMessage(long groupNumber, string message)
	{
		CQ.SendGroupMessage(groupNumber, FormatMsg(message));
	}
	//
	// Summary:
	//     发送赞(本地C++模式调用一次只能发送一个赞）。
	//
	// Parameters:
	//   qqNumber:
	//     被操作的QQ。
	//
	//   count:
	//     发赞次数。
	public static void SendPraise(long qqNumber, int count = 1) { CQ.SendPraise(qqNumber, count); }
	//
	// Summary:
	//     发送私聊消息。
	//
	// Parameters:
	//   qqNumber:
	//     QQ号码。
	//
	//   message:
	//     私聊消息内容。
	public static void SendPrivateMessage(long qqNumber, string message)
	{
		CQ.SendPrivateMessage(qqNumber, FormatMsg(message));
	}
	//
	// Summary:
	//     置讨论组退出。
	//
	// Parameters:
	//   discussNumber:
	//     讨论组号码。
	public static void SetDiscussExit(long discussNumber)
	{
		CQ.SetDiscussExit(discussNumber);
	}
	//
	// Summary:
	//     置好友添加请求。
	//
	// Parameters:
	//   react:
	//     请求反馈标识。
	//
	//   reactType:
	//     反馈类型。
	//
	//   description:
	//     备注。
	public static void SetFriendAddRequest(string react, CQReactType reactType, string description = "")
	{
		CQ.SetFriendAddRequest(react, reactType, description);
	}
	//
	// Summary:
	//     置群添加请求。
	//
	// Parameters:
	//   react:
	//     请求反馈标识。
	//
	//   requestType:
	//     请求类型。
	//
	//   reactType:
	//     反馈类型。
	//
	//   reason:
	//     加群原因。
	public static void SetGroupAddRequest(string react, CQRequestType requestType, CQReactType reactType, string reason = "")
	{
		CQ.SetGroupAddRequest(react, requestType, reactType, reason);
	}
	//
	// Summary:
	//     置群管理员。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   qqNumber:
	//     被操作的QQ号码。
	//
	//   admin:
	//     是否设置为管理员。
	public static void SetGroupAdministrator(long groupNumber, long qqNumber, bool admin)
	{
		CQ.SetGroupAdministrator(groupNumber, qqNumber, admin);
	}
	//
	// Summary:
	//     置全群禁言。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   gag:
	//     设置或关闭全群禁言。
	public static void SetGroupAllGag(long groupNumber, bool gag)
	{
		CQ.SetGroupAllGag(groupNumber, gag);
	}
	//
	// Summary:
	//     置群匿名设置。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   allow:
	//     开启或关闭匿名功能。
	public static void SetGroupAllowAnonymous(long groupNumber, bool allow)
	{
		CQ.SetGroupAllowAnonymous(groupNumber, allow);
	}
	//
	// Summary:
	//     置匿名群员禁言。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   anomymous:
	//     被操作的匿名成员名称。
	//
	//   time:
	//     禁言时长（以秒为单位)
	public static void SetGroupAnonymousMemberGag(long groupNumber, string anomymous, long time)
	{
		CQ.SetGroupAnonymousMemberGag(groupNumber, anomymous, time);
	}
	//
	// Summary:
	//     置群退出。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   dissolution:
	//     是否解散。
	public static void SetGroupExit(long groupNumber, bool dissolution = false)
	{
		CQ.SetGroupExit(groupNumber, dissolution);
	}
	//
	// Summary:
	//     置群成员专属头衔
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   qqNumber:
	//     被操作的QQ号码。
	//
	//   newName:
	//     头衔名称。
	//
	//   time:
	//     过期时间（以秒为单位）。
	public static void SetGroupHonorName(long groupNumber, long qqNumber, string newName, int time)
	{
		CQ.SetGroupHonorName(groupNumber, qqNumber, newName, time);
	}
	//
	// Summary:
	//     置群员禁言。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   qqNumber:
	//     被操作的QQ号码。
	//
	//   time:
	//     禁言时长（以秒为单位)
	public static void SetGroupMemberGag(long groupNumber, long qqNumber, long time)
	{
		CQ.SetGroupMemberGag(groupNumber, qqNumber, time);
	}
	//
	// Summary:
	//     置群员移除。
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   qqNumber:
	//     被操作的QQ号码。
	//
	//   refuse:
	//     是否拒绝再次加群。
	public static void SetGroupMemberRemove(long groupNumber, long qqNumber, bool refuse = false)
	{
		CQ.SetGroupMemberRemove(groupNumber, qqNumber, refuse);
	}
	//
	// Summary:
	//     置群成员名片
	//
	// Parameters:
	//   groupNumber:
	//     群号码。
	//
	//   qqNumber:
	//     被操作的QQ号码。
	//
	//   newName:
	//     新的群名称。
	public static void SetGroupNickName(long groupNumber, long qqNumber, string newName)
	{
		CQ.SetGroupNickName(groupNumber, qqNumber, newName);
	}

	public static CookieCollection GetCookies(string domain)
	{
		return CQE.GetCookies(domain);
	}

	public static List<CQGroupInfo> GetGroupList() { return CQE.GetGroupList(); }

	public static List<CQGroupMemberInfo> GetGroupMemberList(long groupNumber)
	{
		return CQE.GetGroupMemberList(groupNumber);
	}

	public static Image GetQQFace(long qqNumber)
	{
		return CQE.GetQQFace(qqNumber);
	}

	public static string GetQQName(long qqNumber)
	{
		return CQE.GetQQName(qqNumber); ;
	}

	private static string FormatMsg(string msg)
	{
		string newMsg = "";
		int index = msg.IndexOf("[CQ:");
		while (index >= 0)
		{
			newMsg += msg.Substring(0, index);
			msg = msg.Substring(index);
			index = msg.IndexOf("]");
			if (index >= 0)
			{
				var cqCode = msg.Substring(0, index + 1);
				msg = msg.Substring(index + 1);
				newMsg += FormatMsgCQCode(cqCode);
			}
			index = msg.IndexOf("[CQ:");
		}
		newMsg += msg;
		return newMsg;
	}

	private static string FormatMsgCQCode(string cqCode)
	{
		cqCode = cqCode.Trim('[', ']');
		string[] cos = cqCode.Split(',');
		string type = cos[0].Split(':')[1];
		string[] names = new string[cos.Length - 1];
		string[] values = new string[cos.Length - 1];
		for (int i = 1; i < cos.Length; i++)
		{
			var nv = cos[i].Split('=');
			names[i - 1] = nv[0];
			values[i - 1] = nv[1];
		}

		var newCqCode = "[" + cos[0];
		for (int i = 0; i < names.Length; i++)
		{
			MsgFormatTypeHandle(type, names[i], values[i]); // handle
			var val = MsgFormatTypeValue(type, names[i], values[i]); // format
			newCqCode += "," + names[i] + "=" + val;
		}
		newCqCode += "]";
		return newCqCode;
	}

	private static string MsgFormatTypeValue(string type, string name, string value)
	{
		if (type == "image" && name == "file")
		{
			return Path.GetFileName(value);
		}
		return value;
	}

	private static void MsgFormatTypeHandle(string type, string name, string value)
	{
		if (type == "image" && name == "file")
		{
			if (File.Exists(value))
			{
				File.Copy(value
					, Path.Combine(Config.CQ_ROOT, "data/image/" + Path.GetFileName(value))
					, true);
			}
		}
	}
}
