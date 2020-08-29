#pragma once
#include<string>
#include<list>
#include "Block.cpp"


using namespace std;
class FileBlocker
{
public:
	char* FullFileName;
	double BlockEntropy;
	double FileEntropy;
	list<Block> Blocks;
	FileBlocker(char* fullFileName, double blockEntropy = 4)
	{
		FullFileName = fullFileName;
		BlockEntropy = blockEntropy;
	}
	void Blocking() {
		unsigned int fullCountBytes[256];
		unsigned char blockCountBytes[256];

		//FILE* ptrFile = fopen(FullFileName, "rb");
		char buffer[512];

		//fread(buffer, sizeof(char), 512, ptrFile);
		//fread(buffer, sizeof(char), 512, ptrFile);
	//	fclose(ptrFile);

	}
};