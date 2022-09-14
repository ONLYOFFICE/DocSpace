import React, { useEffect } from "react";
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
  tReady,
  isLoading,
  isLoadedSettingsTree,
  setFirstLoad,
}) => {
  const { setting } = match.params;

  useEffect(() => {
    setFirstLoad(false);
  }, [setFirstLoad]);

  useEffect(() => {
    if (isLoading) {
      showLoader();
    } else {
      hideLoader();
    }
  }, [isLoading]);

  //console.log("render settings");

  useEffect(() => {
    setDocumentTitle(t("Common:Settings"));
  }, [t, tReady]);

  const inLoad = (!isLoadedSettingsTree && isLoading) || isLoading || !tReady;

  return (
    <Section isInfoPanelAvailable={false}>
      <Section.SectionHeader>
        {inLoad ? <Loaders.SectionHeader /> : <SectionHeaderContent />}
      </Section.SectionHeader>

      <Section.SectionBody>
        {inLoad ? (
          <Loaders.SettingsFiles />
        ) : (
          <SectionBodyContent setting={setting} />
        )}
      </Section.SectionBody>
    </Section>
  );
};

const Settings = withTranslation(["FilesSettings", "Common"])(PureSettings);

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
})(observer(Settings));
