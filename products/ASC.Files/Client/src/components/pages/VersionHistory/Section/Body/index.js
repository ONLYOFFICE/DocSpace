import React from "react";
import { withRouter } from "react-router";
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
    return <Text>There must be the list of the file's version info</Text>
  }
}

export default withRouter(withTranslation()(SectionBodyContent));
