#include <iostream>
#include <vector>
#include <map>
#include <set>
#include <ctime>
#include <string>
//#include <thread>
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
	bool* flag, int depth = 1)
{
	if (depth < 1)
		return true;

	vector<int> fr = db.at(ThisId);
	for each (int f in fr)
	{
		if (f == TargetId)
		{
			vector<int> v = done.at(ThisId);
			v.push_back(f);
			done.insert(make_pair(f, v));
			*flag = true;
			return false;
		}
		else if (*flag)
			return false;
		else if (depth > 1)
		{
			_FriendWay(f, TargetId, db, done, flag, depth - 1);
		}
		else if (done.find(f) != done.end())
			continue;
		else
		{
			vector<int> v = done.at(ThisId);
			v.push_back(f);
			done.insert(make_pair(f, v));
			_FriendWay(f, TargetId, db, done, flag, depth - 1);
		}
	}
	return true;
}

vector<int> FriendWay(int ThisId, int TargetId, map<int, vector<int>>& db)
{
	map<int, vector<int>>  done;
	vector<int> path;
	path.push_back(ThisId);
	done.insert(make_pair(ThisId, path));

	int depth = 1;
	bool flag = false;
	while (_FriendWay(ThisId, TargetId, db, done, &flag, depth))
		depth++;
	cout << depth << endl;
	return (done.at(TargetId));
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

	__declspec(dllexport) map<int, vector<int>>* InitMap()
	{
		map<int, vector<int>> *db = new map<int, vector<int>>;
		return db;
	}

	__declspec(dllexport) map<int, vector<int>>* SendDB(_SDB* sdb, int Count, map<int, vector<int>>* db)
	{
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