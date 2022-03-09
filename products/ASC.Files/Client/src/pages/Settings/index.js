import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import PageLayout from "@appserver/common/components/PageLayout";
import Loaders from "@appserver/common/components/Loaders";
import { showLoader, hideLoader } from "@appserver/common/utils";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../components/Article";
import {
  CatalogBodyContent,
  CatalogHeaderContent,
  CatalogMainButtonContent,
} from "../../components/Catalog";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "../../helpers/utils";
import { inject, observer } from "mobx-react";

const PureSettings = ({
  match,
  t,
  isLoading,
  isLoadedSettingsTree,
  setFirstLoad,
  tReady,
  showCatalog,
}) => {
  const [title, setTitle] = useState("");
  const { setting } = match.params;

  useEffect(() => {
    setFirstLoad(false);
  }, [setFirstLoad]);

  useEffect(() => {
    switch (setting) {
      case "common":
        setTitle(t("CommonSettings"));
        break;
      case "admin":
        setTitle(t("Common:AdminSettings"));
        break;
      case "thirdParty":
        setTitle(t("ThirdPartySettings"));
        break;
      default:
        setTitle(t("CommonSettings"));
        break;
    }
  }, [setting, t, tReady]);

  useEffect(() => {
    if (isLoading) {
      showLoader();
    } else {
      hideLoader();
    }
  }, [isLoading]);

  //console.log("render settings");

  useEffect(() => {
    setDocumentTitle(title);
  }, [title, t]);

  return (
    <>
      <PageLayout>
        {!showCatalog && (
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>
        )}

        {!showCatalog && (
          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent isDisabled={true} />
          </PageLayout.ArticleMainButton>
        )}

        {!showCatalog && (
          <PageLayout.ArticleBody>
            <ArticleBodyContent />
          </PageLayout.ArticleBody>
        )}

        {showCatalog && (
          <PageLayout.CatalogHeader>
            <CatalogHeaderContent />
          </PageLayout.CatalogHeader>
        )}
        {showCatalog && (
          <PageLayout.CatalogMainButton>
            <CatalogMainButtonContent />
          </PageLayout.CatalogMainButton>
        )}
        {showCatalog && (
          <PageLayout.CatalogBody>
            <CatalogBodyContent />
          </PageLayout.CatalogBody>
        )}

        <PageLayout.SectionHeader>
          {(!isLoadedSettingsTree && isLoading) || isLoading || !tReady ? (
            <Loaders.SectionHeader />
          ) : (
            <SectionHeaderContent title={title} />
          )}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {(!isLoadedSettingsTree && isLoading) || isLoading || !tReady ? (
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

const Settings = withTranslation(["Settings", "Common"])(PureSettings);

export default inject(
  ({ auth, filesStore, settingsStore, treeFoldersStore }) => {
    const { setFirstLoad, isLoading } = filesStore;
    const { setSelectedNode } = treeFoldersStore;
    const { getFilesSettings, isLoadedSettingsTree } = settingsStore;

    return {
      isLoading,
      isLoadedSettingsTree,

      setFirstLoad,
      setSelectedNode,
      getFilesSettings,

      showCatalog: auth.settingsStore.showCatalog,
    };
  }
)(withRouter(observer(Settings)));
