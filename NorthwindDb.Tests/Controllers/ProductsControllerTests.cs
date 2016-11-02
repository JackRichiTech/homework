using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindDb.Controllers;
using NorthwindDb.Models;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDb.Controllers.Tests
{
    [TestClass()]
    public class ProductsControllerTests
    {
        // 方法名稱會切割成三段，每段以底線(_ ) 分隔
        // 第 1 段：被測試的 Method 名稱
        // 第 2 段：測試的情境(Scenario)
        // 第 3 段：預期測試的結果

        [TestMethod()]
        public void GetProducts_取得所有產品_數量大於1()
        {
            // arrange
            var expected = 1;

            var client = new RestClient("http://localhost:9735/api/products");
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "350c30d9-33dc-2a75-e6ca-824fecdd599d");
            request.AddHeader("cache-control", "no-cache");

            // act            
            var products = new List<Product>();
            var response = client.Execute(request);
            var deserial = new JsonDeserializer();
            products = deserial.Deserialize<List<Product>>(response);
            // assert

            Assert.IsTrue(products.Count > expected);
        }

        [TestMethod()]
        public void GetProduct_取得ID為1的產品_ID相同()
        {
            // arrange
            var expected = 1;

            var client = new RestClient("http://localhost:9735/api/products/1");
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "350c30d9-33dc-2a75-e6ca-824fecdd599d");
            request.AddHeader("cache-control", "no-cache");

            // act
            // Execute<T> 執自動進行反序列化
            var response = client.Execute<Product>(request);
            // assert
            // response.Data. <-- 自己點點看，強型別才有辦法驗證
            Assert.AreEqual(expected, response.Data.ProductID);
        }

        [TestMethod()]
        public void PutProduct_更新ID為1的產品_HttpStatus狀態為204()
        {
            // arrange
            var expected = HttpStatusCode.NoContent;

            var client = new RestClient("http://localhost:9735/api/Products/1");
            var request = new RestRequest(Method.PUT);
            request.AddHeader("postman-token", "e9ac9f92-99a8-34d9-9a4d-7575f372df12");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\n  \"ProductID\": 1,\n  \"ProductName\": \"Update_ProductName\",\n  \"SupplierID\": 1,\n  \"CategoryID\": 1,\n  \"QuantityPerUnit\": \"10 boxes x 20 bags\",\n  \"UnitPrice\": 18,\n  \"UnitsInStock\": 39,\n  \"UnitsOnOrder\": 0,\n  \"ReorderLevel\": 10,\n  \"Discontinued\": false\n}", ParameterType.RequestBody);

            //act
            IRestResponse response = client.Execute(request);

            // assert
            Assert.AreEqual(expected, response.StatusCode);
        }

        [TestMethod()]
        public void PostProduct_新增一個產品_HttpStatus狀態為201()
        {
            // arrange
            var expected = HttpStatusCode.Created;

            // act
            IRestResponse response = AddProduct();

            // assert
            Assert.AreEqual(expected, response.StatusCode);
        }

        [TestMethod()]
        public void DeleteProduct_刪除一個產品資料_HttpStatus狀態為200()
        {

            // arrange
            var expected = HttpStatusCode.OK;

            #region 先新增一筆產品
            IRestResponse addProductResonse = AddProduct();
            var deserial = new JsonDeserializer();
            var product = deserial.Deserialize<Product>(addProductResonse);

            if (addProductResonse.StatusCode != HttpStatusCode.Created) {
                Assert.Fail();
            }
            #endregion

            // act
            var uri = $"http://localhost:9735/api/Products/{product.ProductID}";
            var client = new RestClient(uri);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("postman-token", "e8ec6a5b-e1de-3c53-fe39-e37601d8b2c5");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            IRestResponse response = client.Execute(request);

            // assert
            Assert.AreEqual(expected, response.StatusCode);

        }

        [TestMethod()]
        public void PatchProductName_僅更新產品ID為1的產品名稱_HttpStatus狀態為201且產品名稱為Patch_ProductName()
        {
            // arrange
            var expected = "Patch_ProductName";

            // act
            var client = new RestClient("http://localhost:9735/api/Products/1");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("postman-token", "ce590f4b-f0ed-94e8-2835-a17122d18d26");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\n  \"ProductID\": 1,\n  \"ProductName\": \"Patch_ProductName\",\n  \"SupplierID\": 2\n}", ParameterType.RequestBody);
            var response = client.Execute<Product>(request);
            
            // assert            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expected, response.Data.ProductName);
        }

        ///<summary>
        /// 新增一個產品
        ///</summary>
        /// <returns>IRestResponse</returns>
        private IRestResponse AddProduct()
        {
            var client = new RestClient("http://localhost:9735/api/Products");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "dd376871-2a8c-a68c-4cad-7c0db5099f1f");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\n  \"ProductID\": 1,\n  \"ProductName\": \"Update_ProductName\",\n  \"SupplierID\": 1,\n  \"CategoryID\": 1,\n  \"QuantityPerUnit\": \"10 boxes x 20 bags\",\n  \"UnitPrice\": 18,\n  \"UnitsInStock\": 39,\n  \"UnitsOnOrder\": 0,\n  \"ReorderLevel\": 10,\n  \"Discontinued\": false\n}", ParameterType.RequestBody);

            return client.Execute<Product>(request);
        }
    }
}