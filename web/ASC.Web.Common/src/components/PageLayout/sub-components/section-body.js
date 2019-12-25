import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils, Scrollbar } from "asc-web-components";
const { tablet } = utils.device;

const StyledSectionBody = styled.div`
  margin: 16px 8px 16px 0;
  ${props => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
  height: 100%;

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
    <StyledSectionBody>
      {withScroll ? (
        <Scrollbar stype="mediumBlack">
          {children}
          <StyledSpacer pinned={pinned}/>
        </Scrollbar>
      ) : (
        <>
          {children}
          <StyledSpacer pinned={pinned}/>
        </>
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
