import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Scrollbar from "../../scrollbar";

const StyledSectionBody = styled.div`
  margin: 16px 0;
  ${props => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
`;

const SectionBody = React.memo(props => {
  console.log("PageLayout SectionBody render");
  const { children, withScroll } = props;

  return (
    <StyledSectionBody>
      {withScroll 
        ? <Scrollbar stype="mediumBlack">{children}</Scrollbar> 
        : <>{children}</>
      }
    </StyledSectionBody>
  );
});

SectionBody.propTypes = {
  withScroll: PropTypes.bool
};

SectionBody.defaultProps = {
  withScroll: true
};

export default SectionBody;
