import React from "react";
import { withRouter } from "react-router";
//import { connect } from "react-redux";
import { ModalDialog, Button } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils, toastr, api } from "asc-web-common";
// import {
//   deleteThirdParty,
//   setThirdPartyProviders,
//fetchFiles,
//   setUpdateTree,
//setTreeFolders,
// } from "../../../store/files/actions";
import {
  //getThirdPartyProviders,
  //getSelectedFolderId,
  loopTreeFolders,
  //getTreeFolders,
  //getCommonFolderId,
  //getMyFolderId,
} from "../../../store/files/selectors";
import { createI18N } from "../../../helpers/i18n";
import { inject, observer } from "mobx-react";
const i18n = createI18N({
  page: "DeleteThirdPartyDialog",
  localesPath: "dialogs/DeleteThirdPartyDialog",
});

const { changeLanguage } = utils;

class DeleteThirdPartyDialogComponent extends React.Component {
  constructor(props) {
    super(props);
    changeLanguage(i18n);
  }

  updateTree = (path, folders) => {
    const {
      t,
      treeFolders,
      removeItem,
      setTreeFolders,
      //setUpdateTree,
    } = this.props;

    const newTreeFolders = treeFolders;
    loopTreeFolders(path, newTreeFolders, folders, null);
    setTreeFolders(newTreeFolders);
    //setUpdateTree(true);
    toastr.success(t("SuccessDeleteThirdParty", { service: removeItem.title }));
  };

  onDeleteThirdParty = () => {
    const {
      setThirdPartyProviders,
      removeItem,
      providers,
      onClose,
      fetchFiles,
      currentFolderId,
      commonId,
      myId,
      deleteThirdParty,
    } = this.props;

    const providerItem = providers.find((x) => x.provider_id === removeItem.id);
    const newProviders = providers.filter(
      (x) => x.provider_id !== removeItem.id
    );

    deleteThirdParty(+removeItem.id)
      .then(() => {
        setThirdPartyProviders(newProviders);
        if (currentFolderId) {
          fetchFiles(currentFolderId).then((data) => {
            const path = data.selectedFolder.pathParts;
            const folders = data.selectedFolder.folders;
            this.updateTree(path, folders);
          });
        } else {
          const folderId = providerItem.corporate ? commonId : myId;
          api.files.getFolder(folderId).then((data) => {
            const path = [folderId];
            this.updateTree(path, data.folders);
          });
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => onClose());
  };

  render() {
    const { onClose, visible, t, removeItem } = this.props;

    return (
      <ModalDialog visible={visible} zIndex={310} onClose={onClose}>
        <ModalDialog.Header>{t("DeleteThirdParty")}</ModalDialog.Header>
        <ModalDialog.Body>
          {t("DeleteThirdPartyAlert", { service: removeItem.title })}
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            label={t("OKButton")}
            size="big"
            primary
            onClick={this.onDeleteThirdParty}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

const ModalDialogContainerTranslated = withTranslation()(
  DeleteThirdPartyDialogComponent
);

const DeleteThirdPartyDialog = (props) => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

// const mapStateToProps = (state) => {
//   return {
//     providers: getThirdPartyProviders(state),
//currentFolderId: getSelectedFolderId(state),
//treeFolders: getTreeFolders(state),
//commonId: getCommonFolderId(state),
//myId: getMyFolderId(state),
//   };
// };

// export default connect(mapStateToProps, {
//   setThirdPartyProviders,
//   fetchFiles,
//   setUpdateTree,
//   setTreeFolders,
// })(withRouter(DeleteThirdPartyDialog));

export default inject(({ auth, filesStore, thirdParty, treeFoldersStore }) => {
  const { providers, setThirdPartyProviders, deleteThirdParty } = thirdParty;
  const { fetchFiles } = filesStore;

  const {
    treeFolders,
    setTreeFolders,
    myFolderId,
    commonFolderId,
  } = treeFoldersStore;

  return {
    currentFolderId: filesStore.selectedFolderStore.id,
    treeFolders,
    myId: myFolderId,
    commonId: commonFolderId,
    providers,

    fetchFiles,
    setTreeFolders,
    setThirdPartyProviders,
    deleteThirdParty,
  };
})(withRouter(observer(DeleteThirdPartyDialog)));
