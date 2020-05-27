import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Backdrop, Heading, Aside } from "asc-web-components";
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
import {
  getProgress,
  fetchFiles,
  setFilter,
  setTreeFolders,
  getFolder,
  copyToFolder,
  moveToFolder
} from "../../../store/files/actions";
import { default as filesStore } from "../../../store/store";
import { loopTreeFolders, checkFolderType } from "../../../store/files/selectors";

const { changeLanguage } = commonUtils;

class OperationsPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = { visible: false };
  }

  setNewFiler = () => {
    const { filter, selection, setFilter } = this.props;
    const newFilter = filter.clone();
    for(let item of selection) {
      const expandedIndex = newFilter.treeFolders.findIndex(x => x == item.id);
      if(expandedIndex !== -1) {
        newFilter.treeFolders.splice(expandedIndex, 1);
      }
    }
    
    setFilter(newFilter);
  }

  loop = (id, destFolderId) => {
    const {
      getProgress,
      setProgressValue,
      finishFilesOperations,
      filter,
      currentFolderId,
      treeFolders,
      getFolder,
      isCopy,
      isRecycleBinFolder
    } = this.props;
    getProgress().then(res => {
      const currentItem = res.find(x => x.id === id);
      if(currentItem && currentItem.progress !== 100) {
        setProgressValue(currentItem.progress);
        setTimeout(() => this.loop(id, destFolderId), 1000);
      } else {
        getFolder(destFolderId).then(data => {
          let newTreeFolders = treeFolders;
          let path = data.pathParts.slice(0);
          let folders = data.folders;
          let foldersCount = data.current.foldersCount;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);

          if(!isCopy) {
            fetchFiles(currentFolderId, filter, filesStore.dispatch).then((data) => {
              if(!isRecycleBinFolder) {
                newTreeFolders = treeFolders;
                path = data.selectedFolder.pathParts.slice(0);
                folders = data.selectedFolder.folders;
                foldersCount = data.selectedFolder.foldersCount;
                loopTreeFolders(path, newTreeFolders, folders, foldersCount);
                setTreeFolders(newTreeFolders);
              }
              this.setNewFiler();
            }).catch(err => finishFilesOperations(err))
              .finally(() => { 
                setProgressValue(100);
                finishFilesOperations();
              })
          } else {
            setProgressValue(100);
            finishFilesOperations();
            setTreeFolders(newTreeFolders);
          }
          
        }).catch(err => finishFilesOperations(err))
      }
    }).catch(err => finishFilesOperations(err));
  }

  onSelect = e => {
    const {
      t,
      isCopy,
      selection,
      startFilesOperations,
      finishFilesOperations,
      copyToFolder,
      moveToFolder
    } = this.props;

    const destFolderId = Number(e);
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;
    const folderIds = [];
    const fileIds = [];

    for(let item of selection) {
      if(item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }
    this.onClose();

    if(isCopy) {
      startFilesOperations(t("CopyOperation"));
      copyToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          this.loop(id, destFolderId);
        })
        .catch(err => finishFilesOperations(err))
    } else {
      const progressLabel = t("MoveToOperation");
      startFilesOperations(progressLabel);
      moveToFolder(destFolderId, folderIds, fileIds, conflictResolveType, deleteAfter)
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          this.loop(id, destFolderId);
        })
        .catch(err => finishFilesOperations(err))
    }
  }

  onClose = () => {
    this.setState({ visible: false });
    setTimeout(() => this.props.onClose(), 1000);
  }

  componentDidMount() {
    setTimeout(() => this.setState({visible: this.props.visible}), 1000);
  }

  render() {
    //console.log("Operations panel render");
    const { t, onLoading, isLoading, filter, treeFolders, isCopy, isRecycleBinFolder } = this.props;
    const { visible } = this.state;
    const zIndex = 310;
    const fakeNewDocuments = 8;
    const data = treeFolders.slice(0, 3);
    const expandedKeys = this.props.expandedKeys.map(item => item.toString());

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop onClick={this.onClose} visible={visible} zIndex={zIndex} />
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
                fakeNewDocuments={fakeNewDocuments}
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
  setTreeFolders,
  getFolder,
  setFilter,
  getProgress,
  copyToFolder,
  moveToFolder,
})(withRouter(OperationsPanel));
