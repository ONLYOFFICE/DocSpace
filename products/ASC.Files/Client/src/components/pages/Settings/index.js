import React, { useEffect } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { PageLayout, utils, Loaders } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import { getFilesSettings, setIsLoading } from "../../../store/files/actions";
import { getSettingsTree, getIsLoading } from "../../../store/files/selectors";

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings",
});

const { changeLanguage } = utils;

const PureSettings = ({
  match,
  t,
  isLoading,
  settingsTree,
  getFilesSettings,
  setIsLoading,
}) => {
  //console.log("Settings render()");
  const { setting } = match.params;

  useEffect(() => {
    setIsLoading(true);
    getFilesSettings().then(() => {
      setIsLoading(false);
    });
  }, [getFilesSettings, setIsLoading]);

  useEffect(() => {
    if (isLoading) {
      utils.showLoader();
    } else {
      utils.hideLoader();
    }
  }, [isLoading]);

  //console.log("render settings");

  return (
    <>
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
          {Object.keys(settingsTree).length === 0 && isLoading ? (
            <Loaders.SectionHeader />
          ) : (
            <SectionHeaderContent setting={setting} t={t} />
          )}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {Object.keys(settingsTree).length === 0 && isLoading ? (
            <Loaders.SettingsFiles />
          ) : (
            <SectionBodyContent setting={setting} t={t} />
          )}
        </PageLayout.SectionBody>
      </PageLayout>
    </>
  );
};

const SettingsContainer = withTranslation()(PureSettings);

const Settings = (props) => {
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
    isLoading: getIsLoading(state),
    settingsTree: getSettingsTree(state),
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    setIsLoading: (isLoading) => dispatch(setIsLoading(isLoading)),
    getFilesSettings: () => dispatch(getFilesSettings()),
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(withRouter(Settings));
