using TugasAsinkron2.Interfaces;
using TugasAsinkron2.Models;
using TugasAsinkron2.Models.DataContext;
using System.Data;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace TugasAsinkron2.Data
{
    public class DataFunctions : IDataFunctions
    {
        private readonly IDataAccess _dataAccess;

        public DataFunctions(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        //Method dengan menggunakan Dapper
        public Task<List<ProductViewModel>> ReadProducts()
        {
            string sql = "select [Product].*, [Supplier].CompanyName from [Product] " +
                "left join [Supplier] on [Supplier].Id = [Product].SupplierId";
            return _dataAccess.Read<ProductViewModel, dynamic>(sql, new { });
        }


        //Buat method yang sama dengan EF Core
        //Untuk EF Core, kita sudah membuat data context secara otomatis melalui nuget
        //console, sehingga kita bisa langsung membuat querynya
        //query pada EF Core menggunakan basis LINQ

        public List<ProductViewModel> ReadProcuctsEfCore()
        {
            using (var db = new DataContext())
            {
                var products = (from p in db.Products
                                join s in db.Suppliers
                                on p.SupplierId equals s.Id
                                select new ProductViewModel
                                {
                                    Id = p.Id,
                                    ProductName = p.ProductName,
                                    SupplierId = p.SupplierId,
                                    UnitPrice = p.UnitPrice,
                                    Package = p.Package,
                                    IsDiscontinued = p.IsDiscontinued,
                                    CompanyName = s.CompanyName
                                }).ToList();
                return products;
            }
        }


        //Method-method tersebut akan saya buat dengan basis Dapper

        public Task CreateProduct(Product product)
        {
            //Pada tabel product, Id bersifat identity yang digenerate otomatis, sehingga tidak perlu dituliskan
            //Kurang kurung tutup.
            //Salah satu kelehaman Dapper dimana query tidak bisa divalidasi dari sisi kode, dan harus divalidasi
            //melalui apps run
            string sql = "insert into [Product] values (@ProductName, @SupplierId, @UnitPrice, @Package, @IsDiscontinued)";
            return _dataAccess.Write(sql, product);
        }


        public Task UpdateProduct(Product product)
        {
            string sql = "update [Product] set " +
                "ProductName = @ProductName," +
                "SupplierId = @SupplierId," +
                "UnitPrice = @UnitPrice," +
                "Package = @Package," +
                "IsDiscontinued = @IsDiscontinued " +
                "where Id = @Id";
            return _dataAccess.Write(sql, product);
        }



        public Task DeleteProduct(int Id)
        {
            string sql = "delete [Product] where Id = @Id";
            return _dataAccess.Write(sql, new { Id });
        }

        public Task<List<ProductViewModel>> ReadProduct(int Id)
        {
            string sql = "select [Product].*, [Supplier].CompanyName from [Product] " +
                "left join [Supplier] on [Supplier].Id = [Product].SupplierId " +
                "where [Product].Id = @Id";
            return _dataAccess.Read<ProductViewModel, dynamic>(sql, new { Id });
        }




		private KeyValuePair<string, dynamic> DeleteOrder(int Id)
        {
            string sql = "delete [Order] where Id = @Id";
            return new KeyValuePair<string, dynamic>(sql, new { Id });
        }

        private KeyValuePair<string, dynamic> DeleteOrderItems(int Id)
        {
            string sql = "delete [OrderItem] where OrderId = @Id";
            return new KeyValuePair<string, dynamic>(sql, new { Id });
        }

        //Gunakan Method transact yang sudah dibuat tadi;
        public async Task<string> DeleteOrderTransact(int Id)
        {
            List<KeyValuePair<string, dynamic>> sqlDict = new List<KeyValuePair<string, dynamic>>
            {
                DeleteOrderItems(Id),
                DeleteOrder(Id)
            };
            return await _dataAccess.ExecuteTransaction(sqlDict);
        }

        public async Task<string> BulkInsertProduct(DataTable data)
        {
            return await _dataAccess.BulkInsert(data, "product");
        }

		// Suppliers Methods

		public Task<List<SupplierViewModel>> ReadSuppliersAndProduct()
		{
			string sql = @"
        SELECT 
            s.*, 
            p.ProductName, 
            p.UnitPrice
        FROM [Supplier] s
        LEFT JOIN [Product] p ON s.Id = p.SupplierId";
			return _dataAccess.Read<SupplierViewModel, dynamic>(sql, new { });
		}

		public Task<List<SupplierViewModel>> ReadSuppliers()
		{
			string sql = "select * from [Supplier]";
			return _dataAccess.Read<SupplierViewModel, dynamic>(sql, new { });
		}


		public List<SupplierViewModel> ReadSuppliersAndProductEfCore()
		{
			using (var db = new DataContext())
			{
				var suppliers = (from s in db.Suppliers
								 join p in db.Products
								 on s.Id equals p.SupplierId
								 select new SupplierViewModel
								 {
									 Id = s.Id,
									 CompanyName = s.CompanyName,
									 ContactName = s.ContactName,
									 City = s.City,
									 Country = s.Country,
									 Phone = s.Phone,
									 ProductName = p.ProductName,
									 UnitPrice = p.UnitPrice
								 }).ToList();
				return suppliers;
			}
		}

		public List<SupplierViewModel> ReadSuppliersEfCore()
		{
			using (var db = new DataContext())
			{
				var suppliers = (from s in db.Suppliers
								 select new SupplierViewModel
								 {
									 Id = s.Id,
									 CompanyName = s.CompanyName,
									 ContactName = s.ContactName,
									 City = s.City,
									 Country = s.Country,
									 Phone = s.Phone
								 }).ToList();
				return suppliers;
			}
		}

		public Task CreateSupplier(Supplier supplier)
		{
			string sql = "insert into [Supplier] values (@CompanyName, @ContactName, @ContactTitle, @City, @Country, @Phone, @Fax)";
			return _dataAccess.Write(sql, supplier);
		}

		public Task UpdateSupplier(Supplier supplier)
		{
			string sql = "update [Supplier] set " +
				"CompanyName = @CompanyName," +
				"ContactName = @ContactName," +
				"ContactTitle = @ContactTitle," +
				"City = @City," +
				"Country = @Country," +
				"Phone = @Phone," +
				"Fax = @Fax " +
				"where Id = @Id";
			return _dataAccess.Write(sql, supplier);
		}

		public Task<List<SupplierViewModel>> ReadSupplier(int Id)
		{
			string sql = "SELECT * FROM [Supplier] WHERE Id = @Id";
			return _dataAccess.Read<SupplierViewModel, dynamic>(sql, new { Id });
		}


		public Task DeleteSupplier(int Id)
        {
            string sql = "delete [Supplier] where Id = @Id";
            return _dataAccess.Write(sql, new { Id });
        }
    }
}
