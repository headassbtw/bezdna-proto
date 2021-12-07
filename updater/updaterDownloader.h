#ifndef UPDATERDOWNLOADER_H
#define UPDATERDOWNLOADER_H

#include <QObject>
#include <QByteArray>
#include <qt/QtNetwork/QNetworkAccessManager>
#include <qt/QtNetwork/QNetworkRequest>
#include <qt/QtNetwork/QNetworkReply>

class FileDownloader : public QObject
{
Q_OBJECT
public:
    QNetworkReply* reply;
    explicit FileDownloader(QUrl imageUrl, QObject *parent = 0);
    virtual ~FileDownloader();
    QByteArray downloadedData() const;
public slots:
    void downloadProg(qint64 ist, qint64 max);


signals:
    void downloaded();


private slots:
    void fileDownloaded(QNetworkReply* pReply);

    void loadImage();
private:
    QNetworkAccessManager m_WebCtrl;
    QByteArray m_DownloadedData;
};

#endif // UPDATERDOWNLOADER_H
