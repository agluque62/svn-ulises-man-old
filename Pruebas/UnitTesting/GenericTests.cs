﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

using System.Threading.Tasks;

using Utilities;

namespace UnitTesting
{
    [TestClass]
    public class GenericTests
    {
        [TestMethod]
        public void HttpClientTests()
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: Test START");

            HttpHelper.GetSync("http://192.168.1.121/pepe", TimeSpan.FromSeconds(5), (succes, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: GetSync. Res {succes}, data: {data}");
            });

            HttpHelper.GetSync("http://192.168.0.212:1234/pepe", TimeSpan.FromSeconds(5), (succes, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: GetSync. Res {succes}, data: {data}");
            });

            HttpHelper.GetSync("http://192.168.0.212/pepe", TimeSpan.FromSeconds(5), (succes, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: GetSync. Res {succes}, data: {data}");
            });

            HttpHelper.GetSync("http://192.168.0.50:8080/test", TimeSpan.FromSeconds(5), (succes, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: GetSync. Res {succes}, data: {data}");
            });

            HttpHelper.GetSync("http://192.168.0.223:8080/test", TimeSpan.FromSeconds(5), (succes, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: GetSync. Res {succes}, data: {data}");
            });

            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: Test END");
        }

        [TestMethod]
        public void HttpPostTests()
        {
            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: Test START");

            HttpHelper.PostSync(HttpHelper.URL("10.12.60.130","1023","/rd11"), new { id = "test " }, TimeSpan.FromSeconds(5), (success, data) =>
            {            
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: PostSync. Res {success}, data: {data}");
            });

            HttpHelper.PostSync(HttpHelper.URL("10.12.60.130", "1023", "/rdhf"), new { id = "test " }, TimeSpan.FromSeconds(5), (success, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: PostSync. Res {success}, data: {data}");
            });

            HttpHelper.PostSync(HttpHelper.URL("10.12.60.130", "1023", "/rdhfhf"), new { id = "test " }, TimeSpan.FromSeconds(5), (success, data) =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: PostSync. Res {success}, data: {data}");
            });

            Debug.WriteLine($"{DateTime.Now.ToLongTimeString()}: Test END");
        }
        [TestMethod]
        public void TestExceptionFlow()
        {
            try
            {
                var tmp = new TestingClass(() =>
                {
                    InnerFunction(() =>
                    {
                        Debug.WriteLine("Throwing primary exception...");
                        throw new Exception("Primary Exception.");
                    });
                });
            }
            catch
            {
                StackTrace stack = new StackTrace(true);
                Debug.WriteLine("Exception Catched: " + stack.ToString());
            }
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();
        }

        void InnerFunction(Action execute)
        {
            try
            {
                execute();
            }
            catch (Exception x)
            {
                Debug.WriteLine("Primary Exception cached: ", x.Message);
                throw x;
            }
            finally
            {
                Debug.WriteLine("Cierre de Bucle de Gestion de Excepcion.");
            }
        }

        class TestingClass
        {
            public TestingClass(Action action)
            {
                //Task.Run(() =>
                //{
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    action();
                //});
            }
        }
    }
}
