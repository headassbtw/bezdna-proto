//
// Created by headass on 12/7/21.
//

#ifndef UNZIP_H
#define UNZIP_H
class zipt{
public:
    static void safe_create_dir(const char *dir);
    static int unzip(char* archive);
};


#endif //UNZIP_H
