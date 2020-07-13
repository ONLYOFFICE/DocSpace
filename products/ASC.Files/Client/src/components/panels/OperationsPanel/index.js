import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Backdrop, Heading, Aside, toastr } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils as commonUtils } from "asc-web-common";
import i18n from "./i18n";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody
} from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import { copyToFolder, moveToFolder, setProgressBarData, clearProgressData } from "../../../store/files/actions";
import { checkFolderType } from "../../../store/files/selectors";
import store from "../../../store/store";

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
      copyToFolder,
      moveToFolder,
      loopFilesOperations,
      setProgressBarData
    } = this.props;

    const destFolderId = Number(e);
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;
    const folderIds = [];
    const fileIds = [];

    for(let item of selection) {
      if(item.fileExst) {
        fileIds.push(item.id);
      } else if(item.id === destFolderId) {
        toastr.error(t("MoveToFolderMessage"));
      } else {
        folderIds.push(item.id);
      }
    }
    this.props.onClose();

    if(isCopy) {
      setProgressBarData({ visible: true, percent: 0, label: t("CopyOperation")});
      copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          loopFilesOperations(id, destFolderId, isCopy);
        })
        .catch(err => {
          toastr.error(err);
          clearProgressData(store.dispatch);
        })
    } else {
      setProgressBarData({ visible: true, percent: 0, label: t("MoveToOperation")});
      moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          loopFilesOperations(id, destFolderId, false);
        })
        .catch(err => {
          toastr.error(err);
          clearProgressData(store.dispatch);
        })
    }
  }

  render() {
    //console.log("Operations panel render");
    const { t, onLoading, isLoading, filter, treeFolders, isCopy, isRecycleBinFolder, visible, onClose } = this.props;
    const zIndex = 310;
    const data = treeFolders.slice(0, 3);
    const expandedKeys = this.props.expandedKeys.map(item => item.toString());

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop onClick={onClose} visible={visible} zIndex={zIndex} />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledHeaderContent className="files-operations-panel">
              <Heading size="medium" truncate>
                {isRecycleBinFolder ? t("Restore"): isCopy ? t("Copy") : t("Move")}
              </Heading>
            </StyledHeaderContent>
            <StyledBody className="files-operations-body">
              <TreeFolders
                expandedKeys={expandedKeys}
                data={data}
                filter={filter}
                onLoading={onLoading}
                isLoading={isLoading}
                onSelect={this.onSelect}
                needUpdate={false}
              />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledAsidePanel>
    );
  }
}

OperationsPanelComponent.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool,
};

const OperationsPanelContainerTranslated = withTranslation()(OperationsPanelComponent);

const OperationsPanel = (props) => (
  <OperationsPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {

  const { selectedFolder, selection, treeFolders, filter } = state.files;
  const { pathParts, id } = selectedFolder;
  const indexOfTrash = 3;

  return { 
    treeFolders,
    filter,
    selection,
    expandedKeys: pathParts,
    currentFolderId: id,
    isRecycleBinFolder: checkFolderType(id, indexOfTrash, treeFolders),
  };
};

export default connect(mapStateToProps, {
  copyToFolder,
  moveToFolder,
  setProgressBarData
})(withRouter(OperationsPanel));
