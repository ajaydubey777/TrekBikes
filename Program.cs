using System;
using Newtonsoft.Json;
using System.IO;
using TrekBikes.DomainModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;

namespace TrekBikes
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProcessUsingLineByLineJsonreader();
            ProcessByLoadingWholeJson();
            Console.ReadLine();
        }

        /// <summary>
        /// Load json data one by one and perfrom calculation logic.
        /// assumption :
        /// 1. We are only interested in combination i.e. considering only those responses that have more than one bikes.
        ///     if this is not the case please comment line number  : 51
        /// 2. We are considering partial combinations also like if we have two resposne like 
        ///  response 1: Bike A , BikeB, BikeC
        ///  response 2 : BikeA, BikeB
        ///  in this case frequency calculate result will be as below :
        ///  BikeA, BikeB, BikeC =1
        ///  BikeA, BikeB =2
        ///  if above assumption is not correct and we are interested only in full match then please go and comment "Matching Partial combination" region.
        /// </summary>
        private static void ProcessUsingLineByLineJsonreader()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Dictionary<string, int> bikesComboFrequency = new Dictionary<string, int>();

            using (FileStream fs = new FileStream("data/bikes.json", FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        // Load each object from the stream and do something with it
                        JObject obj = JObject.Load(reader);
                        string jsondata = obj.ToString(Newtonsoft.Json.Formatting.None);
                        var SurveyObject = JsonConvert.DeserializeObject<SurveyResponse>(jsondata);
                        if (SurveyObject.bikes.Length > 1)
                        {
                            string bikesCombination = string.Join(",", SurveyObject.bikes.OrderBy(q => q));
                            int combiNationFreq = 0;
                            bikesComboFrequency.TryGetValue(bikesCombination, out combiNationFreq);
                            bikesComboFrequency[bikesCombination] = combiNationFreq + 1;

                        }

                    }



                }
            }


            bikesComboFrequency = bikesComboFrequency.OrderByDescending(d => d.Key.Length).ToDictionary(x => x.Key, x => x.Value);

            ////code for matching
            #region Matching Partial combination
            bikesComboFrequency.ToList().ForEach(bikecombofreq =>
            {
                List<string> keys = bikesComboFrequency.GetKeysLike<int>(bikecombofreq.Key);
                int freq = 0;
                foreach (string key in keys)
                {
                    freq += bikesComboFrequency[key];
                }

                bikesComboFrequency[bikecombofreq.Key] = freq;
            });
            #endregion



            bikesComboFrequency = bikesComboFrequency.OrderByDescending(d => d.Value).Take(15).ToDictionary(x => x.Key, x => x.Value);
            foreach (var dict in bikesComboFrequency)
            {
                Console.WriteLine(string.Format("bikes combinaton :{0} , Families count :{1}", dict.Key, dict.Value));

            }
            sw.Stop();
            //Console.WriteLine(" time elapsed :" + sw.ElapsedMilliseconds / 1000);
        }

        /// <summary>
        /// Load Complete Json data in One Go, that may cause issue if we have a very large json file..
        /// assumption :
        /// 1. We are only interested in combination i.e. considering only those responses that have more than one bikes.
        ///     if this is not the case please comment line number  : 123
        /// 2. We are considering partial combinations also like if we have two resposne like 
        ///  response 1: Bike A , BikeB, BikeC
        ///  response 2 : BikeA, BikeB
        ///  in this case frequency calculate result will be as below :
        ///  BikeA, BikeB, BikeC =1
        ///  BikeA, BikeB =2
        ///  if above assumption is not correct and we are interested only full match then please go and comment "Matching Partial combination" region.
        /// </summary>

        private static void ProcessByLoadingWholeJson()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Dictionary<string, int> bikesComboFrequency = new Dictionary<string, int>();
            //string path=
            using (FileStream fs = new FileStream("data/bikes.json", FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                var jsondata = sr.ReadToEnd();
                var jsonobject = JsonConvert.DeserializeObject<List<SurveyResponse>>(jsondata);
                foreach (SurveyResponse obj in jsonobject)
                {
                    if (obj.bikes.Length > 1)
                    {
                        string bikesCombination = string.Join(",", obj.bikes.OrderBy(q => q));
                        int combiNationFreq = 0;
                        bikesComboFrequency.TryGetValue(bikesCombination, out combiNationFreq);
                        bikesComboFrequency[bikesCombination] = combiNationFreq + 1;

                    }
                }
            }

            bikesComboFrequency = bikesComboFrequency.OrderByDescending(d => d.Key.Length).ToDictionary(x => x.Key, x => x.Value);

            ////code for matching
            #region Matching Partial combination
            bikesComboFrequency.ToList().ForEach(bikecombofreq =>
                        {
                            List<string> keys = bikesComboFrequency.GetKeysLike<int>(bikecombofreq.Key);
                            int freq = 0;
                            foreach (string key in keys)
                            {
                                freq += bikesComboFrequency[key];
                            }

                            bikesComboFrequency[bikecombofreq.Key] = freq;
                        });
            #endregion

            bikesComboFrequency = bikesComboFrequency.OrderByDescending(d => d.Value).Take(15).ToDictionary(x => x.Key, x => x.Value);
            foreach (var dict in bikesComboFrequency)
            {
                Console.WriteLine(string.Format("bikes combinaton :{0} , Families count :{1}", dict.Key, dict.Value));

            }
            sw.Stop();
            // Console.WriteLine(" time elapsed :" + sw.ElapsedMilliseconds / 1000);
        }


    }
}
