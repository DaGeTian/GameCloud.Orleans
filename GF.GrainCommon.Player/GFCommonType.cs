// Copyright (c) Cragon. All rights reserved.

namespace GF.GrainCommon.Player
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf;
    using GF.Unity.Common;

    public enum GFMethodType
    {
        None = 0,

        // GF
        C2SGFRequest = 10,// c->s, GF请求
        S2CGFResponse,// s->c, GF响应
        S2CGFNotify,// s->c, GF通知

        End
    }

    public enum GFResult : ushort
    {
        Success = 0,// 通用，成功
        Failed,// 失败
        Exist,// 已存在
        Timeout,// 超时
        DbError,// 通用，数据库内部错误
        LogoutNewLogin,// 重复登录，踢出前一帐号
        EnterWorldAccountVerifyFailed,// 角色进入游戏，帐号验证失败
        EnterWorldTokenError,// 角色进入游戏，Token错误
        EnterWorldTokenExpire,// 角色进入游戏，Token过期
        EnterWorldNotExistPlayer,// 角色进入游戏，角色不存在
        EnterWorldAlready,// 已经进入世界
    }

    public enum GFRequestId : byte
    {
        None = 0,// 无效
        EnterWorld = 10,// c->s, 请求进入游戏世界
    }

    public enum GFResponseId : byte
    {
        None = 0,// 无效
        EnterWorld = 10,// s->c, 响应进入游戏世界
    }

    public enum GFNotifyId : byte
    {
        None = 0,// 无效
        Logout = 10,// s->c，登出
    }

    [Serializable]
    [ProtoContract]
    public struct GFRequest
    {
        [ProtoMember(1)]
        public GFRequestId id;
        [ProtoMember(2)]
        public byte[] data;
    }

    [Serializable]
    [ProtoContract]
    public struct GFResponse
    {
        [ProtoMember(1)]
        public GFResponseId id;
        [ProtoMember(2)]
        public byte[] data;
    }

    [Serializable]
    [ProtoContract]
    public struct GFNotify
    {
        [ProtoMember(1)]
        public GFNotifyId id;
        [ProtoMember(2)]
        public byte[] data;
    }

    [Serializable]
    [ProtoContract]
    public class GFEnterWorldRequest
    {
        [ProtoMember(1)]
        public string acc_id;
        [ProtoMember(2)]
        public string acc_name;
        [ProtoMember(3)]
        public string token;
        [ProtoMember(4)]
        public string nick_name;
        [ProtoMember(5)]
        public string et_player_guid;
    }

    [Serializable]
    [ProtoContract]
    public class GFEnterWorldResponse
    {
        [ProtoMember(1)]
        public GFResult result;
        [ProtoMember(2)]
        public string acc_id;
        [ProtoMember(3)]
        public string acc_name;
        [ProtoMember(4)]
        public string token;
        [ProtoMember(5)]
        public EntityData et_player_data;
    }
}
