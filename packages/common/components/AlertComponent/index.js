import React from "react";
import { inject, observer } from "mobx-react";

import styled from "styled-components";

import Text from "@docspace/components/text";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import ArrowRightIcon from "PUBLIC_DIR/images/arrow.right.react.svg";
import CrossReactSvg from "PUBLIC_DIR/images/cross.react.svg";

import Loaders from "../Loaders";
import { StyledAlertComponent } from "./StyledComponent";
import Link from "@docspace/components/link";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  margin: auto 0;
  path {
    fill: ${(props) => props.theme.alertComponent.iconColor};
  }
`;
const StyledCrossIcon = styled(CrossReactSvg)`
  position: absolute;
  right: 0px;
  margin-right: 8px;
  margin-top: 8px;
  cursor: pointer;

  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

const AlertComponent = (props) => {
  const {
    id,
    description,
    title,
    titleFontSize,
    additionalDescription,
    needArrowIcon = false,
    needCloseIcon = false,
    link,
    linkColor,
    linkTitle,
    onAlertClick,
    onCloseClick,
    titleColor,
    borderColor,
    theme,
  } = props;
  return (
    <StyledAlertComponent
      theme={theme}
      titleColor={titleColor}
      borderColor={borderColor}
      onClick={onAlertClick}
      needArrowIcon={needArrowIcon}
      id={id}
    >
      <div>
        <Text
          className="alert-component_title"
          fontSize={titleFontSize ?? "12px"}
          fontWeight={600}
        >
          {title}
        </Text>
        {additionalDescription && (
          <Text fontWeight={600}>{additionalDescription}</Text>
        )}
        <Text
          noSelect
          fontSize="12px"
          color={theme.alertComponent.descriptionColor}
        >
          {description}
        </Text>
        {link && (
          <Link type="page" href={link} noHover color={linkColor}>
            {linkTitle}
          </Link>
        )}
      </div>
      {needCloseIcon && (
        <StyledCrossIcon size="extraSmall" onClick={onCloseClick} />
      )}
      {needArrowIcon && (
        <StyledArrowRightIcon className="alert-component_arrow" />
      )}
    </StyledAlertComponent>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;

  const { theme } = settingsStore;

  return {
    theme,
  };
})(observer(AlertComponent));
