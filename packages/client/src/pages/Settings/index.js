import React, { useEffect } from "react";
import Section from "@docspace/common/components/Section";
import Loaders from "@docspace/common/components/Loaders";
import { showLoader, hideLoader } from "@docspace/common/utils";

import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import { inject, observer } from "mobx-react";

const PureSettings = ({
  t,
  tReady,
  isLoading,
  isLoadedSettingsTree,
  setFirstLoad,
  isAdmin,
}) => {
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

  const setting = window.location.pathname.endsWith("/settings/common")
    ? "common"
    : "admin";

  return (
    <Section isInfoPanelAvailable={false} viewAs={"settings"}>
      <Section.SectionHeader>
        {inLoad ? <Loaders.SettingsHeader /> : <SectionHeaderContent />}
      </Section.SectionHeader>

      <Section.SectionBody>
        {inLoad ? (
          setting === "common" ? (
            <Loaders.SettingsCommon isAdmin={isAdmin} />
          ) : (
            <Loaders.SettingsAdmin />
          )
        ) : (
          <SectionBodyContent />
        )}
      </Section.SectionBody>
    </Section>
  );
};

/* 
SettingsHeader,
  SettingsAdmin,
  SettingsCommon

*/

const Settings = withTranslation(["FilesSettings", "Common"])(PureSettings);

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
      isAdmin: auth.isAdmin,
    };
  }
)(observer(Settings));
