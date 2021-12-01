import { makeObservable } from "mobx";
import { presentInArray } from "../helpers/files-helpers";

class DocserviceStore {
  coauthorDocs = [
    ".csv",
    ".docx",
    ".docxf",
    ".oform",
    ".ppsx",
    ".pptx",
    ".txt",
    ".xlsx",
  ];
  commentedDocs = [".docx", ".docxf", ".xlsx", ".pptx"];
  convertDocs = [
    ".doc",
    ".docm",
    ".dot",
    ".dotm",
    ".dotx",
    ".fodp",
    ".fods",
    ".fodt",
    ".odp",
    ".ods",
    ".odt",
    ".otp",
    ".ots",
    ".ott",
    ".pot",
    ".potm",
    ".potx",
    ".pps",
    ".ppsm",
    ".ppt",
    ".pptm",
    ".rtf",
    ".xls",
    ".xlsm",
    ".xlt",
    ".xltm",
    ".xltx",
  ];
  editedDocs = [
    ".csv",
    ".doc",
    ".docm",
    ".docx",
    ".docxf",
    ".dot",
    ".dotm",
    ".dotx",
    ".fodp",
    ".fods",
    ".fodt",
    ".htm",
    ".html",
    ".mht",
    ".odp",
    ".ods",
    ".odt",
    ".oform",
    ".otp",
    ".ots",
    ".ott",
    ".pot",
    ".potm",
    ".potx",
    ".pps",
    ".ppsm",
    ".ppsx",
    ".ppt",
    ".pptm",
    ".pptx",
    ".rtf",
    ".txt",
    ".xls",
    ".xlsm",
    ".xlsx",
    ".xlt",
    ".xltm",
    ".xltx",
  ];
  encryptedDocs = [".docx", ".docxf", ".xlsx", ".pptx"];
  formfillingDocs = [".oform"];
  customfilterDocs = [".xlsx"];
  reviewedDocs = [".docx", ".docxf"];
  viewedDocs = [
    ".csv",
    ".djvu",
    ".doc",
    ".docm",
    ".docx",
    ".docxf",
    ".dot",
    ".dotm",
    ".dotx",
    ".epub",
    ".fodp",
    ".fods",
    ".fodt",
    ".gdoc",
    ".gsheet",
    ".gslides",
    ".htm",
    ".html",
    ".mht",
    ".odp",
    ".ods",
    ".odt",
    ".oform",
    ".otp",
    ".ots",
    ".ott",
    ".pdf",
    ".pot",
    ".potm",
    ".potx",
    ".pps",
    ".ppsm",
    ".ppsx",
    ".ppt",
    ".pptm",
    ".pptx",
    ".rtf",
    ".txt",
    ".xls",
    ".xlsm",
    ".xlsx",
    ".xlt",
    ".xltm",
    ".xltx",
    ".xps",
  ];

  filesConverts = [
    { ".csv": [".ods", ".pdf", ".xlsx"] },
    { ".doc": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".docm": [".docx", ".odt", ".pdf", ".rtf", ".txt"] },
    { ".doct": [".docx"] },
    { ".docx": [".docxf", ".odt", ".pdf", ".rtf", ".txt"] },
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
    {
      ".docxf": [
        ".docx",
        ".dotx",
        ".epub",
        ".fb2",
        ".html",
        ".odt",
        ".oform",
        ".ott",
        ".pdf",
        ".rtf",
        ".txt",
      ],
    },
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
