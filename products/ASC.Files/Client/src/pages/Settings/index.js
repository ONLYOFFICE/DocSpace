import React, { useEffect, useState } from "react";

import { withRouter } from "react-router";
import Section from "@appserver/common/components/Section";
import Loaders from "@appserver/common/components/Loaders";
import { showLoader, hideLoader } from "@appserver/common/utils";

import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "../../helpers/utils";
import { inject, observer } from "mobx-react";

const PureSettings = ({
  match,
  t,
  isLoading,
  isLoadedSettingsTree,
  history,
  setFirstLoad,
  tReady,
}) => {
  const [title, setTitle] = useState("");
  const { setting } = match.params;

  useEffect(() => {
    setFirstLoad(false);
  }, [setFirstLoad]);

  useEffect(() => {
    setTitle(t("Common:Settings"));
  }, [t, tReady]);

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
    <Section>
      <Section.SectionHeader>
        {(!isLoadedSettingsTree && isLoading) || isLoading || !tReady ? (
          <Loaders.SectionHeader />
        ) : (
          <SectionHeaderContent title={title} />
        )}
      </Section.SectionHeader>

      <Section.SectionBody>
        {(!isLoadedSettingsTree && isLoading) || isLoading || !tReady ? (
          setting === "thirdParty" ? (
            <Loaders.Rows />
          ) : (
            <Loaders.SettingsFiles />
          )
        ) : (
          <SectionBodyContent
            title={title}
            setting={setting}
            history={history}
            t={t}
          />
        )}
      </Section.SectionBody>
    </Section>
  );
};

const Settings = withTranslation(["Settings", "Common"])(PureSettings);

export default inject(({ filesStore, settingsStore, treeFoldersStore }) => {
  const { setFirstLoad, isLoading } = filesStore;
  const { setSelectedNode } = treeFoldersStore;
  const { getFilesSettings, isLoadedSettingsTree } = settingsStore;

  return {
    isLoading,
    isLoadedSettingsTree,
    setFirstLoad,
    setSelectedNode,
    getFilesSettings,
  };
})(withRouter(observer(Settings)));
