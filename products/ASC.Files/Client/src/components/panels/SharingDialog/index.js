import React, { useEffect } from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { getShareFiles } from "@appserver/common/api/files";
import SharingPanel from "../SharingPanel";

import initFilesStore from "../../../store/InitFilesStore";
import filesStore from "../../../store/FilesStore";
import uploadDataStore from "../../../store/UploadDataStore";
import dialogsStore from "../../../store/DialogsStore";
import treeFoldersStore from "../../../store/TreeFoldersStore";
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
        initFilesStore={initFilesStore}
        filesStore={filesStore}
        dialogsStore={dialogsStore}
        treeFoldersStore={treeFoldersStore}
        uploadDataStore={uploadDataStore}
      >
        <SharingDialogWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SharingModal;
