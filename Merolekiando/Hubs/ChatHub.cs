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
        public async Task SendMessage(string message, string userId)
        {
            var tkn = Context.User.Claims;
            var sType = "";
            var connectIdUser = "";
            int usrid = 0;
            int stakrid = 0;
            var UserNameRMsg = "";
            string json = "";
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
                dto.Time = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                dto.Message = message;
                dto.ConnTo = ToId;
                dto.ConnFrom = Context.ConnectionId;
                dto.ConnId = userId;
                dto.Type = "Text";

                _Context.Chats.Add(dto);
                _Context.SaveChanges();


                if (dto.Type == "Link")
                {
                    dto.Message = dto.Message.Replace("/wwwroot", "");
                }
                json = "{" + @"""text""" + ":" + @"""" + dto.Message + @"""" + "," + @"""connId""" + ":" + @"""" + Context.ConnectionId + @"""" + "," + @"""from""" + ":" + usrid + "," + @"""when""" + ":" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "," + @"""to""" + ":" + userId + "," + @"""type""" + ":" + @"""" + dto.Type.ToString() + @"""" + "," + @"""Link""" + ":" + @""""+ "" + @"""" + "}";
            }
            

            
            await Clients.Clients(dto.ConnTo).SendAsync(method: "ReceiveMessage", message, Context.ConnectionId, UserNameRMsg, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(), json);
            await Clients.Clients(Context.ConnectionId).SendAsync("OwnMessage", message, Context.ConnectionId, connectIdUser, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(), json);
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

                if (dt != null)
                {
                    dt.ConnId = connectionId;
                    dt.From = Convert.ToInt32(connectIdUser);
                    //dt.IsOnline = true;
                    _Context.Messages.Update(dt);
                    _Context.SaveChanges();
                }
                else
                {
                    Message lstd = new();
                    lstd.ConnId = connectionId;
                    lstd.From = Convert.ToInt32(connectIdUser);
                    //lstd.IsOnline = true;
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
