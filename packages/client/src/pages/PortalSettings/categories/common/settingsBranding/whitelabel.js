import React, { useState, useEffect, useCallback } from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import FieldContainer from "@docspace/components/field-container";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import TextInput from "@docspace/components/text-input";
import HelpButton from "@docspace/components/help-button";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";

import { Base } from "@docspace/components/themes";
import LoaderWhiteLabel from "../sub-components/loaderWhiteLabel";

const StyledComponent = styled.div`
  .subtitle {
    margin-top: 5px;
    margin-bottom: 20px;
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

  .margin-top {
    margin-top: 20px;
  }

  .margin-left {
    margin-left: 20px;
  }

  .input {
    max-width: 350px;
  }

  .border-img {
    border: ${(props) =>
      props.theme.client.settings.common.whiteLabel.borderImg};
    box-sizing: content-box;
  }

  .logo-light-small {
    width: 142px;
    height: 23px;
    padding: 10px;
    background-color: ${(props) =>
      props.theme.client.settings.common.whiteLabel.backgroundColor};
  }

  .logo-dark {
    max-width: 216px;
    max-height: 35px;
    padding: 10px;
  }

  .logo-favicon {
    width: 16px;
    height: 16px;
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

  .display {
    display: none;
  }
`;

StyledComponent.defaultProps = { theme: Base };

const WhiteLabel = (props) => {
  const {
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
  const [isPortalPaid, setIsPortalPaid] = useState(true);
  const [isCanvasProcessing, setIsCanvasProcessing] = useState(false);
  const [isUseTextAsLogo, setIsUseTextAsLogo] = useState(false);

  const [logoTextWhiteLabel, setLogoTextWhiteLabel] = useState(logoText);

  const [logoUrlsWhiteLabel, setLogoUrlsWhiteLabel] = useState(logoUrls);
  const [logoUrlsChange, setLogoUrlsChange] = useState([]);

  const [portalHeaderLabel, setPortalHeaderLabel] = useState();
  const [loginPageLabel, setLoginPageLabel] = useState();
  const [faviconLabel, setFaviconLabel] = useState();
  const [editorsHeaderLabel, setEditorsHeaderLabel] = useState();

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

    const loginPageLabel = isSizesExist
      ? `${t("LogoDark")} (${logoSizesWhiteLabel[1].width}x${
          logoSizesWhiteLabel[1].height
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

    setPortalHeaderLabel(portalHeaderLabel);
    setLoginPageLabel(loginPageLabel);
    setFaviconLabel(faviconLabel);
    setEditorsHeaderLabel(editorsHeaderLabel);
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

  const onSaveImageBase64 = (url) => {
    let img = document.createElement("img");
    img.src = url;

    let key = encodeURIComponent(url),
      canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    let ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);

    return canvas.toDataURL("image/png");
  };

  const onSave = () => {
    if (logoUrlsChange) {
      let fd = new FormData();
      fd.append("logoText", logoTextWhiteLabel);

      fd.append(`logo[${0}][key]`, 1);
      fd.append(`logo[${0}][value]`, logoUrlsChange[0].src);

      const data = new URLSearchParams(fd);

      setWhiteLabelSettings(data).finally(() => {
        getWhiteLabelLogoText();
        getWhiteLabelLogoSizes();
        getWhiteLabelLogoUrls();
      });
    }

    let elem = document.getElementById("canvas_logo_1");

    if (elem) {
      let dataURL = elem.toDataURL();

      let fd = new FormData();
      fd.append("logoText", logoTextWhiteLabel);
      fd.append(`logo[${0}][key]`, 1);
      fd.append(`logo[${0}][value]`, dataURL);

      const data = new URLSearchParams(fd);

      setWhiteLabelSettings(data).finally(() => {
        getWhiteLabelLogoText();
        getWhiteLabelLogoSizes();
        getWhiteLabelLogoUrls();
      });
    }
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

      setLogoUrlsChange([...logoUrlsChange, changeImg]);

      let fd = new FormData();
      fd.append("logoText", "asas");
      fd.append(`logo[${0}][key]`, 1);
      fd.append(`logo[${0}][value]`, e.target.result);

      const data = new URLSearchParams(fd);
    };
  };

  return !isLoadedData ? (
    <LoaderWhiteLabel />
  ) : (
    <StyledComponent>
      <Text className="subtitle" color="#657077">
        {t("BrandingSubtitle")}
      </Text>

      <Text fontSize="16px" fontWeight="700">
        {t("WhiteLabel")}
      </Text>
      <Text className="wl-subtitle" fontSize="12px">
        {t("WhiteLabelSubtitle")}
      </Text>

      <div className="wl-helper">
        <Text>{t("WhiteLabelHelper")}</Text>
        <HelpButton
          tooltipContent={t("WhiteLabelTooltip")}
          place="right"
          offsetRight={0}
        />
      </div>

      <div className="settings-block">
        <FieldContainer
          id="fieldContainerCompanyName"
          labelText={`${t("CompanyNameForCanvasLogo")}:`}
          isVertical={true}
        >
          <TextInput
            className="input"
            value={logoTextWhiteLabel}
            onChange={onChangeCompanyName}
            isDisabled={!isPortalPaid}
            isReadOnly={!isPortalPaid}
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
          />
          {isPortalPaid && (
            <Button
              id="btnUseAsLogo"
              className="use-as-logo"
              size="small"
              label={t("UseAsLogoButton")}
              onClick={onUseTextAsLogo}
              tabIndex={2}
            />
          )}
        </FieldContainer>

        <FieldContainer
          id="fieldContainerLogoLightSmall"
          className="field-container"
          labelText={portalHeaderLabel}
          isVertical={true}
        >
          <div>
            {isCanvasProcessing &&
            !logoUrlsChange.some((obj) => obj.id === "1") ? (
              <canvas
                id="canvas_logo_1"
                className="border-img logo-light-small"
                width="284"
                height="46"
                data-fontsize="36"
                data-fontcolor={
                  theme.client.settings.common.whiteLabel.dataFontColor
                }
              >
                {t("BrowserNoCanvasSupport")}
              </canvas>
            ) : (
              <img
                className="border-img logo-light-small"
                src={
                  logoUrlsChange && logoUrlsChange.some((obj) => obj.id === "1")
                    ? logoUrlsChange.find((obj) => obj.id === "1").src
                    : logoUrlsWhiteLabel[0]
                }
                alt={t("LogoLightSmall")}
              />
            )}
          </div>
          {isPortalPaid && (
            <label>
              <input
                id="logoUploader_1"
                type="file"
                className="display"
                onChange={onChangeLogo}
              />
              <a>{t("ChangeLogoButton")}</a>
            </label>
          )}
        </FieldContainer>

        <FieldContainer
          id="fieldContainerLogoDark"
          className="field-container"
          labelText={loginPageLabel}
          isVertical={true}
        >
          <div>
            {isCanvasProcessing &&
            !logoUrlsChange.some((obj) => obj.id === "2") ? (
              <canvas
                id="canvas_logo_2"
                className="border-img logo-dark"
                width="432"
                height="70"
                data-fontsize="54"
                data-fontcolor={
                  theme.client.settings.common.whiteLabel.dataFontColorBlack
                }
              >
                {t("BrowserNoCanvasSupport")}
              </canvas>
            ) : (
              // <img
              //   className="border-img logo-dark"
              //   src={
              //     logoUrlsChange && logoUrlsChange.some((obj) => obj.id === "2")
              //       ? logoUrlsChange.find((obj) => obj.id === "2").src
              //       : logoUrlsWhiteLabel[1]
              //   }
              //   alt={t("LogoDark")}
              // />

              <div className="border-img logo-dark">
                <object
                  type="image/svg+xml"
                  data={
                    logoUrlsChange &&
                    logoUrlsChange.some((obj) => obj.id === "2")
                      ? logoUrlsChange.find((obj) => obj.id === "2").src
                      : logoUrlsWhiteLabel[1]
                  }
                ></object>
              </div>
            )}
          </div>
          {isPortalPaid && (
            <label>
              <input
                id="logoUploader_2"
                type="file"
                className="display"
                onChange={onChangeLogo}
              />
              <a>{t("ChangeLogoButton")}</a>
            </label>
          )}
        </FieldContainer>

        <FieldContainer
          id="fieldContainerLogoFavicon"
          className="field-container"
          labelText={faviconLabel}
          isVertical={true}
        >
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
                  logoUrlsChange && logoUrlsChange.some((obj) => obj.id === "3")
                    ? logoUrlsChange.find((obj) => obj.id === "3").src
                    : logoUrlsWhiteLabel[2]
                }
                alt={t("LogoFavicon")}
              />
            )}
          </div>
          {isPortalPaid && (
            <label>
              <input
                id="logoUploader_3"
                type="file"
                className="display"
                onChange={onChangeLogo}
              />
              <a>{t("ChangeLogoButton")}</a>
            </label>
          )}
        </FieldContainer>

        <FieldContainer
          id="fieldContainerEditorHeaderLogo"
          className="field-container"
          labelText={editorsHeaderLabel}
          isVertical={true}
        >
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

          {isPortalPaid && (
            <label>
              <input
                id="logoUploader_4"
                type="file"
                className="display"
                onChange={onChangeLogo}
              />
              <a>{t("ChangeLogoButton")}</a>
            </label>
          )}
        </FieldContainer>

        <SaveCancelButtons
          id="buttonsCompanyInfoSettings"
          className="save-cancel-buttons"
          onSaveClick={onSave}
          onCancelClick={onRestoreLogo}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("RestoreDefaultButton")}
          displaySettings={true}
        />
      </div>
    </StyledComponent>
  );
};

export default inject(({ setup, auth, common }) => {
  const { setWhiteLabelSettings, restoreWhiteLabelSettings } = setup;

  const {
    whiteLabel,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
  } = common;

  const { logoText, logoSizes, logoUrls } = whiteLabel;

  return {
    theme: auth.settingsStore.theme,
    logoText,
    logoSizes,
    logoUrls,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
  };
})(withTranslation(["Settings", "Common"])(observer(WhiteLabel)));
