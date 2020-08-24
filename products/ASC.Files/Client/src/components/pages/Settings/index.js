import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from "react-router";
import { PageLayout, utils } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";

import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings"
})

const { changeLanguage } = utils;

class PureSettings extends React.Component {
  constructor(props) {
    super(props)

    this.state = {
      isLoading: false,
      intermediateVersion: false,
      thirdParty: false,
      originalCopy: false,
      trash: false,
      recent: false,
      favorites: false,
      templates: false,
      updateOrCreate: false,
      keepIntermediate: false
    }
  }

  onLoading = status => {
    this.setState({ isLoading: status });
  };

  render() {
    console.log('Settings render()');
    const { 
      intermediateVersion,
      thirdParty,
      originalCopy,
      trash,
      recent,
      favorites,
      templates,
      updateOrCreate,
      keepIntermediate,
    } = this.state;
    const { match, t } = this.props;
    const { setting } = match.params;

    return (
      <PageLayout>
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleMainButton>
          <ArticleMainButtonContent onLoading={this.onLoading} />
        </PageLayout.ArticleMainButton>

        <PageLayout.ArticleBody>
          <ArticleBodyContent 
            onLoading={this.onLoading}
            isLoading={this.state.isLoading}
          />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent title={t(`${setting}`)}/>
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent
            setting={setting}
            thirdParty={thirdParty}
            intermediateVersion={intermediateVersion}
            originalCopy={originalCopy}
            trash={trash}
            recent={recent}
            favorites={favorites}
            templates={templates}
            updateOrCreate={updateOrCreate}
            keepIntermediate={keepIntermediate}
            t={t}
            onLoading={this.onLoading}
            isCheckedThirdParty={this.isCheckedThirdParty}
            isCheckedIntermediate={this.isCheckedIntermediate}
          />
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
  return {};
}

export default connect(mapStateToProps)(withRouter(Settings));