import { makeObservable, action, observable } from "mobx";
import api from "@appserver/common/api";

class VersionHistoryStore {
  isVisible = false;
  fileId = null;
  versions = null;
  filesStore = null;
  showProgressBar = false;

  constructor(filesStore) {
    makeObservable(this, {
      isVisible: observable,
      fileId: observable,
      versions: observable,
      showProgressBar: observable,

      setIsVerHistoryPanel: action,
      setVerHistoryFileId: action,
      setVerHistoryFileVersions: action,
      markAsVersion: action,
      restoreVersion: action,
      updateCommentVersion: action,
    });
    this.filesStore = filesStore;
  }

  setIsVerHistoryPanel = (isVisible) => {
    this.isVisible = isVisible;
    !isVisible && this.setVerHistoryFileId(null);
  };

  setVerHistoryFileId = (fileId) => {
    this.fileId = fileId;
  };

  //setFileVersions
  setVerHistoryFileVersions = (versions) => {
    const file = this.filesStore.files.find((item) => item.id == this.fileId);

    const currentVersionGroup = Math.max.apply(
      null,
      versions.map((ver) => ver.versionGroup)
    );
    const isVerHistoryPanel = this.isVisible;

    if (
      isVerHistoryPanel &&
      (versions.length !== file.version ||
        currentVersionGroup !== file.versionGroup)
    ) {
      const newFile = {
        ...file,
        version: versions.length,
        versionGroup: currentVersionGroup,
      };

      this.filesStore.setFile(newFile);
    }

    this.versions = versions;
  };

  fetchFileVersions = (fileId) => {
    if (this.fileId !== fileId || !this.versions) {
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
    this.setShowProgressBar(true);

    return api.files
      .versionRestore(id, version)
      .then((newVersion) => {
        const updatedVersions = this.versions.slice();
        updatedVersions.splice(1, 0, newVersion);
        this.setVerHistoryFileVersions(updatedVersions);
        this.setShowProgressBar(false);
      })
      .catch(() => this.setShowProgressBar(false));
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

  setShowProgressBar = (show) => {
    this.showProgressBar = show;
  };
}

export default VersionHistoryStore;
