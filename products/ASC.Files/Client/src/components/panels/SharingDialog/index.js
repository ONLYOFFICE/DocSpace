import React, { useEffect } from "react";
import { Provider as MobxProvider, inject, observer } from "mobx-react";
import { getShareFiles } from "@appserver/common/api/files";
import SharingPanel from "../SharingPanel";

import stores from "../../../store/index";
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
  const { getShareUsers, setSelection } = filesStore;
  const { setSharingPanelVisible } = dialogsStore;
  return {
    setSharingPanelVisible,
    getShareUsers,
    setSelection,
  };
})(observer(SharingDialog));

class SharingModal extends React.Component {
  static getSharingSettings = (fileId) => {
    return getShareFiles([+fileId], []).then((users) =>
      SharingPanel.convertSharingUsers(users)
    );
  };

  componentDidMount() {
    authStore.init(true);
  }

  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <SharingDialogWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default SharingModal;
