using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FriendsDllWorker
{
    class DllWorker
    {
        private const string path2dll = @"D:\EleksTutorials\GraphDLLworker\Graph\Release\Graph.dll";

        // (must be such as in c++ dll)
        private const int MAX_Friends_for_1_user = 255;

        // Max send in 1 time (must be such as in c++ dll); depends of MAX_Friends_for_1_user number
        private const int MAX_Send = 50;

        public int Get_MAX_Friends_for_1_user() { return MAX_Friends_for_1_user; }

        public int Get_MAX_Send() { return MAX_Send; }

        // Create new random DB
        public void InitDB(ref Dictionary<int, List<int>> db, int n)
        {
            Dictionary<int, HashSet<int>> tmp = new Dictionary<int, HashSet<int>>();

            for (int i = 0; i < n; i++)
            {
                List<int> v = new List<int>();
                HashSet<int> s = new HashSet<int>();
                db.Add(i, v);
                tmp.Add(i, s);
            }

            Random random = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int x = 0; x < 2; x++)
                {
                    int ins;
                    do { ins = random.Next(0, n); } while (ins == i);
                    tmp[i].Add(ins);
                    tmp[ins].Add(i);
                }
            }

            for (int i = 0; i < n; i++)
            {
                List<int> v = new List<int>();

                foreach (int var in tmp[i])
                {
                    v.Add(var);
                }

                db[i] = v;
            }
        }

        // ThisId - Id of current user, TargetId - Id of someone else user; ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr FriendsWay(int ThisId, int TargetId, IntPtr ptr);

        // sdb - struct which contains MAX_Send elements of DB; Count - number of elemets; ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr SendDB(ref _SDB sdb, int Count, IntPtr ptr);

        // returns pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr InitMap();

        // ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Show_DB(IntPtr ptr);

        // ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void DeleteMap(IntPtr ptr);

        // ThisId - Id of current user, TargetId - Id of someone else user; ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr DllDeleteFriedship(int ThisId, int TargetId, IntPtr ptr);

        // ThisId - Id of current user, ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr DllDeleteUser(int ThisId, IntPtr ptr);

        // ThisId - Id of current user, TargetId - Id of someone else user; ptr - pointer to c++ map container
        [DllImport(path2dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr AddNewFriendship(int ThisId, int TargetId, IntPtr ptr);

        // Element of DB; Can contain MAX_Friends_for_1_user friends
        [StructLayout(LayoutKind.Explicit)]
        unsafe private struct SDB
        {
            [FieldOffset(0)]
            public int key;

            [FieldOffset(4)]
            public int lengthVal;

            [FieldOffset(8)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_Friends_for_1_user)]
            unsafe public int[] value;
        }

        // Struct DB which contains MAX_Send elements of DB
        [StructLayout(LayoutKind.Explicit)]
        unsafe private struct _SDB
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_Send)]
            unsafe public SDB[] sdb;
        };

        // Convert interval of DB to struct _SDB
        private SDB[] DB_2_Struct(int from, int to, ref Dictionary<int, List<int>> cdb)
        {
            SDB[] sdb = new SDB[MAX_Send];

            var keys = cdb.Select(z => z.Key).ToArray();
            var values = cdb.Select(z => z.Value).ToArray();
            
            for (int i = from; i < to; i++)
            {
                sdb[i % MAX_Send].key = keys[i];

                sdb[i % MAX_Send].lengthVal = values[i].Count;

                //*
                int x = MAX_Friends_for_1_user - values[i].Count;
                for (int j = 0; j < x; j++)
                {
                    values[i].Add(-1);
                }
                //*/
                sdb[i % MAX_Send].value = values[i].ToArray();
            }

            for (int j = sdb.Length; j < MAX_Send; j++)
            {
                sdb[j].key = -1;
                sdb[j].lengthVal = -1;
                sdb[j].value[0] = -1;
            }

            return sdb;
        }

        // Is DB init in c++
        private bool init = false;

        // Pointer to c++ map container
        private IntPtr ptr = new IntPtr(0);

        // Send DataBase to c++
        private IntPtr Send_DB(ref Dictionary<int, List<int>> cdb)
        {
            int remainder = cdb.Count % MAX_Send;
            int fraction = (cdb.Count - remainder) / MAX_Send;

            var keys = db.Select(z => z.Key).ToArray();
            var values = db.Select(z => z.Value).ToArray();

            ptr = InitMap();

            init = true;

            for (int i = 0, point = 0; i < fraction; i++, point += MAX_Send)
            {
                SDB[] sdb = DB_2_Struct(point, point + MAX_Send, ref cdb);
                _SDB _sdb = new _SDB();
                _sdb.sdb = sdb;

                ptr = SendDB(ref _sdb, MAX_Send, ptr);
            }

            SDB[] rem_sdb = DB_2_Struct(fraction * MAX_Send, cdb.Count, ref cdb);
            _SDB _rem_sdb = new _SDB();
            _rem_sdb.sdb = rem_sdb;
            ptr = SendDB(ref _rem_sdb, remainder, ptr);

            return ptr;
        }

        // Initialize DataBase in c++ dll
        public IntPtr Init_DLL_DB(ref Dictionary<int, List<int>> cdb)
        {
            return Send_DB(ref cdb);
        }

        // Show DataBase 
        public void ShowDataBase_Cpp()
        {
            if(init)
            {
                Show_DB(ptr);
            }
        }

        // Show DataBase 
        public void ShowDataBase_CSharp(ref Dictionary<int, List<int>> cdb)
        {
            foreach(var v in cdb)
            {
                Console.Write(v.Key + " - ");
                foreach(int fr in v.Value)
                {
                    if (fr == -1)
                        continue;
                    Console.Write(fr + " , ");
                }
                Console.WriteLine("");
            }
        }

        // Delete DataBase from c++ dll
        public void EraseMemmory()
        {
            if (init)
            {
                init = false;
                DeleteMap(ptr);
            }
        }

        // Get pointer to c++ map container
        public IntPtr GetPointer()
        {
            return ptr;
        }

        public void DeleteFriendship(int ThisId, int TargetId, ref Dictionary<int, List<int>> cdb)
        {
            if (!init)
                Init_DLL_DB(ref cdb);

            ptr = DllDeleteFriedship(ThisId, TargetId, this.GetPointer());
        }

        public void DeleteUser(int ThisId, ref Dictionary<int, List<int>> cdb)
        {
            if (!init)
                Init_DLL_DB(ref cdb);

            ptr = DllDeleteUser(ThisId, this.GetPointer());
        }

        public void NewFriendship(int ThisId, int TargetId, ref Dictionary<int, List<int>> cdb)
        {
            if (!init)
                Init_DLL_DB(ref cdb);

            ptr = AddNewFriendship(ThisId, TargetId, this.GetPointer());
        }

        // The shortest friedship 
        public List<int> FriendsWay(int ThisId, int TargetId, ref Dictionary<int, List<int>> cdb)
        {
            if (!init)
                Init_DLL_DB(ref cdb);

            IntPtr ptr = FriendsWay(ThisId, TargetId, this.GetPointer());

            // returning value
            int[] result = new int[20];
            Marshal.Copy(ptr, result, 0, 20);

            List<int> res = new List<int>();

            foreach (int val in result)
                if (val != -1)
                    res.Add(val);

            return res;
        }

        // DataBase
        public Dictionary<int, List<int>> db = new Dictionary<int, List<int>>();

        ~DllWorker()
        {
            EraseMemmory();
        }
    }
}
