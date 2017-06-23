#include <iostream>
#include <vector>
#include <map>
#include <set>
#include <algorithm>
#include <ctime>
#include <string>
#include <thread>
#include <ctime>

#define MAX_Friends 255
#define MAX_Send 50

using namespace std;

struct SDB
{
	int key;
	int lengthVal;
	int value[MAX_Friends];
};

struct _SDB
{
	SDB sdb[MAX_Send];
};

void ShowDB(map<int, vector<int>>& db)
{
	for each (auto var in db)
	{
		cout << var.first << " - ";
		for each (int fr in var.second)
		{
			cout << fr << " , ";
		}
		cout << endl;
	}
}

void Str2Map(_SDB& _sdb, int Count, map<int, vector<int>>* db)
{
	for (int i = 0; i < Count; i++)
	{
		vector<int> v;

		for (int j = 0; j < _sdb.sdb[i].lengthVal; j++)
		{
			v.push_back(_sdb.sdb[i].value[j]);
		}

		db->insert(make_pair(_sdb.sdb[i].key, v));
	}
}

bool _FriendWay(int ThisId, int TargetId,
	map<int, vector<int>>& db,
	map<int, vector<int>>&  done,
	map<int, vector<int>>&  done2,
	bool* flag, int depth = 1)
{
	// recursion end
	if (depth < 1)
		return true;

	vector<int> fr = db.at(ThisId);
	for each (int f in fr)
	{
		// found
		if (f == TargetId)
		{
			vector<int> v = done.at(ThisId);
			v.push_back(f);
			done[-2].clear();
			done2[-2].clear();
			done[-2].insert(end(done[-2]), begin(v), end(v));
			done2[-2].insert(end(done2[-2]), begin(v), end(v));
			*flag = true;
			return false;
		}
	}
	for each (int f in fr)
	{
		// found
		if (f == TargetId)
		{
			vector<int> v = done.at(ThisId);
			v.push_back(f);
			done[-2].clear();
			done2[-2].clear();
			done[-2].insert(end(done[-2]), begin(v), end(v));
			done2[-2].insert(end(done2[-2]), begin(v), end(v));
			*flag = true;
			return false;
		}

		// some errors
		else if (depth > 15 || TargetId > db.size() || ThisId > db.size() || TargetId < 0 || ThisId < 0)
		{
			vector<int> v;
			v.push_back(-1);
			done[-2].clear();
			done2[-2].clear();
			done[-2].insert(end(done[-2]), begin(v), end(v));
			done2[-2].insert(end(done2[-2]), begin(v), end(v));
			*flag = true;
			string str = "depth = " + depth;
			str += "; database size = " + db.size();
			str += "; TargetId = " + TargetId;
			str += "; ThisId = " + ThisId;
			throw(new exception(str.c_str()));
			return false;
		}

		// if this element was found in antother thread
		else if ((done.find(f) != done.end()) && (done2.find(f) != done2.end()))
		{
			vector<int> v;
			v.insert(end(v), begin(done[f]), end(done[f]));
			v.insert(end(v), done2[f].rbegin() + 1, done2[f].rend()); // inverse data from another thread
			done[-2].clear();
			done2[-2].clear();
			done[-2].insert(end(done[-2]), begin(v), end(v));
			done2[-2].insert(end(done2[-2]), begin(v), end(v));
			*flag = true;
			return false;
		}

		// return from recursion
		else if (*flag)
			return false;

		// go to recursion at previous elements
		else if (depth > 1)
		{
			_FriendWay(f, TargetId, db, done, done2, flag, depth - 1);
		}

		// if this element already added
		else if (done.find(f) != done.end())
			continue;

		// go to recursion at this element
		else
		{
			vector<int> v = done.at(ThisId);
			v.push_back(f);
			done.insert(make_pair(f, v));
			_FriendWay(f, TargetId, db, done, done2, flag, depth - 1);
		}
	}
	return true;
}

// first thread
map<int, vector<int>> foo(int ThisId, int TargetId,
	map<int, vector<int>>& db,
	map<int, vector<int>>* done1,
	map<int, vector<int>>* done2)
{
	int depth = 1;
	bool flag = false;
	try
	{
		while (_FriendWay(ThisId, TargetId, db, *done1, *done2, &flag, depth))
			depth++;
	}
	catch (char* ch)
	{
		// TODO
	}

	return *done1;
}

vector<int> FriendWay(int ThisId, int TargetId, map<int, vector<int>>& db)
{
	// creat
	map<int, vector<int>>  done1;
	map<int, vector<int>>  done2;
	vector<int> path1;
	vector<int> path2;

	// initialize
	path1.push_back(ThisId);
	path2.push_back(TargetId);
	done1.insert(make_pair(ThisId, path1));
	done2.insert(make_pair(TargetId, path2));
	done1.insert(make_pair(-2, path1));
	done2.insert(make_pair(-2, path1));

	// start functions
	thread th1(foo, ThisId, TargetId, db, &done1, &done2);
	thread th2(foo, TargetId, ThisId, db, &done2, &done1);
	th1.join();
	th2.join();

	// inverse result if it need
	if (done1.at(-2).at(0) == ThisId)
		path1.insert(end(path1), begin(done1.at(-2)), end(done1.at(-2)));
	if (done1.at(-2).at(0) == TargetId)
		path1.insert(end(path1), done1.at(-2).rbegin(), done1.at(-2).rend());

	// every element only 1 time in vector
	vector<int> res;
	res.push_back(path1[0]);
	for each (int var in path1)
	{
		if (res[res.size() - 1] != var)
			res.push_back(var);
	}

	return res;
}

extern "C"
{
	__declspec(dllexport) int* FriendsWay(int ThisId, int TargetId, map<int, vector<int>>* db)
	{
		auto vec = FriendWay(ThisId, TargetId, *db);

		int* res = new int[20];

		for (int i = 0; i < 20; i++)
		{
			if (i < vec.size())
				res[i] = vec[i];
			else res[i] = -1;
		}

		return res;
	}

	__declspec(dllexport) map<int, vector<int>>* DllDeleteFriedship(int ThisId, int TargetId, map<int, vector<int>>* db)
	{
		if ((db->find(ThisId) == db->end()) || (db->find(TargetId) == db->end()))
		{
			return db;
		}
		if ((find(db->at(TargetId).begin(), db->at(TargetId).end(), ThisId) == db->at(TargetId).end()) 
			&& (find(db->at(ThisId).begin(), db->at(ThisId).end(), TargetId) == db->at(ThisId).end()))
		{
			return db;
		}
		db->at(ThisId).erase(std::remove(db->at(ThisId).begin(), db->at(ThisId).end(), TargetId), db->at(ThisId).end());
		db->at(TargetId).erase(std::remove(db->at(TargetId).begin(), db->at(TargetId).end(), ThisId), db->at(TargetId).end());
		
		return db;
	}

	__declspec(dllexport) map<int, vector<int>>* DllDeleteUser(int ThisId, map<int, vector<int>>* db)
	{
		if (db->find(ThisId) == db->end())
		{
			return db;
		}
		while (db->at(ThisId).size() > 0)
		{
			db = DllDeleteFriedship(ThisId, db->at(ThisId).at(0), db);
		}

		db->erase(ThisId);

		return db;
	}

	__declspec(dllexport) map<int, vector<int>>* AddNewFriendship(int ThisId, int TargetId, map<int, vector<int>>* db)
	{
		if ((db->find(ThisId) == db->end()) || (db->find(TargetId) == db->end()))
		{
			return db;
		}

		if ((find(db->at(TargetId).begin(), db->at(TargetId).end(), ThisId) == db->at(TargetId).end())
			&& (find(db->at(ThisId).begin(), db->at(ThisId).end(), TargetId) == db->at(ThisId).end()))
		{
			db->at(TargetId).insert(db->at(TargetId).begin(), ThisId);
			db->at(ThisId).insert(db->at(ThisId).begin(), TargetId);
		}
		
		

		return db;
	}

	__declspec(dllexport) map<int, vector<int>>* InitMap()
	{
		map<int, vector<int>> *db = new map<int, vector<int>>;
		return db;
	}

	__declspec(dllexport) map<int, vector<int>>* SendDB(_SDB* sdb, int Count, map<int, vector<int>>* db)
	{
		if (db == 0)
		{
			db = new map<int, vector<int>>;
		}

		Str2Map(*sdb, Count, db);

		return db;
	}

	__declspec(dllexport) void Show_DB(map<int, vector<int>>* db)
	{
		ShowDB(*db);
	}

	__declspec(dllexport) void DeleteMap(map<int, vector<int>>* db)
	{
		delete db;
	}
}