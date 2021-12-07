/****************************************************************************
** Meta object code from reading C++ file 'updaterDownloader.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.15.2)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include <memory>
#include "../../../updaterDownloader.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'updaterDownloader.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.15.2. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
QT_WARNING_PUSH
QT_WARNING_DISABLE_DEPRECATED
struct qt_meta_stringdata_FileDownloader_t {
    QByteArrayData data[10];
    char stringdata0[95];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_FileDownloader_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_FileDownloader_t qt_meta_stringdata_FileDownloader = {
    {
QT_MOC_LITERAL(0, 0, 14), // "FileDownloader"
QT_MOC_LITERAL(1, 15, 10), // "downloaded"
QT_MOC_LITERAL(2, 26, 0), // ""
QT_MOC_LITERAL(3, 27, 12), // "downloadProg"
QT_MOC_LITERAL(4, 40, 3), // "ist"
QT_MOC_LITERAL(5, 44, 3), // "max"
QT_MOC_LITERAL(6, 48, 14), // "fileDownloaded"
QT_MOC_LITERAL(7, 63, 14), // "QNetworkReply*"
QT_MOC_LITERAL(8, 78, 6), // "pReply"
QT_MOC_LITERAL(9, 85, 9) // "loadImage"

    },
    "FileDownloader\0downloaded\0\0downloadProg\0"
    "ist\0max\0fileDownloaded\0QNetworkReply*\0"
    "pReply\0loadImage"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_FileDownloader[] = {

 // content:
       8,       // revision
       0,       // classname
       0,    0, // classinfo
       4,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       1,       // signalCount

 // signals: name, argc, parameters, tag, flags
       1,    0,   34,    2, 0x06 /* Public */,

 // slots: name, argc, parameters, tag, flags
       3,    2,   35,    2, 0x0a /* Public */,
       6,    1,   40,    2, 0x08 /* Private */,
       9,    0,   43,    2, 0x08 /* Private */,

 // signals: parameters
    QMetaType::Void,

 // slots: parameters
    QMetaType::Void, QMetaType::LongLong, QMetaType::LongLong,    4,    5,
    QMetaType::Void, 0x80000000 | 7,    8,
    QMetaType::Void,

       0        // eod
};

void FileDownloader::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        auto *_t = static_cast<FileDownloader *>(_o);
        (void)_t;
        switch (_id) {
        case 0: _t->downloaded(); break;
        case 1: _t->downloadProg((*reinterpret_cast< qint64(*)>(_a[1])),(*reinterpret_cast< qint64(*)>(_a[2]))); break;
        case 2: _t->fileDownloaded((*reinterpret_cast< QNetworkReply*(*)>(_a[1]))); break;
        case 3: _t->loadImage(); break;
        default: ;
        }
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        switch (_id) {
        default: *reinterpret_cast<int*>(_a[0]) = -1; break;
        case 2:
            switch (*reinterpret_cast<int*>(_a[1])) {
            default: *reinterpret_cast<int*>(_a[0]) = -1; break;
            case 0:
                *reinterpret_cast<int*>(_a[0]) = qRegisterMetaType< QNetworkReply* >(); break;
            }
            break;
        }
    } else if (_c == QMetaObject::IndexOfMethod) {
        int *result = reinterpret_cast<int *>(_a[0]);
        {
            using _t = void (FileDownloader::*)();
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&FileDownloader::downloaded)) {
                *result = 0;
                return;
            }
        }
    }
}

QT_INIT_METAOBJECT const QMetaObject FileDownloader::staticMetaObject = { {
    QMetaObject::SuperData::link<QObject::staticMetaObject>(),
    qt_meta_stringdata_FileDownloader.data,
    qt_meta_data_FileDownloader,
    qt_static_metacall,
    nullptr,
    nullptr
} };


const QMetaObject *FileDownloader::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *FileDownloader::qt_metacast(const char *_clname)
{
    if (!_clname) return nullptr;
    if (!strcmp(_clname, qt_meta_stringdata_FileDownloader.stringdata0))
        return static_cast<void*>(this);
    return QObject::qt_metacast(_clname);
}

int FileDownloader::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QObject::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 4)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 4;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 4)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 4;
    }
    return _id;
}

// SIGNAL 0
void FileDownloader::downloaded()
{
    QMetaObject::activate(this, &staticMetaObject, 0, nullptr);
}
QT_WARNING_POP
QT_END_MOC_NAMESPACE
