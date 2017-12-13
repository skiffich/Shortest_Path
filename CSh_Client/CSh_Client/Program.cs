using System;
using FriendsDllWorker;


namespace CSh_Client
{
    class Program
    {
        

        static void Main(string[] args)
        {
            do
            {
                DllWorker mc = new DllWorker();

                //mc.InitDB(ref mc.db, 100000); // init random DB with 10 elements
                //*
                mc.db.Add(0, new System.Collections.Generic.List<int>() { 5, 9, 6 });
                mc.db.Add(1, new System.Collections.Generic.List<int>() { 4, 7 });
                mc.db.Add(2, new System.Collections.Generic.List<int>() { 9, 6 });
                mc.db.Add(3, new System.Collections.Generic.List<int>() { 7, 8 });
                mc.db.Add(4, new System.Collections.Generic.List<int>() { 1, 5, 7, 8 });
                mc.db.Add(5, new System.Collections.Generic.List<int>() { 0, 4, 9, 6 });
                mc.db.Add(6, new System.Collections.Generic.List<int>() { 2, 5, 0, 7, 9 });
                mc.db.Add(7, new System.Collections.Generic.List<int>() { 1, 3, 6, 4, 8 });
                mc.db.Add(8, new System.Collections.Generic.List<int>() { 3, 7, 4, 9 });
                mc.db.Add(9, new System.Collections.Generic.List<int>() { 0, 2, 5, 8, 6 });
                //*/

                //*
                mc.Init_DLL_DB(ref mc.db);

                Console.WriteLine("_____________________Cpp__________________________");
                mc.ShowDataBase_Cpp();
                Console.WriteLine("_____________________C#____________________________");
                mc.ShowDataBase_CSharp(ref mc.db);

                Console.WriteLine("");
                Console.WriteLine("Deleting 8 and 4..........");
                Console.WriteLine("");
                mc.DeleteFriendship(8, 4, ref mc.db);
                Console.WriteLine("_____________________Cpp__________________________");
                mc.ShowDataBase_Cpp();
                Console.WriteLine("_____________________C#____________________________");
                mc.ShowDataBase_CSharp(ref mc.db);

                Console.WriteLine("");
                Console.WriteLine("Deleting 6..........");
                Console.WriteLine("");
                mc.DeleteUser(6, ref mc.db);
                Console.WriteLine("_____________________Cpp__________________________");
                mc.ShowDataBase_Cpp();
                Console.WriteLine("_____________________C#____________________________");
                mc.ShowDataBase_CSharp(ref mc.db);

                Console.WriteLine("");
                Console.WriteLine("Adding 1 and 3..........");
                Console.WriteLine("");
                mc.NewFriendship(1, 3, ref mc.db);
                Console.WriteLine("_____________________Cpp__________________________");
                mc.ShowDataBase_Cpp();
                Console.WriteLine("_____________________C#____________________________");
                mc.ShowDataBase_CSharp(ref mc.db);
                //*/

                /*
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var res = mc.FriendsWay(3, 4, ref mc.db); // find way and init DB in cpp
                sw.Stop();
                
                Console.WriteLine("_____________________Way__________________________");
                foreach (int val in res)
                    Console.Write(val + " ");
                Console.Write("      |  Count = ");
                Console.WriteLine(res.Count.ToString());
                Console.WriteLine((sw.Elapsed).ToString());
                //*/
                mc.EraseMemmory();
            }
            while(Console.ReadLine() == "a");
        }
    }
}
