using Merolekando.Models.Dtos;
using Merolekiando.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Merolekando.Services.Product
{
    public interface IProductService
    {
        public Task<Productdto> ManageProduct(Productdto dto);
        public Task<List<Productdto>> GetProductAsync(int id/*, int pageSize, int pageNumber*/);
        public Task<List<Productdto>> GetProductAsyncWithOutToken(int pageSize, int pageNumber);
        public Task<List<Productdto>> GetProductAsyncWithOutTokenWithCatSubCatProvncMunci(int cat, int subCat, int prvnc, int munc, int pageSize, int pageNumber, string search);
        public Task<List<Productdto>> GetUserProduct(int id, int cnt, int skip);
        public Task<Productdto> GetProductId(int id);
        public Task<List<Productdto>> GetProductSellerId(int sellerId);
        public Task<List<Productdto>> GetProductCategoryId(int id);
        public Task<List<Productdto>> GetProductCategoryId(int id, int uid);
        public Task<List<Productdto>> GetProductSubCategoryId(int id);
        public Task<List<Productdto>> GetProductSubCategoryId(int id, int uid);
        public Task<List<Productdto>> GetFavProducts(int id);
        public Task<List<Productdto>> SearchProducts(string Search, int id);
        public Task<List<Productdto>> SearchProducts(string Search);
        public Task<string> DeleteProduct(int id);
        public Task<Productdto> ProductSold(SoldDto dto);
        public Task<Productdto> ProductReport(ReportDto dto);
        public Task<Productdto> SetCountOfViewProduct(int id, int pid);
        public Task<Productdto> SetCountOfViewProduct(int pid);
        public Task<List<ImagesDto>> GetImagesByPId(int id);
    }
}
