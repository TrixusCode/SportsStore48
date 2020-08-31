using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;
using SportsStore.Models.ViewModels;


namespace SportsStore.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Can_Send_Pagination_View_Model() {
            //Arrange
        Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
        mock.Setup(m => m.Products).Returns((new Product[] {
            new Product {ProductID = 1, Name = "P1"},
            new Product {ProductID = 2, Name = "P2"},
            new Product {ProductID = 3, Name = "P3"},
            new Product {ProductID = 4, Name = "P4"},
            new Product {ProductID = 5, Name = "P5"}
        }).AsQueryable<Product>());

        //Arrange
        HomeController controller = 
        new HomeController(mock.Object) {PageSize = 3};

        //Act
        ProductListViewModel result = 
        (controller.Index(null,2) as ViewResult).ViewData.Model as ProductListViewModel;
        //Assert 
        PagingInfo pageInfo = result.PagingInfo;
        Assert.Equal(2, pageInfo.CurrentPage);
        Assert.Equal(3, pageInfo.ItemsPerPage);
        Assert.Equal(5, pageInfo.TotalItems);
        Assert.Equal(2, pageInfo.TotalPages);

        }

        [Fact]
        public void Can_Use_Repository() {
            //Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2,  Name = "P2"},
                 new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4,  Name = "P4"},
                 new Product {ProductID = 5, Name = "P5"}
                
            }).AsQueryable<Product>());

            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductListViewModel result = (controller.Index(null) as ViewResult).ViewData.Model as ProductListViewModel;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }
        [Fact]
        public void Can_Paginate() {
            //Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product [] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
                new Product { ProductID = 4, Name = "P4"},
                new Product { ProductID = 5, Name = "P5"}
            }).AsQueryable<Product>());

            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductListViewModel result = (controller.Index(null,2) as ViewResult).ViewData.Model as ProductListViewModel;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }
        [Fact]
        public void Can_Filter_Products() {
            //Arrange
            //-create the mock repository
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cart1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cart2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cart1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cart2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cart3"}
            }).AsQueryable<Product>());

            //Arrange - Create a controller and make the page size 3 items
            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 3;

            //Action
            Product[] result = 
            ((controller.Index("Cart2",1) as ViewResult).ViewData.Model as  ProductListViewModel).Products.ToArray();

            //Assert
            Assert.Equal(2,result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cart2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cart2");
        }
         [Fact]
        public void Generate_Category_Specific_Product_Count() {
            //Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product [] {
                new Product {ProductID = 1, Name = "P1" , Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2" , Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3" , Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4" , Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5" , Category = "Cat3"},
            }).AsQueryable<Product>());

            HomeController target = new HomeController(mock.Object);
            target.PageSize = 3;

            Func<ViewResult, ProductListViewModel> GetModel = result =>
            result?.ViewData?.Model as ProductListViewModel;

            //Action
            int? res1 = GetModel((ViewResult)target.Index("Cart1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel((ViewResult)target.Index("Cart2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel((ViewResult)target.Index("Cart3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel((ViewResult)target.Index("null"))?.PagingInfo.TotalItems;

            //Assert
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1,res3);
            Assert.Equal(5, resAll);
 
        }
    }
}