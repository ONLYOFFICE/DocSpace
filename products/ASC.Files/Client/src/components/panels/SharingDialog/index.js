import React, { useEffect } from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { getShareFiles } from "@appserver/common/api/files";
import SharingPanel from "../SharingPanel";

import FilesStore from "../../../store/FilesStore";
import UploadDataStore from "../../../store/UploadDataStore";
import dialogsStore from "../../../store/DialogsStore";
import TreeFoldersStore from "../../../store/TreeFoldersStore";
import store from "studio/store";

const { auth: authStore } = store;

const SharingDialog = ({
  sharingObject,
  onSuccess,
  isVisible,
  setSharingPanelVisible,
  onCancel,
  setSelection,
}) => {
  useEffect(() => {
    setSharingPanelVisible(isVisible);
  }, [isVisible]);

  useEffect(() => {
    setSelection([sharingObject]);
  }, []);

  return (
    <>
      {isVisible && (
        <SharingPanel
          key="sharing-panel"
          uploadPanelVisible={false}
          onSuccess={onSuccess}
          onCancel={onCancel}
        />
      )}
    </>
  );
};

const SharingDialogWrapper = inject(({ dialogsStore, filesStore }) => {
  const { getShareUsers, setSelection, selection } = filesStore;
  const { setSharingPanelVisible } = dialogsStore;
  return {
    setSharingPanelVisible,
    getShareUsers,
    setSelection,
    selection,
  };
})(observer(SharingDialog));

class SharingModal extends React.Component {
  static getSharingSettings = (fileId) => {
    return getShareFiles([+fileId], []).then((users) =>
      SharingPanel.convertSharingUsers(users)
    );
  };

  render() {
    return (
      <MobxProvider
        auth={authStore}
        filesStore={new FilesStore()}
        dialogsStore={dialogsStore}
        treeFoldersStore={new TreeFoldersStore()}
        uploadDataStore={new UploadDataStore()}
      >
        <SharingDialogWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SharingModal;
