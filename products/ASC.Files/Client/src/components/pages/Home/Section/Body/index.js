import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  Row,
  Avatar,
  toastr,
  EmptyScreenContainer,
  Icons,
  Link,
  RowContainer,
  RowContent,
  Text,
  Badge,
  utils
} from "asc-web-components";

import i18n from '../../i18n';

class SectionBodyContent extends React.PureComponent {

  componentDidMount() {

  }

  badgeStyle = (isFile) => {
    return {
      marginRight: '8px',
      marginTop: isFile ? '-4px' : '0px'
    }
  };

  render() {
    const { files, folders, viewer, user } = this.props;

    const items = [...folders, ...files];

    const extStyle = {
      marginLeft: '-8px',
      marginRight: '8px'
    }

    return (
      <RowContainer useReactWindow={false}>
        {items.map(item => {
          const contextOptions = [] || this.getUserContextOptions(user, viewer).filter(o => o);
          const contextOptionsProps = !contextOptions.length
            ? {}
            : { contextOptions };
          //const checked = isUserSelected(selection, user.id);
          const checkedProps = {};
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
              <RowContent
                sideColor="#333"
              >
                <Link
                  type='page'
                  title={item.title}
                  fontWeight="bold"
                  onClick={() => { }}
                  fontSize='15px'
                  color="#333"
                  isTextOverflow={true}
                >
                  {item.title}
                </Link>
                <div>
                  {item.fileExst &&
                    <>
                      <Text
                        style={extStyle}
                        as="span"
                        color="#A3A9AE"
                        fontSize='15px'
                        fontWeight={600}
                        title={item.fileExst}
                        truncate={true}
                      >
                        {item.fileExst}
                      </Text>
                      <Icons.FileActionsConvertIcon style={this.badgeStyle(item.fileExst)} size='small' isfill={true} color='#A3A9AE' />
                      <Icons.FileActionsConvertEditDocIcon style={this.badgeStyle(item.fileExst)} size='small' isfill={true} color='#3B72A7' />
                      <Icons.FileActionsLockedIcon style={this.badgeStyle(item.fileExst)} size='small' isfill={true} color='#3B72A7' />
                    </>
                  }
                </div>
                <Text
                  containerWidth='10%'
                  as="div"
                  color="#333"
                  fontSize='12px'
                  fontWeight={600}
                  title={item.createdBy.displayName}
                  truncate={true}
                >
                  {item.createdBy.displayName}
                </Text>
                <Link
                  containerWidth='12%'
                  type='page'
                  title={item.created}
                  fontSize='12px'
                  fontWeight={400}
                  color="#333"
                  onClick={() => { }}
                  isTextOverflow={true}
                >
                  {item.created}
                </Link>
                <Text
                  containerWidth='10%'
                  as="div"
                  color="#333"
                  fontSize='12px'
                  fontWeight={600}
                  title=''
                  truncate={true}
                >
                  {item.fileExst 
                    ? item.contentLength
                    : `Dcs: ${item.filesCount} / Flds: ${item.filesCount}`}
                </Text>
              </RowContent>
            </Row>
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
