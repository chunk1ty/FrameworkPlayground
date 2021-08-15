using System;

namespace DiContainer.Services
{
    public interface IRepository
    {
    }
    
    public class Repository : IRepository
    {
        public Repository()
        {
            Console.WriteLine("Repository constructor!");
        }
    }
}