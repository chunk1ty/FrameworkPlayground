using Cache;
using System;
using System.Threading.Tasks;

var myCache = new MyMemoryCache<int, string>(new TimeSpan(0, 0, 50));

myCache.Set(17, "ank");

// await Task.Delay(3000);

var myEntry = myCache.Get(17);

Console.ReadLine();
