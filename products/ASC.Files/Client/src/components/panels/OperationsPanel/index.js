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
  itemOperationToFolder,
} from "../../../store/files/actions";
import {
  getFilter,
  getSelection,
  getPathParts,
  getSelectedFolderId,
  getIsRecycleBinFolder,
  getOperationsFolders,
} from "../../../store/files/selectors";
import { ThirdPartyMoveDialog } from "../../dialogs";
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

    this.state = {
      showProviderDialog: false,
      operationPanelVisible: props.visible,
      provider: "",
      destFolderId: null,
    };
  }

  onSelect = (folder) => {
    const { currentFolderId, onClose, selection, isCopy } = this.props;
    const destFolderId = isNaN(+folder[0]) ? folder[0] : +folder[0];

    const provider = selection.find((x) => x.providerKey);

    if (currentFolderId === destFolderId) {
      return onClose();
    } else {
      provider && !isCopy
        ? this.setState({
            provider: provider.providerKey,
            operationPanelVisible: false,
            showProviderDialog: true,
            destFolderId,
          })
        : this.startOperation(isCopy, destFolderId);
    }
  };

  componentDidUpdate(prevProps) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ operationPanelVisible: this.props.visible });
    }
  }

  startMoveOperation = () => {
    this.startOperation(false);
  };

  startCopyOperation = () => {
    this.startOperation(true);
  };

  startOperation = (isCopy, folderId) => {
    const {
      setProgressBarData,
      itemOperationToFolder,
      t,
      selection,
      onClose,
    } = this.props;

    const destFolderId = folderId ? folderId : this.state.destFolderId;
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

    setProgressBarData({
      visible: true,
      percent: 0,
      label: isCopy ? t("CopyOperation") : t("MoveToOperation"),
    });

    itemOperationToFolder(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter,
      isCopy
    );

    onClose();
  };

  render() {
    //console.log("Operations panel render");
    const {
      t,
      filter,
      isCopy,
      isRecycleBin,
      operationsFolders,
      onClose,
    } = this.props;
    const { showProviderDialog, operationPanelVisible, provider } = this.state;

    const zIndex = 310;
    const expandedKeys = this.props.expandedKeys.map((item) => item.toString());

    return (
      <>
        {showProviderDialog && (
          <ThirdPartyMoveDialog
            visible={showProviderDialog}
            onClose={onClose}
            startMoveOperation={this.startMoveOperation}
            startCopyOperation={this.startCopyOperation}
            provider={provider}
          />
        )}

        <StyledAsidePanel visible={operationPanelVisible}>
          <ModalDialog
            visible={operationPanelVisible}
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
      </>
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
    operationsFolders: getOperationsFolders(state),
  };
};

export default connect(mapStateToProps, {
  setProgressBarData,
  itemOperationToFolder,
})(withRouter(OperationsPanel));
