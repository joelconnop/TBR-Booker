//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TBRBooker.Model.Entities;

//namespace TBRBooker.Business
//{

//    public class BaseItemFactory
//    {
//        public static T CreateItem<T>()
//        {
//            var type = typeof(T);
//            var fac = new GenericItemFactory<T>();
//            return fac.CreateItemFromInstance();
//        }
//    }

//    public class GenericItemFactory<T> where T : new()
//    {
//        public T CreateItemFromInstance()
//        {
//            return new T();
//        }

//    }
//}
