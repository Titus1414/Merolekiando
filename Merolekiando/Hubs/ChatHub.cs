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
        public async Task SendMessage(string message, string userId, string Pid, string from)
        {
            var tkn = Context.User.Claims;
            var sType = "";
            var connectIdUser = "";
            int usrid = 0;
            int stakrid = 0;
            var UserNameRMsg = "";
            string json = "";
            string ConnectionId = _Context.Messages.Where(a => a.From == Convert.ToInt32(from)).Select(a => a.ConnId).FirstOrDefault();

            var chngeFrom = _Context.Messages.Where(a => a.To == Convert.ToInt32(userId)).ToList();
            foreach (var item in chngeFrom)
            {
                item.ConnId = Context.ConnectionId;
                _Context.Messages.Update(item);
                _Context.SaveChanges();
            }
            if (!string.IsNullOrEmpty(Pid))
            {
                var prod = _Context.ProdImages.Where(a => a.PId == Convert.ToInt32(Pid)).FirstOrDefault();
                if (prod != null)
                {
                    var rsMsg = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId) || (a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)) && a.ProductId == Convert.ToInt32(Pid)).ToList();
                    foreach (var item in rsMsg)
                    {
                        item.Image = prod.Image;
                        _Context.Messages.Update(item);
                        _Context.SaveChanges();
                    }
                }
                var rsMsga = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).FirstOrDefault();
                if (rsMsga != null)
                {
                    rsMsga.ProductId = Convert.ToInt32(Pid);
                    _Context.Messages.Update(rsMsga);
                    _Context.SaveChanges();
                }
                var rsMsg1 = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).FirstOrDefault();
                if (rsMsg1 != null)
                {
                    rsMsg1.ProductId = Convert.ToInt32(Pid);
                    _Context.Messages.Update(rsMsg1);
                    _Context.SaveChanges();
                }
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
                    usrid = Convert.ToInt32(from);
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
                usrid = Convert.ToInt32(from);
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

            var msg = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId || (a.From == dto.RecieverId && a.To == dto.SenderId)).ToList();
            foreach (var msgs in msg)
            {
                if (Pid != "")
                {
                    msg = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId && a.ProductId == Convert.ToInt32(Pid)).ToList();
                }

                if (message.Contains("/Resources/Images/Messages"))
                {
                    msgs.LastMessage = "Image";
                    msgs.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
                    msgs.LastMessage = message;
                    msgs.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
                _Context.Messages.Update(msgs);
                _Context.SaveChanges();
            }
            


            var Messagess = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId).ToList();
            if (Messagess.Count > 1)
            {
                var Messagessa = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId && a.Image == null).FirstOrDefault();
                if (Messagessa != null)
                {
                    _Context.Messages.Remove(Messagessa);
                    _Context.SaveChanges();
                }
            }


            var WhoUser = _Context.Messages.Where(a => a.From == Convert.ToInt32(from)).FirstOrDefault();
            if (WhoUser != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Pid))
                    {
                        var check = _Context.Messages.Where(a => a.From == WhoUser.From && a.To == Convert.ToInt32(userId)).FirstOrDefault();

                        if (check == null)
                        {
                            var nextCheck = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == null).FirstOrDefault();
                            if (nextCheck != null)
                            {
                                nextCheck.LastMessage = message;
                                nextCheck.ProductId = Convert.ToInt32(Pid);
                                var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                                nextCheck.Name = ausr.Name;
                                nextCheck.From = WhoUser.From;
                                nextCheck.To = Convert.ToInt32(userId);
                                nextCheck.ConnId = ConnectionId;
                                nextCheck.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                _Context.Messages.Update(nextCheck);
                                _Context.SaveChanges();
                            }
                            else
                            {
                                Message msgsfrom = new();
                                msgsfrom.LastMessage = message;
                                msgsfrom.ProductId = Convert.ToInt32(Pid);
                                var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                                msgsfrom.Name = ausr.Name;
                                msgsfrom.From = WhoUser.From;
                                msgsfrom.To = Convert.ToInt32(userId);
                                msgsfrom.ConnId = ConnectionId;
                                msgsfrom.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                _Context.Messages.Add(msgsfrom);
                                _Context.SaveChanges();
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {

                    throw;
                }
                
                
                
            }

            var WhoUser1 = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).FirstOrDefault();
            if (WhoUser1 != null)
            {
                if (!string.IsNullOrEmpty(Pid))
                {
                    var check = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == WhoUser1.From).FirstOrDefault();

                    if (check == null)
                    {
                        var nextCheck = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == null).FirstOrDefault();
                        if (nextCheck != null)
                        {
                            var conId = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).FirstOrDefault();
                            nextCheck.LastMessage = message;
                            nextCheck.ProductId = Convert.ToInt32(Pid);
                            var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                            nextCheck.Name = ausr.Name;
                            nextCheck.To = WhoUser.From;
                            nextCheck.From = Convert.ToInt32(userId);
                            nextCheck.ConnId = conId.ConnId;
                            nextCheck.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            _Context.Messages.Update(nextCheck);
                            _Context.SaveChanges();
                        }
                        else
                        {
                            Message msgsfrom = new();
                            msgsfrom.LastMessage = message;
                            msgsfrom.ProductId = Convert.ToInt32(Pid);
                            var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                            msgsfrom.Name = ausr.Name;
                            msgsfrom.From = WhoUser.From;
                            msgsfrom.To = Convert.ToInt32(userId);
                            msgsfrom.ConnId = ConnectionId;
                            msgsfrom.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            _Context.Messages.Add(msgsfrom);
                            _Context.SaveChanges();
                        }
                    }
                }
                
            }


            var delList = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).ToList();
            if (delList.Count > 1)
            {
                var dlList = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).FirstOrDefault();
                var dleList = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from) && a.Id != dlList.Id).ToList();
                _Context.Messages.RemoveRange(dleList);
                _Context.SaveChanges();
            }

            var remList = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).ToList();
            if (remList.Count > 1)
            {
                var rmList = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).FirstOrDefault();
                var rmeeList = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId) && a.Id != rmList.Id).ToList();
                _Context.Messages.RemoveRange(rmeeList);
                _Context.SaveChanges();
            }

            var ReverseChat = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).FirstOrDefault();
            if (ReverseChat == null)
            {
                Message msgs = new();
                msgs.From = Convert.ToInt32(userId);
                msgs.To = Convert.ToInt32(from);
                msgs.LastMessage = message;
                var pidset = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).FirstOrDefault();
                if (!string.IsNullOrEmpty(Pid))
                {
                    
                    if (pidset != null)
                    {
                        msgs.ProductId = pidset.ProductId;
                    }
                }
                msgs.Image = pidset.Image;
                var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                msgs.Name = ausr.Name;
                string ConnectionIdas = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();
                msgs.ConnId = ConnectionIdas;
                msgs.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                _Context.Messages.Add(msgs);
                _Context.SaveChanges();
            }

            MsgNotification msgNotification = new();
            msgNotification.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            msgNotification.Message = message;
            msgNotification.Fid = Convert.ToInt32(from);
            msgNotification.Uid = Convert.ToInt32(userId);
            msgNotification.IsRead = false;
            var ausra = _Context.Users.Where(a => a.Id == Convert.ToInt32(from)).FirstOrDefault();
            msgNotification.Fname = ausra.Name;
            _Context.MsgNotifications.Add(msgNotification);
            _Context.SaveChanges();

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
