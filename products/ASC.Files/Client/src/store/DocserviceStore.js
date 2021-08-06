import { makeObservable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";

class DocserviceStore {
  coauthorDocs = [".pptx", ".ppsx", ".xlsx", ".csv", ".docx", ".txt"];
  commentedDocs = [".docx", ".xlsx", ".pptx"];
  convertDocs = [
    ".pptm",
    ".ppt",
    ".ppsm",
    ".pps",
    ".potx",
    ".potm",
    ".pot",
    ".odp",
    ".fodp",
    ".otp",
    ".xlsm",
    ".xls",
    ".xltx",
    ".xltm",
    ".xlt",
    ".ods",
    ".fods",
    ".ots",
    ".docm",
    ".doc",
    ".dotx",
    ".dotm",
    ".dot",
    ".odt",
    ".fodt",
    ".ott",
    ".rtf",
  ];
  editedDocs = [
    ".pptx",
    ".pptm",
    ".ppt",
    ".ppsx",
    ".ppsm",
    ".pps",
    ".potx",
    ".potm",
    ".pot",
    ".odp",
    ".fodp",
    ".otp",
    ".xlsx",
    ".xlsm",
    ".xls",
    ".xltx",
    ".xltm",
    ".xlt",
    ".ods",
    ".fods",
    ".ots",
    ".csv",
    ".docx",
    ".docm",
    ".doc",
    ".dotx",
    ".dotm",
    ".dot",
    ".odt",
    ".fodt",
    ".ott",
    ".txt",
    ".rtf",
    ".mht",
    ".html",
    ".htm",
  ];
  encryptedDocs = [".docx", ".xlsx", ".pptx"];
  formfillingDocs = [".docx"];
  customfilterDocs = [".xlsx"];
  reviewedDocs = [".docx"];
  viewedDocs = [
    ".pptx",
    ".pptm",
    ".ppt",
    ".ppsx",
    ".ppsm",
    ".pps",
    ".potx",
    ".potm",
    ".pot",
    ".odp",
    ".fodp",
    ".otp",
    ".gslides",
    ".xlsx",
    ".xlsm",
    ".xls",
    ".xltx",
    ".xltm",
    ".xlt",
    ".ods",
    ".fods",
    ".ots",
    ".gsheet",
    ".csv",
    ".docx",
    ".docm",
    ".doc",
    ".dotx",
    ".dotm",
    ".dot",
    ".odt",
    ".fodt",
    ".ott",
    ".gdoc",
    ".txt",
    ".rtf",
    ".mht",
    ".html",
    ".htm",
    ".epub",
    ".pdf",
    ".djvu",
    ".xps",
  ];

  constructor() {
    makeObservable(this, {});
  }

  canWebEdit = (extension) => presentInArray(this.editedDocs, extension);

  canViewedDocs = (extension) => presentInArray(this.viewedDocs, extension);

  canConvert = (extension) => presentInArray(this.convertDocs, extension);

  canWebComment = (extension) => presentInArray(this.commentedDocs, extension);

  canWebReview = (extension) => presentInArray(this.reviewedDocs, extension);

  canFormFillingDocs = (extension) =>
    presentInArray(this.formfillingDocs, extension);

  canWebFilterEditing = (extension) =>
    presentInArray(this.customfilterDocs, extension);
}
export default new DocserviceStore();
