import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ModalDialog, Button } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils, toastr } from "asc-web-common";
import {
  deleteThirdParty,
  setThirdPartyProviders,
  fetchFiles,
  setUpdateTree,
  setTreeFolders,
} from "../../../store/files/actions";
import {
  getThirdPartyProviders,
  getSelectedFolderId,
  loopTreeFolders,
  getTreeFolders,
} from "../../../store/files/selectors";
import { createI18N } from "../../../helpers/i18n";
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

  onDeleteThirdParty = () => {
    const {
      setThirdPartyProviders,
      removeItem,
      providers,
      onClose,
      fetchFiles,
      currentFolder,
      treeFolders,
      setUpdateTree,
      setTreeFolders,
      t,
    } = this.props;
    const newProviders = providers.filter(
      (x) => x.provider_id !== removeItem.id
    );

    deleteThirdParty(+removeItem.id)
      .then(() => {
        setThirdPartyProviders(newProviders);
        fetchFiles(currentFolder).then((data) => {
          const path = data.selectedFolder.pathParts;
          const newTreeFolders = treeFolders;
          const folders = data.selectedFolder.folders;
          loopTreeFolders(path, newTreeFolders, folders, null);
          setUpdateTree(true);
          setTreeFolders(newTreeFolders);
          toastr.success(
            t("SuccessDeleteThirdParty", { service: removeItem.title })
          );
        });
      })
      .catch((err) => toastr(err))
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

const mapStateToProps = (state) => {
  return {
    providers: getThirdPartyProviders(state),
    currentFolder: getSelectedFolderId(state),
    treeFolders: getTreeFolders(state),
  };
};

export default connect(mapStateToProps, {
  setThirdPartyProviders,
  fetchFiles,
  setUpdateTree,
  setTreeFolders,
})(withRouter(DeleteThirdPartyDialog));
