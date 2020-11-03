import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { ModalDialog } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils as commonUtils, toastr } from "asc-web-common";
import { StyledAsidePanel } from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import {
  setProgressBarData,
  itemOperationToFolder
} from "../../../store/files/actions";
import {
  getFilter,
  getSelection,
  getPathParts,
  getSelectedFolderId,
  getIsRecycleBinFolder,
  getOperationsFolders,
} from "../../../store/files/selectors";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "OperationsPanel",
  localesPath: "panels/OperationsPanel",
});

const { changeLanguage } = commonUtils;

class OperationsPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);
  }

  onSelect = (e) => {
    const {
      t,
      isCopy,
      selection,
      setProgressBarData,
      currentFolderId,
      onClose,
      itemOperationToFolder
    } = this.props;

    const destFolderId = Number(e);
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;
    const folderIds = [];
    const fileIds = [];

    if (currentFolderId === destFolderId) {
      return onClose();
    }
    else {
      for (let item of selection) {
        if (item.fileExst) {
          fileIds.push(item.id);
        } else if (item.id === destFolderId) {
          toastr.error(t("MoveToFolderMessage"));
        } else {
          folderIds.push(item.id);
        }
      }
      onClose();
      setProgressBarData({
        visible: true,
        percent: 0,
        label: isCopy ? t("CopyOperation") : t("MoveToOperation"),
      });
      itemOperationToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter, isCopy)
    }
  };

  render() {
    //console.log("Operations panel render");
    const {
      t,
      filter,
      isCopy,
      isRecycleBin,
      visible,
      onClose,
      operationsFolders
    } = this.props;
    const zIndex = 310;
    const expandedKeys = this.props.expandedKeys.map((item) => item.toString());

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
              data={operationsFolders}
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
  visible: PropTypes.bool,
};

const OperationsPanelContainerTranslated = withTranslation()(
  OperationsPanelComponent
);

const OperationsPanel = (props) => (
  <OperationsPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {
    filter: getFilter(state),
    selection: getSelection(state),
    expandedKeys: getPathParts(state),
    currentFolderId: getSelectedFolderId(state),
    isRecycleBin: getIsRecycleBinFolder(state),
    operationsFolders: getOperationsFolders(state)
  };
};

export default connect(mapStateToProps, {
  setProgressBarData,
  itemOperationToFolder
})(withRouter(OperationsPanel));
