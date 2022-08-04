import React, { useEffect } from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import { getShareFiles } from "@docspace/common/api/files";
import SharingPanel from "../SharingPanel";
import store from "SRC_DIR/store";
const { auth: authStore } = store;

const SharingDialog = ({
  sharingObject,
  onSuccess,
  isVisible,
  setSharingPanelVisible,
  onCancel,
  setSelection,
  theme,
  sharingPanelVisible,
  settings,
}) => {
  useEffect(() => {
    setSharingPanelVisible(isVisible);
  }, [isVisible]);

  useEffect(() => {
    setSelection([sharingObject]);
  }, []);

  return (
    <>
      {sharingPanelVisible && (
        <SharingPanel
          key="sharing-panel"
          uploadPanelVisible={false}
          onSuccess={onSuccess}
          onCancel={onCancel}
          theme={theme}
          settings={settings}
        />
      )}
    </>
  );
};

const SharingDialogWrapper = inject(({ dialogsStore, filesStore }) => {
  const { getShareUsers, setSelection } = filesStore;
  const { setSharingPanelVisible, sharingPanelVisible } = dialogsStore;
  return {
    setSharingPanelVisible,
    getShareUsers,
    setSelection,
    sharingPanelVisible,
  };
})(observer(SharingDialog));

class SharingModal extends React.Component {
  static getSharingSettings = (fileId) =>
    getShareFiles([+fileId], []).then((users) =>
      this.convertSharingUsers(users)
    );

  static convertSharingUsers = (users) =>
    Promise.resolve(SharingPanel.convertSharingUsers(users));

  componentDidMount() {
    authStore.init(true);
  }

  render() {
    return (
      <MobxProvider {...store}>
        <SharingDialogWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SharingModal;
