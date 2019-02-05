using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JaSonRelated
{
    class Program
    {
        static void Main(string[] args)
        {

            using (StreamReader r = File.OpenText("some_json_file.json"))
            {
                string json = r.ReadToEnd();
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);
                var hobby = result.hobby;
                Console.WriteLine(json);
                Console.WriteLine(hobby);
              
            }
        }
    }

}
