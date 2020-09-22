import React, { useEffect } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { PageLayout, utils } from "asc-web-common";
import { RequestLoader } from "asc-web-components";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import { 
  setIsErrorSettings,
  getFilesSettings,
  setIsLoading
} from "../../../store/files/actions";

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings"
});

const { changeLanguage } = utils;

const PureSettings = ({
  match,
  t,
  isLoading,
  setIsErrorSettings,
  getFilesSettings,
  setIsLoading
}) => {
  //console.log("Settings render()");
  const { setting } = match.params;

  useEffect(() => {
    setIsLoading(true);
    getFilesSettings()
      .then(() => setIsLoading(false))
      .catch(e => {
        setIsErrorSettings(true);
        setIsLoading(false)
      });
  }, []);

  return (
    <>
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
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent title={t(`${setting}`)} />
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent setting={setting} t={t} />
        </PageLayout.SectionBody>
      </PageLayout>
    </>
  );
};

const SettingsContainer = withTranslation()(PureSettings);

const Settings = props => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <SettingsContainer {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    isLoading: state.files.isLoading
  };
}

export default connect(
  mapStateToProps,
  {
    setIsErrorSettings,
    getFilesSettings,
    setIsLoading
  }
)(withRouter(Settings));
