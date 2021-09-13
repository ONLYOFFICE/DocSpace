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

  filesConverts = [
    { ".csv": [".ods", ".pdf", ".xlsx"] },
    { ".doc": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".docm": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".doct": [".docx"] },
    { ".docx": [".odt", ".pdf", ".rtf", ".txt"] },
    { ".dot": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".dotm": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".dotx": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".epub": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".fb2": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".fodp": [".odp", ".pdf", ".pptx"] },
    { ".fods": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".fodt": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".html": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".mht": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".odp": [".pdf", ".pptx"] },
    { ".otp": [".odp", ".pdf", ".pptx"] },
    { ".ods": [".csv", ".pdf", ".xlsx"] },
    { ".ots": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".odt": [".docx", ".pdf", ".rtf", ".txt"] },
    { ".ott": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".pot": [".odp", ".pdf", ".pptx"] },
    { ".potm": [".odp", ".pdf", ".pptx"] },
    { ".potx": [".odp", ".pdf", ".pptx"] },
    { ".pps": [".odp", ".pdf", ".pptx"] },
    { ".ppsm": [".odp", ".pdf", ".pptx"] },
    { ".ppsx": [".odp", ".pdf", ".pptx"] },
    { ".ppt": [".odp", ".pdf", ".pptx"] },
    { ".pptm": [".odp", ".pdf", ".pptx"] },
    { ".pptt": [".pptx"] },
    { ".pptx": [".odp", ".pdf"] },
    { ".rtf": [".docx", ".odt", ".pdf", ".txt"] },
    { ".txt": [".docx", ".odt", ".pdf", ".rtf"] },
    { ".xls": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".xlsm": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".xlst": [".xlsx"] },
    { ".xlsx": [".csv", ".ods", ".pdf"] },
    { ".xlt": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".xltm": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".xltx": [".csv", ".ods", ".pdf", ".xlsx"] },
    { ".xps": [".pdf"] },
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
