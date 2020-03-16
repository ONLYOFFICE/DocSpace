import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import isEqual from "lodash/isEqual";
import {
  Row,
  toastr,
  Icons,
  RowContainer
} from "asc-web-components";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesRowContent from "./FilesRowContent";
import { api } from 'asc-web-common';
import { fetchFiles, deleteFile, deleteFolder, fetchFolder, setAction } from '../../../../../store/files/actions';
import store from "../../../../../store/store";
import { getFilterByLocation } from "../../../../../helpers/converters";
import config from "../../../../../../package.json";

const { FilesFilter } = api;

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      editingId: null
    };
  }

  componentDidMount() {
    const { fetchFiles } = this.props;

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

  onClickRename = (itemId) => {
    if (this.state.editingId === itemId)
      return;

    this.setState({ editingId: itemId }, () => {
      this.props.setAction({ type: 'rename', tempId: itemId });
    });
  };

  onEditComplete = () => {
    const { folderId, fileAction } = this.props;

    if (fileAction.type === 'create') {
      fetchFolder(folderId, store.dispatch)
    }

    this.setState({ editingId: null }, () => {
      this.props.setAction({
        type: null,
        exst: null,
        tempId: null
      });
    })
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
    const { files, folders, viewer, parentId, folderId, settings, fileAction } = this.props;
    const { editingId } = this.state;

    let items = [...folders, ...files];

    if (fileAction && fileAction.type === 'create') {
      items.unshift({
        id: -1,
        title: '',
        parentId: folderId,
        fileExst: fileAction.exst
      })
    }

    return items.length > 0 ? (
      <RowContainer useReactWindow={false}>
        {items.map(item => {
          const contextOptions = this.getFilesContextOptions(item, viewer).filter(o => o);
          const contextOptionsProps = !contextOptions.length || fileAction.type
            ? {}
            : { contextOptions };
          const checked = false; //isUserSelected(selection, user.id);
          const checkedProps = /* isAdmin(viewer) */ fileAction.type && (editingId === item.id || item.id === -1) ? {} : { checked };
          const element = item.fileExst
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
              <FilesRowContent item={item} viewer={viewer} culture={settings.culture} onEditComplete={this.onEditComplete} />
            </Row>
          );
        })}
      </RowContainer>
    ) : parentId !== 0 ? (
      <EmptyFolderContainer parentId={parentId} />
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
  { fetchFiles, deleteFile, deleteFolder, setAction }
)(withRouter(withTranslation()(SectionBodyContent)));
