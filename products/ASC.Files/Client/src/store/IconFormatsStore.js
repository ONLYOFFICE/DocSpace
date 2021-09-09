import { makeAutoObservable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";

class IconFormatsStore {
  archive = [
    ".ace",
    ".arc",
    ".arj",
    ".bh",
    ".cab",
    ".enc",
    ".gz",
    ".gz",
    ".ha",
    ".jar",
    ".lha",
    ".lzh",
    ".pak",
    ".pk3",
    ".rar",
    ".tar",
    ".tgz",
    ".uu",
    ".uue",
    ".xxe",
    ".z",
    ".zip",
    ".zoo",
  ];
  image = [
    ".ai",
    ".bmp",
    ".cmx",
    ".cod",
    ".gif",
    ".ico",
    ".ief",
    ".jpe",
    ".jpeg",
    ".jpg",
    ".pbm",
    ".png",
    ".pnm",
    ".ppm",
    ".psd",
    ".rgb",
    ".tif",
    ".tiff",
    ".webp",
    ".xbm",
    ".xpm",
    ".xwd",
  ];
  sound = [
    ".aac",
    ".ac3",
    ".aiff",
    ".amr",
    ".ape",
    ".cda",
    ".flac",
    ".m4a",
    ".mid",
    ".mka",
    ".mp3",
    ".mpc",
    ".oga",
    ".ogg",
    ".pcm",
    ".ra",
    ".raw",
    ".wav",
    ".wma",
  ];
  video = [
    ".3gp",
    ".asf",
    ".avi",
    ".f4v",
    ".fla",
    ".flv",
    ".m2ts",
    ".m4v",
    ".mkv",
    ".mov",
    ".mp4",
    ".mpeg",
    ".mpg",
    ".mts",
    ".ogv",
    ".svi",
    ".vob",
    ".webm",
    ".wmv",
  ];
  html = [".htm", ".mht", ".html"];
  ebook = [".fb2", ".ibk", ".prc", ".epub"];
  document = [
    ".doc",
    ".docx",
    ".docm",
    ".dot",
    ".dotx",
    ".dotm",
    ".odt",
    ".fodt",
    ".ott",
    ".rtf",
    ".txt",
    ".html",
    ".htm",
    ".mht",
    ".pdf",
    ".djvu",
    ".fb2",
    ".epub",
    ".xps",
    ".doct",
    ".docy",
    ".gdoc",
  ];
  presentation = [
    ".pps",
    ".ppsx",
    ".ppsm",
    ".ppt",
    ".pptx",
    ".pptm",
    ".pot",
    ".potx",
    ".potm",
    ".odp",
    ".fodp",
    ".otp",
    ".pptt",
    ".ppty",
    ".gslides",
  ];
  spreadsheet = [
    ".xls",
    ".xlsx",
    ".xlsm",
    ".xlt",
    ".xltx",
    ".xltm",
    ".ods",
    ".fods",
    ".ots",
    ".csv",
    ".xlst",
    ".xlsy",
    ".xlsb",
    ".gsheet",
  ];

  constructor() {
    makeAutoObservable(this);
    //makeObservable(this, {});
  }

  isArchive = (extension) => presentInArray(this.archive, extension);

  isImage = (extension) => presentInArray(this.image, extension);

  isSound = (extension) => presentInArray(this.sound, extension);

  isHtml = (extension) => presentInArray(this.html, extension);

  isEbook = (extension) => presentInArray(this.ebook, extension);

  isDocument = (extension) => presentInArray(this.document, extension);

  isPresentation = (extension) => presentInArray(this.presentation, extension);

  isSpreadsheet = (extension) => presentInArray(this.spreadsheet, extension);

  getIcon = (
    size = 24,
    fileExst = null,
    providerKey = null,
    contentLength = null
  ) => {
    if (fileExst || contentLength) {
      const isArchiveItem = this.isArchive(fileExst);
      const isImageItem = this.isImage(fileExst);
      const isSoundItem = this.isSound(fileExst);
      const isHtmlItem = this.isHtml(fileExst);

      const icon = this.getFileIcon(
        fileExst,
        size,
        isArchiveItem,
        isImageItem,
        isSoundItem,
        isHtmlItem
      );
      return icon;
    } else {
      return this.getFolderIcon(providerKey, size);
    }
  };

  getFolderIcon = (providerKey, size = 32) => {
    const folderPath = `images/icons/${size}`;

    switch (providerKey) {
      case "Box":
      case "BoxNet":
        return `${folderPath}/folder/box.svg`;
      case "DropBox":
      case "DropboxV2":
        return `${folderPath}/folder/dropbox.svg`;
      case "Google":
      case "GoogleDrive":
        return `${folderPath}/folder/google.svg`;
      case "OneDrive":
        return `${folderPath}/folder/onedrive.svg`;
      case "SharePoint":
        return `${folderPath}/folder/sharepoint.svg`;
      case "Yandex":
        return `${folderPath}/folder/yandex.svg`;
      case "kDrive":
        return `${folderPath}/folder/kdrive.svg`;
      case "WebDav":
        return `${folderPath}/folder/webdav.svg`;
      default:
        return `${folderPath}/folder.svg`;
    }
  };

  getFileIcon = (
    extension,
    size = 32,
    archive = false,
    image = false,
    sound = false,
    html = false
  ) => {
    const folderPath = `/static/images/icons/${size}`;

    if (archive) return `${folderPath}/file_archive.svg`;

    if (image) return `${folderPath}/image.svg`;

    if (sound) return `${folderPath}/sound.svg`;

    if (html) return `${folderPath}/html.svg`;

    switch (extension) {
      case ".avi":
        return `${folderPath}/avi.svg`;
      case ".csv":
        return `${folderPath}/csv.svg`;
      case ".djvu":
        return `${folderPath}/djvu.svg`;
      case ".doc":
        return `${folderPath}/doc.svg`;
      case ".docx":
        return `${folderPath}/docx.svg`;
      case ".dotx":
        return `${folderPath}/dotx.svg`;
      case ".dvd":
        return `${folderPath}/dvd.svg`;
      case ".epub":
        return `${folderPath}/epub.svg`;
      case ".pb2":
        return `${folderPath}/fb2.svg`;
      case ".fb2":
        return `${folderPath}/fb2.svg`;
      case ".flv":
        return `${folderPath}/flv.svg`;
      case ".iaf":
        return `${folderPath}/iaf.svg`;
      case ".m2ts":
        return `${folderPath}/m2ts.svg`;
      case ".mht":
        return `${folderPath}/mht.svg`;
      case ".mkv":
        return `${folderPath}/mkv.svg`;
      case ".mov":
        return `${folderPath}/mov.svg`;
      case ".mp4":
        return `${folderPath}/mp4.svg`;
      case ".mpg":
        return `${folderPath}/mpg.svg`;
      case ".odp":
        return `${folderPath}/odp.svg`;
      case ".ods":
        return `${folderPath}/ods.svg`;
      case ".odt":
        return `${folderPath}/odt.svg`;
      case ".otp":
        return `${folderPath}/otp.svg`;
      case ".ots":
        return `${folderPath}/ots.svg`;
      case ".ott":
        return `${folderPath}/ott.svg`;
      case ".pdf":
        return `${folderPath}/pdf.svg`;
      case ".pot":
        return `${folderPath}/pot.svg`;
      case ".pps":
        return `${folderPath}/pps.svg`;
      case ".ppsx":
        return `${folderPath}/ppsx.svg`;
      case ".ppt":
        return `${folderPath}/ppt.svg`;
      case ".pptm":
        return `${folderPath}/pptm.svg`;
      case ".pptx":
        return `${folderPath}/pptx.svg`;
      case ".rtf":
        return `${folderPath}/rtf.svg`;
      case ".svg":
        return `${folderPath}/svg.svg`;
      case ".txt":
        return `${folderPath}/txt.svg`;
      case ".webm":
        return `${folderPath}/webm.svg`;
      case ".xls":
        return `${folderPath}/xls.svg`;
      case ".xlsx":
        return `${folderPath}/xlsx.svg`;
      case ".xps":
        return `${folderPath}/xps.svg`;
      case ".xml":
        return `${folderPath}/xml.svg`;
      default:
        return `${folderPath}/file.svg`;
    }
  };

  getIconSrc = (ext, size = 24) => {
    const folderPath = `/static/images/icons/${size}`;

    if (presentInArray(this.archive, ext, true))
      return `${folderPath}/file_archive.svg`;

    if (presentInArray(this.image, ext, true)) return `${folderPath}/image.svg`;

    if (presentInArray(this.sound, ext, true)) return `${folderPath}/sound.svg`;

    if (presentInArray(this.html, ext, true)) return `${folderPath}/html.svg`;

    const extension = ext.toLowerCase();

    switch (extension) {
      case ".avi":
        return `${folderPath}/avi.svg`;
      case ".csv":
        return `${folderPath}/csv.svg`;
      case ".djvu":
        return `${folderPath}/djvu.svg`;
      case ".doc":
        return `${folderPath}/doc.svg`;
      case ".docx":
        return `${folderPath}/docx.svg`;
      case ".dotx":
        return `${folderPath}/dotx.svg`;
      case ".dvd":
        return `${folderPath}/dvd.svg`;
      case ".epub":
        return `${folderPath}/epub.svg`;
      case ".pb2":
        return `${folderPath}/fb2.svg`;
      case ".fb2":
        return `${folderPath}/fb2.svg`;
      case ".flv":
        return `${folderPath}/flv.svg`;
      case ".iaf":
        return `${folderPath}/iaf.svg`;
      case ".m2ts":
        return `${folderPath}/m2ts.svg`;
      case ".mht":
        return `${folderPath}/mht.svg`;
      case ".mkv":
        return `${folderPath}/mkv.svg`;
      case ".mov":
        return `${folderPath}/mov.svg`;
      case ".mp4":
        return `${folderPath}/mp4.svg`;
      case ".mpg":
        return `${folderPath}/mpg.svg`;
      case ".odp":
        return `${folderPath}/odp.svg`;
      case ".ods":
        return `${folderPath}/ods.svg`;
      case ".odt":
        return `${folderPath}/odt.svg`;
      case ".otp":
        return `${folderPath}/otp.svg`;
      case ".ots":
        return `${folderPath}/ots.svg`;
      case ".ott":
        return `${folderPath}/ott.svg`;
      case ".pdf":
        return `${folderPath}/pdf.svg`;
      case ".pot":
        return `${folderPath}/pot.svg`;
      case ".pps":
        return `${folderPath}/pps.svg`;
      case ".ppsx":
        return `${folderPath}/ppsx.svg`;
      case ".ppt":
        return `${folderPath}/ppt.svg`;
      case ".pptx":
        return `${folderPath}/pptx.svg`;
      case ".pptm":
        return `${folderPath}/pptm.svg`;
      case ".rtf":
        return `${folderPath}/rtf.svg`;
      case ".svg":
        return `${folderPath}/svg.svg`;
      case ".txt":
        return `${folderPath}/txt.svg`;
      case ".webm":
        return `${folderPath}/webm.svg`;
      case ".xls":
        return `${folderPath}/xls.svg`;
      case ".xlsx":
        return `${folderPath}/xlsx.svg`;
      case ".xps":
        return `${folderPath}/xps.svg`;
      case ".xml":
        return `${folderPath}/xml.svg`;
      default:
        return `${folderPath}/file.svg`;
    }
  };
}

export default new IconFormatsStore();
