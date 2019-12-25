import React from "react";
import styled from "styled-components";
import { Heading } from "asc-web-components";

const Container = styled.div`
  margin: 0 0 40px 0;
  max-width: 1024px;
`;

const StyledHeader = styled(Heading)`
  margin: 0 0 24px 0;
  line-height: unset;
`;

const InfoFieldContainer = React.memo(props => {
  const { headerText, children } = props;

  return (
    <Container>
      <StyledHeader level={2} size='small'>{headerText}</StyledHeader>
      {children}
    </Container>
  );
});

export default InfoFieldContainer;
