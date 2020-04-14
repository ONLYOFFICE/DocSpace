import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ReactSVG } from 'react-svg'
import { withTranslation } from "react-i18next";
import isEqual from "lodash/isEqual";
import styled from "styled-components";
import {
  IconButton,
  Row,
  RowContainer,
  toastr,
  Link
} from "asc-web-components";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesRowContent from "./FilesRowContent";
import { api, constants } from 'asc-web-common';
import {
  deleteFile,
  deleteFolder,
  deselectFile,
  fetchFiles,
  fetchFolder,
  //fetchRootFolders,
  selectFile,
  setAction,
  setTreeFolders
} from '../../../../../store/files/actions';
import { isFileSelected, getFileIcon, getFolderIcon, getFolderType } from '../../../../../store/files/selectors';
import store from "../../../../../store/store";
//import { getFilterByLocation } from "../../../../../helpers/converters";
//import config from "../../../../../../package.json";

const { FilesFilter } = api;
const { FileAction } = constants;

const linkStyles = { isHovered: true, type: "action", fontSize: "14px", className: "empty-folder_link", display: "flex" };

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      editingId: null
    };
  }

  componentDidMount() {
    //const { fetchFiles } = this.props;

    //TODO: use right algorithm, fix fetching in src/index.html

    // var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    // const match = window.location.pathname.match(re);

    // if (match && match.length > 0) {
    //   const newFilter = getFilterByLocation(window.location);
    //   if (newFilter) {
    //     return fetchFiles(newFilter)
    //       .catch(error => toastr.error(error));
    //   } else {
    //     const filter = FilesFilter.getDefault();

    //     fetchFiles(filter)
    //       .catch(error => toastr.error(error));
    //   }
    // }
  }

  onClickRename = (item) => {
    const { id, fileExst } = item;

    this.setState({ editingId: id }, () => {
      this.props.setAction(
        {
          type: FileAction.Rename,
          extension: fileExst,
          id
        }
      );
    });
  };

  onEditComplete = item => {
    const { folderId, fileAction, filter, treeFolders, setTreeFolders, onLoading } = this.props;

    if (fileAction.type === FileAction.Create || fileAction.type === FileAction.Rename) {
      onLoading(true);
      fetchFiles(folderId, filter, store.dispatch).then(data => {
        if (!item.fileExst) {
          const path = data.selectedFolder.pathParts;
          const newTreeFolders = treeFolders;
          const folders = data.selectedFolder.folders;
          this.loop(path, newTreeFolders, item.parentId, folders);
          setTreeFolders(newTreeFolders);
        }
      }).finally(() => onLoading(false))
    }

    this.setState({ editingId: null }, () => {
      this.props.setAction({
        type: null
      });
    })
  }

  onClickDelete = (item) => {
    item.fileExst
      ? this.onDeleteFile(item.id, item.folderId)
      : this.onDeleteFolder(item.id, item.parentId);
  }

  onDeleteFile = (fileId, currentFolderId) => {
    const { deleteFile, filter } = this.props;

    deleteFile(fileId)
      .catch(err => toastr.error(err))
      .then(() => fetchFiles(currentFolderId, filter, store.dispatch))
      .then(() => toastr.success(`File moved to recycle bin`));
  }

  loop = (path, item, folderId, folders, foldersCount) => {
    const newPath = path;
    while (path.length !== 0) {
      const newItems = item.find(x => x.id === path[0]);
      newPath.shift();
      if (path.length === 0) {
        const currentItem = item.find(x => x.id === folderId);
        currentItem.folders = folders;
        currentItem.foldersCount = foldersCount;
        return;
      }
      this.loop(newPath, newItems.folders, folderId, folders, foldersCount);
    }
  };

  onDeleteFolder = (folderId, currentFolderId) => {
    const { deleteFolder, filter, treeFolders, setTreeFolders, onLoading } = this.props;
    onLoading(true);
    deleteFolder(folderId, currentFolderId)
      .catch(err => toastr.error(err))
      .then(() =>
        fetchFiles(currentFolderId, filter, store.dispatch).then(data => {
          const path = data.selectedFolder.pathParts;
          const newTreeFolders = treeFolders;
          const folders = data.selectedFolder.folders;
          const foldersCount = data.selectedFolder.foldersCount;
          this.loop(path, newTreeFolders, currentFolderId, folders, foldersCount);
          setTreeFolders(newTreeFolders);
        })
      )
      .then(() => toastr.success(`Folder moved to recycle bin`))
      .finally(() => onLoading(false));
  }

  onClickLinkForPortal = (folderId) => {
    return fetchFolder(folderId, store.dispatch);
  }

  getFilesContextOptions = (item, viewer) => {
    return [
      {
        key: "sharing-settings",
        label: "Sharing settings",
        onClick: () => { },
        disabled: true
      },
      {
        key: "link-for-portal-users",
        label: "Link for portal users",
        onClick: this.onClickLinkForPortal.bind(this, item.folderId),
        disabled: true
      },
      {
        key: "sep",
        isSeparator: true
      },
      {
        key: "download",
        label: "Download",
        onClick: () => { },
        disabled: true
      },
      {
        key: "rename",
        label: "Rename",
        onClick: this.onClickRename.bind(this, item),
        disabled: false
      },
      {
        key: "delete",
        label: "Delete",
        onClick: this.onClickDelete.bind(this, item),
        disabled: false
      },
    ]
  };

  needForUpdate = (currentProps, nextProps) => {
    if (currentProps.checked !== nextProps.checked) {
      return true;
    }
    if (currentProps.editing !== nextProps.editing) {
      return true;
    }
    if (!isEqual(currentProps.data, nextProps.data)) {
      return true;
    }
    return false;
  };

  onContentRowSelect = (checked, file) => {

    if (!file) return;

    if (checked) {
      this.props.selectFile(file);
    } else {
      this.props.deselectFile(file);
    }
  };

  getItemIcon = (item, isEdit) => {
    const extension = item.fileExst;
    const icon = extension
      ? getFileIcon(extension, 24)
      : getFolderIcon(item.providerKey, 24);

    return <ReactSVG
      beforeInjection={svg => {
        svg.setAttribute('style', 'margin-top: 4px');
        isEdit && svg.setAttribute('style', 'margin-left: 24px');
      }}
      src={icon}
    />;
  };

  onCreate = (format) => {
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onResetFilter = () => {
    //ClearButton
    //const newFilter = filter.clone(true);
    const { selectedFolderId, onLoading } = this.props;
    onLoading(true);
    const newFilter = FilesFilter.getDefault();
    fetchFiles(selectedFolderId, newFilter, store.dispatch).catch(err =>
      toastr.error(err)
    ).finally(() => onLoading(false));
  }

  onGoToMyDocuments = () => {
    const { filter, myDocumentsId, onLoading } = this.props;
    const newFilter = filter.clone();
    onLoading(true);
    fetchFiles(myDocumentsId, newFilter, store.dispatch).finally(() =>
      onLoading(false)
    );
  };

  onBackToParentFolder = () => {
    const { filter, parentId, onLoading } = this.props;
    const newFilter = filter.clone();
    onLoading(true);
    fetchFiles(parentId, newFilter, store.dispatch).finally(() =>
      onLoading(false)
    );
  };

  renderEmptyRootFolderContainer = () => {
    const { currentFolderType, title } = this.props;
    const subheadingText = "No files to be displayed in this section";

    const myDescription =
      "The documents and image files you create or upload to the portal are kept here in 'My Documents' section. You can open and edit them using the ONLYOFFICE™ portal editor, share them with friends or colleagues, organize into folders. Drag-and-drop the files from your computer here to upload them to your portal even more easily.";

    const shareDescription =
      "The 'Shared with Me' section is used to show the files which your friends or colleagues gave you access to. In case you haven't seen the latest changes in the documents they are marked 'new'. You can remove the files from the list clicking the appropriate button.";

    const commonDescription =
      "The 'Common Documents' section shows all the documents shared by portal administrator for common access. Only portal administrator can create folders in this section, but with the access granted the portal users can also upload their files here. Drag-and-drop the files from your computer here to upload them to your portal even more easily.";

    const commonButtons = (
      <>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            noHover
            onClick={() => console.log("Create document click")}
          >
            +
          </Link>
          <Link onClick={this.onCreate.bind(this, "docx")} {...linkStyles}>
            Документ,
          </Link>
          <Link onClick={this.onCreate.bind(this, "xlsx")} {...linkStyles}>
            Таблица,
          </Link>
          <Link onClick={this.onCreate.bind(this, "pptx")} {...linkStyles}>
            Презентация
          </Link>
        </div>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            onClick={this.onCreate.bind(this, null)}
            noHover
          >
            +
          </Link>
          <Link {...linkStyles} onClick={this.onCreate.bind(this, null)}>
            Папка
          </Link>
        </div>
      </>
    );

    const trashDescription =
      "The 'Recycle Bin' section is where all the deleted files are moved. You can either restore them in case they are deleted by mistake or delete them permanently. Please note, that when you delete the files from the 'Recycle Bin' they cannot be restored any longer.";
    const trashButtons = (
      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_up-image"
          src="images/empty_screen_people.svg"
          alt=""
          onClick={this.onGoToMyDocuments}
        />
        <Link onClick={this.onGoToMyDocuments} {...linkStyles}>
          Go to My Documents
        </Link>
      </div>
    );

    switch (currentFolderType) {
      case "My":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={myDescription}
            imageSrc="images/empty_screen.png"
            buttons={commonButtons}
          />
        );
      case "Share":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={shareDescription}
            imageSrc="images/empty_screen_forme.png"
          />
        );
      case "Common":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={commonDescription}
            imageSrc="images/empty_screen_corporate.png"
            buttons={commonButtons}
          />
        );
      case "Trash":
        return (
          <EmptyFolderContainer
            headerText={title}
            subheadingText={subheadingText}
            descriptionText={trashDescription}
            imageSrc="images/empty_screen_trash.png"
            buttons={trashButtons}
          />
        );
      default:
        return;
    }
  };

  renderEmptyFolderContainer = () => {
    const buttons = (
      <>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            noHover
            onClick={() => console.log("Create document click")}
          >
            +
          </Link>
          <Link onClick={this.onCreate.bind(this, "docx")} {...linkStyles}>
            Документ,
          </Link>
          <Link onClick={this.onCreate.bind(this, "xlsx")} {...linkStyles}>
            Таблица,
          </Link>
          <Link onClick={this.onCreate.bind(this, "pptx")} {...linkStyles}>
            Презентация
          </Link>
        </div>
        <div className="empty-folder_container-links">
          <Link
            className="empty-folder_container_plus-image"
            color="#83888d"
            fontSize="26px"
            fontWeight="800"
            onClick={this.onCreate.bind(this, null)}
            noHover
          >
            +
          </Link>
          <Link {...linkStyles} onClick={this.onCreate.bind(this, null)}>
            Папка
          </Link>
        </div>
        <div className="empty-folder_container-links">
          <img
            className="empty-folder_container_up-image"
            src="images/up.svg"
            onClick={this.onBackToParentFolder}
            alt=""
          />
          <Link onClick={this.onBackToParentFolder} {...linkStyles}>
            Вернутся в папку на уровень выше
          </Link>
        </div>
      </>
    );

    return (
      <EmptyFolderContainer
        headerText="В этой папке нет файлов"
        imageSrc="images/empty_screen.png"
        buttons={buttons}
      />
    );
  };

  renderEmptyFilterContainer = () => {
    const subheadingText = "No files to be displayed for this filter here";
    const descriptionText = "No files or folders matching your filter can be displayed in this section. Please select other filter options or clear filter to view all the files in this section. You can also look for the file you need in other sections.";

    const buttons = (
      <div className="empty-folder_container-links">
        <IconButton
          className="empty-folder_container-icon"
          size="12"
          onClick={this.onResetFilter}
          iconName="CrossIcon"
          isFill
          color="A3A9AE"
        />
        <Link onClick={this.onResetFilter} {...linkStyles}>
          Clear Filter
        </Link>
      </div>
    );
    
    return (
      <EmptyFolderContainer
        headerText="Filter"
        subheadingText={subheadingText}
        descriptionText={descriptionText}
        imageSrc="images/empty_screen_filter.png"
        buttons={buttons}
    />
    )
  }

  render() {
    const {
      files,
      folders,
      viewer,
      parentId,
      folderId,
      settings,
      selection,
      fileAction,
      onLoading,
      currentFolderCount,
    } = this.props;
    const { editingId } = this.state;

    let items = [...folders, ...files];

    const SimpleFilesRow = styled(Row)`
      ${(props) =>
        !props.contextOptions &&
        `
          & > div:last-child {
              width: 0px;
            }
        `}
    `;

    if (fileAction && fileAction.type === FileAction.Create) {
      items.unshift({
        id: -1,
        title: "",
        parentId: folderId,
        fileExst: fileAction.extension,
      });
    }

    return currentFolderCount === 0 ? (
      parentId === 0 ? (
        this.renderEmptyRootFolderContainer()
      ) : (
        this.renderEmptyFolderContainer()
      )
    ) : items.length === 0 ? (
      this.renderEmptyFilterContainer()
    ) : (
      <RowContainer useReactWindow={false}>
        {items.map((item) => {
          const isEdit =
            fileAction.type &&
            (editingId === item.id || item.id === -1) &&
            item.fileExst === fileAction.extension;
          const contextOptions = this.getFilesContextOptions(
            item,
            viewer
          ).filter((o) => o);
          const contextOptionsProps =
            !contextOptions.length || isEdit ? {} : { contextOptions };
          const checked = isFileSelected(selection, item.id, item.parentId);
          const checkedProps = /* isAdmin(viewer) */ isEdit ? {} : { checked };
          const element = this.getItemIcon(item, isEdit);

          return (
            <SimpleFilesRow
              key={item.id}
              data={item}
              element={element}
              onSelect={this.onContentRowSelect}
              editing={editingId}
              {...checkedProps}
              {...contextOptionsProps}
              needForUpdate={this.needForUpdate}
            >
              <FilesRowContent
                item={item}
                viewer={viewer}
                culture={settings.culture}
                onEditComplete={this.onEditComplete.bind(this, item)}
                onLoading={onLoading}
              />
            </SimpleFilesRow>
          );
        })}
      </RowContainer>
    );
  }
}

SectionBodyContent.defaultProps = {
  files: null
};

const mapStateToProps = state => {
  const { selectedFolder, treeFolders } = state.files;
  const { id, title, foldersCount, filesCount } = selectedFolder;
  const currentFolderType = getFolderType(id, treeFolders);

  const myFolderIndex = 0;
  const currentFolderCount = filesCount + foldersCount;

  return {
    fileAction: state.files.fileAction,
    files: state.files.files,
    filter: state.files.filter,
    folderId: state.files.selectedFolder.id,
    folders: state.files.folders,
    parentId: state.files.selectedFolder.parentId,
    selected: state.files.selected,
    selection: state.files.selection,
    settings: state.auth.settings,
    viewer: state.auth.user,
    treeFolders: state.files.treeFolders,
    currentFolderType,
    title,
    myDocumentsId: treeFolders[myFolderIndex].id,
    currentFolderCount,
    selectedFolderId: id
  };
};

export default connect(
  mapStateToProps,
  {
    deleteFile,
    deleteFolder,
    deselectFile,
    fetchFiles,
    //fetchRootFolders,
    selectFile,
    setAction,
    setTreeFolders
  }
)(withRouter(withTranslation()(SectionBodyContent)));
