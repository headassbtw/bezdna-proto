#include <cstdio>
#include <iostream>
#include <fstream>
#include <QApplication>
#include <QPushButton>
#include <QList>
#include <QThread>
#include <QWindow>
#include <QProgressBar>
#include <QLabel>
#include <QFile>
#include <QHeaderView>
#include <chrono>
#include <qapplication.h>
#include <thread>
#include <zip.h>
#include <paths.h>
#include "updaterDownloader.h"
#include "paths.h"
#include "unzip.h"

QProgressBar* workBar;
FileDownloader *dl;
void barWork() {
    for(float i = 0; i < 101; i++){
        std::this_thread::sleep_for(std::chrono::milliseconds(500));
        workBar->setValue(i);
    }
};
void setup(){
    FileDownloader::connect(dl, SIGNAL (downloaded()), dl, SLOT (loadImage()));
}


int main(int argc, char *argv[]) {
    QApplication a(argc, argv);

    Paths::executable_path();

    char* dlVer = argv[1];

    auto* wid = new QWidget();
    wid->setBaseSize(400,300);
    auto* title = new QLabel(wid);
    title->setText("Updating...");

    QFont titleFont = QFont(title->font());
    titleFont.setPixelSize(40);
    title->setFont(titleFont);


    a.setActiveWindow(wid);



    workBar = new QProgressBar(wid);
    workBar->setFixedSize(400,20);
    workBar->move(0,50);
    workBar->setMaximum(100);
    workBar->setMinimum(0);
    workBar->show();
    wid->show();
    float i = 0;
    auto* qthr = QThread::create(barWork);
    //qthr->start();
    const char* verURL = "https://raw.githubusercontent.com/headassbtw/rpak2l/master/latestversion.txt";
    QString str = QString(verURL);



    std::string ln = "releases/download/0.1.0/";
    std::string url = "https://github.com/headassbtw/rpak2l/" + ln
          #ifdef Q_OS_WIN
            + "Windows"
          #endif
          #ifdef Q_OS_LINUX
            + "Linux"
          #endif
            + "_x64_Release.zip";

    QUrl imageUrl(url.c_str());
    dl = new FileDownloader(imageUrl, wid);
    std::cout << url << std::endl;
    auto* dlThread = QThread::create(setup);
    dlThread->start();

    return QApplication::exec();
}


void Unzip(){

}


void FileDownloader::downloadProg(qint64 ist, qint64 max) {
        workBar->setMaximum(max);
        workBar->setValue(ist);
        std::cout << ((float)ist / (float)max) << std::endl;
}


void FileDownloader::loadImage(){
    std::cout << "Downloaded, i think" << std::endl;
    QFile file("/tmp/update.zip");
    
    file.open(QIODevice::WriteOnly);
    QDataStream out(&file);
    //fun fact, don't do "file << data" because it writes 4 bullshit bytes at the beginning that doesn't let it get unzipped!
    out.writeRawData(dl->downloadedData(), dl->downloadedData().size());
    file.close();

    char* arc = "/tmp/update.zip";
    zipt::unzip(arc);

  #ifdef Q_OS_LINUX
  system("chmod +x ./RPAK2L"); //weird thing where linux won't mark it executable when unzipping??
  #endif
  remove(arc);
  QApplication::closeAllWindows();

}



