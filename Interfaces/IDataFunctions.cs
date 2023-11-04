using TugasAsinkron2.Models;
using TugasAsinkron2.Models.DataContext;
using System.Data;

namespace TugasAsinkron2.Interfaces
{
    public interface IDataFunctions
    {
        // Method Untuk data produk
        Task<List<ProductViewModel>> ReadProducts();
        List<ProductViewModel> ReadProcuctsEfCore();


        //tambahkan method tersebut pada interface yang sudah dibuat

        Task CreateProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int Id);

        //tambahkan juga method untuk membaca sebuah produk tertentu berdasarkan Id
        Task<List<ProductViewModel>> ReadProduct(int Id);

        Task<string> DeleteOrderTransact(int Id);
        Task<string> BulkInsertProduct(DataTable data);

        //Method Untuk data supplier
        Task<List<SupplierViewModel>> ReadSuppliers();
        List<SupplierViewModel> ReadSuppliersEfCore();

		//tambahkan juga method untuk membaca sebuah produk tertentu berdasarkan Id
		Task<List<SupplierViewModel>> ReadSupplier(int Id);

		//tambahkan method tersebut pada interface yang sudah dibuat
		Task CreateSupplier(Supplier supplier);
        Task UpdateSupplier(Supplier supplier);
        Task DeleteSupplier(int Id);






	}

}
