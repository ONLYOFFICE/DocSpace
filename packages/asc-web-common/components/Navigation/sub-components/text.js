import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

import ExpanderDownIcon from "@appserver/components/public/static/images/expander-down.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

import Headline from "@appserver/common/components/Headline";

import { tablet } from "@appserver/components/utils/device";
import { isMobile } from "react-device-detect";
import { Base } from "@appserver/components/themes";

const StyledTextContainer = styled.div`
  width: fit-content;

  position: relative;
  display: grid;

  grid-template-columns: ${(props) =>
    props.isRootFolder ? "auto" : "auto 12px"};

  align-items: center;

  ${(props) => !props.isRootFolder && "cursor: pointer"};
`;

const StyledHeadline = styled(Headline)`
  width: calc(100% + 1px);
  font-weight: 700;
  font-size: ${isMobile ? "21px !important" : "18px"};
  line-height: ${isMobile ? "28px !important" : "24px"};
  @media ${tablet} {
    font-size: 21px;
    line-height: 28px;
  }
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

StyledExpanderDownIcon.defaultProps = { theme: Base };

const StyledExpanderDownIconRotate = styled(ExpanderDownIcon)`
  min-width: 8px !important;
  width: 8px !important;
  min-height: 18px !important;
  padding: 0 4px 0 1px;
  transform: rotate(-180deg);

  path {
    fill: ${(props) => props.theme.navigation.expanderColor};
  }

  ${commonIconsStyles};
`;

StyledExpanderDownIconRotate.defaultProps = { theme: Base };

const Text = ({ title, isRootFolder, isOpen, onClick, ...rest }) => {
  return (
    <StyledTextContainer
      isRootFolder={isRootFolder}
      onClick={onClick}
      {...rest}
    >
      <StyledHeadline type="content" truncate={true}>
        {title}
      </StyledHeadline>
      {!isRootFolder ? (
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
