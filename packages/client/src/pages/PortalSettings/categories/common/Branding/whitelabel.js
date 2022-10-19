import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import FieldContainer from "@docspace/components/field-container";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import TextInput from "@docspace/components/text-input";
import HelpButton from "@docspace/components/help-button";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import Badge from "@docspace/components/badge";

import { Base } from "@docspace/components/themes";
import LoaderWhiteLabel from "../sub-components/loaderWhiteLabel";

const StyledComponent = styled.div`
  .subtitle {
    margin-top: 5px;
    margin-bottom: 20px;
  }

  .header-container {
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .wl-subtitle {
    margin-top: 8px;
    margin-bottom: 20px;
  }

  .wl-helper {
    display: flex;
    gap: 4px;
    align-items: center;
    margin-bottom: 16px;
  }

  .use-as-logo {
    margin-top: 12px;
    margin-bottom: 24px;
  }

  .input {
    max-width: 350px;
  }

  .logos-container {
    display: flex;
    flex-direction: column;
    gap: 40px;
  }

  .logo-wrapper {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .border-img {
    border: ${(props) =>
      props.theme.client.settings.common.whiteLabel.borderImg};
    box-sizing: content-box;
  }

  .logo-header {
    width: 142px;
    height: 23px;
    padding: 10px;
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.backgroundColor};
  }

  .logo-compact {
    width: 56px;
    height: 56px;
  }

  .logo-big {
    max-width: 216px;
    max-height: 35px;
    padding: 10px;
  }

  .logo-favicon {
    width: 32px;
    height: 32px;
  }

  .logo-docs-editor {
    width: 86px;
    height: 20px;
    padding: 10px;
  }

  .background-green {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.greenBackgroundColor};
  }

  .background-blue {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.blueBackgroundColor};
  }

  .background-orange {
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.orangeBackgroundColor};
  }

  .hidden {
    display: none;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }
`;

StyledComponent.defaultProps = { theme: Base };

const WhiteLabel = (props) => {
  const {
    isSettingPaid,
    logoText,
    logoSizes,
    logoUrls,
    t,
    theme,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
  } = props;

  const mapSizesToArray = (sizes) => {
    return sizes.map((size) => {
      return { height: size.height, width: size.width };
    });
  };

  const [logoSizesWhiteLabel, setLogoSizes] = useState(
    mapSizesToArray(logoSizes)
  );

  const [isLoadedData, setIsLoadedData] = useState(false);
  const [isCanvasProcessing, setIsCanvasProcessing] = useState(false);
  const [isUseTextAsLogo, setIsUseTextAsLogo] = useState(false);

  const [logoTextWhiteLabel, setLogoTextWhiteLabel] = useState(null);
  const [logoUrlsWhiteLabel, setLogoUrlsWhiteLabel] = useState(null);
  const [logoUrlsChange, setLogoUrlsChange] = useState([]);

  const [portalHeaderLabel, setPortalHeaderLabel] = useState();
  const [logoCompactLabel, setLogoCompactLabel] = useState();
  const [loginPageLabel, setLoginPageLabel] = useState();
  const [logoAboutLabel, setLogoAboutLabel] = useState();
  const [faviconLabel, setFaviconLabel] = useState();
  const [editorsHeaderLabel, setEditorsHeaderLabel] = useState();
  const [logoEditorsEmbeddedLabel, setLogoEditorsEmbeddedLabel] = useState();

  useEffect(() => {
    if (logoText) {
      setLogoTextWhiteLabel(logoText);
    }
  }, [logoText]);

  useEffect(() => {
    if (logoUrls) {
      setLogoUrlsWhiteLabel(logoUrls);
    }
  }, [logoUrls]);

  useEffect(() => {
    if (
      logoTextWhiteLabel &&
      logoSizes.length &&
      logoUrlsWhiteLabel.length &&
      !isLoadedData
    ) {
      setIsLoadedData(true);
    }
  }, [
    isLoadedData,
    logoTextWhiteLabel,
    logoSizesWhiteLabel,
    logoUrlsWhiteLabel,
  ]);

  useEffect(() => {
    const isSizesExist = logoSizesWhiteLabel.length;

    const portalHeaderLabel = isSizesExist
      ? `${t("LogoLightSmall")} (${logoSizesWhiteLabel[0].width}x${
          logoSizesWhiteLabel[0].height
        }):`
      : "";

    const logoCompactLabel = isSizesExist
      ? `${t("LogoCompact")} (${logoSizesWhiteLabel[5].width}x${
          logoSizesWhiteLabel[5].height
        }):`
      : "";

    const loginPageLabel = isSizesExist
      ? `${t("LogoLogin")} (${logoSizesWhiteLabel[1].width}x${
          logoSizesWhiteLabel[1].height
        }):`
      : "";

    const logoAboutLabel = isSizesExist
      ? `${t("LogoAbout")} (${logoSizesWhiteLabel[6].width}x${
          logoSizesWhiteLabel[6].height
        }):`
      : "";

    const faviconLabel = isSizesExist
      ? `${t("LogoFavicon")} (${logoSizesWhiteLabel[2].width}x${
          logoSizesWhiteLabel[2].height
        }):`
      : "";

    const editorsHeaderLabel = isSizesExist
      ? `${t("LogoDocsEditor")} (${logoSizesWhiteLabel[3].width}x${
          logoSizesWhiteLabel[3].height
        }):`
      : "";

    const editorsEmbeddedLabel = isSizesExist
      ? `${t("LogoDocsEditorEmbedded")} (${logoSizesWhiteLabel[4].width}x${
          logoSizesWhiteLabel[4].height
        }):`
      : "";

    setPortalHeaderLabel(portalHeaderLabel);
    setLogoCompactLabel(logoCompactLabel);
    setLoginPageLabel(loginPageLabel);
    setLogoAboutLabel(logoAboutLabel);
    setFaviconLabel(faviconLabel);
    setEditorsHeaderLabel(editorsHeaderLabel);
    setLogoEditorsEmbeddedLabel(editorsEmbeddedLabel);
  }, [logoSizesWhiteLabel, t]);

  useEffect(() => {
    if (isCanvasProcessing) {
      const canvas = document.querySelectorAll("[id^=canvas_logo_]");
      const canvasLength = canvas.length;

      const text = logoTextWhiteLabel;

      for (let i = 0; i < canvasLength; i++) {
        const cnv = canvas[i];
        const fontsize = cnv.getAttribute("data-fontsize");
        const fontcolor = cnv.getAttribute("data-fontcolor");
        let logotype = cnv.getAttribute("id").replace("canvas_logo_", "");
        const x = parseInt(logotype) === 3 ? cnv.width / 2 : 0;
        let firstChar = text.trim().charAt(0);
        const firstCharCode = firstChar.charCodeAt(0);
        const ctx = cnv.getContext("2d");

        if (logotype.indexOf("_") !== -1) logotype = logotype.split("_")[0]; // for docs editor

        if (firstCharCode >= 0xd800 && firstCharCode <= 0xdbff)
          firstChar = text.trim().substr(0, 2); // Note: for surrogates pairs only

        ctx.fillStyle = "transparent";
        ctx.clearRect(0, 0, cnv.width, cnv.height);
        ctx.fillStyle = fontcolor;
        ctx.textAlign = parseInt(logotype) === 3 ? "center" : "start";
        ctx.textBaseline = "top";

        ctx.font = fontsize + "px Arial";

        ctx.fillText(
          parseInt(logotype) === 3 ? firstChar : text,
          x,
          (cnv.height - parseInt(fontsize)) / 2
        );
      }
    }
    setIsUseTextAsLogo(false);
  }, [isCanvasProcessing, isUseTextAsLogo]);

  const onUseTextAsLogo = () => {
    setIsCanvasProcessing(true);
    setIsUseTextAsLogo(true);
    setLogoUrlsChange([]);
  };

  const onChangeCompanyName = (e) => {
    const value = e.target.value;
    setLogoTextWhiteLabel(value);
  };

  const onRestoreLogo = () => {
    restoreWhiteLabelSettings(true);
    setIsCanvasProcessing(false);
  };

  const onSave = () => {
    let fd = new FormData();
    fd.append("logoText", logoTextWhiteLabel);

    for (let i = 0; i < 7; i++) {
      fd.append(`logo[${i}][key]`, i + 1);
      fd.append(`logo[${i}][value]`, logoUrlsWhiteLabel[i]);
    }

    const data = new URLSearchParams(fd);
    console.log(data);

    setWhiteLabelSettings(data).finally(() => {
      getWhiteLabelLogoText();
      getWhiteLabelLogoSizes();
      getWhiteLabelLogoUrls();
    });
  };

  const onChangeLogo = (e) => {
    const id = e.target.id.slice(-1);

    let file = e.target.files[0];

    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = (e) => {
      const imgsrc = e.target.result;

      const changeImg = {
        id,
        src: imgsrc,
      };

      const newArr = logoUrlsWhiteLabel;
      newArr[id - 1] = imgsrc;
      setLogoUrlsWhiteLabel(newArr);

      setLogoUrlsChange([...logoUrlsChange, changeImg]);
    };
  };

  return !isLoadedData ? (
    <LoaderWhiteLabel />
  ) : (
    <StyledComponent>
      <Text className="subtitle" color="#657077">
        {t("BrandingSubtitle")}
      </Text>

      <div className="header-container">
        <Text fontSize="16px" fontWeight="700">
          {t("WhiteLabel")}
        </Text>
        {!isSettingPaid && <Badge backgroundColor="#EDC409" label="Paid" />}
      </div>

      <Text className="wl-subtitle settings_unavailable" fontSize="12px">
        {t("WhiteLabelSubtitle")}
      </Text>

      <div className="wl-helper">
        <Text className="settings_unavailable">{t("WhiteLabelHelper")}</Text>
        <HelpButton
          tooltipContent={t("WhiteLabelTooltip")}
          place="right"
          offsetRight={0}
          className="settings_unavailable"
        />
      </div>

      <div className="settings-block">
        <FieldContainer
          id="fieldContainerCompanyName"
          labelText={t("CompanyNameForCanvasLogo")}
          isVertical={true}
          className="settings_unavailable"
        >
          <TextInput
            className="input"
            value={logoTextWhiteLabel}
            onChange={onChangeCompanyName}
            isDisabled={!isSettingPaid}
            isReadOnly={!isSettingPaid}
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
          />
          <Button
            id="btnUseAsLogo"
            className="use-as-logo"
            size="small"
            label={t("UseAsLogoButton")}
            onClick={onUseTextAsLogo}
            tabIndex={2}
            isDisabled={!isSettingPaid}
          />
        </FieldContainer>

        <div className="logos-container">
          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable"
            >
              {portalHeaderLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "1") ? (
                <canvas
                  id="canvas_logo_1"
                  className="border-img logo-header"
                  width="251"
                  height="48"
                  data-fontsize="36"
                  data-fontcolor={
                    theme.client.settings.common.whiteLabel.dataFontColor
                  }
                >
                  {t("BrowserNoCanvasSupport")}
                </canvas>
              ) : (
                <img
                  className="border-img logo-header"
                  src={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "1")
                      ? logoUrlsChange.find((obj) => obj.id === "1").src
                      : logoUrlsWhiteLabel[0]
                  }
                  alt={t("LogoLightSmall")}
                />
              )}
            </div>
            <label>
              <input
                id="logoUploader_1"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>

          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable "
            >
              {logoCompactLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "6") ? (
                <canvas
                  id="canvas_logo_6"
                  className="border-img logo-compact"
                  width="56"
                  height="56"
                  data-fontsize="36"
                  data-fontcolor={
                    theme.client.settings.common.whiteLabel.dataFontColor
                  }
                >
                  {t("BrowserNoCanvasSupport")}
                </canvas>
              ) : (
                <img
                  className="border-img logo-compact"
                  src={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "6")
                      ? logoUrlsChange.find((obj) => obj.id === "6").src
                      : logoUrlsWhiteLabel[5]
                  }
                  alt={t("LogoLightSmall")}
                />
              )}
            </div>
            <label>
              <input
                id="logoUploader_6"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>

          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable"
            >
              {loginPageLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "2") ? (
                <canvas
                  id="canvas_logo_2"
                  className="border-img logo-big"
                  width="429"
                  height="70"
                  data-fontsize="54"
                  data-fontcolor={
                    theme.client.settings.common.whiteLabel.dataFontColorBlack
                  }
                >
                  {t("BrowserNoCanvasSupport")}
                </canvas>
              ) : (
                <img
                  className="border-img logo-big"
                  src={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "2")
                      ? logoUrlsChange.find((obj) => obj.id === "2").src
                      : logoUrlsWhiteLabel[1]
                  }
                  alt={t("LogoLogin")}
                />
              )}
            </div>
            <label>
              <input
                id="logoUploader_2"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>

          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable"
            >
              {logoAboutLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "7") ? (
                <canvas
                  id="canvas_logo_7"
                  className="border-img logo-big"
                  width="429"
                  height="70"
                  data-fontsize="54"
                  data-fontcolor={
                    theme.client.settings.common.whiteLabel.dataFontColorBlack
                  }
                >
                  {t("BrowserNoCanvasSupport")}
                </canvas>
              ) : (
                <img
                  className="border-img logo-big"
                  src={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "7")
                      ? logoUrlsChange.find((obj) => obj.id === "7").src
                      : logoUrlsWhiteLabel[6]
                  }
                  alt={t("LogoAbout")}
                />
              )}
            </div>
            <label>
              <input
                id="logoUploader_7"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>

          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable"
            >
              {faviconLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "3") ? (
                <canvas
                  id="canvas_logo_3"
                  className="border-img logo-favicon"
                  width="32"
                  height="32"
                  data-fontsize="28"
                  data-fontcolor={
                    theme.client.settings.common.whiteLabel.dataFontColorBlack
                  }
                >
                  {t("BrowserNoCanvasSupport")}
                </canvas>
              ) : (
                <img
                  className="border-img logo-favicon"
                  src={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "3")
                      ? logoUrlsChange.find((obj) => obj.id === "3").src
                      : logoUrlsWhiteLabel[2]
                  }
                  alt={t("LogoFavicon")}
                />
              )}
            </div>

            <label>
              <input
                id="logoUploader_3"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>

          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable"
            >
              {editorsHeaderLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "4") ? (
                <>
                  <canvas
                    id="canvas_logo_4_1"
                    className="border-img logo-docs-editor background-green"
                    width="172"
                    height="40"
                    data-fontsize="22"
                    data-fontcolor={
                      theme.client.settings.common.whiteLabel.dataFontColor
                    }
                  >
                    {t("BrowserNoCanvasSupport")}
                  </canvas>
                  <canvas
                    id="canvas_logo_4_2"
                    className="border-img logo-docs-editor background-blue"
                    width="172"
                    height="40"
                    data-fontsize="22"
                    data-fontcolor={
                      theme.client.settings.common.whiteLabel.dataFontColor
                    }
                  >
                    {t("BrowserNoCanvasSupport")}
                  </canvas>
                  <canvas
                    id="canvas_logo_4_3"
                    className="border-img logo-docs-editor background-orange"
                    width="172"
                    height="40"
                    data-fontsize="22"
                    data-fontcolor={
                      theme.client.settings.common.whiteLabel.dataFontColor
                    }
                  >
                    {t("BrowserNoCanvasSupport")}
                  </canvas>
                </>
              ) : (
                <>
                  <img
                    className="border-img logo-docs-editor background-green"
                    src={
                      logoUrlsChange &&
                      logoUrlsChange.some((obj) => obj.id === "4")
                        ? logoUrlsChange.find((obj) => obj.id === "4").src
                        : logoUrlsWhiteLabel[3]
                    }
                    alt={t("LogoDocsEditor")}
                  />
                  <img
                    className="border-img logo-docs-editor background-blue"
                    src={
                      logoUrlsChange &&
                      logoUrlsChange.some((obj) => obj.id === "4")
                        ? logoUrlsChange.find((obj) => obj.id === "4").src
                        : logoUrlsWhiteLabel[3]
                    }
                    alt={t("LogoDocsEditor")}
                  />
                  <img
                    className="border-img logo-docs-editor background-orange"
                    src={
                      logoUrlsChange &&
                      logoUrlsChange.some((obj) => obj.id === "4")
                        ? logoUrlsChange.find((obj) => obj.id === "4").src
                        : logoUrlsWhiteLabel[3]
                    }
                    alt={t("LogoDocsEditor")}
                  />
                </>
              )}
            </div>

            <label>
              <input
                id="logoUploader_4"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>

          <div className="logo-wrapper">
            <Text
              fontSize="15px"
              fontWeight="600"
              className="settings_unavailable"
            >
              {logoEditorsEmbeddedLabel}
            </Text>
            <div>
              {isCanvasProcessing &&
              !logoUrlsChange.some((obj) => obj.id === "5") ? (
                <canvas
                  id="canvas_logo_5"
                  className="border-img logo-docs-editor"
                  width="172"
                  height="40"
                  data-fontsize="28"
                  data-fontcolor={
                    theme.client.settings.common.whiteLabel.dataFontColorBlack
                  }
                >
                  {t("BrowserNoCanvasSupport")}
                </canvas>
              ) : (
                <img
                  className="border-img logo-docs-editor"
                  src={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "5")
                      ? logoUrlsChange.find((obj) => obj.id === "5").src
                      : logoUrlsWhiteLabel[4]
                  }
                  alt={t("LogoDocsEditorEmbedded")}
                />
              )}
            </div>

            <label>
              <input
                id="logoUploader_5"
                type="file"
                className="hidden"
                onChange={onChangeLogo}
                disabled={!isSettingPaid}
              />
              <Link
                fontWeight="600"
                isHovered
                type="action"
                className="settings_unavailable"
              >
                {t("ChangeLogoButton")}
              </Link>
            </label>
          </div>
        </div>

        {isSettingPaid && (
          <SaveCancelButtons
            tabIndex={3}
            className="save-cancel-buttons"
            onSaveClick={onSave}
            onCancelClick={onRestoreLogo}
            saveButtonLabel={t("Common:SaveButton")}
            cancelButtonLabel={t("RestoreDefaultButton")}
            displaySettings={true}
            showReminder={true}
          />
        )}
      </div>
    </StyledComponent>
  );
};

export default inject(({ setup, auth, common }) => {
  const { setWhiteLabelSettings, restoreWhiteLabelSettings } = setup;

  const { whiteLabelLogoSizes, whiteLabelLogoText } = common;

  const {
    whiteLabelLogoUrls,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
  } = auth.settingsStore;

  return {
    theme: auth.settingsStore.theme,
    logoText: whiteLabelLogoText,
    logoSizes: whiteLabelLogoSizes,
    logoUrls: whiteLabelLogoUrls,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
  };
})(withTranslation(["Settings", "Common"])(observer(WhiteLabel)));
