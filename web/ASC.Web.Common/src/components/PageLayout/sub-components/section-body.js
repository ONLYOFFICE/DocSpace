import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils, Scrollbar } from "asc-web-components";
const { tablet } = utils.device;

const StyledSectionBody = styled.div`
  ${props => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
  height: 100%;
  
  /* TODO: Fix overflow scrollbar */
  ${props => props.withScroll && `
    margin-left: -24px;

    .scroll-body {
      padding-left: 24px;
    }
  `}
`;

const StyledSectionWrapper = styled.div`
  margin: 16px 8px 16px 0;

  @media ${tablet} {
    margin: 16px 0;
  }
`;

const StyledSpacer = styled.div`
  display: none;
  min-height: 64px;

  @media ${tablet} {
    display: ${props => (props.pinned ? "none" : "block")};
  }
`;

const SectionBody = React.memo(props => {
  //console.log("PageLayout SectionBody render");
  const { children, withScroll, pinned } = props;

  return (
    <StyledSectionBody withScroll={withScroll}>
      {withScroll ? (
        <Scrollbar stype="mediumBlack">
          <StyledSectionWrapper>
            {children}
            <StyledSpacer pinned={pinned}/>
          </StyledSectionWrapper>
        </Scrollbar>
      ) : (
        <StyledSectionWrapper>
          {children}
          <StyledSpacer pinned={pinned}/>
        </StyledSectionWrapper>
      )}
    </StyledSectionBody>
  );
});

SectionBody.displayName = "SectionBody";

SectionBody.propTypes = {
  withScroll: PropTypes.bool,
  pinned: PropTypes.bool,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
    PropTypes.any
  ])
};

SectionBody.defaultProps = {
  withScroll: true,
  pinned: false
};

export default SectionBody;
