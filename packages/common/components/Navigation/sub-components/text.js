import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import ExpanderDownIcon from "PUBLIC_DIR/images/expander-down.react.svg";
import ArrowIcon from "PUBLIC_DIR/images/arrow.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import Heading from "@docspace/components/heading";

import { tablet } from "@docspace/components/utils/device";
import { isMobile, isTablet } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const StyledTextContainer = styled.div`
  display: flex;

  align-items: center;

  flex-direction: row;

  position: relative;

  ${(props) =>
    !props.isRootFolder && !props.isRootFolderTitle && "cursor: pointer"};
  ${(props) => props.isRootFolderTitle && "padding-right: 3px"};

  ${(props) =>
    !props.isRootFolderTitle &&
    css`
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    `};
`;

const StyledHeading = styled(Heading)`
  font-weight: 700;
  font-size: 18px;
  line-height: 24px;

  margin: 0;

  ${(props) =>
    props.isRootFolderTitle &&
    `color: ${props.theme.navigation.rootFolderTitleColor}`};

  @media ${tablet} {
    font-size: 21px;
    line-height: 28px;
  }

  ${isMobile &&
  css`
    font-size: 18px !important;
    line-height: 24px !important;
  `}

  ${isTablet &&
  css`
    font-size: 21px;
    line-height: 28px;
  `}
`;

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  min-width: 8px !important;
  width: 8px !important;
  min-height: 18px !important;
  padding: 0 2px 0 4px;
  path {
    fill: ${(props) => props.theme.navigation.expanderColor};
  }

  ${commonIconsStyles};
`;

const StyledArrowIcon = styled(ArrowIcon)`
  height: 12px;
  min-width: 12px;

  padding-left: 6px;
  path {
    fill: ${(props) => props.theme.navigation.rootFolderTitleColor};
  }
`;

StyledExpanderDownIcon.defaultProps = { theme: Base };

const StyledExpanderDownIconRotate = styled(ExpanderDownIcon)`
  min-width: 8px !important;
  width: 8px !important;
  min-height: 18px !important;
  padding: 0 4px 0 2px;
  transform: rotate(-180deg);

  path {
    fill: ${(props) => props.theme.navigation.expanderColor};
  }

  ${commonIconsStyles};
`;

StyledExpanderDownIconRotate.defaultProps = { theme: Base };

const Text = ({
  title,
  isRootFolder,
  isOpen,
  isRootFolderTitle,
  onClick,
  ...rest
}) => {
  return (
    <StyledTextContainer
      isRootFolder={isRootFolder}
      onClick={onClick}
      isOpen={isOpen}
      isRootFolderTitle={isRootFolderTitle}
      {...rest}
    >
      <StyledHeading
        type="content"
        title={title}
        truncate={true}
        isRootFolderTitle={isRootFolderTitle}
      >
        {title}
      </StyledHeading>

      {isRootFolderTitle && <StyledArrowIcon />}

      {!isRootFolderTitle && !isRootFolder ? (
        isOpen ? (
          <StyledExpanderDownIconRotate />
        ) : (
          <StyledExpanderDownIcon />
        )
      ) : (
        <></>
      )}
    </StyledTextContainer>
  );
};

Text.propTypes = {
  title: PropTypes.string,
  isOpen: PropTypes.bool,
  isRootFolder: PropTypes.bool,
  onCLick: PropTypes.func,
};

export default React.memo(Text);
