import { makeObservable, action, observable } from "mobx";

class UploadDataStore {
  files = [];
  filesSize = 0;
  convertFiles = [];
  convertFilesSize = 0;
  uploadStatus = null;
  uploadToFolder = null;
  uploadedFiles = 0;
  percent = 0;
  uploaded = true;

  selectedUploadFile = [];

  constructor() {
    makeObservable(this, {
      files: observable,
      filesSize: observable,
      convertFiles: observable,
      convertFilesSize: observable,
      uploadStatus: observable,
      uploadToFolder: observable,
      uploadedFiles: observable,
      percent: observable,
      uploaded: observable,
      selectedUploadFile: observable,

      selectUploadedFile: action,
    });
  }

  /*export function setUploadData(uploadData) {
    return {
      type: SET_UPLOAD_DATA,
      uploadData,
    };
  }*/

  /*
  export function updateUploadedFile(id, info) {
  return {
    type: UPDATE_UPLOADED_FILE,
    id,
    info,
  };
}
  */

  selectUploadedFile = (file) => {
    this.selectedUploadFile = file;
  };
}

export default UploadDataStore;
