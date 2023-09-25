using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Merolekando.Services.Extra
{
    public interface IExtra
    {
        public Task<string> ManageProvince(Province province);
        public Task<string> ManageMunicipality(Municipality municipality);
        public Task<string> ManageCategory(Category category);
        public Task<string> ManageSubCategory(SubCategory category);
        public Task<string> ManageBanner(BannerDto banner);
        public Task<List<ProvinceDto>> GetProvinces();
        public Task<Province> GetProvinceById(int id);
        public Task<List<Municipality>> GetMunicipalities();
        public Task<Municipality> GetMunicipalityById(int id);
        public Task<List<CategoryDto>> GetCategories();
        public Task<Category> GetCategoryById(int id);
        public Task<List<SubCategory>> GetSubCategories();
        public Task<SubCategory> GetSubCategoryById(int id);
        public Task<List<Banner>> GetBanners();
        public Task<List<JsonApiDatum>> GetJsonApiData();
        public Task<Banner> GetBannerById(int id);
        public Task<UserDto> GiveRate(Rating dto);
        public Task<string> SetMessageUser(MessaveDto dto);
        public Task<List<MessagesDto>> GetChatUsers(int id);
        public Task<List<MessagesDto>> GetChatsByProduct(int id, int uid);
        public Task<List<Chat>> GetChatById(int id, int fromId);
        public Task<string> SendMessage(ChatsDto dto);
        public Task<MessageDto> SendImage(SendImageDto dto);
        public Task<UserDto> Follow(Folower dto);
        public Task<string> Notify(int Id, int Pid);
        public Task<List<NotificationDto>> GetNotify(int id);
        public Task<List<MsgNotification>> GetMSgNotify(int id);
        public Task<ContactDto> AllIfon();
        public Task<long?> CheckSubs(int id);
        public Task<string> Chat(string message, string userId, string Pid, string from, string conn);
        public Task<string> ChangeConnId(string conn, int userId, int sellerId, int pId);
    }
}
