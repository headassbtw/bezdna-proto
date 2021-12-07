//
// Created by headass on 12/7/21.
//

#include "updaterDownloader.h"
#include <string>
#include <QObject>
#include <QByteArray>
#include <qt/QtNetwork/QNetworkAccessManager>
#include <qt/QtNetwork/QNetworkRequest>
#include <qt/QtNetwork/QNetworkReply>

FileDownloader::FileDownloader(QUrl imageUrl, QObject *parent) :
        QObject(parent)
{
    m_WebCtrl.setRedirectPolicy(QNetworkRequest::NoLessSafeRedirectPolicy);
    connect(
            &m_WebCtrl, SIGNAL (finished(QNetworkReply*)),
            this, SLOT (fileDownloaded(QNetworkReply*))
    );

    QNetworkRequest request(imageUrl);

    QNetworkReply* reply = m_WebCtrl.get(request);
    connect(reply, &QNetworkReply::downloadProgress, this, &FileDownloader::downloadProg);
}

FileDownloader::~FileDownloader() { }

void FileDownloader::fileDownloaded(QNetworkReply* pReply) {
    m_DownloadedData = pReply->readAll();
    //emit a signal
    pReply->deleteLater();
    emit downloaded();
}

QByteArray FileDownloader::downloadedData() const {
    return m_DownloadedData;
}


