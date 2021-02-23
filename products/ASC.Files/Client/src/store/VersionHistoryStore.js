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
      markAsVersion: action,
      restoreVersion: action,
      updateCommentVersion: action,
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

  markAsVersion = (id, isVersion, version) => {
    return api.files
      .markAsVersion(id, isVersion, version)
      .then((versions) => this.setVerHistoryFileVersions(versions));
  };

  restoreVersion = (id, version) => {
    return api.files.versionRestore(id, version).then((newVersion) => {
      const updatedVersions = this.versions.slice();
      updatedVersions.splice(1, 0, newVersion);
      this.setVerHistoryFileVersions(updatedVersions);
    });
  };

  updateCommentVersion = (id, comment, version) => {
    return api.files
      .versionEditComment(id, comment, version)
      .then((updatedComment) => {
        const copyVersions = this.versions.slice();
        const updatedVersions = copyVersions.map((item) => {
          if (item.version === version) {
            item.comment = updatedComment;
          }
          return item;
        });
        this.setVerHistoryFileVersions(updatedVersions);
      });
  };
}

export default new VersionHistoryStore();
