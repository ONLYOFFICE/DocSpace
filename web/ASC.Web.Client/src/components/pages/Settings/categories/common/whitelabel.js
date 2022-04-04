import React from "react";

import { withTranslation, Trans } from "react-i18next";
import styled from "styled-components";
import FieldContainer from "@appserver/components/field-container";
import Text from "@appserver/components/text";
import Loader from "@appserver/components/loader";
import Button from "@appserver/components/button";
import toastr from "@appserver/components/toast/toastr";
import Link from "@appserver/components/link";
import TextInput from "@appserver/components/text-input";
import FileInput from "@appserver/components/file-input";

import { inject, observer } from "mobx-react";
import { Base } from "@appserver/components/themes";

const StyledComponent = styled.div`
  .margin-top {
    margin-top: 20px;
  }

  .margin-left {
    margin-left: 20px;
  }

  .settings-block {
    margin-bottom: 70px;
  }

  .field-container {
    margin-top: 45px;
  }

  .input-width {
    max-width: 500px;
  }

  .border-img {
    border: ${(props) =>
      props.theme.studio.settings.common.whiteLabel.borderImg};
    box-sizing: content-box;
  }

  .logo-light-small {
    width: 142px;
    height: 23px;
    padding: 10px;
    background-color: ${(props) =>
      props.theme.studio.settings.common.whiteLabel.backgroundColor};
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
      props.theme.studio.settings.common.whiteLabel.greenBackgroundColor};
  }

  .background-blue {
    background-color: ${(props) =>
      props.theme.studio.settings.common.whiteLabel.blueBackgroundColor};
  }

  .background-orange {
    background-color: ${(props) =>
      props.theme.studio.settings.common.whiteLabel.orangeBackgroundColor};
  }
`;

StyledComponent.defaultProps = { theme: Base };

const mapSizesToArray = (sizes) => {
  return sizes.map((size) => {
    return { height: size.height, width: size.width };
  });
};

const generateLabelsForLogos = (sizes, t) => {
  const isSizesExist = sizes.length;
  const portalHeaderLabel = isSizesExist
    ? `${t("LogoLightSmall")} (${sizes[0].width}x${sizes[0].height}):`
    : "";
  const loginPageLabel = isSizesExist
    ? `${t("LogoDark")} (${sizes[1].width}x${sizes[1].height}):`
    : "";
  const faviconLabel = isSizesExist
    ? `${t("LogoFavicon")} (${sizes[2].width}x${sizes[2].height}):`
    : "";
  const editorsHeaderLabel = isSizesExist
    ? `${t("LogoDocsEditor")} (${sizes[3].width}x${sizes[3].height}):`
    : "";
  return {
    portalHeaderLabel,
    loginPageLabel,
    faviconLabel,
    editorsHeaderLabel,
  };
};
class WhiteLabel extends React.Component {
  constructor(props) {
    super(props);

    const { logoText, rawSizes, logoUrls, t } = props;
    const logoSizes = mapSizesToArray(rawSizes);
    const labels = generateLabelsForLogos(logoSizes, t);

    this.state = {
      isLoadedData: false,
      isPortalPaid: true,
      isCanvasProcessing: false,
      logoText,
      logoSizes,
      logoUrls,
      ...labels,
    };
  }

  componentDidMount() {
    const {
      getWhiteLabelLogoText,
      getWhiteLabelLogoSizes,
      getWhiteLabelLogoUrls,
      t,
    } = this.props;
    const { logoText, logoSizes, logoUrls } = this.state;

    if (!logoText) {
      getWhiteLabelLogoText().then(() =>
        this.setState({ logoText: this.props.logoText })
      );
    }

    if (!logoSizes.length) {
      getWhiteLabelLogoSizes().then(() => {
        const logoSizes = mapSizesToArray(this.props.rawSizes);
        const labels = generateLabelsForLogos(logoSizes, t);
        this.setState({ logoSizes, ...labels });
      });
    }

    if (!logoUrls.length) {
      getWhiteLabelLogoUrls().then(() => {
        this.setState({ logoUrls: this.props.logoUrls });
      });
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const { logoText, logoSizes, logoUrls } = this.state;
    if (
      logoText &&
      logoSizes.length &&
      logoUrls.length &&
      !prevState.isLoadedData
    ) {
      this.setState({ isLoadedData: true });
    }
  }

  onChangeCompanyName = (e) => {
    const value = e.target.value;
    this.setState({ logoText: value });
  };

  onUseTextAsLogo = () => {
    this.setState({ isCanvasProcessing: true }, function () {
      const canvas = document.querySelectorAll("[id^=canvas_logo_]");
      const canvasLength = canvas.length;
      const text = this.state.logoText;

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
    });
  };

  onChangeLogo = () => {
    console.log("Click to Change logo button");
  };

  onRestoreLogo = () => {
    const { restoreWhiteLabelSettings } = this.props;
    console.log("restore button action");
    restoreWhiteLabelSettings(true);
    this.setState({ isCanvasProcessing: false });
  };

  onSaveImageBase64 = (url) => {
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

  onSave = () => {
    const { setWhiteLabelSettings } = this.props;
    const { logoText } = this.state;

    // TODO: Add logic to all pictures
    let fd = new FormData();
    fd.append("logoText", logoText);

    let elem = document.getElementById("canvas_logo_1");
    let dataURL = elem.toDataURL();

    fd.append(`logo[${0}][key]`, 1);
    fd.append(`logo[${0}][value]`, dataURL);

    // for (let i = 1; i < logoUrls.length; i++) {
    //   fd.append(`logo[${i}][key]`, i);
    //   console.log(this.onSaveImageBase64(logoUrls[i]));
    //   fd.append(`logo[${i}][value]`, this.onSaveImageBase64(logoUrls[i - 1]));
    // }

    const data = new URLSearchParams(fd);

    setWhiteLabelSettings(data);
  };

  onChangeHandler = (e) => {
    const { setWhiteLabelSettings } = this.props;

    // TODO: Add size check

    let file = e.target.files[0];

    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = (e) => {
      this.imgsrc = e.target.result;

      let fd = new FormData();
      fd.append("logoText", "asas");
      fd.append(`logo[${0}][key]`, 1);
      fd.append(`logo[${0}][value]`, e.target.result);

      const data = new URLSearchParams(fd);
      setWhiteLabelSettings(data);
    };
  };

  render() {
    const { t, theme } = this.props;
    const {
      isLoadedData,
      isPortalPaid,
      logoText,
      portalHeaderLabel,
      loginPageLabel,
      faviconLabel,
      editorsHeaderLabel,
      logoUrls,
      isCanvasProcessing,
    } = this.state;
    console.log("WhiteLabelSettings render");
    return !isLoadedData ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <>
        <StyledComponent>
          <div className="settings-block">
            <Text fontSize="16px">{t("LogoSettings")}</Text>
            <Text className="margin-top" fontSize="14px">
              <Trans t={t} i18nKey="LogoUploadRecommendation" ns="Settings">
                We recommended that you use images in <strong>PNG</strong>{" "}
                format with transparent background
              </Trans>
            </Text>

            <FieldContainer
              id="fieldContainerCompanyName"
              className="field-container"
              labelText={`${t("CompanyNameForCanvasLogo")}:`}
              isVertical={true}
            >
              <TextInput
                className="input-width"
                value={logoText}
                onChange={this.onChangeCompanyName}
                isDisabled={!isPortalPaid}
                isReadOnly={!isPortalPaid}
                scale={true}
                isAutoFocussed={true}
                tabIndex={1}
              />
              {isPortalPaid && (
                <Button
                  id="btnUseAsLogo"
                  className="margin-top"
                  size="small"
                  label={t("UseAsLogoButton")}
                  onClick={this.onUseTextAsLogo}
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
                {isCanvasProcessing ? (
                  <canvas
                    id="canvas_logo_1"
                    className="border-img logo-light-small"
                    width="284"
                    height="46"
                    data-fontsize="36"
                    data-fontcolor={
                      theme.studio.settings.common.whiteLabel.dataFontColor
                    }
                  >
                    {t("BrowserNoCanvasSupport")}
                  </canvas>
                ) : (
                  <img
                    className="border-img logo-light-small"
                    src={logoUrls[0]}
                    alt={t("LogoLightSmall")}
                  />
                )}
              </div>
              {isPortalPaid && (
                <FileInput
                  placeholder={t("ChangeLogoButton")}
                  onChange={this.onChangeHandler}
                />
              )}
            </FieldContainer>

            <FieldContainer
              id="fieldContainerLogoDark"
              className="field-container"
              labelText={loginPageLabel}
              isVertical={true}
            >
              <div>
                {isCanvasProcessing ? (
                  <canvas
                    id="canvas_logo_2"
                    className="border-img logo-dark"
                    width="432"
                    height="70"
                    data-fontsize="54"
                    data-fontcolor={
                      theme.studio.settings.common.whiteLabel.dataFontColorBlack
                    }
                  >
                    {t("BrowserNoCanvasSupport")}
                  </canvas>
                ) : (
                  <img
                    className="border-img logo-dark"
                    src={logoUrls[1]}
                    alt={t("LogoDark")}
                  />
                )}
              </div>
            </FieldContainer>
            <FieldContainer
              id="fieldContainerLogoFavicon"
              className="field-container"
              labelText={faviconLabel}
              isVertical={true}
            >
              <div>
                {isCanvasProcessing ? (
                  <canvas
                    id="canvas_logo_3"
                    className="border-img logo-favicon"
                    width="32"
                    height="32"
                    data-fontsize="28"
                    data-fontcolor={
                      theme.studio.settings.common.whiteLabel.dataFontColorBlack
                    }
                  >
                    {t("BrowserNoCanvasSupport")}
                  </canvas>
                ) : (
                  <img
                    className="border-img logo-favicon"
                    src={logoUrls[2]}
                    alt={t("LogoFavicon")}
                  />
                )}
              </div>
              {isPortalPaid && (
                <Link
                  type="action"
                  color={theme.studio.settings.common.linkColorHelp}
                  isHovered
                  onClick={this.onChangeLogo}
                >
                  {t("ChangeLogoButton")}
                </Link>
              )}
            </FieldContainer>

            <FieldContainer
              id="fieldContainerEditorHeaderLogo"
              className="field-container"
              labelText={editorsHeaderLabel}
              isVertical={true}
            >
              <div>
                {isCanvasProcessing ? (
                  <>
                    <canvas
                      id="canvas_logo_4_1"
                      className="border-img logo-docs-editor background-green"
                      width="172"
                      height="40"
                      data-fontsize="22"
                      data-fontcolor={
                        theme.studio.settings.common.whiteLabel.dataFontColor
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
                        theme.studio.settings.common.whiteLabel.dataFontColor
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
                        theme.studio.settings.common.whiteLabel.dataFontColor
                      }
                    >
                      {t("BrowserNoCanvasSupport")}
                    </canvas>
                  </>
                ) : (
                  <>
                    <img
                      className="border-img logo-docs-editor background-green"
                      src={logoUrls[3]}
                      alt={t("LogoDocsEditor")}
                    />
                    <img
                      className="border-img logo-docs-editor background-blue"
                      src={logoUrls[3]}
                      alt={t("LogoDocsEditor")}
                    />
                    <img
                      className="border-img logo-docs-editor background-orange"
                      src={logoUrls[3]}
                      alt={t("LogoDocsEditor")}
                    />
                  </>
                )}
              </div>

              {isPortalPaid && (
                <Link
                  type="action"
                  color={theme.studio.settings.common.linkColorHelp}
                  isHovered
                  onClick={this.onChangeLogo}
                >
                  {t("ChangeLogoButton")}
                </Link>
              )}
            </FieldContainer>

            <Button
              id="btnSaveGreetingSetting"
              className="margin-top"
              primary={true}
              size="small"
              label={t("Common:SaveButton")}
              isLoading={false}
              isDisabled={false}
              //onClick={() => console.log("Save button action")}
              onClick={this.onSave}
            />

            <Button
              id="btnRestoreToDefault"
              className="margin-top margin-left"
              size="small"
              label={t("RestoreDefaultButton")}
              isLoading={false}
              isDisabled={false}
              onClick={this.onRestoreLogo}
            />
          </div>
        </StyledComponent>
      </>
    );
  }
}

export default inject(({ setup, auth }) => {
  const {
    common,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
  } = setup;

  const { logoText, logoSizes: rawSizes, logoUrls } = common.whiteLabel;

  return {
    theme: auth.settingsStore.theme,
    logoText,
    rawSizes,
    logoUrls,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
  };
})(withTranslation(["Settings", "Common"])(observer(WhiteLabel)));
