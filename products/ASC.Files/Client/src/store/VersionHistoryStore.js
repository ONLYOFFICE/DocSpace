import { makeObservable, action, observable } from "mobx";
import { api } from "asc-web-common";

class VersionHistoryStore {
  isVisible = false;
  fileId = null;
  versions = null;

  constructor() {
    makeObservable(this, {
      isVisible: observable,
      fileId: observable,
      versions: observable,

      setIsVerHistoryPanel: action,
      setVerHistoryFileId: action,
      setVerHistoryFileVersions: action,
    });
  }

  setIsVerHistoryPanel = (isVisible) => {
    this.isVisible = isVisible;
  };

  setVerHistoryFileId = (fileId) => {
    this.fileId = fileId;
  };

  //setFileVersions
  setVerHistoryFileVersions = (versions) => {
    this.versions = versions;
  };

  fetchFileVersions = (fileId) => {
    if (this.fileId !== fileId) {
      this.setVerHistoryFileId(fileId);
      return api.files
        .getFileVersionInfo(fileId)
        .then((versions) => this.setVerHistoryFileVersions(versions));
    } else {
      return Promise.resolve(this.versions);
    }
  };
}

export default VersionHistoryStore;
