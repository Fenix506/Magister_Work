#pragma once
#include <list>
using namespace std;

class Block {
public:
	list<unsigned char> Data;
	double Entrory;
	Block() {
		
	}
	~Block()
	{
		Data.clear();
	}
	void Add(unsigned char data) {
		Data.push_back(data);
	}
	
};