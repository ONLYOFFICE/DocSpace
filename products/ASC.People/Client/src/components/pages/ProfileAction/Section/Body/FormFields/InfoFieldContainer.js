import React from "react";
import styled from "styled-components";
import { Headline } from 'asc-web-common';

const Container = styled.div`
  margin: 0 0 40px 0;
`;

const StyledHeader = styled(Headline)`
  margin: 0 0 24px 0;
  line-height: unset;
`;

const InfoFieldContainer = React.memo(props => {
  const { headerText, children } = props;

  return (
    <Container>
      <StyledHeader type='content'>{headerText}</StyledHeader>
      {children}
    </Container>
  );
});

export default InfoFieldContainer;
