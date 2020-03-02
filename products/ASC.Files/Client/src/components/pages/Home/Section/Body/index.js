import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  Row,
  //toastr,
  Icons,
  RowContainer
} from "asc-web-components";
import EmptyFolderContainer from "./EmptyFolderContainer";
import FilesRowContent from "./FilesRowContent";

//import i18n from '../../i18n';

class SectionBodyContent extends React.PureComponent {

  render() {
    const { files, folders, viewer, user } = this.props;

    const items = [...folders, ...files];

    const fakeContext = [
      {
        key: "sharing-settings",
        label: "Sharing settings",
        onClick: () => { },
        disabled: true
      },
      {
        key: "link-for-portal-users",
        label: "Link for portal users",
        onClick: () => { },
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
        onClick: () => { },
        disabled: true
      },
      {
        key: "delete",
        label: "Delete",
        onClick: () => { },
        disabled: true
      },
    ];

    return items.length > 0 ? (
      <RowContainer useReactWindow={false}>
        {items.map(item => {
          const contextOptions = fakeContext || this.getUserContextOptions(user, viewer).filter(o => o);
          const contextOptionsProps = !contextOptions.length
            ? {}
            : { contextOptions };
          const checked = false; //isUserSelected(selection, user.id);
          const checkedProps = /* isAdmin(viewer) */ true ? { checked } : {};;
          const element = item.fileExst
            ? (<Icons.ActionsDocumentsIcon size='big' isfill={true} color="#A3A9AE" />)
            : (<Icons.CatalogFolderIcon size='big' isfill={true} color="#A3A9AE" />);

          return (
            <Row
              key={item.id}
              data={item}
              element={element}
              onSelect={() => { }}
              {...checkedProps}
              {...contextOptionsProps}
              needForUpdate={() => { }}
            >
              <FilesRowContent item={item} viewer={viewer} />
            </Row>
          );
        })}
      </RowContainer>
    ) : (
        <EmptyFolderContainer />
      );
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
    //filter: state.people.filter
  };
};

export default connect(
  mapStateToProps,
  // { selectUser, deselectUser, setSelection, updateUserStatus, resetFilter, fetchPeople }
)(withRouter(withTranslation()(SectionBodyContent)));
