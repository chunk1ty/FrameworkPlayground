using DiContainer.Core;
using DiContainer.Core.MyAdvancedDiContainer;
using DiContainer.Services;

namespace DiContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var mySimpleContainer = new MySimpleContainer();
            //mySimpleContainer.Register<IRepository, Repository>(new Repository());

            //mySimpleContainer.Resolve<IRepository>();

            var myServiceCollection = new MyServiceCollection();
            myServiceCollection.RegisterTransient<IEmailSender, SmtpEmailSender>();
            myServiceCollection.RegisterSingleton<IRepository, Repository>();
            myServiceCollection.RegisterTransient<LoginController>();

            var myContainer = myServiceCollection.GenerateContainer();
            var hash1 = myContainer.GetService<IRepository>().GetHashCode();
            var hash2 = myContainer.GetService<IRepository>().GetHashCode();

            var loginController = myContainer.GetService<LoginController>();
        }
    }
}