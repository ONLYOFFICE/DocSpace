import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import toastr from "@docspace/components/toast/toastr";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import LoaderWhiteLabel from "../sub-components/loaderWhiteLabel";

import {
  generateLogo,
  getLogoOptions,
  uploadLogo,
} from "@docspace/common/utils/whiteLabelHelper";
import CommonWhiteLabel from "./CommonWhiteLabel";

import isEqual from "lodash/isEqual";

const WhiteLabel = ({
  isSettingPaid,
  logoText,
  logoUrls,
  restoreWhiteLabelSettings,
  getWhiteLabelLogoUrls,
  setWhiteLabelSettings,
  defaultWhiteLabelLogoUrls,
  getWhiteLabelLogoText,
  getWhiteLabelLogoUrlsAction,
}) => {
  const { t } = useTranslation("Settings");

  const [isLoadedData, setIsLoadedData] = useState(false);
  const [logoTextWhiteLabel, setLogoTextWhiteLabel] = useState("");
  const [defaultLogoTextWhiteLabel, setDefaultLogoTextWhiteLabel] = useState(
    ""
  );

  const [logoUrlsWhiteLabel, setLogoUrlsWhiteLabel] = useState(null);
  const [isSaving, setIsSaving] = useState(false);

  const companyNameFromSessionStorage = getFromSessionStorage("companyName");

  useEffect(() => {
    if (!companyNameFromSessionStorage) {
      setLogoTextWhiteLabel(logoText);
      saveToSessionStorage("companyName", logoText);
    } else {
      setLogoTextWhiteLabel(companyNameFromSessionStorage);
      saveToSessionStorage("companyName", companyNameFromSessionStorage);
    }
  }, [logoText]);

  useEffect(() => {
    if (logoUrls) {
      setLogoUrlsWhiteLabel(logoUrls);
    }
  }, [logoUrls]);

  useEffect(() => {
    if (logoTextWhiteLabel && logoUrlsWhiteLabel.length && !isLoadedData) {
      setDefaultLogoTextWhiteLabel(logoText);
      setIsLoadedData(true);
    }
  }, [isLoadedData, logoTextWhiteLabel, logoUrlsWhiteLabel]);

  const onResetCompanyName = async () => {
    const whlText = await getWhiteLabelLogoText();
    saveToSessionStorage("companyName", whlText);
    setLogoTextWhiteLabel(logoText);
  };

  const onChangeCompanyName = (e) => {
    console.log(defaultLogoTextWhiteLabel);

    const value = e.target.value;
    setLogoTextWhiteLabel(value);
    saveToSessionStorage("companyName", value);
  };

  const onUseTextAsLogo = () => {
    let newLogos = logoUrlsWhiteLabel;
    for (let i = 0; i < logoUrlsWhiteLabel.length; i++) {
      const width = logoUrlsWhiteLabel[i].size.width / 2;
      const height = logoUrlsWhiteLabel[i].size.height / 2;
      const options = getLogoOptions(i, logoTextWhiteLabel);
      const isDocsEditorName = logoUrlsWhiteLabel[i].name === "DocsEditor";

      const logoLight = generateLogo(
        width,
        height,
        options.text,
        options.fontSize,
        isDocsEditorName ? "#fff" : "#000",
        options.isEditorLogo
      );
      const logoDark = generateLogo(
        width,
        height,
        options.text,
        options.fontSize,
        "#fff",
        options.isEditorLogo
      );
      newLogos[i].path.light = logoLight;
      newLogos[i].path.dark = logoDark;
    }

    setLogoUrlsWhiteLabel(newLogos);
  };

  const onRestoreDefault = async () => {
    try {
      await restoreWhiteLabelSettings(true);
      await getWhiteLabelLogoUrls();
      await getWhiteLabelLogoUrlsAction(); //TODO: delete duplicate request
      await onResetCompanyName();
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }
  };

  const onChangeLogo = async (e) => {
    const id = e.target.id.split("_");
    const index = id[1];
    const theme = id[2];

    let file = e.target.files[0];

    const { data } = await uploadLogo(file);

    if (data.Success) {
      const url = data.Message;
      const newArr = logoUrlsWhiteLabel;

      if (theme === "light") {
        newArr[index - 1].path.light = url;
      } else if (theme === "dark") {
        newArr[index - 1].path.dark = url;
      }

      setLogoUrlsWhiteLabel(newArr);
    } else {
      console.error(data.Message);
      toastr.error(data.Message);
    }
  };

  const onSave = async () => {
    let logosArr = [];

    for (let i = 0; i < logoUrlsWhiteLabel.length; i++) {
      const currentLogo = logoUrlsWhiteLabel[i];
      const defaultLogo = defaultWhiteLabelLogoUrls[i];

      if (!isEqual(currentLogo, defaultLogo)) {
        let value = {};

        if (!isEqual(currentLogo.path.light, defaultLogo.path.light))
          value.light = currentLogo.path.light;
        if (!isEqual(currentLogo.path.dark, defaultLogo.path.dark))
          value.dark = currentLogo.path.dark;

        logosArr.push({
          key: String(i + 1),
          value: value,
        });
      }
    }
    const data = {
      logoText: logoTextWhiteLabel,
      logo: logosArr,
    };

    try {
      setIsSaving(true);
      await setWhiteLabelSettings(data);
      await getWhiteLabelLogoUrls();
      await getWhiteLabelLogoUrlsAction(); //TODO: delete duplicate request
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    } finally {
      setIsSaving(false);
    }
  };

  const isEqualLogo = isEqual(logoUrlsWhiteLabel, defaultWhiteLabelLogoUrls);
  const isEqualText = defaultLogoTextWhiteLabel === logoTextWhiteLabel;

  return !isLoadedData ? (
    <LoaderWhiteLabel />
  ) : (
    <CommonWhiteLabel
      isSettingPaid={isSettingPaid}
      logoTextWhiteLabel={logoTextWhiteLabel}
      onChangeCompanyName={onChangeCompanyName}
      onUseTextAsLogo={onUseTextAsLogo}
      logoUrlsWhiteLabel={logoUrlsWhiteLabel}
      onChangeLogo={onChangeLogo}
      onSave={onSave}
      onRestoreDefault={onRestoreDefault}
      isEqualLogo={isEqualLogo}
      isEqualText={isEqualText}
      isSaving={isSaving}
    />
  );
};

export default inject(({ setup, auth, common }) => {
  const { setWhiteLabelSettings } = setup;

  const {
    whiteLabelLogoText,
    getWhiteLabelLogoText,
    whiteLabelLogoUrls,
    restoreWhiteLabelSettings,
    getWhiteLabelLogoUrls: getWhiteLabelLogoUrlsAction,
  } = common;

  const {
    getWhiteLabelLogoUrls,
    whiteLabelLogoUrls: defaultWhiteLabelLogoUrls,
  } = auth.settingsStore;

  return {
    theme: auth.settingsStore.theme,
    logoText: whiteLabelLogoText,
    logoUrls: whiteLabelLogoUrls,
    getWhiteLabelLogoText,
    getWhiteLabelLogoUrls,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
    defaultWhiteLabelLogoUrls,
    getWhiteLabelLogoUrlsAction,
  };
})(observer(WhiteLabel));
