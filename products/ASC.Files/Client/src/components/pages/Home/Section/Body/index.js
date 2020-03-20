import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import isEqual from "lodash/isEqual";
import styled from "styled-components";
import {
  Icons,
  Row,
  RowContainer,
  toastr
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
  setAction
} from '../../../../../store/files/actions';
import { isFileSelected } from '../../../../../store/files/selectors';
import store from "../../../../../store/store";
//import { getFilterByLocation } from "../../../../../helpers/converters";
//import config from "../../../../../../package.json";

//const { FilesFilter } = api;
const { FileAction } = constants;

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

  onEditComplete = () => {
    const { folderId, fileAction, filter } = this.props;

    if (fileAction.type === FileAction.Create) {
      fetchFiles(folderId, filter, store.dispatch);
    }

    this.setState({ editingId: null }, () => {
      this.props.setAction({
        type: null
      });
    })

    //fetchRootFolders(store.dispatch);
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

  onDeleteFolder = (folderId, currentFolderId) => {
    const { deleteFolder, filter } = this.props;

    deleteFolder(folderId)
      .catch(err => toastr.error(err))
      .then(() => fetchFiles(currentFolderId, filter, store.dispatch))
      //.then(() => fetchRootFolders(store.dispatch))
      .then(() => toastr.success(`Folder moved to recycle bin`));
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

  getItemIcon = (extension, isEdit) => {
    const setEditIconStyle = isEdit ? { style: { marginLeft: '24px' } } : {};

    return extension
      ? <Icons.ActionsDocumentsIcon
        {...setEditIconStyle}
        size='big'
        isfill={true}
        color="#A3A9AE"
      />
      : <Icons.CatalogFolderIcon
        {...setEditIconStyle}
        size='big'
        isfill={true}
        color="#A3A9AE"
      />
  };

  render() {
    const { files, folders, viewer, parentId, folderId, settings, selection, fileAction, onLoading, filter } = this.props;
    const { editingId } = this.state;

    let items = [...folders, ...files];

    const SimpleFilesRow = styled(Row)`
      ${props => !props.contextOptions && `
          & > div:last-child {
              width: 0px;
            }
        `}
    `;

    if (fileAction && fileAction.type === FileAction.Create) {
      items.unshift({
        id: -1,
        title: '',
        parentId: folderId,
        fileExst: fileAction.extension
      })
    }

    return items.length > 0 ? (
      <RowContainer useReactWindow={false}>
        {items.map(item => {
          const isEdit = fileAction.type && (editingId === item.id || item.id === -1) && (item.fileExst === fileAction.extension);
          const contextOptions = this.getFilesContextOptions(item, viewer).filter(o => o);
          const contextOptionsProps = !contextOptions.length || isEdit
            ? {}
            : { contextOptions };
          const checked = isFileSelected(selection, item.id, item.parentId);
          const checkedProps = /* isAdmin(viewer) */ isEdit ? {} : { checked };
          const element = this.getItemIcon(item.fileExst, isEdit);

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
              <FilesRowContent item={item} viewer={viewer} culture={settings.culture} onEditComplete={this.onEditComplete} onLoading={onLoading} />
            </SimpleFilesRow>
          );
        })}
      </RowContainer>
    ) : parentId !== 0 ? (
      <EmptyFolderContainer parentId={parentId} filter={filter} />
    ) : <p>RootFolderContainer</p>;
  }
}

SectionBodyContent.defaultProps = {
  files: null
};

const mapStateToProps = state => {
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
    viewer: state.auth.user
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
    setAction
  }
)(withRouter(withTranslation()(SectionBodyContent)));
