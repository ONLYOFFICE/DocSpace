import React from "react";
import styled from "styled-components";
import Heading from "@docspace/components/heading";
import PropTypes from "prop-types";
const Container = styled.div`
  margin: ${(props) => `0 0 ${props.marginBottom} 0`};
  max-width: 1024px;
`;

const StyledHeader = styled(Heading)`
  margin: 0 0 24px 0;
  line-height: unset;
`;

const InfoFieldContainer = React.memo((props) => {
  const { headerText, children, marginBottom } = props;

  return (
    <Container marginBottom={marginBottom}>
      <StyledHeader level={2} size="small">
        {headerText}
      </StyledHeader>
      {children}
    </Container>
  );
});

InfoFieldContainer.propTypes = {
  marginBottom: PropTypes.string,
};

InfoFieldContainer.defaultProps = {
  marginBottom: "40px",
};
export default InfoFieldContainer;
