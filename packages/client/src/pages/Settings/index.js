import React, { useEffect, useState } from "react";

import { withRouter } from "react-router";
import Section from "@docspace/common/components/Section";
import Loaders from "@docspace/common/components/Loaders";
import { showLoader, hideLoader } from "@docspace/common/utils";

import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import { inject, observer } from "mobx-react";

const PureSettings = ({
  match,
  t,
  isLoading,
  isLoadedSettingsTree,
  history,
  setFirstLoad,
  capabilities,
  tReady,
  isPersonal,
}) => {
  const [title, setTitle] = useState("");
  const { setting } = match.params;

  useEffect(() => {
    setFirstLoad(false);
  }, [setFirstLoad]);

  useEffect(() => {
    isPersonal
      ? setTitle(t("ThirdPartySettings"))
      : setTitle(t("Common:Settings"));
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
    <Section isInfoPanelAvailable={false}>
      <Section.SectionHeader>
        {(!isLoadedSettingsTree && isLoading) || isLoading || !tReady ? (
          <Loaders.SectionHeader />
        ) : (
          <SectionHeaderContent title={title} />
        )}
      </Section.SectionHeader>

      <Section.SectionBody>
        {(!isLoadedSettingsTree && isLoading) ||
        isLoading ||
        !tReady ||
        !capabilities ? (
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

const Settings = withTranslation(["FilesSettings", "Common"])(PureSettings);

export default inject(
  ({ auth, filesStore, settingsStore, treeFoldersStore }) => {
    const { setFirstLoad, isLoading } = filesStore;
    const { setSelectedNode } = treeFoldersStore;
    const {
      getFilesSettings,
      isLoadedSettingsTree,
      thirdPartyStore,
    } = settingsStore;
    const { capabilities } = thirdPartyStore;

    return {
      isLoading,
      isLoadedSettingsTree,
      setFirstLoad,
      setSelectedNode,
      getFilesSettings,
      capabilities,
      isPersonal: auth.settingsStore.personal,
    };
  }
)(withRouter(observer(Settings)));
