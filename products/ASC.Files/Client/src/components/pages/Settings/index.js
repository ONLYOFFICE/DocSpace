import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from "react-router";
import { PageLayout, utils, Error403 } from "asc-web-common";
import { RequestLoader } from "asc-web-components";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { setIsLoading } from '../../../store/files/actions';

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
      intermediateVersion: false,
      originalCopy: false,
      trash: false,
      recent: false,
      favorites: false,
      templates: false,
      updateOrCreate: false,
      keepIntermediate: false
    }
  }

  render() {
    console.log('Settings render()');
    const { 
      intermediateVersion,
      originalCopy,
      trash,
      recent,
      favorites,
      templates,
      updateOrCreate,
      keepIntermediate,
    } = this.state;
    const { match, t, isLoading, setIsLoading, thirdParty, isAdmin } = this.props;
    const { setting } = match.params;

    const settings = <>
      <RequestLoader
        visible={isLoading}
        zIndex={256}
        loaderSize="16px"
        loaderColor={"#999"}
        label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
        fontSize="12px"
        fontColor={"#999"}
      />
      <PageLayout>
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleMainButton>
          <ArticleMainButtonContent isDisabled={true} />
        </PageLayout.ArticleMainButton>

        <PageLayout.ArticleBody>
          <ArticleBodyContent 
            onLoading={setIsLoading}
            isLoading={isLoading}
          
          />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent title={t(`${setting}`)}/>
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent
            setting={setting}
            intermediateVersion={intermediateVersion}
            originalCopy={originalCopy}
            trash={trash}
            recent={recent}
            favorites={favorites}
            templates={templates}
            updateOrCreate={updateOrCreate}
            keepIntermediate={keepIntermediate}
            t={t}
            onLoading={setIsLoading}
            isCheckedThirdParty={this.isCheckedThirdParty}
            isCheckedIntermediate={this.isCheckedIntermediate}
          />
        </PageLayout.SectionBody>
      </PageLayout>
    </>;

    return (!thirdParty && setting === 'thirdParty') || (!isAdmin && setting === 'admin') 
      ? <Error403 /> 
      : settings;
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
  return {
    isLoading: state.files.isLoading,
    thirdParty: state.files.settingsTree.thirdParty,
    isAdmin: state.auth.user.isAdmin
  };
}

export default connect(
  mapStateToProps,
  {
    setIsLoading
  }
  )(withRouter(Settings));