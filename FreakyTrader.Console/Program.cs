using Microsoft.Extensions.DependencyInjection;

namespace FreakyTrader.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new AppBuilder().Build();

            app.Run();
        }
    }
}

