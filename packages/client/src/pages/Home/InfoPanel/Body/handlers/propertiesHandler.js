const { FileType } = require("@docspace/common/constants");
const { TableText, TableLink } = require("./tableComponents");

class PropertiesHandler {
  constructor(t, item, getFolderIcon, getIcon, getCorrectDate) {
    this.t = t;
    this.item = item;
    this.getFolderIcon = getFolderIcon;
    this.getIcon = getIcon;
    this.getCorrectDate = getCorrectDate;
  }

  replaceUnicode(str) {
    const regex = /&#([0-9]{1,4});/gi;
    return str
      ? str.replace(regex, (match, numStr) => String.fromCharCode(+numStr))
      : "...";
  }

  getItemIcon(size) {
    return this.item.isFolder
      ? this.getFolderIcon(this.item.providerKey, size)
      : this.getIcon(size, this.item.fileExst || ".file");
  }

  getItemDate(date) {
    return getCorrectDate(localStorage.getItem(LANGUAGE), date);
  }

  getItemType = (fileType) => {
    switch (fileType) {
      case FileType.Unknown:
        return t("Common:Unknown");
      case FileType.Archive:
        return t("Common:Archive");
      case FileType.Video:
        return t("Common:Video");
      case FileType.Audio:
        return t("Common:Audio");
      case FileType.Image:
        return t("Common:Image");
      case FileType.Spreadsheet:
        return t("Home:Spreadsheet");
      case FileType.Presentation:
        return t("Home:Presentation");
      case FileType.Document:
        return t("Home:Document");
      default:
        return t("Home:Folder");
    }
  };

  //

  getItemProperties() {
    let result = [
      {
        id: "Owner",
        title: this.t("Common:Owner"),
        content: TableLink(
          this.t,
          this.replaceUnicode(this.item.createdBy?.displayName),
          this.item.createdBy?.profileUrl
        ),
      },
      // {
      //   id: "Location",
      //   title: t("InfoPanel:Location"),
      //   content: TableText(this.t, "..."),
      // },
      {
        id: "Type",
        title: this.t("Common:Type"),
        content: TableText(this.t, this.getItemType(this.item.fileType)),
      },
      {
        id: "Size",
        title: this.item.fileType
          ? this.t("Common:Size")
          : this.t("Common:Content"),
        content: TableText(this.t, itemSize),
      },
      {
        id: "ByLastModifiedDate",
        title: this.t("Home:ByLastModifiedDate"),
        content: TableText(this.t, this.getItemDate(this.item.updated)),
      },
      {
        id: "LastModifiedBy",
        title: this.t("LastModifiedBy"),
        content: personal
          ? TableText(
              this.t,
              this.replaceUnicode(this.item.updatedBy?.displayName)
            )
          : TableLink(
              this.t,
              this.replaceUnicode(this.item.updatedBy?.displayName),
              this.item.updatedBy?.profileUrl
            ),
      },
      {
        id: "ByCreationDate",
        title: this.t("Home:ByCreationDate"),
        content: TableText(this.t, this.getItemDate(this.item.created)),
      },
    ];

    if (this.item.providerKey && this.item.isFolder)
      result = result.filter((x) => x.id !== "Size");

    if (dontShowOwner) result.shift();
    if (this.item.isFolder) return result;

    result.splice(3, 0, {
      id: "FileExtension",
      title: t("FileExtension"),
      content: TableText(
        this.t,
        this.item.fileExst
          ? this.item.fileExst.split(".")[1].toUpperCase()
          : "-"
      ),
    });

    result.push(
      {
        id: "Versions",
        title: this.t("Versions"),
        content: TableText(this.t, this.item.version),
      },
      {
        id: "Comments",
        title: this.t("Common:Comments"),
        content: TableText(this.t, this.item.comment),
      }
    );

    return result;
  }
}

export default PropertiesHandler;
