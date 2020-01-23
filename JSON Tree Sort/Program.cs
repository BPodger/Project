using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Data;

namespace JSON_Tree_Sort
{


    public class Json_Output{
        public string id { get; set; }
        public string parentId { get; set; }
        public string[] Children_ID { get; set; }
    }

    public class Operations {
        List<Json_Output> arraylistoutput = new List<Json_Output>();
        List<Json_Output> arraylistinput;
        string filename = "";
        string parentnode = "";
        int parentnodeloc = 0;
        List<int> childnodeloc = new List<int>();
        List<string> children = new List<string>();


        public void File_Read(string filelocation) {
            string readline;
            filename = filelocation;
            StreamReader read = new StreamReader(filelocation);
            readline = read.ReadToEnd();
            arraylistinput = (List<Json_Output>)JsonConvert.DeserializeObject(readline, typeof(List<Json_Output>));
        }

        public void File_Write() {
            StreamWriter fileout = new StreamWriter(filename + "output.json");
            string jsonout = JsonConvert.SerializeObject(arraylistoutput);
            fileout.WriteLine(jsonout);
            fileout.Close();
        }

        public void Build(int depth) {
            switch (depth)
            {

                case 0:
                    arraylistoutput = null;
                break; //empty

                case 1:
                    
                    for (int i = 0; i < arraylistinput.Count(); i++) {

                        if (arraylistinput[i].parentId == null) {
                            parentnode = arraylistinput[i].id;
                            parentnodeloc = i;
                            break;
                        }
                    }
                    for (int j = 0; j < arraylistinput.Count(); j++) {
                        if (arraylistinput[j].parentId == arraylistinput[parentnodeloc].id) {
                            children.Add(arraylistinput[j].id);
                            childnodeloc.Add(j);
                        }
                    }

                break;  //top level

                case 2: break;
                default:
                    if (depth < 0)//no limits
                    {
                        arraylistoutput = arraylistinput.ToList();
                        for (int i = 0; i < arraylistoutput.Count(); i++)
                        {
                            
                            for (int j = 0; j < arraylistoutput.Count(); j++)
                            {
                                if (arraylistoutput[j].parentId == arraylistoutput[i].id)
                                {
                                    children.Add(arraylistoutput[j].id);
                                }
                            }
                            arraylistoutput[i].Children_ID = children.ToArray();
                            children.Clear();
                        }
                    }
                    break;
            }

        }
        

    }



    class Program
    {
        private static void Main(string[] args)
        {
            Operations FileOp = new Operations();
            string fileloc;
            int depth = 0;
            if (args.Length == 0)
            {
                Console.WriteLine("Location of Json File:");
                fileloc = Console.ReadLine();
                FileOp.File_Read(fileloc);
                Console.WriteLine("Depth Parameter:");
                depth = Convert.ToInt32(Console.ReadLine());
            }
            else {
                fileloc = args[0];
                FileOp.File_Read(fileloc);
                Console.WriteLine("Depth Parameter:");
                depth = Convert.ToInt32(Console.ReadLine());
            }

            FileOp.Build(depth);
            FileOp.File_Write();
            string waitonexit = Console.ReadLine();

        }
    }
}


/*
GOALS:

    1. import json file and convert it into an array
        -filereader, import into a string
        -deserialise object into a generic array
    2. perform a search on the array to find each members child/parent pairings
        -build search loop and add data to the child array of each object
        -after search complete put object into array to be serialised
    3. convert back into json and output the file in the same location
        -serialise object array into Json string
        -filewriter to location of read object    
*/