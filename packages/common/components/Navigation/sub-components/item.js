import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Text from "@docspace/components/text";

import DefaultIcon from "../svg/default.react.svg";
import RootIcon from "../svg/root.react.svg";
import DefaultTabletIcon from "../svg/default.tablet.react.svg";
import RootTabletIcon from "../svg/root.tablet.react.svg";

import { isMobile } from "react-device-detect";
import {
  tablet,
  isTablet,
  isMobile as IsMobileUtils,
} from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

const StyledItem = styled.div`
  height: auto;
  width: auto !important;
  position: relative;
  display: grid;
  align-items: ${(props) => (props.isRoot ? "baseline" : "end")};
  grid-template-columns: 17px auto;
  cursor: pointer;
`;

const StyledIconWrapper = styled.div`
  width: 17px;
  display: flex;
  align-items: ${(props) => (props.isRoot ? "center" : "flex-end")};
  justify-content: center;

  svg {
    path {
      fill: ${(props) => props.theme.navigation.icon.fill};
    }

    circle {
      stroke: ${(props) => props.theme.navigation.icon.fill};
    }

    path:first-child {
      fill: none !important;
      stroke: ${(props) => props.theme.navigation.icon.stroke};
    }
  }
`;

StyledIconWrapper.defaultProps = { theme: Base };

const StyledText = styled(Text)`
  margin-left: 10px;
  position: relative;
  bottom: ${(props) => (props.isRoot ? "2px" : "-1px")};
`;

const Item = ({ id, title, isRoot, isRootRoom, onClick, ...rest }) => {
  const onClickAvailable = () => {
    onClick && onClick(id, isRootRoom);
  };

  return (
    <StyledItem id={id} isRoot={isRoot} onClick={onClickAvailable} {...rest}>
      <StyledIconWrapper isRoot={isRoot}>
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
      </StyledIconWrapper>
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
