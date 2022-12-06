import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation, Trans } from "react-i18next";
import { ReactSVG } from "react-svg";
import { isMobileOnly, isMobile } from "react-device-detect";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import { getBgPattern } from "@docspace/common/utils";

import Heading from "@docspace/components/heading";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import { checkProtocol } from "../helpers/utils";

import {
  StyledDeepLinkWrapper,
  StyledContentContainer,
} from "./StyledDeepLink";

const downloadDesktopAppLink = "https://www.onlyoffice.com/desktop.aspx";

const DeepLink = ({
  fileId,
  currentColorScheme,
  privacyInstructions,
  whiteLabelLogoUrls,
}) => {
  const { t } = useTranslation(["DeepLink"]);

  const bgPattern = getBgPattern(currentColorScheme.id);

  const logo = isMobileOnly ? whiteLabelLogoUrls[0] : whiteLabelLogoUrls[1];
  const buttonLabel = !isMobile ? t("OpenApp") : t("Instructions");

  const isSvgLogo = logo.includes(".svg");

  const onOpenHomePage = () => {
    window.open("/rooms/shared/filter", "_self");
  };

  const onOpenInstruction = () => {
    window.open(privacyInstructions, "_blank");
  };

  const onButtonAction = () => {
    if (isMobile) {
      return onOpenInstruction();
    }

    checkProtocol(fileId);
  };

  React.useEffect(() => {
    if (!isMobile) {
      checkProtocol(fileId);
    }
  }, [fileId]);

  const mobileBlock = (
    <>
      <Text className="deep-link-content__mobile-header">
        {t("NotAvailable")}
      </Text>
      <Text className="deep-link-content__mobile-description">
        {t("UseApp")}
      </Text>
    </>
  );

  return (
    <StyledDeepLinkWrapper bgPattern={bgPattern}>
      <div className="deep-link__logo-container">
        {isSvgLogo ? (
          <ReactSVG
            className="deep-link__logo"
            src={logo}
            onClick={onOpenHomePage}
          />
        ) : (
          <img
            className="deep-link__logo"
            src={logo}
            onClick={onOpenHomePage}
          />
        )}
      </div>

      {!isMobile && (
        <>
          <Heading className={"deep-link__header"}> {t("OpeningDoc")}</Heading>
          <Text className={"deep-link__description"}>
            {t("OpeningDocDesc")}
          </Text>
        </>
      )}
      <StyledContentContainer>
        {isMobile ? (
          mobileBlock
        ) : (
          <Text className={"deep-link-content__description"}>
            <Trans t={t} i18nKey="ClickDescription" ns="DeepLink">
              Click{" "}
              <Text as={"span"} fontWeight={600}>
                {{ firstButton: t("OpenAppOnlyoffice") }}
              </Text>{" "}
              in the browser dialog box. If the dialog box does not appear,
              click{" "}
              <Text as={"span"} fontWeight={600}>
                {{ secondButton: t("OpenApp") }}
              </Text>{" "}
              below.
            </Trans>
          </Text>
        )}

        <Button
          className={"deep-link-content__button"}
          primary
          scale
          label={buttonLabel}
          size={"medium"}
          onClick={onButtonAction}
          currentColorScheme={currentColorScheme}
        />

        {!isMobile && (
          <>
            <Text className={"deep-link-content__without-app"}>
              {t("MissApp")}
            </Text>

            <ColorTheme
              className={"deep-link-content__download-now"}
              href={downloadDesktopAppLink}
              target={"_blank"}
              type={"action"}
              isHovered
              themeId={ThemeType.Link}
              currentColorScheme={currentColorScheme}
            >
              {t("DownloadNow")}
            </ColorTheme>
          </>
        )}
      </StyledContentContainer>
    </StyledDeepLinkWrapper>
  );
};

export default inject(({ filesStore }) => {
  const { privacyInstructions } = filesStore;

  return { privacyInstructions };
})(observer(DeepLink));
