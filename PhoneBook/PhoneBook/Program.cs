using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhoneBook
{
    class main
    {
        static void Main(string[] args)
        {
            PhoneBook MyPhoneBook = new PhoneBook(@"C:\efrat\test1.txt");
            /*
             Example of file format:
                Yarden,054-3333333,Home
                Shira,054-4444444,Home
                Tamir,054-8592017,Home
             */
            List<PhoneBook.Entry> PhoneBookList = MyPhoneBook.Iterate().ToList();

            //GetByName
            PhoneBook.Entry t1 = MyPhoneBook.GetByName("Yarden");

            //Update
            PhoneBook.Entry t2 = new PhoneBook.Entry() 
                {Name = "Shira", Phone = "057-123456789", Type = "Home" };
            MyPhoneBook.InsertOrUpdate(t2);

            //Insert
            PhoneBook.Entry t3 = new PhoneBook.Entry() 
                { Name = "Tamir", Phone = "057-123456789", Type = "Home" };
            MyPhoneBook.InsertOrUpdate(t3);

        }

    }


    public class PhoneBook
    {
        public string FileName { get; set; }

        public class Entry
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Type { get; set; }// home | work, etc
        }

        public PhoneBook(string filename) 
        {
            FileName = filename;
        }

        public Entry GetByName(string name)
        {
            string line;
            Entry entry = new Entry();
            List<Entry> EntryList = new List<Entry>();

            // Read the file line by line  
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(',');
                EntryList.Add(new Entry() { Name = words[0], Phone = words[1], Type = words[2] });
            }
            file.Close();
            return EntryList.Find(e => e.Name == name);
        }
        public void InsertOrUpdate(Entry e)
        {
            string StrToFind = FindLineOfEntry(e.Name); //old string
            
            if(StrToFind != null) //Update in middle of the file 
            {
                //replace into the file the old string with the new string
                string StrToAdd = e.Name + "," + e.Phone + "," + e.Type; //new string
                string StrFile = File.ReadAllText(FileName);
                int pos = StrFile.IndexOf(StrToFind);
                string NextPartFile = StrFile.Substring(pos + StrToFind.Length);
                string NewNextPart = StrToAdd + NextPartFile;
                for (int i = StrToFind.Length - StrToAdd.Length; i > 0; i--) NewNextPart = NewNextPart + " ";
                using (var stream = File.Open(FileName, FileMode.Open))
                {
                    stream.Position = pos;
                    byte[] byte_StrToAdd = Encoding.ASCII.GetBytes(NewNextPart);
                    stream.Write(byte_StrToAdd, 0, byte_StrToAdd.Length);
                    //stream.WriteByte(0x1A); //EOF
                    stream.Close();
                }
            }
            else
            {
                // this is a new entry, write it to the end of the file
                using (StreamWriter sw = File.AppendText(FileName))
                {
                    sw.WriteLine(e.Name + "," + e.Phone + "," + e.Type);
                }
            }
        }

        // Get a list of all the entries, in Name order
        public IEnumerable<Entry> Iterate()
        {
            string line;
            Entry entry = new Entry();
            List<Entry> EntryList = new List<Entry>();

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(',');
                EntryList.Add(new Entry() { Name = words[0], Phone = words[1], Type = words[2] });
            }
            file.Close();
            return from ent in EntryList
                   orderby ent.Name.Substring(0, 1)
                   select ent;
        }

        public string FindLineOfEntry(string Name)
        {
            string line;
            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(FileName);
            while ((line = file.ReadLine()) != null)
            {
                if(line.Contains(Name))
                {
                    file.Close();
                    return line;
                }
            }
            file.Close();
            return null;

        }


    }





}
