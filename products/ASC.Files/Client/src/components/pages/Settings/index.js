import React from 'react';
import { connect } from 'react-redux';
import styled from 'styled-components';
import { withRouter } from "react-router";

import { 
  Box,
  ToggleButton 
} from 'asc-web-components';
import { PageLayout, utils } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";

import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

import { setSettingsIsLoad } from '../../../store/files/actions'

const i18n = createI18N({
  page: "SettingsTree",
  localesPath: "pages/Settings"
})

const { changeLanguage } = utils;

const StyledSettings = styled(Box)`
  display: grid;
  
  .toggle-btn {
    display: block;
  }
`;

class PureSettings extends React.Component {
  constructor(props) {
    super(props)

    this.state = {
      intermediateVersion: false,
      thirdParty: false
    }
  }

  componentDidMount() {
    this.switchHeader();
  }

  componentDidUpdate() {
    this.switchHeader();
  }

  componentWillUnmount() {
    const { setSettingsIsLoad } = this.props;
    setSettingsIsLoad(null);
  }

  switchHeader = () => {
    const { match, setSettingsIsLoad } = this.props;
    const { setting } = match.params;
    console.log(setting)
    let header; 

    switch (setting) {
      case "common-settings":
        header = "Common Setting"; break;
      case "settings":
        header = "Common Setting"; break;
      case "admin-settings":
        header = "Admin Settings"; break;
      case "connected-clouds":
        header = "Connected Clouds"; break;
      default:
        header = null;
    }
    console.log(header)
    setSettingsIsLoad(header);
  }

  isCheckedIntermediate = () => {
    this.setState({
      intermediateVersion: !this.state.intermediateVersion
    })
  }

  isCheckedThirdParty = () => {
    this.setState({
      thirdParty: !this.state.thirdParty
    })
  }
 
  renderAdminSettings = () => {
    const { 
      intermediateVersion, 
      thirdParty 
    } = this.state;
    return (
      <StyledSettings>
        <ToggleButton 
          className="toggle-btn"
          label="Keep all saved intermediate versions"
          onChange={this.isCheckedIntermediate}
          isChecked={intermediateVersion}
        />
        <ToggleButton
          className="toggle-btn"
          label="Allow users to connect third-party storages"
          onChange={this.isCheckedThirdParty}
          isChecked={thirdParty}
        />
      </StyledSettings>
    )
  }

  renderCommonSettings = () => {
    return <span>CommonSettings</span>
  }

  renderClouds = () => {
    return <span>Clouds</span>
  }

  renderSettings = setting => {
    switch (setting) {
      case "common-settings":
        return this.renderCommonSettings();
      case "admin-settings":
        return this.renderAdminSettings();
      case "connected-clouds":
        return this.renderClouds();
      default:
        return this.renderCommonSettings();
    }
  }

  render() {
    console.log('render settings')
    const { match, t } = this.props;
    const { setting } = match.params;
    console.log(match)
    const content = this.renderSettings(setting);

    return (
      <PageLayout>
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleMainButton>
          <ArticleMainButtonContent />
        </PageLayout.ArticleMainButton>

        <PageLayout.ArticleBody>
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent title={t(`${setting}`)}/>
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent />
          {content}
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
} 

const SettingsContainer = withTranslation()(PureSettings);

const Settings = props => {
  changeLanguage(i18n);
  return (
    <I18nextProvider i18n={i18n}>
      <SettingsContainer {...props} />
    </I18nextProvider>
  );
}

function mapStateToProps(state) {
  const { settingsIsLoad } = state.files;

  return { 
    settingsIsLoad
  };
}

export default connect(
  mapStateToProps,
  {
    setSettingsIsLoad
  }
)(withRouter(Settings));