import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  Text
} from "asc-web-components";

import i18n from '../../i18n';

class SectionBodyContent extends React.PureComponent {

  componentDidMount() {

  }

  render() {
    console.log("Home SectionBodyContent render()");
    return <Text>There must be the list of files and folders</Text>
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
    viewer: state.auth.user,
    settings: state.auth.settings,
    //filter: state.people.filter
  };
};

export default connect(
  mapStateToProps,
  // { selectUser, deselectUser, setSelection, updateUserStatus, resetFilter, fetchPeople }
)(withRouter(withTranslation()(SectionBodyContent)));
