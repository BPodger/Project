using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace JSON_Tree_Sort
{


    public class Json_Output{
        public string id { get; set; }
        public string parentId { get; set; }
        public string[] Children_ID { get; set; }
    }
    /*Multiple exceptions for extensibility and future additions*/
    public class FileParseException : Exception
    {
        public FileParseException() { }
        public FileParseException(string message) : base(message) { }

    }

    public class MultipleParentNodeException : Exception {
        public MultipleParentNodeException() { }
        public MultipleParentNodeException(string message) :base(message) { }

    }

    public class OrphanNodeException : Exception
    {
        public OrphanNodeException() { }
        public OrphanNodeException(string message) : base(message) { }

    }

    public class ErrorHandling
    {/*I kept the error handling as a seperate class for readability and testing, making it more obvious whats going on*/
        public ErrorHandling() { }   
        public ErrorHandling(List<Json_Output> TreeToCheck) {
            RootErrorHandling(TreeToCheck);
            ChildErrorHandling(TreeToCheck);
            TreeIsTreeHandling(TreeToCheck);

        }
        int ParentNode;
        int TopLevelChildren;
        public void RootErrorHandling(List<Json_Output> TreeCheck)
        {
            try
            {
                foreach (Json_Output NodeToCheck in TreeCheck) {
                    if (NodeToCheck.parentId == null)
                    {
                        ParentNode++; /*count to see if theres more than one node with no parents*/
                        if (ParentNode >= 2)
                        {
                            throw new MultipleParentNodeException("Found More Than One Node Without Parents");
                        }
                    }
                }
            }
            catch (MultipleParentNodeException e)
            {
                Console.WriteLine("error handling data:" + e.Message);
                Console.ReadLine();
                System.Environment.Exit(1);

            }


        }
        public void ChildErrorHandling(List<Json_Output> TreeCheck) {     
            try {
                string TopId = "";
                    foreach (Json_Output Node in TreeCheck)
                {
                    if (Node.parentId == null) { TopId = Node.id; }
                    if (Node.parentId == TopId)
                    {
                        TopLevelChildren++;
                    }
                    
                }
                if (TopLevelChildren == 0)
                {
                    throw new OrphanNodeException("Top Level Node Has No Children");
                }

            } catch (OrphanNodeException e) {
                Console.WriteLine("error handling data:" + e.Message);
                Console.ReadLine();
                System.Environment.Exit(1);
            }
        }
        public void TreeIsTreeHandling(List<Json_Output> TreeCheck) {
            /*check to make sure all nodes are part of the same tree, 
             *there are no orphan nodes that have parents that are not part of the tree*/
            int CountTree = 0;
            try
            {
                foreach (Json_Output node in TreeCheck)
                {
                    for (int i = 0; i < TreeCheck.Count(); i++)
                    {
                        if (node.parentId == TreeCheck[i].id)
                        {
                            CountTree++;
                            break;
                        }

                    }

                    if (node.parentId != null && CountTree == 0) {
                        throw new OrphanNodeException("Node Is Not Part Of Tree");

                    }
                    CountTree = 0;
                }
            }
            catch(OrphanNodeException e) {

                Console.WriteLine("error handling data:" + e.Message);
                Console.ReadLine();
                System.Environment.Exit(1);

            }


        }
    }

    public class Operations {
        List<Json_Output> arraylistoutput = new List<Json_Output>();
        List<Json_Output> arraylistinput;
        string filename = "";
        string parentnode = "";
        int depth;
        List<int> nodeloc = new List<int>();
        List<string> children = new List<string>();
        ErrorHandling handler;


        public void File_Read(string filelocation) {
            string readline;
            Match Match;
            string MatchString;
            StreamReader read;
            Regex Trunc = new Regex("],.*]$",RegexOptions.Singleline);
            filename = filelocation;

            try
            {
                read = new StreamReader(filelocation);
            
            

                readline = read.ReadToEnd();
                readline.Trim();
                Match = Trunc.Match(readline);
                MatchString = Match.Value;
                MatchString = Regex.Replace(MatchString, "[^0-9-]+", "");
                readline = Regex.Replace(readline, @"][\s\S]*]$", "]");/*Regex to remove the extra brackets 
                                                                         and the depth parameter from each file read*/
                readline = Regex.Replace(readline, @"[[\s\S]*\[", "[");
                depth = Int32.Parse(MatchString);

                arraylistinput = (List<Json_Output>)JsonConvert.DeserializeObject(readline, typeof(List<Json_Output>));
            }
            catch (Newtonsoft.Json.JsonReaderException e) {
                Console.WriteLine("error handling file input:" + e.Message);
                Console.ReadLine();
                System.Environment.Exit(1);

            }
            catch (Exception e)
            {

                Console.WriteLine("File Not Found: " + e.Message);
                Console.ReadLine();
                System.Environment.Exit(1);
            }


            if (depth > 0)
            handler = new ErrorHandling(arraylistinput);
            
        }

        public void File_Write() {          
            string jsonout = JsonConvert.SerializeObject(arraylistoutput);
            jsonout = Regex.Replace(jsonout, ",", ",\n"); /*replace the commas with carrage returns for readability
                                                            maybe add in some more output formatting in the future to get
                                                            it as close to the input as possible
                                                          */
            StreamWriter fileout = new StreamWriter(filename + "output.json");
            if(arraylistoutput != null)
            fileout.WriteLine(jsonout);
            fileout.Close();
        }

        public void Build() {
            switch (depth)
            {
                /*error checking is done after file read to reduce code duplication, I could probably abstract some of the loops too*/
                case 0:
                    arraylistoutput = null;/*Its not quite empty but the output file should be*/
                    break; 

                case 1:/*Mode that selects only the top level node and its children, 
                        spec wasn't clear if I should include the top level childrens children but I did it anyway*/

                    for (int i = 0; i < arraylistinput.Count(); i++) {
                        if (arraylistinput[i].parentId == null) {
                            parentnode = arraylistinput[i].id;
                            nodeloc.Add(i);
                        }
                    }
                    for (int j = 0; j < arraylistinput.Count(); j++) {
                        if (arraylistinput[j].parentId == arraylistinput[nodeloc[0]].id) {
                            nodeloc.Add(j);
                        }
                    }
                    foreach (int loc in nodeloc) {
                        arraylistoutput.Add(arraylistinput[loc]);
                    }

                    for (int k = 0; k < arraylistoutput.Count(); k++) {
                        for (int l = 0; l < arraylistinput.Count(); l++) {
                            if (arraylistoutput[k].id == arraylistinput[l].parentId) {
                                children.Add(arraylistinput[l].id);
                            }
                        }
                        arraylistoutput[k].Children_ID = children.ToArray();
                        children.Clear();
                    }

                break; 

                case 2: /*the whole tree*/
                    arraylistoutput = arraylistinput;
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




                    break;
                default:
                    if (depth < 0)//no limits
                    {
                        /*no limits truly means no limits, theres no sanity checking and if everything parsed fine then the 
                         * code will operate on the entire tree, even if its broken or incomplete 
                         */
                        arraylistoutput = arraylistinput;
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
                /*at first I was thinking of taking the two inputs seperatly but after
                seeing all the files were in the same format, just reading that felt like a much better UX */
            if (args.Length == 0) /*the program exe will take a file dragged onto it as input,
                                    returning a file in the same location if it can perform valid operations on it*/ 
            {
                Console.WriteLine("Location of Json File:");
                fileloc = Console.ReadLine();
                FileOp.File_Read(fileloc);
            }
            else {
                fileloc = args[0];
                FileOp.File_Read(fileloc);
            }

            FileOp.Build();
            FileOp.File_Write();
            Console.WriteLine("Operation Completed");
            Console.ReadLine();

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
    4. add exception handling and data checks
        -file in right format for conversion
        -data sanity checks (one root node, root node has children, child node parents are a part of the tree)
        -data checks not needed for -1 since its 'without limit'
*/