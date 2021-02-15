import React, { useEffect, useState } from "react";
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
import {
  getFilesSettings,
  //setFirstLoad,
  //setSelectedNode,
} from "../../../store/files/actions";
import {
  getSettingsTree /* getIsLoading */,
} from "../../../store/files/selectors";

import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings",
});

const { changeLanguage } = utils;

const PureSettings = ({
  match,
  history,
  t,
  isLoading,
  settingsTree,
  setFirstLoad,
}) => {
  const [title, setTitle] = useState("");
  const { setting } = match.params;

  useEffect(() => {
    setFirstLoad(false);
  }, [setFirstLoad]);

  useEffect(() => {
    switch (setting) {
      case "common":
        setTitle("CommonSettings");
        break;
      case "admin":
        setTitle("AdminSettings");
        break;
      case "thirdParty":
        setTitle("ThirdPartySettings");
        break;
      default:
        setTitle("CommonSettings");
        break;
    }
  }, [setting]);

  useEffect(() => {
    if (isLoading) {
      utils.showLoader();
    } else {
      utils.hideLoader();
    }
  }, [isLoading]);

  //console.log("render settings");

  useEffect(() => {
    setDocumentTitle(t(`${title}`));
  }, [title, t]);

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
          {(Object.keys(settingsTree).length === 0 && isLoading) ||
          isLoading ? (
            <Loaders.SectionHeader />
          ) : (
            <SectionHeaderContent title={t(`${title}`)} />
          )}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {(Object.keys(settingsTree).length === 0 && isLoading) ||
          isLoading ? (
            setting === "thirdParty" ? (
              <Loaders.Rows />
            ) : (
              <Loaders.SettingsFiles />
            )
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
    //isLoading: getIsLoading(state),
    settingsTree: getSettingsTree(state),
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    getFilesSettings: () => dispatch(getFilesSettings()),
    //setFirstLoad: (firstLoad) => dispatch(setFirstLoad(firstLoad)),
    //setSelectedNode: (node) => dispatch(setSelectedNode(node)),
  };
};

// export default connect(
//   mapStateToProps,
//   mapDispatchToProps
// )(withRouter(Settings));

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(
  inject(({ mainFilesStore }) => {
    const { filesStore, isLoading } = mainFilesStore;
    const { setFirstLoad, treeFoldersStore } = filesStore;
    const { setSelectedNode } = treeFoldersStore;

    return {
      isLoading,

      setFirstLoad,
      setSelectedNode,
    };
  })(withRouter(observer(Settings)))
);
