import { makeAutoObservable, runInAction } from "mobx";
import api from "@docspace/common/api";
import { size } from "@docspace/components/utils/device";
import { FileStatus } from "@docspace/common/constants";

class VersionHistoryStore {
  isVisible = false;
  fileId = null;

  fileSecurity = null;
  versions = null;
  filesStore = null;
  showProgressBar = false;
  timerId = null;
  isEditing = false;

  constructor(filesStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;

    if (this.versions) {
      //TODO: Files store in not initialized on versionHistory page. Need socket.

      const { socketHelper } = this.filesStore.settingsStore;

      socketHelper.on("s:start-edit-file", (id) => {
        //console.log(`VERSION STORE Call s:start-edit-file (id=${id})`);
        const verIndex = this.versions.findIndex((x) => x.id == id);
        if (verIndex == -1) return;

        runInAction(() => (this.isEditing = true));
      });

      socketHelper.on("s:stop-edit-file", (id) => {
        //console.log(`VERSION STORE Call s:stop-edit-file (id=${id})`);
        const verIndex = this.files.findIndex((x) => x.id === id);
        if (verIndex == -1) return;

        runInAction(() => (this.isEditing = false));
      });
    }
  }

  get isEditingVersion() {
    if (this.fileId && this.filesStore.files.length) {
      const file = this.filesStore.files.find((x) => x.id === +this.fileId);
      return file
        ? (file.fileStatus & FileStatus.IsEditing) === FileStatus.IsEditing
        : false;
    }
    return false;
  }

  setIsVerHistoryPanel = (isVisible) => {
    this.isVisible = isVisible;

    if (!isVisible) {
      this.setVersions(null);
      this.setVerHistoryFileId(null);
    }
  };

  setVerHistoryFileId = (fileId) => {
    this.fileId = fileId;
  };

  setVerHistoryFileSecurity = (security) => {
    this.fileSecurity = security;
  };
  setVersions = (versions) => {
    this.versions = versions;
  };

  //setFileVersions
  setVerHistoryFileVersions = (versions) => {
    const file = this.filesStore.files.find((item) => item.id == this.fileId);

    const currentVersionGroup = Math.max.apply(
      null,
      versions.map((ver) => ver.versionGroup)
    );

    const currentComment =
      versions[versions.length - currentVersionGroup].comment;

    const newFile = {
      ...file,
      comment: currentComment,
      version: versions.length,
      versionGroup: currentVersionGroup,
    };

    this.filesStore.setFile(newFile);

    this.versions = versions;
  };

  fetchFileVersions = (fileId, access) => {
    if (this.fileId !== fileId || !this.versions) {
      this.setVerHistoryFileId(fileId);
      this.setVerHistoryFileSecurity(access);

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
    this.timerId = setTimeout(() => this.setShowProgressBar(true), 100);

    return api.files
      .versionRestore(id, version)
      .then((newVersion) => {
        const updatedVersions = this.versions.slice();
        updatedVersions.unshift(newVersion);
        this.setVerHistoryFileVersions(updatedVersions);
      })
      .catch((e) => console.error(e))
      .finally(() => {
        clearTimeout(this.timerId);
        this.timerId = null;
        this.setShowProgressBar(false);
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

  setShowProgressBar = (show) => {
    this.showProgressBar = show;
  };
}

export default VersionHistoryStore;
