cmake_minimum_required(VERSION 3.20)
project(updater)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOUIC ON)


find_package(Qt5 COMPONENTS
        Core
        Gui
        Widgets
        Network
        REQUIRED)

add_executable(updater main.cpp updaterDownloader.cpp updaterDownloader.h paths.cpp paths.h unzip.cpp unzip.h)
target_link_libraries(updater
        zip
        pthread
        Qt5::Network
        Qt5::Core
        Qt5::Gui
        Qt5::Widgets
        )

