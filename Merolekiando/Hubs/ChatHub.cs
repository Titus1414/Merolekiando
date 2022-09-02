using Merolekiando.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekiando.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MerolikandoDBContext _Context;
        public ChatHub(MerolikandoDBContext dbContext)
        {
            _Context = dbContext;
        }
        public async Task SendMessage(string message, string userId, string Pid)
        {
            var tkn = Context.User.Claims;
            var sType = "";
            var connectIdUser = "";
            int usrid = 0;
            int stakrid = 0;
            var UserNameRMsg = "";
            string json = "";

            var chngeFrom = _Context.Messages.Where(a => a.To == Convert.ToInt32(userId)).ToList();
            foreach (var item in chngeFrom)
            {
                item.ConnId = Context.ConnectionId;
                _Context.Messages.Update(item);
                _Context.SaveChanges();
            }

            


            Chat dto = new();
            if (message.Contains("/Resources/Images/Messages"))
            {
                //message = "/wwwroot" + message;
                var img = _Context.Chats.Where(a => a.Link == "/wwwroot" + message).FirstOrDefault();
                if (img != null)
                {
                    img.Link = img.Link.Replace("/wwwroot", "");
                    connectIdUser = Context.User.Claims.FirstOrDefault().Value;
                    var sd = _Context.Messages.AsQueryable().Where(a => a.ConnId == Context.ConnectionId).FirstOrDefault().From;
                    usrid = Convert.ToInt32(sd);
                    dto.SenderId = usrid;
                    UserNameRMsg = _Context.Users.AsQueryable().Where(a => a.Id == usrid).Select(a => a.Name).FirstOrDefault();
                    var ToId = _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();
                    dto.ConnTo = ToId;

                    json = "{" + @"""text""" + ":" + @"""" + "" + @"""" + "," + @"""connId""" + ":" + @"""" + Context.ConnectionId + @"""" + "," + @"""from""" + ":" + usrid + "," + @"""when""" + ":" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "," + @"""to""" + ":" + userId + "," + @"""type""" + ":" + @"""" + img.Type.ToString() + @"""" + "," + @"""Link""" + ":" + @"""" + img.Link + @"""" + "}";
                }
            }
            else
            {
                connectIdUser = Context.User.Claims.FirstOrDefault().Value;
                var sd = _Context.Messages.AsQueryable().Where(a => a.ConnId == Context.ConnectionId).FirstOrDefault().From;
                usrid = Convert.ToInt32(sd);
                dto.SenderId = usrid;
                UserNameRMsg = _Context.Users.AsQueryable().Where(a => a.Id == usrid).Select(a => a.Name).FirstOrDefault();
                var ToId = _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();

                dto.SenderId = usrid;
                dto.RecieverId = Convert.ToInt32(userId);
                dto.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                dto.Message = message;
                dto.ConnTo = ToId;
                dto.ConnFrom = Context.ConnectionId;
                dto.ConnId = userId;
                dto.Type = "Text";
                if (Pid != "")
                {
                    dto.ProductId = Convert.ToInt32(Pid);
                }
                _Context.Chats.Add(dto);
                _Context.SaveChanges();

                if (dto.Type == "Link")
                {
                    dto.Message = dto.Message.Replace("/wwwroot", "");
                }
                json = "{" + @"""text""" + ":" + @"""" + dto.Message + @"""" + "," + @"""connId""" + ":" + @"""" + Context.ConnectionId + @"""" + "," + @"""from""" + ":" + usrid + "," + @"""when""" + ":" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "," + @"""to""" + ":" + userId + "," + @"""type""" + ":" + @"""" + dto.Type.ToString() + @"""" + "," + @"""Link""" + ":" + @""""+ "" + @"""" + "}";
            }

            var msg = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId || (a.From == dto.RecieverId && a.To == dto.SenderId)).FirstOrDefault();
            if (Pid != "")
            {
                msg = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId && a.ProductId == Convert.ToInt32(Pid)).FirstOrDefault();
            }
            
            if (message.Contains("/Resources/Images/Messages"))
            {
                msg.LastMessage = "Image";
                msg.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var msgN = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == null).FirstOrDefault();
                if (msgN != null)
                {
                    msgN.To = dto.RecieverId;
                    if (Pid != "")
                    {
                        msgN.ProductId = Convert.ToInt32(Pid);
                    }
                    msgN.LastMessage = message;
                    _Context.Messages.Update(msgN);
                    _Context.SaveChanges();
                }
            }
            else
            {
                msg.LastMessage = message;
                msg.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var msgN = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == null).FirstOrDefault();
                if (msgN != null)
                {
                    msgN.To = dto.RecieverId;
                    if (Pid != "")
                    {
                        msgN.ProductId = Convert.ToInt32(Pid);
                    }
                    msgN.LastMessage = message;
                    _Context.Messages.Update(msgN);
                    _Context.SaveChanges();
                }
            }
            _Context.Messages.Update(msg);
            _Context.SaveChanges();


            var Messagess = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId).ToList();
            if (Messagess.Count > 1)
            {
                var Messagessa = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId && a.Image == null).FirstOrDefault();
                _Context.Messages.Remove(Messagessa);
                _Context.SaveChanges();
            }


            await Clients.Clients(dto.ConnTo).SendAsync(method: "ReceiveMessage", message, Context.ConnectionId, UserNameRMsg, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(), json);
            await Clients.Clients(Context.ConnectionId).SendAsync("OwnMessage", message, Context.ConnectionId, connectIdUser, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(), json);
            await Clients.Clients(dto.ConnTo).SendAsync("MessageNotification", message, UserNameRMsg);
        }
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var connectIdUserasdf = Context.User.Claims.FirstOrDefault();
            if (connectIdUserasdf != null)
            {
                var connectIdUser = Context.User.Claims.FirstOrDefault().Value;
                var dt = await _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(connectIdUser)).FirstOrDefaultAsync();

                if (dt != null)
                {
                    var sd = _Context.Chats.AsQueryable().Where(a => a.ConnId == dt.ConnId).ToList();
                    if (sd.Count > 0)
                    {
                        foreach (var item in sd)
                        {
                            item.ConnId = connectionId;
                            _Context.Chats.Update(item);
                            _Context.SaveChanges();
                        }
                    }
                }

                if (dt != null)
                {
                    var sd = await _Context.Chats.AsQueryable().Where(a => a.ConnFrom == dt.ConnId).ToListAsync();
                    if (sd.Count > 0)
                    {
                        foreach (var item in sd)
                        {
                            item.ConnFrom = connectionId;
                            _Context.Chats.Update(item);
                            _Context.SaveChanges();
                        }
                    }
                }

                var dt1 = await _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(connectIdUser)).FirstOrDefaultAsync();
                if (dt1 != null)
                {
                    var sd1 = await _Context.Chats.AsQueryable().Where(a => a.ConnTo == dt1.ConnId).ToListAsync();
                    if (sd1.Count > 0)
                    {
                        foreach (var item in sd1)
                        {
                            item.ConnTo = connectionId;
                            _Context.Chats.Update(item);
                            _Context.SaveChanges();
                        }
                    }
                }
                var lsts = await _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(connectIdUser)).ToListAsync();
                foreach (var item in lsts) 
                {
                    item.ConnId = connectionId;
                    _Context.Messages.Update(item);
                    _Context.SaveChanges();
                }

                if (dt != null)
                {


                    //dt.ConnId = connectionId;
                    //dt.From = Convert.ToInt32(connectIdUser);
                    ////var user = _Context.Users.Where(a => a.Id == Convert.ToInt32(connectIdUser)).FirstOrDefault();
                    ////dt.Name = user.Name;
                    //_Context.Messages.Update(dt);
                    //_Context.SaveChanges();
                }
                else
                {
                    Message lstd = new();
                    lstd.ConnId = connectionId;
                    lstd.From = Convert.ToInt32(connectIdUser);
                    //var user = _Context.Users.Where(a => a.Id == Convert.ToInt32(connectIdUser)).FirstOrDefault();
                    //lstd.Name = user.Name;
                    _Context.Messages.Add(lstd);
                    _Context.SaveChanges();

                }

            }

            _ = Clients.All.SendAsync("OnlineUserList", connectionId);
            //return base.OnConnectedAsync();
        }
        public async Task OnlineUsers()
        {
            var connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("OnlineUserList", connectionId);
        }
    }
}
