import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { PageLayout, utils, Loaders } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";

const PureSettings = ({
  match,
  //history,
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

const Settings = withTranslation("Settings")(PureSettings);

export default inject(
  ({ initFilesStore, filesStore, settingsStore, treeFoldersStore }) => {
    const { isLoading } = initFilesStore;
    const { setFirstLoad } = filesStore;
    const { setSelectedNode } = treeFoldersStore;
    const { getFilesSettings, settingsTree: settings } = settingsStore;

    const settingsTree = Object.keys(settings).length !== 0 ? settings : {};

    return {
      isLoading,
      settingsTree,

      setFirstLoad,
      setSelectedNode,
      getFilesSettings,
    };
  }
)(withRouter(observer(Settings)));
