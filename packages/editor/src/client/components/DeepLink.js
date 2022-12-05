import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import { isMobileOnly } from "react-device-detect";

import { getBgPattern } from "@docspace/common/utils";

import Heading from "@docspace/components/heading";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";

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
  const bgPattern = getBgPattern(currentColorScheme.id);

  const logo = isMobileOnly ? whiteLabelLogoUrls[0] : whiteLabelLogoUrls[1];

  const isSvgLogo = logo.includes(".svg");

  const onOpenHomePage = () => {
    window.open("/rooms/shared/filter", "_self");
  };

  window.onbeforeunload = null;

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
    checkProtocol(fileId);
  }, [fileId]);

  return (
    <StyledDeepLinkWrapper bgPattern={bgPattern}>
      {isSvgLogo ? (
        <ReactSVG
          className="deep-link__logo"
          src={logo}
          onClick={onOpenHomePage}
        />
      ) : (
        <img className="deep-link__logo" src={logo} onClick={onOpenHomePage} />
      )}

      <Heading className={"deep-link__header"}>Открытие документа</Heading>
      <Text className={"deep-link__description"}>
        Данный документ зашифрован и может быть открыт только в десктопном
        редакторе Onlyoffice
      </Text>
      <StyledContentContainer>
        <Text className={"deep-link-content__description"}>
          Нажмите{" "}
          <Text as={"span"} fontWeight={600}>
            Открыть приложение “Onlyoffice”{" "}
          </Text>
          в диалоговом окне в браузере. Если диалоговое окно не отображается,
          нажмите{" "}
          <Text as={"span"} fontWeight={600}>
            Открыть приложение ниже
          </Text>
          .
        </Text>

        <Button
          className={"deep-link-content__button"}
          primary
          scale
          label={"Открыть приложение"}
          size={"medium"}
          onClick={onButtonAction}
        />

        <Text className={"deep-link-content__without-app"}>
          Приложение Onlyoffice не установлено?
        </Text>

        <Link
          className={"deep-link-content__download-now"}
          href={downloadDesktopAppLink}
          target={"_blank"}
          type={"action"}
          isHovered
        >
          Загрузить сейчас
        </Link>
      </StyledContentContainer>
    </StyledDeepLinkWrapper>
  );
};

export default inject(({ filesStore }) => {
  const { privacyInstructions } = filesStore;

  return { privacyInstructions };
})(observer(DeepLink));
