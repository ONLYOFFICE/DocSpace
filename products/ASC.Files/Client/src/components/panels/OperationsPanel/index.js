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
  setSecondaryProgressBarData,
  itemOperationToFolder,
} from "../../../store/files/actions";
import {
  getFilter,
  getSelection,
  //getPathParts,
  //getSelectedFolderId,
  getIsRecycleBinFolder,
  getOperationsFolders,
} from "../../../store/files/selectors";
import { ThirdPartyMoveDialog } from "../../dialogs";
import { createI18N } from "../../../helpers/i18n";
import { inject, observer } from "mobx-react";
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
      providerKey: "",
      destFolderId: null,
    };
  }

  onSelect = (folder, treeNode) => {
    const { currentFolderId, onClose, selection, isCopy } = this.props;
    const destFolderId = isNaN(+folder[0]) ? folder[0] : +folder[0];

    const provider = selection.find((x) => x.providerKey);
    const isProviderFolder = selection.find((x) => !x.providerKey);

    if (currentFolderId === destFolderId) {
      return onClose();
    } else {
      provider &&
      !isProviderFolder &&
      !isCopy &&
      treeNode.node.props.providerKey !== provider.providerKey
        ? this.setState({
            providerKey: provider.providerKey,
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
      itemOperationToFolder,
      t,
      selection,
      setSecondaryProgressBarData,
      currentFolderId,
      onClose,
    } = this.props;

    const destFolderId = folderId ? folderId : this.state.destFolderId;
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;
    const folderIds = [];
    const fileIds = [];

    if (currentFolderId === destFolderId) {
      return onClose();
    } else {
      const isProviderFolder = selection.find((x) => !x.providerKey);
      const items =
        isProviderFolder && !isCopy
          ? selection.filter((x) => !x.providerKey)
          : selection;

      for (let item of items) {
        if (item.fileExst) {
          fileIds.push(item.id);
        } else if (item.id === destFolderId) {
          toastr.error(t("MoveToFolderMessage"));
        } else {
          folderIds.push(item.id);
        }
      }
      onClose();
      setSecondaryProgressBarData({
        icon: isCopy ? "duplicate" : "move",
        visible: true,
        percent: 0,
        label: isCopy ? t("CopyOperation") : t("MoveToOperation"),
        alert: false,
      });
      itemOperationToFolder(
        destFolderId,
        folderIds,
        fileIds,
        conflictResolveType,
        deleteAfter,
        isCopy
      );
    }

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
    const {
      showProviderDialog,
      operationPanelVisible,
      providerKey,
    } = this.state;

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
            provider={providerKey}
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
    //expandedKeys: getPathParts(state),
    //currentFolderId: getSelectedFolderId(state),
    isRecycleBin: getIsRecycleBinFolder(state),
    operationsFolders: getOperationsFolders(state),
  };
};

// export default connect(mapStateToProps, {
//   setSecondaryProgressBarData,
//   itemOperationToFolder,
// })(withRouter(OperationsPanel));

export default connect(mapStateToProps, {
  setSecondaryProgressBarData,
  itemOperationToFolder,
})(
  inject(({ store, mainFilesStore }) => {
    const { filesStore } = mainFilesStore;

    return {
      expandedKeys: filesStore.selectedFolderStore.pathParts,
      currentFolderId: filesStore.selectedFolderStore.id,
    };
  })(withRouter(observer(OperationsPanel)))
);
