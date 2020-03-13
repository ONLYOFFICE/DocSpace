import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import isEqual from "lodash/isEqual";
import {
  Row,
  toastr,
  Icons,
  RowContainer,
  Loader
} from "asc-web-components";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesRowContent from "./FilesRowContent";
import { api } from 'asc-web-common';
import { fetchFiles, deleteFile, deleteFolder, fetchFolder } from '../../../../../store/files/actions';
import store from "../../../../../store/store";
import { getFilterByLocation } from "../../../../../helpers/converters";
import config from "../../../../../../package.json";

const { FilesFilter } = api;

class SectionBodyContent extends React.PureComponent {

  constructor(props) {
    super(props);

    this.state = {
      editingId: -1,
      isEdit: false,
      isCreating: ''
    };
  }

  componentDidMount() {
    const { fetchFiles } = this.props;

    //TODO: use right algorithm, fix fetching in src/index.html

    var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    const match = window.location.pathname.match(re);

    if (match && match.length > 0) {
      const newFilter = getFilterByLocation(window.location);
      if (newFilter) {
        return fetchFiles(newFilter)
          .catch(error => toastr.error(error));
      } else {
        const filter = FilesFilter.getDefault();

        fetchFiles(filter)
          .catch(error => toastr.error(error));
      }
    }
  }

  componentDidUpdate(prevProps) {
    if (this.props.isCreating !== prevProps.isCreating) {
      let tempId = this.state.editingId;

      if (this.props.isCreating !== '') {
        tempId = -2;
      }

      this.setState({
        editingId: tempId,
        isCreating: this.props.isCreating
      });
    }
  }

  onClickRename = (itemId) => {
    this.setState({
      editingId: itemId,
      isEdit: true
    });
  };

  onEditComplete = () => {
    const { folderId, onCreate } = this.props;

    onCreate(false);

    if (this.state.isCreating !== '') {
      fetchFolder(folderId, store.dispatch)
    }

    this.setState({
      editingId: -1,
      isEdit: false,
      isCreating: ''
    });
  }

  onClickDelete = (item) => {
    item.fileExst
      ? this.onDeleteFile(item.id, item.folderId)
      : this.onDeleteFolder(item.id, item.parentId);
  }

  onDeleteFile = (fileId, currentFolderId) => {
    const { deleteFile } = this.props;

    deleteFile(fileId)
      .catch(err => toastr.error(err))
      .then(() => fetchFolder(currentFolderId, store.dispatch))
      .then(() => toastr.success(`File moved to recycle bin`));
  }

  onDeleteFolder = (folderId, currentFolderId) => {
    const { deleteFolder } = this.props;

    deleteFolder(folderId)
      .catch(err => toastr.error(err))
      .then(() => fetchFolder(currentFolderId, store.dispatch))
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
        onClick: this.onClickRename.bind(this, item.id),
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

  render() {
    const { files, folders, viewer, parentId, folderId, settings } = this.props;
    const { editingId, isEdit, isCreating } = this.state;

    let items = [...folders, ...files];

    if (isCreating !== '') {
      items.unshift({
        id: -2,
        title: '',
        parentId: folderId,
        fileExst: isCreating
      })
    }

    return items.length > 0 ? (
      <RowContainer useReactWindow={false}>
        {items.map(item => {
          const contextOptions = this.getFilesContextOptions(item, viewer).filter(o => o);
          const contextOptionsProps = !contextOptions.length || item.id === -2
            ? {}
            : { contextOptions };
          const checked = false; //isUserSelected(selection, user.id);
          const checkedProps = /* isAdmin(viewer) */ item.id !== -2 && true ? { checked } : {};
          const element = (isEdit || isCreating) && (item.id === editingId || item.id === -2)
            ? <Loader type='oval' color="black" size='24px' label="Editing..." />
            : item.fileExst
              ? <Icons.ActionsDocumentsIcon size='big' isfill={true} color="#A3A9AE" />
              : <Icons.CatalogFolderIcon size='big' isfill={true} color="#A3A9AE" />;

          return (
            <Row
              key={item.id}
              data={item}
              element={element}
              onSelect={() => { }}
              editing={editingId}
              {...checkedProps}
              {...contextOptionsProps}
              needForUpdate={this.needForUpdate}
            >
              <FilesRowContent item={item} viewer={viewer} editingId={editingId} culture={settings.culture} onEditComplete={this.onEditComplete} />
            </Row>
          );
        })}
      </RowContainer>
    ) : parentId !== 0 ? (
      <EmptyFolderContainer parentId={parentId} />
    ) : <a>RootFolderContainer</a>;
  }
}

SectionBodyContent.defaultProps = {
  files: null
};

const mapStateToProps = state => {
  return {
    selection: state.files.selection,
    selected: state.files.selected,
    files: state.files.files,
    folders: state.files.folders,
    viewer: state.auth.user,
    settings: state.auth.settings,
    filter: state.files.filter,
    parentId: state.files.selectedFolder.parentId,
    folderId: state.files.selectedFolder.id
  };
};

export default connect(
  mapStateToProps,
  { fetchFiles, deleteFile, deleteFolder }
)(withRouter(withTranslation()(SectionBodyContent)));
