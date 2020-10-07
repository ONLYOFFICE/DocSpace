import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { ModalDialog } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils as commonUtils, toastr, api } from "asc-web-common";
import { StyledAsidePanel } from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import {
  setProgressBarData,
  clearProgressData
} from "../../../store/files/actions";
import {
  getTreeFolders,
  getFilter,
  getSelection,
  getPathParts,
  getSelectedFolderId,
  getIsRecycleBinFolder,
} from "../../../store/files/selectors";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "OperationsPanel",
  localesPath: "panels/OperationsPanel"
});

const { changeLanguage } = commonUtils;

class OperationsPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);
  }

  onSelect = e => {
    const {
      t,
      isCopy,
      selection,
      loopFilesOperations,
      setProgressBarData,
      clearProgressData
    } = this.props;

    const destFolderId = Number(e);
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;
    const folderIds = [];
    const fileIds = [];

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else if (item.id === destFolderId) {
        toastr.error(t("MoveToFolderMessage"));
      } else {
        folderIds.push(item.id);
      }
    }
    this.props.onClose();

    if (isCopy) {
      setProgressBarData({
        visible: true,
        percent: 0,
        label: t("CopyOperation")
      });
      api.files.copyToFolder(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter
      )
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          loopFilesOperations(id, destFolderId, isCopy);
        })
        .catch(err => {
          toastr.error(err);
          clearProgressData();
        });
    } else {
      setProgressBarData({
        visible: true,
        percent: 0,
        label: t("MoveToOperation")
      });
      api.files.moveToFolder(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter
      )
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          loopFilesOperations(id, destFolderId, false);
        })
        .catch(err => {
          toastr.error(err);
          clearProgressData();
        });
    }
  };

  render() {
    //console.log("Operations panel render");
    const {
      t,
      filter,
      treeFolders,
      isCopy,
      isRecycleBin,
      visible,
      onClose
    } = this.props;
    const zIndex = 310;
    const data = treeFolders.slice(0, 3);
    const expandedKeys = this.props.expandedKeys.map(item => item.toString());

    return (
      <StyledAsidePanel visible={visible}>
        <ModalDialog
          visible={visible}
          displayType="aside"
          zIndex={zIndex}
          onClose={onClose}
        >
          <ModalDialog.Header>
            {isRecycleBin ? t("Restore") : isCopy ? t("Copy") : t("Move")}
          </ModalDialog.Header>
          <ModalDialog.Body>
            <TreeFolders
              expandedKeys={expandedKeys}
              data={data}
              filter={filter}
              onSelect={this.onSelect}
              needUpdate={false}
            />
          </ModalDialog.Body>
        </ModalDialog>
      </StyledAsidePanel>
    );
  }
}

OperationsPanelComponent.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool
};

const OperationsPanelContainerTranslated = withTranslation()(
  OperationsPanelComponent
);

const OperationsPanel = props => (
  <OperationsPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = state => {
  return {
    treeFolders: getTreeFolders(state),
    filter: getFilter(state),
    selection: getSelection(state),
    expandedKeys: getPathParts(state),
    currentFolderId: getSelectedFolderId(state),
    isRecycleBin: getIsRecycleBinFolder(state),
  };
};

export default connect(mapStateToProps, { setProgressBarData, clearProgressData })(withRouter(OperationsPanel));
