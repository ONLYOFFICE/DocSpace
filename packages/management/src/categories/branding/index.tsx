import React, { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import toastr from "@docspace/components/toast/toastr";
import {
  uploadLogo,
  getNewLogoArr,
  getLogosAsText,
} from "@docspace/common/utils/whiteLabelHelper";
import { useIsSmallWindow } from "@docspace/common/utils/useIsSmallWindow";

import CommonWhiteLabel from "client/CommonWhiteLabel";
import LoaderWhiteLabel from "client/LoaderWhiteLabel";
import BreakpointWarning from "client/BreakpointWarning";

import { useStore } from "SRC_DIR/store";

const Branding = () => {
  const { t } = useTranslation(["Settings"]);
  const isSmallWindow = useIsSmallWindow(795);

  const { brandingStore } = useStore();
  const {
    initStore,
    whiteLabelLogos,
    defaultWhiteLabelLogos,
    getWhiteLabelLogoUrls,
    setWhiteLabelLogos,
    whiteLabelLogoText,
    getWhiteLabelLogoText,
    setWhiteLabelLogoText,
    isEqualLogo,
    isEqualText,
    saveWhiteLabelSettings,
    restoreDefault,
  } = brandingStore;

  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      await initStore();
      setIsLoading(false);
    };

    fetchData();
  }, []);

  const onChangeCompanyName = (e: React.FormEvent<HTMLInputElement>) => {
    setWhiteLabelLogoText(e.currentTarget.value);
  };

  const onChangeLogo = async (e: React.FormEvent<HTMLInputElement>) => {
    const target = e.currentTarget;
    const id = target.id.split("_");
    const index = Number(id[1]);
    const theme = id[2];

    let file = target.files && target.files[0];
    target.value = ""; //hack to be able to reselect a file after a default reset

    const { data } = await uploadLogo(file);

    if (data.Success) {
      const url = data.Message;
      const newArr = whiteLabelLogos;

      if (!newArr) return toastr.error("Undefined Error");
      if (theme === "light") {
        newArr[index - 1].path.light = url;
      } else if (theme === "dark") {
        newArr[index - 1].path.dark = url;
      }
      setWhiteLabelLogos(whiteLabelLogos);
    } else {
      console.error(data.Message);
      toastr.error(data.Message);
    }
  };

  const onUseTextAsLogo = () => {
    const newLogos = getLogosAsText(whiteLabelLogos, whiteLabelLogoText);
    setWhiteLabelLogos(newLogos);
  };

  const onSave = async () => {
    setIsSaving(true);
    const arr = getNewLogoArr(whiteLabelLogos, defaultWhiteLabelLogos);
    const data = {
      logoText: whiteLabelLogoText,
      logo: arr,
    };

    try {
      await saveWhiteLabelSettings(data);
      await getWhiteLabelLogoUrls();
      await getWhiteLabelLogoText();
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    } finally {
      setIsSaving(false);
    }
  };

  const onRestoreDefault = async () => {
    await restoreDefault(true);
  };

  if (isSmallWindow)
    return (
      <BreakpointWarning sectionName={t("Settings:Branding")} isSmallWindow />
    );

  return isLoading ? (
    <LoaderWhiteLabel />
  ) : (
    <CommonWhiteLabel
      isSettingPaid={true}
      logoTextWhiteLabel={whiteLabelLogoText}
      onChangeCompanyName={onChangeCompanyName}
      onUseTextAsLogo={onUseTextAsLogo}
      logoUrlsWhiteLabel={whiteLabelLogos}
      onChangeLogo={onChangeLogo}
      onSave={onSave}
      onRestoreDefault={onRestoreDefault}
      isEqualLogo={isEqualLogo}
      isEqualText={isEqualText}
      isSaving={isSaving}
    />
  );
};

export default observer(Branding);
