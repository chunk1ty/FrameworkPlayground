using System;

namespace IoCContainer.Services
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