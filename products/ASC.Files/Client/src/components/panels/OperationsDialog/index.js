import React, { useEffect } from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { getShareFiles } from "@appserver/common/api/files";
import OperationsPanel from "../OperationsPanel";
import TreeFolders from "../../Article/Body/TreeFolders";
import stores from "../../../store/index";
import store from "studio/store";
import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsidePanel } from "../StyledPanels";
import { useTranslation, withTranslation } from "react-i18next";

const { auth: authStore } = store;

const OperationsDialog = ({
  commonFolder,
  operationsFolders,
  treeFolders,
  fetchTreeFolders,
  expandedKeys,
  filter,
  commonTreeFolder,
  getCommonFolder,
  setPanelVisible,
  getFolderPath,
}) => {
  useEffect(() => {
    fetchTreeFolders().then(() => getCommonFolder());
  }, []);

  const { t } = useTranslation("OperationsPanel");
  //   console.log("commonTreeFolder", commonTreeFolder);
  //   console.log("treeFolders", treeFolders);
  const visible = true;
  const zIndex = 310;
  const onClose = () => {
    setPanelVisible(false);
  };
  const onSelect = (folder, treeNode) => {
    //debugger;
    const folderTitle = treeNode.node.props.title;
    const destFolderId = isNaN(+folder[0]) ? folder[0] : +folder[0];
    //console.log("treeNode", treeNode.node);
    getFolderPath(folder).then((res) => console.log("res!!!", res));
    // if (currentFolderId === destFolderId) {
    //   return onClose();
    // }

    onClose();
  };
  return (
    <>
      <StyledAsidePanel visible={visible}>
        <ModalDialog
          visible={visible}
          displayType="aside"
          zIndex={zIndex}
          onClose={onClose}
        >
          <ModalDialog.Header>{t("ChooseFolder")}</ModalDialog.Header>
          <ModalDialog.Body>
            {commonTreeFolder && (
              <TreeFolders
                expandedPanelKeys={expandedKeys}
                data={[commonTreeFolder]}
                filter={filter}
                onSelect={onSelect}
                needUpdate={false}
                isCommonWithoutProvider
              />
            )}
          </ModalDialog.Body>
        </ModalDialog>
      </StyledAsidePanel>
    </>
  );
};

const OperationsDialogWrapper = inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { setPanelVisible } = auth;
    const { getFolderPath } = auth.settingsStore;
    const {
      commonFolder,
      operationsFolders,
      treeFolders,
      fetchTreeFolders,
      expandedPanelKeys,
      commonTreeFolder,
      getCommonFolder,
    } = treeFoldersStore;
    return {
      getFolderPath,
      setPanelVisible,
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,

      commonFolder,
      operationsFolders,
      treeFolders,
      fetchTreeFolders,
      commonTreeFolder,
      getCommonFolder,
    };
  }
)(observer(withTranslation("OperationsPanel")(OperationsDialog)));

class OperationsModal extends React.Component {
  static getSharingSettings = (fileId) => {
    return getShareFiles([+fileId], []).then((users) =>
      SharingPanel.convertSharingUsers(users)
    );
  };

  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <OperationsDialogWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default OperationsModal;
