import React from "react";
import { withRouter } from "react-router";
import { Headline } from 'asc-web-common';
import { withTranslation } from 'react-i18next';
import { getKeyByLink, settingsTree, getTKeyByKey } from '../../../utils';

class SectionHeaderContent extends React.Component {

  constructor(props) {
    super(props);

    const { match, location } = props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;

    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const header = getTKeyByKey(key, settingsTree);
    this.state = {
      header
    };

  }

  componentDidUpdate() {
    const { match, location } = this.props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;


    const fullSettingsUrlLength = fullSettingsUrl.length;
    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const header = getTKeyByKey(key, settingsTree);
    header !== this.state.header && this.setState({ header });

  }

  render() {
    const { t } = this.props;
    const { header } = this.state;

    return (
      <Headline type='content' truncate={true}>
        {t(header)}
      </Headline>
    );
  }
};

export default withRouter(withTranslation()(SectionHeaderContent));
