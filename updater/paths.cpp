//
// Created by headass on 12/7/21.
//

#include "paths.h"
#include <qglobal.h>
#include <string>
#include <filesystem>
#include <iostream>
#ifdef Q_OS_WIN
#include <windows.h>
#endif
#ifdef Q_OS_LINUX
#include <limits.h>
#include <unistd.h>
#endif



std::string Paths::temp_path(){
    return std::filesystem::temp_directory_path();
}
std::string Paths::executable_path(){
#ifdef Q_OS_LINUX
    char result[ PATH_MAX ];
    ssize_t count = readlink( "/proc/self/exe", result, PATH_MAX );
    std::string res = std::string(result);
    std::string::size_type loc = res.find_last_of( "/", 64);
    return std::string( res.substr(0,loc), (loc > 0) ? loc : 0 );
#endif
#ifdef Q_OS_WIN
    char result[ MAX_PATH ];
    return std::string( result, GetModuleFileName( NULL, result, MAX_PATH ) );
#endif
}