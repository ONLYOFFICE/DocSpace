import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

import Text from "@docspace/components/text";

import DefaultIcon from "PUBLIC_DIR/images/default.react.svg";
import RootIcon from "PUBLIC_DIR/images/root.react.svg";
import DefaultTabletIcon from "PUBLIC_DIR/images/default.tablet.react.svg";
import RootTabletIcon from "PUBLIC_DIR/images/root.tablet.react.svg";

import { isMobile } from "react-device-detect";
import {
  tablet,
  isTablet,
  isMobile as IsMobileUtils,
  hugeMobile,
} from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const StyledItem = styled.div`
  height: auto;
  width: auto !important;
  position: relative;
  display: grid;
  align-items: ${(props) => (props.isRoot ? "baseline" : "end")};
  grid-template-columns: 17px auto;
  cursor: pointer;

  margin-left: 0;

  @media ${tablet} {
    ${({ withLogo }) =>
      withLogo &&
      css`
        margin-left: 44px;
      `};
  }

  ${isMobile &&
  css`
    margin-left: 0;
  `};

  @media ${hugeMobile} {
    margin-left: 0;
  }
`;

const StyledText = styled(Text)`
  margin-left: 10px;
  position: relative;
  bottom: ${(props) => (props.isRoot ? "2px" : "-1px")};
`;

const Item = ({
  id,
  title,
  isRoot,
  isRootRoom,
  onClick,
  withLogo,
  ...rest
}) => {
  const onClickAvailable = () => {
    onClick && onClick(id, isRootRoom);
  };

  return (
    <StyledItem
      id={id}
      isRoot={isRoot}
      onClick={onClickAvailable}
      withLogo={withLogo}
      {...rest}
    >
      <ColorTheme isRoot={isRoot} themeId={ThemeType.IconWrapper}>
        {isMobile || isTablet() || IsMobileUtils() ? (
          isRoot ? (
            <RootTabletIcon />
          ) : (
            <DefaultTabletIcon />
          )
        ) : isRoot ? (
          <RootIcon />
        ) : (
          <DefaultIcon />
        )}
      </ColorTheme>

      <StyledText
        isRoot={isRoot}
        fontWeight={isRoot ? "600" : "400"}
        fontSize={"15px"}
        truncate={true}
        title={title}
      >
        {title}
      </StyledText>
    </StyledItem>
  );
};

Item.propTypes = {
  id: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  title: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isRoot: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(Item);
